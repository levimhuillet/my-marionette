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
        currSequenceData = sequence;
    }

    public void BeginSequence() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence Manager] Beginning Sequence " + currSequenceData.ID); }

        // TEMP HACK: print first narration subtitles in sequence
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Sequence (will be Narration) Manager] Subtitles: " + NarrationManager.Instance.GetNarrationData(currSequenceData.NarrationDataIDs[0]).Subtitle); }

        // Hand off next clip to to Narration Manager
        //{
            // TODO: pass in puppet choice
        string nextSequenceID = EvaluateNextSequence(null);

        // If next sequence is null, return control to Act Manager to trigger next sequence
        if (nextSequenceID == null) {
            OnSequenceCompleted.Invoke();
        }
        //}

        // When Narration Manager has finished the clip, come back for next clip
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
        // TODO: this
    }

    #endregion

}
