using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetManager : MonoBehaviour
{
    public static PuppetManager Instance;

    [SerializeField] private Puppet startPuppet1, startPuppet2; // TEMPORARY SOLUTION
    [SerializeField] private Transform leftController, rightController; // transforms (positions) of the controller hands

    private Puppet currPuppet;
    private List<GameObject> puppetParts; // tracks the puppet components in the scene

    private void Start() {
        puppetParts = new List<GameObject>();

        SetCurrPuppet(startPuppet1); // TEMPORARY SOLUTION
    }

    public void SetCurrPuppet(Puppet puppet) {
        if (currPuppet != null) {
            ClearPuppet();
        }

        // Set new puppet reference
        currPuppet = puppet;

        /* Create the puppet parts */
        // Controller sticks

        GameObject leftStick = Instantiate(puppet.LeftStick, leftController);
        puppetParts.Add(leftStick);

        GameObject rightStick = Instantiate(puppet.RightStick, rightController);
        puppetParts.Add(rightStick);

        // Puppet body

        // Strings 

        // Hook the parts together (stick-to-puppet anchors)
    }

    // TODO: implement this
    private void ClearPuppet() {
        // destroy puppet gameObjects (hands, body, strings, etc.)
        foreach(GameObject part in puppetParts) {
            Destroy(part);
        }
        puppetParts.Clear();

        // remove reference
        currPuppet = null;
    }
}
