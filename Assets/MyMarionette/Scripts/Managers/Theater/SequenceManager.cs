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
    [SerializeField] private SequenceData[] sequenceData;

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

        // Iterate through branches to find one triggered by given puppet
        foreach (Branch b in currSequenceData.Branches) {
            if (b.PuppetKey == puppetKey || b.PuppetKey == null) {
                return b.SequenceValueID;
            }
        }

        // No matching sequence
        return null;
    }

    private void LoadSequence(SequenceData sequence) {
        currSequenceData = sequence;
    }

    private void BeginSequence() {
        // TODO: this
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
