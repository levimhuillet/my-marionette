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

    #endregion // Member Variables

    #region Events

    public UnityEvent OnSequenceCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        OnSequenceCompleted = new UnityEvent();
    }

    private void Start() {
        NarrationManager.OnNarrationClipCompleted.AddListener(HandleNarrationClipCompleted);
    }

    #endregion // Unity Callbacks

    #region Member Functions

    private string EvaluateNextSequence(Puppet puppetKey) {

        if (puppetKey == null) {
            return null;
        }

        // Iterate through branches to find one triggered by given puppet
        foreach (Branch b in currSequenceData.Branches) {
            if (b.PuppetKey == puppetKey || b.PuppetKey == null) {
                return b.SequenceValueID;
            }
        }

        // No matching sequence
        return null;
    }

    public void LoadSequence(SequenceData sequence) {
        // set current sequence
        currSequenceData = sequence;

        // load current sequence's narration data IDs
        currNarrationIDs = currSequenceData.NarrationDataIDs;
    }

    public void BeginSequence() {
        if (currNarrationIDs.Length == 0) {
            Debug.Log("[Sequence Manager] WARNING: no narrations in sequence!");

            // TODO: Handle no narrations

            return;
        }

        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Beginning Sequence " + currSequenceData.ID); }

        // Hand off first clip to to Narration Manager
        NarrationManager.Instance.StartNarration(currNarrationIDs[0]);
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
        // When Narration Manager has finished the clip, come back for next clip
        // TODO: pass in puppet choice
        string nextSequenceID = EvaluateNextSequence(null);

        // If next sequence is null, return control to Act Manager to trigger next sequence
        if (nextSequenceID == null) {
            OnSequenceCompleted.Invoke();
        }
    }

    #endregion

}
