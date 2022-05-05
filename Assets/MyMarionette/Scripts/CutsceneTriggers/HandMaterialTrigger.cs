using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMaterialTrigger : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] handRenders;
    [SerializeField] private Material[] handMaterials;

    private static float TRANSITION_TIME = 3f;

    private void OnEnable() {

        handRenders[0].material = handMaterials[1];
        handRenders[1].material = handMaterials[1];

        // TODO: ease transition between materials
        // start material transition
        //StartCoroutine(TransitionHands(TRANSITION_TIME));
    }

    private IEnumerator TransitionHands(float duration) {
        float elapsedTime = 0;
        while (elapsedTime <= duration) {
            elapsedTime += Time.deltaTime;

            handRenders[0].material.Lerp(handMaterials[0], handMaterials[1], (elapsedTime / duration));
            handRenders[1].material.Lerp(handMaterials[0], handMaterials[1], (elapsedTime / duration));

            yield return null;
        }
    }

    private void Update() {
        /*
        // ping-pong between the materials over the duration
        float lerp = Mathf.PingPong(Time.time, TRANSITION_TIME) / TRANSITION_TIME;
        handRenders[0].material.Lerp(handMaterials[0], handMaterials[1], lerp);
        handRenders[1].material.Lerp(handMaterials[0], handMaterials[1], lerp);
        */
    }
}
