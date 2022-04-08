using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtains : MonoBehaviour
{
    [SerializeField] private GameObject curtainLeft, curtainRight;
    [SerializeField] private float openSpeed;

    private bool openComplete;

    private void Start() {
        openComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (openComplete) { return; }

        curtainLeft.transform.Translate(new Vector3(0, -openSpeed * Time.deltaTime, 0));
        curtainRight.transform.Translate(new Vector3(0, openSpeed * Time.deltaTime, 0));

        if (curtainLeft.transform.position.x < -.04) {
            openComplete = true;
        }
    }
}
