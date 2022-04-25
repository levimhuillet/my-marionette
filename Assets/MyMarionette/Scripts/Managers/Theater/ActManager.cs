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

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        OnActCompleted = new UnityEvent();
    }

    private void Start() {
        sequenceManager.OnAllSequencesCompleted.AddListener(HandleAllSequencesCompleted);
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
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

        // load first sequence
        sequenceManager.LoadSequence(currActData.FirstSequenceID);

        // Turn off ambient lights
        LightManager.Instance.TurnOffAmbiance(2);
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

    private void HandleAllSequencesCompleted() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Act Manager] Received SequenceManager end of all sequences. Loading next sequence..."); }

        // Turn on ambient lights
        LightManager.Instance.TurnOnAmbiance(2);

        // Pass control back to Theater Manager
        OnActCompleted.Invoke();
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
