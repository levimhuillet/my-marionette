using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TheaterManager : MonoBehaviour
{
    public static TheaterManager Instance;

    #region Debug

    public bool DEBUGGING = true; // for developer use; if true, prints debug statements 

    #endregion // Debug

    public enum State
    {
        PrePlay,
        Act1,
        Intermission,
        Act2,
        PostPlay
     }

    #region Member Variables

    // Sub-Managers
    [SerializeField] private ChestManager chestManager;
    [SerializeField] private ActManager actManager;
    [SerializeField] private NarrationManager narrationManager;
    [SerializeField] private AudienceManager audienceManager;
    [SerializeField] private GameplayManager gameplayManager;

    // State
    private State[] states;
    private int currStateIndex;
    private State currState;

    #endregion // Member Variables

    #region Events

    public UnityEvent<State> OnStateAdvanced;

    #endregion // Events

    #region Unity Callbacks

    private void Awake() {
        OnStateAdvanced = new UnityEvent<State>();

        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        actManager.OnActCompleted.AddListener(HandleActCompleted);

        // define the game phases (states)
        InitializeStateProgression();
    }

    private void Update() {
        if (currStateIndex < 6) {
            AdvanceState();
        }
    }

    #endregion // Unity Callbacks

    #region Event Handlers

    private void HandleActCompleted() {
        // TODO: this
    }

    #endregion // Event Handlers

    #region Helper Methods

    private void InitializeStateProgression() {
        states = new State[] {
            State.PrePlay,
            State.Act1,
            State.Intermission,
            State.Act2,
            State.PostPlay
        };

        currStateIndex = -1;
        AdvanceState();
    }

    private void AdvanceState() {
        currStateIndex++;

        if (currStateIndex < states.Length) {
            currState = states[currStateIndex];
            OnStateAdvanced.Invoke(currState);
            //if (DEBUGGING) { PrintCurrState(); }
        }
        else {
            // Error: tried to advance a state beyond what is defined
            Debug.Log("ERROR: tried to advance to an undefined state!");
        }
    }

    private void PrintCurrState() {
        switch (currState) {
            case State.PrePlay:
                if (DEBUGGING) { Debug.Log("[Theater Manager] The play has not started yet."); }
                break;
            case State.Act1:
                if (DEBUGGING) { Debug.Log("[Theater Manager] The play is in Act 1."); }
                break;
            case State.Intermission:
                if (DEBUGGING) { Debug.Log("[Theater Manager] The play is in Intermission."); }
                break;
            case State.Act2:
                if (DEBUGGING) { Debug.Log("[Theater Manager] The play is in Act 2."); }
                break;
            case State.PostPlay:
                if (DEBUGGING) { Debug.Log("[Theater Manager] The play has finished."); }
                break;
            default:
                break;
        }
    }

    #endregion // Helper Methods
}
