using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    #region Unity Callbacks

    private void Start() {
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
    }

    #endregion // Unity Callbacks

    #region Event Handlers

    private void HandleTheaterStateAdvanced(TheaterManager.State state) {
        switch (state) {
            case TheaterManager.State.PrePlay:
                break;
            case TheaterManager.State.Intermission:
                break;
            case TheaterManager.State.PostPlay:
                break;
            default:
                break;
        }
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("State advanced to " + state); }
    }
    
    #endregion // Event Handlers

}
