using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    private AudioSource audioSrc;
    [SerializeField] TMP_Text title;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (title.alpha == 0) {
            title.CrossFadeAlpha(1f, 2f, false);
        }
        else if (title.alpha == 1) {
            title.CrossFadeAlpha(0.0f, 2f, false);
        }
        */

        // load main scene after intro music
        if (!audioSrc.isPlaying) {
            SceneManager.LoadScene("MainScene");
        }
    }
}
