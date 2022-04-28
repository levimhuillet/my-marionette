using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance;

    #region Editor

    // Data
    [SerializeField] private NarrationAudioData[] narrationData; // all narration data

    // Subtitles
    [SerializeField] private TMP_Text subtitleText;

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<string, NarrationAudioData> narrationMap;

    // Narration
    private NarrationAudioData currNarrationData;
    private bool startedPlaying;

    #endregion // Member Variables

    #region Events

    [HideInInspector]
    public static UnityEvent OnNarrationClipCompleted;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(this.gameObject);
            return;
        }

        narrationMap = new Dictionary<string, NarrationAudioData>();
        foreach (NarrationAudioData data in narrationData) {
            narrationMap.Add(data.ID, data);
        }

        OnNarrationClipCompleted = new UnityEvent();
        startedPlaying = false;
    }

    private void Update() {
        // check when audio has finished playing
        if (startedPlaying && !AudioManager.Instance.IsPlayingAudio()) {
            startedPlaying = false;
            subtitleText.gameObject.SetActive(false);
            subtitleText.text = "[Subtitles]";
            OnNarrationClipCompleted.Invoke();
        }
    }

    #endregion // Unity Callbacks

    #region Narration

    public void StartNarration(string narrationDataID) {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Narration Manager] Starting narration."); }

        currNarrationData = GetNarrationData(narrationDataID);
        PlayNarration(currNarrationData);
    }

    private void PlayNarration(NarrationAudioData data) {
        AudioManager.Instance.PlayAudioDirect(data, true);
        startedPlaying = true;
        subtitleText.text = data.Subtitle;
        subtitleText.gameObject.SetActive(true);
    }

    #endregion // Narration

    #region Data Retrieval

    public NarrationAudioData GetNarrationData(string id) {
        // initialize the map if it does not exist
        if (narrationMap == null) {
            narrationMap = new Dictionary<string, NarrationAudioData>();
            foreach (NarrationAudioData data in narrationData) {
                narrationMap.Add(data.ID, data);
            }
        }
        if (narrationMap.ContainsKey(id)) {
            return narrationMap[id];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Narration " +
                "with id `{0}' is in the database", id
            ));
        }
    }

    #endregion // Data Retrieval

    #region Get & Set

    public Dictionary<string, NarrationAudioData> GetNarrationMap() {
        return narrationMap;
    }

    public void SetNarrationMap(Dictionary<string, NarrationAudioData> newMap) {
        narrationMap = newMap;
    }

    #endregion // Get & Set
}
