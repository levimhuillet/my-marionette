using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudienceManager : MonoBehaviour
{
    #region Editor

    [SerializeField] private AudioClip cheerClip, sinisterLaughClip;
    [SerializeField] private GameObject audienceMemberPrefab;

    #endregion // Editor

    #region Member Variables

    private AudioSource audioSrc;

    #endregion // Member Variables

    #region Unity Callbacks

    private void OnEnable() {
        audioSrc = GetComponent<AudioSource>();
    }

    #endregion // Unity Callbacks

    #region Member Functions

    public void AssembleAudience() {
        Debug.Log("Assembling Audience");

        // AudioManager.Instance.PlayAudio(cheerClip)
    }

    #endregion // Member Functions
}
