using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private bool debugSpeed; // speed up for debugging
    private static float DEBUG_SPEED = 3;

    public struct AudioLoopPair
    {
        public AudioLoopPair(AudioData data, bool loop) {
            Data = data;
            Loop = loop;
        }

        public AudioData Data { get; set; }
        public bool Loop { get; set; }
    }

    #region Editor

    // Data
    [SerializeField] private AudioData[] audioData;

    #endregion // Editor

    #region Member Variables

    // Components
    private AudioSource audioSrc;

    // Maps
    private Dictionary<string, AudioData> audioMap;

    // Data
    private AudioData currData;

    #endregion // Member Variables

    #region Unity Callbacks

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(this.gameObject);
        }

        audioSrc = GetComponent<AudioSource>();
    }

    #endregion // Unity Callbacks

    #region Data Retrieval
    public static AudioData GetAudioData(string id) {
        // initialize the map if it does not exist
        if (Instance.audioMap == null) {
            Instance.audioMap = new Dictionary<string, AudioData>();
            foreach (AudioData data in Instance.audioData) {
                Instance.audioMap.Add(data.ID, data);
            }
        }
        if (Instance.audioMap.ContainsKey(id)) {
            return Instance.audioMap[id];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Audio " +
                "with id `{0}' is in the database", id
            ));
        }
    }

    #endregion // Data Retrieval

    #region Audio Methods

    public void PlayAudio(string clipID, bool loop = false) {
        AudioData newData = GetAudioData(clipID);
        currData = newData;
        LoadAudio(audioSrc, newData);
        audioSrc.loop = loop;
        audioSrc.Play();

        if (debugSpeed) { audioSrc.pitch = DEBUG_SPEED; }
        else { audioSrc.pitch = 1; }
    }

    public void PlayAudio(AudioClip clip, bool loop = false) {
        audioSrc.clip = clip;
        audioSrc.loop = loop;
        audioSrc.Play();

        if (debugSpeed) { audioSrc.pitch = DEBUG_SPEED; }
        else { audioSrc.pitch = 1; }
    }

    public void PlayAudioDirect(AudioData newData, bool isNarration, bool loop = false) {
        currData = newData;
        LoadAudio(audioSrc, newData);
        audioSrc.loop = loop;
        audioSrc.Play();

        if (debugSpeed) { audioSrc.pitch = DEBUG_SPEED; }
        else { audioSrc.pitch = 1; }

        // TODO: track if is narration
    }

    public void PauseAudio() {
        audioSrc.Pause();
    }

    public void UnPauseAudio() {
        audioSrc.UnPause();
    }

    public void StopAudio() {
        audioSrc.Stop();
    }

    public void PlayOneShot(string clipID) {
        AudioData data = GetAudioData(clipID);
        audioSrc.PlayOneShot(currData.Clip);
    }

    public bool IsPlayingAudio() {
        return audioSrc.isPlaying;
    }

    #endregion // Audio Methods

    #region Static Functions
    public static void LoadAudio(AudioSource source, AudioData data) {
        source.clip = data.Clip;
        source.volume = data.Volume;
        source.panStereo = data.Pan;
    }

    #endregion
}
