using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringsTrigger : MonoBehaviour
{
    [SerializeField] private LineRenderer[] risingStrings;
    [SerializeField] private GameObject[] hands;

    private float m_startHandWidth;

    private void OnEnable() {
        // activate strings
        for (int i = 0; i < risingStrings.Length; i++) {
            risingStrings[i].SetPositions(
                    new Vector3[] {
                        risingStrings[i].gameObject.transform.position,
                        hands[i].transform.position
                    }
                );
            risingStrings[i].gameObject.SetActive(true);
        }

        m_startHandWidth = hands[0].transform.lossyScale.x;

        // TODO: start routine for rising strings
        StartCoroutine(RaiseStrings());
    }

    private void Update() {
        float currMod = hands[0].transform.lossyScale.x / m_startHandWidth;

        for (int i = 0; i < risingStrings.Length; i++) {
            risingStrings[i].SetPositions(
                    new Vector3[] {
                        risingStrings[i].gameObject.transform.position,
                        hands[i].transform.position
                    }
                );

            risingStrings[i].widthMultiplier = currMod;
            Debug.Log("modding string " + risingStrings[i]);
        }
    }

    private IEnumerator RaiseStrings() {
        yield return null;
    }
}
