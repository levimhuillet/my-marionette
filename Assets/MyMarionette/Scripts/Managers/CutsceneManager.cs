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

    [SerializeField] private GameObject XROrigin;
    [SerializeField] private GameObject CutsceneCam;

    [SerializeField] private PlayableDirector openingSceneDirector;
    [SerializeField] private PlayableDirector closingSceneDirector;

    [SerializeField] private Transform gameplayPosTransform;

    // Stage cutscenes
    private PlayableDirector theaterOpenDirector;

    private void StartOpeningCutscene() {
        CutsceneCam.SetActive(true);

        Debug.Log("Is director enabled? " + openingSceneDirector.isActiveAndEnabled);
        openingSceneDirector.Play();
        Debug.Log("director is " + openingSceneDirector);
        openingSceneDirector.played += Play_Director;
        openingSceneDirector.stopped += Stop_Director;

        XROrigin.SetActive(false);
    }

    private void StartIntermissionCutscene() {
        OnCutsceneCompleted.Invoke();
        return;
    }

    private void StartClosingCutscene() {
        closingSceneDirector.Play();

        closingSceneDirector.stopped += HandleDirectorStoppedClosing;
    }

    private void StartTheaterOpen() {
        theaterOpenDirector = GameObject.Find("TheaterDoorGroup(Clone)").GetComponent<PlayableDirector>();
        theaterOpenDirector.Play();

        theaterOpenDirector.stopped += HandleDirectorStoppedTheaterOpen;
    }

    public void StartCutscene(string id) {
        switch(id) {
            case "closing":
                StartClosingCutscene();
                break;
            case "theater-open":
                StartTheaterOpen();
                break;
            default:
                break;
        }
    }

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
        XROrigin.transform.position = gameplayPosTransform.position;
        XROrigin.transform.localScale = new Vector3(3, 2, 3);
        //XROrigin.SetActive(true);

        GameObject cutsceneObj = GameObject.Find("OpeningCutscene");

        cutsceneObj.SetActive(false);


        OnCutsceneCompleted.Invoke();

        openingSceneDirector.played -= Play_Director;
        openingSceneDirector.stopped -= Stop_Director;
    }

    private void HandleDirectorStoppedClosing(PlayableDirector obj) {
        ChestManager.Instance.CloseChest();

        OnCutsceneCompleted.Invoke();
    }

    private void HandleDirectorStoppedTheaterOpen(PlayableDirector obj) {
        // nothing needed
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
    }

    private void Start() {
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
    }

    #endregion // Unity Callbacks

    #region Event Handlers

    private void HandleTheaterStateAdvanced(TheaterManager.State state) {
        switch (state) {
            case TheaterManager.State.PrePlay:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                StartOpeningCutscene();
                //StartClosingCutscene();
                break;
            case TheaterManager.State.Intermission:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                StartIntermissionCutscene();
                break;
            //case TheaterManager.State.PostPlay:
                //if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Cutscene Manager] Beginning the " + state + " Cutscene"); }
                //StartClosingCutscene();
                //break;
            default:
                break;
        }
    }

    #endregion // Event Handlers

}
