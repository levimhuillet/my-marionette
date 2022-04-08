using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance;

    #region Events

    public static UnityEvent OnNarrationClipCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }

        OnNarrationClipCompleted = new UnityEvent();
    }

    #endregion // Unity Callbacks

    #region Narration

    public void PlayNarration(string narrationID) {
        AudioManager.Instance.PlayAudio(narrationID, true);
    }

    #endregion // Narration
}
