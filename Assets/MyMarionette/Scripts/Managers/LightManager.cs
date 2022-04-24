using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;

    [SerializeField] Light[] spotlights, pointLights;
    [SerializeField] float spotlightMaxIntensity, pointLightMaxIntensity;

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        foreach (Light light in spotlights) {
            light.intensity = 0;
        }
        foreach (Light light in pointLights) {
            light.intensity = 0;
        }
    }


    public void TurnOnLights(float time) {
        StartCoroutine(TurnOnRoutine(time));
    }

    public void TurnOffLights(float time) {
        StartCoroutine(TurnOffRoutine(time));
    }

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

}
