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
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("State advanced to " + state); }
    }
    
    #endregion // Event Handlers

}
