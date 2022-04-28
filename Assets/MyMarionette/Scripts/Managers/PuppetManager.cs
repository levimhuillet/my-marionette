using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetManager : MonoBehaviour
{
    public static PuppetManager Instance;

    [SerializeField] private Puppet startPuppet1, startPuppet2; // TEMPORARY SOLUTION
    [SerializeField] private Transform leftController, rightController; // transforms (positions) of the controller hands
    [SerializeField] private Transform stage;

    private struct AnchorPair {
        public GameObject StickAnchor;
        public GameObject PuppetAnchor;

        public AnchorPair(GameObject inStickAnchor, GameObject inPuppetAnchor) {
            StickAnchor = inStickAnchor;
            PuppetAnchor = inPuppetAnchor;
        }
    }

    private List<AnchorPair> anchorPairs;

    private Puppet currPuppet;
    private List<GameObject> puppetParts; // tracks the puppet components in the scene

    private void Start() {
        puppetParts = new List<GameObject>();
        anchorPairs = new List<AnchorPair>();

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
        SingleStick leftStick = Instantiate(puppet.Sticks.LeftStick.gameObject, leftController).GetComponent<SingleStick>();
        puppetParts.Add(leftStick.gameObject);

        SingleStick rightStick = Instantiate(puppet.Sticks.RightStick.gameObject, rightController).GetComponent<SingleStick>();
        puppetParts.Add(rightStick.gameObject);

        // Puppet body
        PuppetBody body = Instantiate(puppet.Body.gameObject).GetComponent<PuppetBody>();
        body.transform.position = new Vector3(stage.transform.position.x, stage.transform.position.y + 5, stage.transform.position.z);
        puppetParts.Add(body.gameObject);

        // TODO: Strings


        // Hook the parts together (stick-to-puppet anchors)
        if (body.AnchorPoints.Length != (leftStick.AnchorPoints.Length + rightStick.AnchorPoints.Length)) {
            Debug.Log("Warning! Mismatch between number of anchor points between puppet and sticks. Anchoring aborted.");
            return;
        }

        // left stick
        int overallAnchorIndex = 0;
        for (int a = 0; a < leftStick.AnchorPoints.Length; a++) {
            AnchorPair newPair = new AnchorPair(leftStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex]);
            anchorPairs.Add(newPair);
            overallAnchorIndex++;
        }

        // right stick
        for (int a = 0; a < rightStick.AnchorPoints.Length; a++) {
            AnchorPair newPair = new AnchorPair(rightStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex]);
            anchorPairs.Add(newPair);
            overallAnchorIndex++;
        }
    }

    // TODO: implement this
    private void ClearPuppet() {
        // destroy puppet gameObjects (hands, body, strings, etc.)
        foreach(GameObject part in puppetParts) {
            Destroy(part);
        }
        puppetParts.Clear();
        anchorPairs.Clear();

        // remove reference
        currPuppet = null;
    }
}
