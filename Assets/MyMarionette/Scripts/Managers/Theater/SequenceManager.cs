using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        OnAllSequencesCompleted = new UnityEvent();
    }

    private void Start() {
        NarrationManager.OnNarrationClipCompleted.AddListener(HandleNarrationClipCompleted);
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

        BeginSequence();
    }

    private void BeginSequence() {
        if (currNarrationIDs.Length == 0) {
            Debug.Log("[Sequence Manager] WARNING: no narrations in sequence!");

            // TODO: Handle no narrations

            return;
        }

        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Beginning Sequence " + currSequenceData.ID); }

        // Hand off first clip to to Narration Manager
        NarrationManager.Instance.StartNarration(currNarrationIDs[currNarrationIndex]);
    }

    #endregion // Member Functions

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

            // reset narration index to initial val
            currNarrationIndex = 0;

            // When all clips are run through, evaluate next sequence
            ChestManager.PuppetChoice currChoice = ChestManager.Instance.GetPuppetChoice(currSequenceData.ID);
            string nextSequenceID = EvaluateNextSequence(currChoice.SelectedPuppet);

            if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Next sequence is " + nextSequenceID); }

            // If next sequence is null, return control to Act Manager to trigger next sequence
            if (nextSequenceID == null) {
                OnAllSequencesCompleted.Invoke();
                return;
            }

            LoadSequence(nextSequenceID);
        }
    }

    #endregion

}
