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
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                break;
            case TheaterManager.State.Intermission:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                break;
            case TheaterManager.State.PostPlay:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                break;
            default:
                break;
        }
    }
    
    #endregion // Event Handlers

}
