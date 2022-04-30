using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [Serializable]
    public struct GameplayEvent
    {
        public string ID;
        public string Prompt;

        public GameplayEvent(string inID, string inPrompt) {
            ID = inID;
            Prompt = inPrompt;
        }
    }

    #region Editor

    [SerializeField] GameplayEvent[] allGameplayEvents;
    [SerializeField] TMP_Text promptText;

    #endregion // Editor

    #region Events

    [HideInInspector]
    public static UnityEvent OnGameplayCompleted;

    #endregion // Events

    #region Member Variables

    // Maps
    private Dictionary<string, GameplayEvent> gameplayMap;

    #endregion // Member Variables

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

    public void BeginGameplay(string gameplayID) {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Gameplay Manager] Beginning gameplay with id " + gameplayID); }

        // set prompt text
        promptText.SetText(GetGameplayEvent(gameplayID).Prompt);
        promptText.gameObject.SetActive(true);

        // Handle each gameplay event separately since they are each so different
        switch (gameplayID) {
            case "raise-hands":
                //StartCoroutine(RaiseHandsCheck());
                StartCoroutine(PlaceholderCheck());
                break;
            case "walk-and-brush":
                StartCoroutine(PlaceholderCheck());
                break;
            case "exit":
                StartCoroutine(PlaceholderCheck());
                break;
            case "try-door":
                StartCoroutine(PlaceholderCheck());
                break;
            case "jump":
                StartCoroutine(PlaceholderCheck());
                break;
            case "enter-quarters":
                StartCoroutine(PlaceholderCheck());
                break;
            case "high-five":
                StartCoroutine(PlaceholderCheck());
                break;
            case "enter-fate":
                StartCoroutine(PlaceholderCheck());
                break;
            default:
                StartCoroutine(PlaceholderCheck());
                break;
        }

    }

    private IEnumerator RaiseHandsCheck() {
        // TODO: check if both hands are above head by at least X distance
        while (false) {
            yield return null;
        }

        CompleteGameplay();
    }

    private IEnumerator PlaceholderCheck() {
        float timer = 6f;

        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        CompleteGameplay();
    }

    #endregion // Member Functions

    #region Helper Functions

    private void CompleteGameplay() {
        promptText.gameObject.SetActive(false);
        OnGameplayCompleted.Invoke();
    }

    #endregion // Helper Functions

    #region Data Retrieval

    public GameplayEvent GetGameplayEvent(string id) {
        // initialize the map if it does not exist
        if (gameplayMap == null) {
            gameplayMap = new Dictionary<string, GameplayEvent>();
            foreach (GameplayEvent gameplay in allGameplayEvents) {
                gameplayMap.Add(gameplay.ID, gameplay);
            }
        }
        if (gameplayMap.ContainsKey(id)) {
            return gameplayMap[id];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Gameplay Event " +
                "with id `{0}' is in the database", id
            ));
        }
    }

    #endregion // Data Retrieval
}
