using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SequenceManager : MonoBehaviour
{
    [Serializable]
    public struct Branch
    {
        public Puppet PuppetKey;
        public string SequenceValueID;
    }

    #region Editor

    // Data
    [SerializeField] private SequenceData[] sequenceData; // all sequence data

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<string, SequenceData> sequenceMap;

    // Data
    private SequenceData currSequenceData;
    private string[] currNarrationIDs;

    // Narration
    private int currNarrationIndex;

    #endregion // Member Variables

    #region Events

    [HideInInspector]
    public UnityEvent OnAllSequencesCompleted;

    private UnityEvent OnStartActionsCompleted;
    private UnityEvent OnEndActionsCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        OnAllSequencesCompleted = new UnityEvent();
        OnStartActionsCompleted = new UnityEvent();
        OnEndActionsCompleted = new UnityEvent();
    }

    private void Start() {
        NarrationManager.OnNarrationClipCompleted.AddListener(HandleNarrationClipCompleted);
        OnStartActionsCompleted.AddListener(HandleStartActionsCompleted);
        OnEndActionsCompleted.AddListener(HandleEndActionsCompleted);

        ChestManager.OnSwapCompleted.AddListener(HandleSwapCompleted);
        GameplayManager.OnGameplayCompleted.AddListener(HandleGameplayCompleted);
    }

    #endregion // Unity Callbacks

    #region Member Functions

    private string EvaluateNextSequence(Puppet puppetKey) {
        // Iterate through branches to find one triggered by given puppet
        foreach (Branch b in currSequenceData.Branches) {
            if (b.PuppetKey == puppetKey || b.PuppetKey == null) {
                return b.SequenceValueID;
            }
        }

        // No matching sequence (end of sequence chain, and thus end of act)
        return null;
    }

    public void LoadSequence(string sequenceID) {
        // set current sequence
        currSequenceData = GetSequenceData(sequenceID);

        // load current sequence's narration data IDs
        currNarrationIDs = currSequenceData.NarrationDataIDs;

        // set the narration index to initial val
        currNarrationIndex = 0;

        // generate props
        if (currSequenceData.Props != null) {
            EffectsManager.Instance.GenerateProps(currSequenceData.Props);
        }

        BeginSequence();
    }

    private void BeginSequence() {
        if (currNarrationIDs.Length == 0) {
            Debug.Log("[Sequence Manager] WARNING: no narrations in sequence!");

            // TODO: Handle no narrations

            return;
        }

        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Beginning Sequence " + currSequenceData.ID); }

        StartCoroutine(StartActionsRoutine());
    }

    #endregion // Member Functions

    #region Helper Functions

    private IEnumerator StartActionsRoutine() {
        // handle sequence StartActions
        yield return HandleEffects(currSequenceData.StartActions);

        // Turn on lights
        //yield return EffectsManager.Instance.TurnOnLights(2);

        OnStartActionsCompleted.Invoke();
    }

    private void ActionsAfterGameplay() {
        if (currSequenceData.TriggersCutscene) {
            CutsceneManager.Instance.StartCutscene(currSequenceData.CutsceneID);
            // cutscene continues on its own, no need to wait
        }

        StartCoroutine(EndActionsRoutine());
    }

    private IEnumerator EndActionsRoutine() {
        // handle sequence endActions
        yield return HandleEffects(currSequenceData.EndActions);

        // wait
        //yield return EffectsManager.Instance.Wait(2);

        OnEndActionsCompleted.Invoke();
    }

    private IEnumerator HandleEffects(List<EffectsManager.EffectAction> effectActions) {
        foreach (EffectsManager.EffectAction action in effectActions) {
            switch (action.EffectType) {
                case EffectsManager.Effect.Curtains:
                    if (action.Activating) {
                        yield return EffectsManager.Instance.OpenCurtains(2);
                    }
                    else {
                        yield return EffectsManager.Instance.CloseCurtains(2);
                    }
                    break;
                case EffectsManager.Effect.Lights:
                    if (action.Activating) {
                        yield return EffectsManager.Instance.TurnOnLights(2);
                    }
                    else {
                        yield return EffectsManager.Instance.TurnOffLights(2);
                    }
                    break;
                case EffectsManager.Effect.LightColor:
                    EffectsManager.Instance.SetLightColor(action.LightColorIndex);
                    break;
                case EffectsManager.Effect.PuppetSwap:
                    yield return ChestManager.Instance.BeginPuppetSwap(action.SwapRole);
                    break;
                case EffectsManager.Effect.ClearProps:
                    yield return EffectsManager.Instance.ClearProps();
                    break;
                case EffectsManager.Effect.Wait:
                    yield return EffectsManager.Instance.Wait(2f);
                    break;
                case EffectsManager.Effect.SkipTransition:
                    // Load credits early
                    SceneManager.LoadScene("Credits");
                    yield return null;
                    break;
                case EffectsManager.Effect.Ambiance:
                    if (action.Activating) {
                        StartCoroutine(EffectsManager.Instance.TurnOnAmbiance(10f));
                    }
                    else {
                        StartCoroutine(EffectsManager.Instance.TurnOffAmbiance(6f));
                    }
                    yield return null;
                    break;
                default:
                    yield return null;
                    break;
            }
        }
    }

    #endregion // Helper Functions

    #region Data Retrieval

    public SequenceData GetSequenceData(string id) {
        // initialize the map if it does not exist
        if (sequenceMap == null) {
            sequenceMap = new Dictionary<string, SequenceData>();
            foreach (SequenceData data in sequenceData) {
                sequenceMap.Add(data.ID, data);
            }
        }
        if (sequenceMap.ContainsKey(id)) {
            return sequenceMap[id];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Sequence " +
                "with id `{0}' is in the database", id
            ));
        }
    }

    #endregion // Data Retrieval

    #region Event Handlers

    private void HandleStartActionsCompleted() {
        // Hand off first clip to to Narration Manager
        NarrationManager.Instance.StartNarration(currNarrationIDs[currNarrationIndex]);
    }

    private void HandleEndActionsCompleted() {
        // reset narration index to initial val
        currNarrationIndex = 0;

        // When all clips are run through, evaluate next sequence
        string nextSequenceID = EvaluateNextSequence(PuppetManager.Instance.GetCurrPuppet());

        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Next sequence is " + nextSequenceID); }

        // If next sequence is null, return control to Act Manager to trigger next sequence
        if (nextSequenceID == null) {
            OnAllSequencesCompleted.Invoke();
            return;
        }

        LoadSequence(nextSequenceID);
    }

    private void HandleNarrationClipCompleted() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Received NarrationManager end of clip."); }

        // When Narration Manager has finished the clip, come back for next clip
        currNarrationIndex++;
        if (currNarrationIndex < currNarrationIDs.Length) {
            if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Loading next clip."); }

            // next clip
            NarrationManager.Instance.StartNarration(currNarrationIDs[currNarrationIndex]);
        }
        else {
            if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] No more clips. Evaluating next sequence."); }

            // handle gameplay
            if (currSequenceData.TriggersGameplay) {
                GameplayManager.Instance.BeginGameplay(currSequenceData.GameplayID);
                return;
            }

            ActionsAfterGameplay();
        }
    }

    private void HandleSwapCompleted() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Received ChestManager end of swap."); }
    }

    private void HandleGameplayCompleted() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Received GameplayManager end of gameplay."); }

        ActionsAfterGameplay();
    }

    #endregion

}
