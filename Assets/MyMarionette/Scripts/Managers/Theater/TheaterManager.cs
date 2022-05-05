using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TheaterManager : MonoBehaviour
{
    public static TheaterManager Instance;

    #region Debug

    public bool DEBUGGING = true; // for developer use; if true, prints debug statements 

    #endregion // Debug

    public enum State
    {
        Init,
        PrePlay,
        AdLib,
        Act1,
        Intermission,
        Act2
        //PostPlay
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

    [HideInInspector]
    public UnityEvent<State> OnStateAdvanced;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        OnStateAdvanced = new UnityEvent<State>();
    }

    private void Start() {
        actManager.OnActCompleted.AddListener(HandleActCompleted);
        CutsceneManager.Instance.OnCutsceneCompleted.AddListener(HandleCutsceneCompleted);
        chestManager.OnChoiceCompleted.AddListener(HandleChoiceCompleted);

        // define the game phases (states)
        InitializeStateProgression();
    }

    private void Update() {
        if (currState == State.Init) {
            AdvanceState();
        }
    }

    #endregion // Unity Callbacks

    #region Event Handlers

    private void HandleActCompleted() {
        if (Instance.DEBUGGING) { Debug.Log("[Theater Manager] Received ActManager end of act. Loading next state..."); }

        // Load next phase
        AdvanceState();
    }

    private void HandleCutsceneCompleted() {
        if (Instance.DEBUGGING) { Debug.Log("[Theater Manager] Received CutsceneManager end of cutscene. Loading next state..."); }

        // Load next phase
        AdvanceState();
    }

    private void HandleChoiceCompleted() {
        if (Instance.DEBUGGING) { Debug.Log("[Theater Manager] Received ChestManager end of choice. Loading next state..."); }

        // Load next phase
        AdvanceState();
    }

    #endregion // Event Handlers

    #region Helper Methods

    private void InitializeStateProgression() {
        states = new State[] {
            State.Init,
            State.PrePlay,
            State.AdLib,
            State.Act1,
            State.Intermission,
            State.Act2
            //State.PostPlay
        };

        currStateIndex = -1;
        AdvanceState();
    }

    private void AdvanceState() {
        currStateIndex++;

        if (currStateIndex < states.Length) {
            currState = states[currStateIndex];
            if (DEBUGGING) { PrintCurrState(); }
            OnStateAdvanced.Invoke(currState);
        }
        else {
            // Roll Credits
            SceneManager.LoadScene("Credits");
        }
    }

    private void PrintCurrState() {
        switch (currState) {
            case State.PrePlay:
                if (DEBUGGING) { Debug.Log("[Theater Manager] The play has not started yet."); }
                break;
            case State.AdLib:
                if (DEBUGGING) { Debug.Log("[Theater Manager] The player is selecting puppets."); }
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
            //case State.PostPlay:
                //if (DEBUGGING) { Debug.Log("[Theater Manager] The play has finished."); }
                //break;
            default:
                break;
        }
    }

    #endregion // Helper Methods
}
