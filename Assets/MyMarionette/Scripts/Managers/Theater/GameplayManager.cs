using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    #region Events

    [HideInInspector]
    public static UnityEvent OnGameplayCompleted;

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

        OnGameplayCompleted = new UnityEvent();
    }

    #endregion // Unity Callbacks

    #region Member Functions

    public void BeginGameplay() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Gameplay Manager] Beginning gameplay..."); }

        StartCoroutine(PlaceholderRoutine());
    }

    private IEnumerator PlaceholderRoutine() {
        float timer = 5f;

        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        OnGameplayCompleted.Invoke();
    }

    #endregion // Member Functions
}
