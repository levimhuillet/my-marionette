using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterManager : MonoBehaviour
{
    public static TheaterManager Instance;

    private enum State
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

    // State
    private State currState;

    #endregion // Member Variables

    #region Unity Callbacks

    private void Start() {
        actManager.OnActCompleted.AddListener(HandleActCompleted);
    }

    #endregion // Unity Callbacks

    #region Event Handlers

    private void HandleActCompleted() {
        // TODO: this
    }

    #endregion // Event Handlers
}
