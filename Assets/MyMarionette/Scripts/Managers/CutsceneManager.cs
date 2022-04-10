using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    #region Placeholder UI

    // Placeholder
    [SerializeField] private GameObject placeholderUI;
    [SerializeField] private Text placeholderHeaderText;
    [SerializeField] private Text placeholderTimeText;
    private float displayTime = 5f;
    private float displayTimer;
    private bool isDisplaying;

    private void StartPlaceholder(TheaterManager.State state) {
        displayTimer = displayTime;
        placeholderHeaderText.text = "You are in the " + state + " Cutscene.\nThis cutscene will end in:";
        placeholderTimeText.text = displayTimer.ToString("F1") + "\nseconds";
        placeholderUI.SetActive(true);
        isDisplaying = true;
    }

    #endregion

    #region Events

    [HideInInspector]
    public UnityEvent OnCutsceneCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        OnCutsceneCompleted = new UnityEvent();
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);

        // Placeholder
        isDisplaying = false;
    }

    private void Update() {
        // Placeholder
        if (isDisplaying) {
            displayTimer -= Time.deltaTime;
            placeholderTimeText.text = displayTimer.ToString("F1") + "\nseconds";
            if (displayTimer <= 0) {
                placeholderUI.gameObject.SetActive(false);
                isDisplaying = false;
                OnCutsceneCompleted.Invoke();
            }
        }
    }

    #endregion // Unity Callbacks

    #region Event Handlers

    private void HandleTheaterStateAdvanced(TheaterManager.State state) {
        switch (state) {
            case TheaterManager.State.PrePlay:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                StartPlaceholder(state);
                break;
            case TheaterManager.State.Intermission:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                StartPlaceholder(state);
                break;
            case TheaterManager.State.PostPlay:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                StartPlaceholder(state);
                break;
            default:
                break;
        }
    }
    
    #endregion // Event Handlers

}
