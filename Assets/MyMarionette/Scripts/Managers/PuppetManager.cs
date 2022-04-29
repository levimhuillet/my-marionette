using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetManager : MonoBehaviour
{
    public static PuppetManager Instance;

    [SerializeField] private Puppet startPuppet1, startPuppet2; // TEMPORARY SOLUTION
    [SerializeField] private Transform leftController, rightController; // transforms (positions) of the controller hands
    [SerializeField] private Transform stage;
    [SerializeField] private float puppetDistance; // distance between puppet and sticks
    [SerializeField] private GameObject stringPrefab;

    private struct AnchorPair
    {
        public GameObject StickAnchor;
        public GameObject PuppetAnchor;
        public float Offset;

        public AnchorPair(GameObject inStickAnchor, GameObject inPuppetAnchor, float inOffset) {
            StickAnchor = inStickAnchor;
            PuppetAnchor = inPuppetAnchor;
            Offset = inOffset;
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

    private void Update() {
        if (currPuppet != null) {
            foreach (AnchorPair pair in anchorPairs) {
                Vector3 currPuppetPos = pair.PuppetAnchor.transform.position;
                Vector3 currStickPos = pair.StickAnchor.transform.position;
                Vector3 targetPuppetPos = new Vector3(
                    currStickPos.x,
                    currStickPos.y - puppetDistance + pair.Offset,
                    currStickPos.z
                    );

                pair.PuppetAnchor.transform.position = targetPuppetPos;
                pair.PuppetAnchor.transform.rotation = pair.StickAnchor.transform.rotation;
            }
        }
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

        // Hook the parts together (stick-to-puppet anchors)
        if (body.AnchorPoints.Length != (leftStick.AnchorPoints.Length + rightStick.AnchorPoints.Length)) {
            Debug.Log("Warning! Mismatch between number of anchor points between puppet and sticks. Anchoring aborted.");
            return;
        }

        // left stick
        int overallAnchorIndex = 0;
        for (int a = 0; a < leftStick.AnchorPoints.Length; a++) {
            AnchorPair newPair = new AnchorPair(leftStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex], body.AnchorPoints[overallAnchorIndex].transform.localPosition.y);
            anchorPairs.Add(newPair);

            //strings
            AttachString(leftStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex]);

            overallAnchorIndex++;
        }

        // right stick
        for (int a = 0; a < rightStick.AnchorPoints.Length; a++) {
            AnchorPair newPair = new AnchorPair(rightStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex], body.AnchorPoints[overallAnchorIndex].transform.localPosition.y);
            anchorPairs.Add(newPair);

            //strings
            AttachString(rightStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex]);

            overallAnchorIndex++;
        }
    }

    private void AttachString(GameObject stickAnchor, GameObject puppetAnchor) {
        // create and position string
        GameObject newStringObj = Instantiate(stringPrefab);
        newStringObj.transform.SetParent(stickAnchor.transform, true);
        newStringObj.transform.position = stickAnchor.transform.position - new Vector3(0, .1f, 0);

        // attach the top segment to the stick
        PuppetString newString = newStringObj.GetComponent<PuppetString>();
        newString.TopJoint.connectedBody = stickAnchor.GetComponent<Rigidbody>();

        // attach the bottom segment to the puppet
        StartCoroutine(AttachBottom(newString, puppetAnchor));

        puppetParts.Add(newStringObj);
    }

    private IEnumerator AttachBottom(PuppetString newString, GameObject puppetAnchor) {
        // wait 1 frame to update
        yield return null;

        newString.BottomSegment.transform.position = puppetAnchor.transform.position;
        FixedJoint newJoint = newString.BottomSegment.AddComponent<FixedJoint>();
        newJoint.connectedBody = puppetAnchor.GetComponent<Rigidbody>();
    }

    private void ClearPuppet() {
        // destroy puppet gameObjects (hands, body, strings, etc.)
        foreach (GameObject part in puppetParts) {
            Destroy(part);
        }
        puppetParts.Clear();
        anchorPairs.Clear();

        // remove reference
        currPuppet = null;
    }
}
