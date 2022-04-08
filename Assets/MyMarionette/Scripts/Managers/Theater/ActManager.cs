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
    [SerializeField] private ActData[] actData;

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<int, ActData> actMap;

    // Data
    private ActData currActData;

    #endregion // Member Variables

    #region Events

    public UnityEvent OnActCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        OnActCompleted = new UnityEvent();
    }

    private void Start() {
        sequenceManager.OnSequenceCompleted.AddListener(HandleSequenceCompleted);
    }

    #endregion // Unity Callbacks

    #region Member Functions

    private void LoadAct(ActData act) {
        currActData = act;
    }

    private void BeginAct() {
        // TODO: this
    }

    #endregion // Member Functions

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

    private void HandleSequenceCompleted() {
        // TODO: this
    }

    #endregion // Event Handlers
}
