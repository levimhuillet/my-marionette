using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;

    [SerializeField] Light[] spotlights, pointLights;
    [SerializeField] float spotlightMaxIntensity, pointLightMaxIntensity;
    [SerializeField] Color[] stageLightColorPresets;
    [SerializeField] Color[] ambientLightColorPresets;

    [SerializeField] private GameObject curtainLeft, curtainRight;

    private static float CURTAIN_DIST = 20f; // how far the curtain travels to open and close
    private float curtainStart;

    private List<GameObject> activeProps;

    public enum Effect
    {
        None,
        Curtains,
        Lights,
        LightColor,
        PuppetSwap,
        ClearProps,
        Wait,
        SkipTransition,
        Ambiance
    }

    [Serializable] 
    public struct EffectAction
    {
        public Effect EffectType; // category of effect (curtain, light, etc.)
        public bool Activating; // whether light turns on/off; whether curtain opens/closes
        public int LightColorIndex; // if light action is change color, this specifies color
        public string SwapRole; // the role of the puppet being swapped
    }

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        curtainStart = curtainLeft.transform.position.x;

        foreach (Light light in spotlights) {
            light.intensity = 0;
        }
        foreach (Light light in pointLights) {
            light.intensity = 0;
        }

        activeProps = new List<GameObject>();
    }

    #region Member Functions

    public IEnumerator OpenCurtains(float time) {
        yield return StartCoroutine(OpenCurtainRoutine(time));
    }

    public IEnumerator CloseCurtains(float time) {
        yield return StartCoroutine(CloseCurtainRoutine(time));
    }

    public IEnumerator TurnOnLights(float time) {
        yield return StartCoroutine(TurnOnRoutine(time));
    }

    public IEnumerator TurnOffLights(float time) {
        yield return StartCoroutine(TurnOffRoutine(time));
    }

    public IEnumerator TurnOnAmbiance(float time) {
        yield return StartCoroutine(TurnOnAmbianceRoutine(time));
    }

    public IEnumerator TurnOffAmbiance(float time) {
        yield return StartCoroutine(TurnOffAmbianceRoutine(time));
    }

    public void GenerateProps(List<GameObject> props) {
        foreach(GameObject prop in props) {
            GameObject newProp = Instantiate(prop);
            activeProps.Add(newProp);
        }
    }

    public IEnumerator ClearProps() {
        foreach(GameObject prop in activeProps) {
            Destroy(prop);
        }
        activeProps.Clear();

        yield return null;
    }

    public IEnumerator Wait(float time) {
        yield return StartCoroutine(WaitRoutine(time));
    }

    #endregion // Member Functions

    #region Coroutines

    // TODO: consolidate open and close routines
    private IEnumerator OpenCurtainRoutine(float time) {
        float speed = CURTAIN_DIST / time;

        while (curtainLeft.transform.position.x < CURTAIN_DIST) {
            curtainLeft.transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
            curtainRight.transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));

            yield return null;
        }
    }

    private IEnumerator CloseCurtainRoutine(float time) {
        float speed = CURTAIN_DIST / time;

        while (curtainLeft.transform.position.x > curtainStart) {
            curtainLeft.transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            curtainRight.transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));

            yield return null;
        }
    }

    public void SetLightColor(int colorIndex) {
        foreach (Light light in spotlights) {
            light.color = stageLightColorPresets[colorIndex];
        }
        foreach (Light light in pointLights) {
            light.color = stageLightColorPresets[colorIndex];
        }
    }

    // TODO: consolidate on and off routines
    private IEnumerator TurnOnRoutine(float time) {
        float spotStep = spotlightMaxIntensity / time;
        float pointStep = pointLightMaxIntensity / time;

        while (spotlights[0].intensity < spotlightMaxIntensity) {
            foreach (Light light in spotlights) {
                light.intensity += Mathf.Min(spotStep * Time.deltaTime, spotlightMaxIntensity);
            }
            foreach (Light light in pointLights) {
                light.intensity += Mathf.Min(pointStep * Time.deltaTime, pointLightMaxIntensity);
            }

            yield return null;
        }
    }

    private IEnumerator TurnOffRoutine(float time) {
        float spotStep = spotlightMaxIntensity / time;
        float pointStep = pointLightMaxIntensity / time;

        while (spotlights[0].intensity > 0) {
            foreach (Light light in spotlights) {
                light.intensity -= Mathf.Max(spotStep * Time.deltaTime, 0);
            }
            foreach (Light light in pointLights) {
                light.intensity -= Mathf.Max(pointStep * Time.deltaTime, 0);
            }

            yield return null;
        }
    }

    private IEnumerator TurnOnAmbianceRoutine(float time) {
        yield return LerpAmbiance(time, ambientLightColorPresets[0]);
    }

    private IEnumerator TurnOffAmbianceRoutine(float time) {
        yield return LerpAmbiance(time, ambientLightColorPresets[1]);
    }

    private IEnumerator LerpAmbiance(float time, Color targetColor) {
        Color startColor = RenderSettings.ambientSkyColor;

        for (float t = 0f; t < time; t += Time.deltaTime) {
            float normalizedTime = t / time;
            RenderSettings.ambientSkyColor = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }
    }

    private IEnumerator WaitRoutine(float time) {
        while(time > 0) {
            time -= Time.deltaTime;
            yield return null;
        }
    }

    #endregion // Coroutines

}
