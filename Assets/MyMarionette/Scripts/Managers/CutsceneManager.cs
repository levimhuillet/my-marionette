using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    #region Placeholder UI

    // Placeholder
    [SerializeField] private GameObject placeholderUI;
    [SerializeField] private Text placeholderHeaderText;
    [SerializeField] private Text placeholderTimeText;

    [SerializeField] private GameObject XROrigin;
    [SerializeField] private GameObject CutsceneCam;

    [SerializeField] private PlayableDirector openingSceneDirector;
    private float displayTime = 30f;
    private float displayTimer;
    private bool isDisplaying;

    private void StartPlaceholder(TheaterManager.State state) {
        displayTimer = displayTime;
        // placeholderHeaderText.text = "You are in the " + state + " Cutscene.\nThis cutscene will end in:";
        // placeholderTimeText.text = displayTimer.ToString("F1") + "\nseconds";
        // placeholderUI.SetActive(true);

        CutsceneCam.SetActive(true);
        isDisplaying = true;

        Debug.Log("Is director enabled? " + openingSceneDirector.isActiveAndEnabled);
        openingSceneDirector.Play();
        Debug.Log("director is " + openingSceneDirector);
        openingSceneDirector.played += Play_Director;
        openingSceneDirector.stopped += Stop_Director;

        XROrigin.SetActive(false);
    }

    #endregion

    #region Events

    [HideInInspector]
    public UnityEvent OnCutsceneCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void Play_Director(PlayableDirector obj) {
        Debug.Log("Playing director");
    }

    private void Stop_Director(PlayableDirector obj) {
        Debug.Log("Sending next state");
        CutsceneCam.SetActive(false);
        XROrigin.SetActive(true);
        
   
        GameObject cutsceneObj = GameObject.Find("OpeningCutscene");

        cutsceneObj.SetActive(false);
    

        OnCutsceneCompleted.Invoke();

        openingSceneDirector.played += Play_Director;
        openingSceneDirector.stopped += Stop_Director;
    }

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        OnCutsceneCompleted = new UnityEvent();

        // Placeholder
        isDisplaying = false;
    }

    private void Start() {
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
    }

    private void Update() {
        // Placeholder
        // if (isDisplaying) {
        //     displayTimer -= Time.deltaTime;
        //     placeholderTimeText.text = displayTimer.ToString("F1") + "\nseconds";
        //     if (displayTimer <= 0) {
        //         placeholderUI.gameObject.SetActive(false);
        //         isDisplaying = false;
        //         OnCutsceneCompleted.Invoke();
        //     }
        // }
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
