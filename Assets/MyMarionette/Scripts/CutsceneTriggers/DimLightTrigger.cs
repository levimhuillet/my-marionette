using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimLightTrigger : MonoBehaviour
{
    [SerializeField] Light chestLight;

    private void OnEnable() {
        //chestLight.gameObject.SetActive(false);
        chestLight.intensity = 5f;
        //StartCoroutine(TurnOffLights(3f));
    }

    public IEnumerator TurnOffLights(float time) {
        yield return StartCoroutine(TurnOffRoutine(time));
    }

    private IEnumerator TurnOffRoutine(float time) {
        float spotStep = 10f / time;

        while (chestLight.intensity > 0) {
            chestLight.intensity -= Mathf.Max(spotStep * Time.deltaTime, 0);

            yield return null;
        }
    }
}
