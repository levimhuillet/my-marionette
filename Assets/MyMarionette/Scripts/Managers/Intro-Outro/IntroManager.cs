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

    private enum State
    {
        Wait,
        FadeIn,
        FadeOut,
        Completed
    }

    private State state;
    private int waitIndex = 0;
    private bool executing;

    // Start is called before the first frame update
    void Start() {
        audioSrc = this.GetComponent<AudioSource>();

        state = State.Wait;
        executing = false;
        audioSrc.Play();
    }

    // Update is called once per frame
    void Update() {
        // load main scene after intro music
        if (!audioSrc.isPlaying) {
            SceneManager.LoadScene("MainScene");
        }

        if (!executing) {
            switch (state) {
                case State.Wait:
                    if (waitIndex == 0) {
                        StartCoroutine(Wait(1f));
                    }
                    else {
                        StartCoroutine(Wait(5f));
                    }
                    break;
                case State.FadeIn:
                    StartCoroutine(FadeInText(4f, title));
                    break;
                case State.FadeOut:
                    StartCoroutine(FadeOutText(4f, title));
                    break;
                case State.Completed:
                    break;
                default:
                    break;
            }
            executing = true;
        }
    }


    private IEnumerator FadeInText(float time, TMP_Text text) {
        float step = 1.0f / time;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a + (Time.deltaTime * step), 0, 1.0f));
            text.SetAllDirty();
            yield return null;
        }

        state = State.Wait;
        executing = false;
    }
    private IEnumerator FadeOutText(float time, TMP_Text text) {
        float step = 1.0f / time;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a - (Time.deltaTime * step), 0, 1f));
            text.SetAllDirty();
            yield return null;
        }

        state = State.Completed;
        executing = false;
    }

    private IEnumerator Wait(float time) {
        while (time > 0) {
            time -= Time.deltaTime;
            yield return null;
        }

        if (waitIndex == 0) {
            state = State.FadeIn;
        }
        else if (waitIndex == 1) {
            state = State.FadeOut;
        }
        executing = false;

        waitIndex++;
    }
}
