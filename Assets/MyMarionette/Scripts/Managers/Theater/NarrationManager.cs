using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance;

    #region Editor

    // Data
    [SerializeField] private NarrationAudioData[] narrationData; // all narration data

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<string, NarrationAudioData> narrationMap;

    #endregion // Member Variables

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
}
