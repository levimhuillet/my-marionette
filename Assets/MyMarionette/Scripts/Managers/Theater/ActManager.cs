using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActManager : MonoBehaviour
{
    #region Editor

    // Sub-Managers
    [SerializeField] private SequenceManager sequenceManager;

    // Data
    [SerializeField] private ActData[] actData; // all act data

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<int, ActData> actMap;

    // Data
    private ActData currActData;

    #endregion // Member Variables

    #region Events

    [HideInInspector]
    public UnityEvent OnActCompleted;
    private UnityEvent OnStartActionsCompleted;
    private UnityEvent OnEndActionsCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        OnActCompleted = new UnityEvent();
        OnStartActionsCompleted = new UnityEvent();
        OnEndActionsCompleted = new UnityEvent();
    }

    private void Start() {
        sequenceManager.OnAllSequencesCompleted.AddListener(HandleAllSequencesCompleted);
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
        OnStartActionsCompleted.AddListener(HandleStartActionsCompleted);
        OnEndActionsCompleted.AddListener(HandleEndActionsCompleted);
    }

    #endregion // Unity Callbacks

    #region Member Functions

    private void LoadAct(ActData act) {
        currActData = act;
    }

    private void BeginAct() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Act Manager] Beginning Act " + currActData.Num); }

        if (currActData.FirstSequenceID == null) {
            Debug.Log("[Act Manager] WARNING: no sequences in act.");

            // TODO: Handle no sequences

            return;
        }

        // wait for transitions to be complete before continuing
        StartCoroutine(StartActionsRoutine());
    }

    #endregion // Member Functions

    #region Helper Functions

    private IEnumerator StartActionsRoutine() {
        // Turn off ambient lights
        yield return StartCoroutine(EffectsManager.Instance.TurnOffAmbiance(2));

        // handle act StartActions
        yield return HandleEffects(currActData.StartActions);

        // open curtains
        yield return EffectsManager.Instance.OpenCurtains(2);

        OnStartActionsCompleted.Invoke();
    }

    private IEnumerator EndActionsRoutine() {
        // Turn on ambient lights
        StartCoroutine(EffectsManager.Instance.TurnOnAmbiance(2));

        // close curtains
        yield return EffectsManager.Instance.CloseCurtains(2);

        // handle act EndActions
        yield return HandleEffects(currActData.EndActions);

        // wait
        yield return EffectsManager.Instance.Wait(2);

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
                default:
                    yield return null;
                    break;
            }
        }
    }

    #endregion // Helper Functions

    #region Data Retrieval

    public ActData GetActData(int actNum) {
        // initialize the map if it does not exist
        if (actMap == null) {
            actMap = new Dictionary<int, ActData>();
            foreach (ActData data in actData) {
                actMap.Add(data.Num, data);
            }
        }
        if (actMap.ContainsKey(actNum)) {
            return actMap[actNum];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Act " +
                "with num `{0}' is in the database", actNum
            ));
        }
    }

    #endregion // Data Retrieval

    #region Event Handlers 

    private void HandleStartActionsCompleted() {
        // load first sequence
        sequenceManager.LoadSequence(currActData.FirstSequenceID);
    }

    private void HandleEndActionsCompleted() {
        // Pass control back to Theater Manager
        OnActCompleted.Invoke();
    }

    private void HandleAllSequencesCompleted() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Act Manager] Received SequenceManager end of all sequences. Loading next sequence..."); }

        // wait for transitions to be complete before continuing
        StartCoroutine(EndActionsRoutine());
    }

    private void HandleTheaterStateAdvanced(TheaterManager.State state) {
        switch (state) {
            case TheaterManager.State.Act1:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Act Manager] Loading " + state); }
                LoadAct(GetActData(1));
                BeginAct();
                break;
            case TheaterManager.State.Act2:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Act Manager] Loading " + state); }
                LoadAct(GetActData(2));
                BeginAct();
                break;
            default:
                break;
        }
    }

    #endregion // Event Handlers
}
