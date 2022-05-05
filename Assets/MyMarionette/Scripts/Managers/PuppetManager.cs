using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuppetManager : MonoBehaviour
{
    public static PuppetManager Instance;

    [SerializeField] private Puppet startPuppet1, startPuppet2; // TEMPORARY SOLUTION
    [SerializeField] private Transform leftController, rightController; // transforms (positions) of the controller hands
    [SerializeField] XRInteractorLineVisual[] interactorLines;
    [SerializeField] private Transform stage;
    [SerializeField] private float puppetDistance; // distance between puppet and sticks
    [SerializeField] private GameObject stringPrefab;

    private List<LineRenderer> leftStickStrings, rightStickStrings;

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

    private struct PuppetParts
    {
        public SingleStick LeftStick;
        public SingleStick RightStick;
        public PuppetBody Body;

        public PuppetParts(SingleStick inLeft, SingleStick inRight, PuppetBody inBody) {
            LeftStick = inLeft;
            RightStick = inRight;
            Body = inBody;
        }
    }

    private Puppet currPuppet;
    private PuppetParts currParts; // tracks the puppet components in the scene for later deletion

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        anchorPairs = new List<AnchorPair>();

        leftStickStrings = new List<LineRenderer>();
        rightStickStrings = new List<LineRenderer>();

        //SetCurrPuppet(startPuppet1); // TEMPORARY SOLUTION

        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
        ChestManager.Instance.OnChoiceCompleted.AddListener(HandleChestChoiceCompleted);
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

            int overallAnchorIndex = 0;
            for (int s = 0; s < currParts.LeftStick.AnchorPoints.Length; s++) {
                leftStickStrings[s].SetPositions(
                new Vector3[] {
                    currParts.LeftStick.AnchorPoints[s].transform.position,
                    currParts.Body.AnchorPoints[overallAnchorIndex].transform.position }
                );
                overallAnchorIndex++;
            }
            for (int s = 0; s < currParts.RightStick.AnchorPoints.Length; s++) {
                rightStickStrings[s].SetPositions(
                new Vector3[] {
                    currParts.RightStick.AnchorPoints[s].transform.position,
                    currParts.Body.AnchorPoints[overallAnchorIndex].transform.position }
                );
                overallAnchorIndex++;
            }
        }
    }

    private void OnDisable() {
        if (currPuppet != null) {
            ClearPuppet();
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

        SingleStick rightStick = Instantiate(puppet.Sticks.RightStick.gameObject, rightController).GetComponent<SingleStick>();

        // Puppet body
        PuppetBody body = Instantiate(puppet.Body.gameObject).GetComponent<PuppetBody>();
        body.transform.position = new Vector3(stage.transform.position.x, stage.transform.position.y + 5, stage.transform.position.z);

        currParts = new PuppetParts(leftStick, rightStick, body);

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
            //AttachString(leftStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex]);
            LineRenderer anchorRender = leftStick.AnchorPoints[a].GetComponent<LineRenderer>();
            anchorRender.SetPositions(
                new Vector3[] {
                    leftStick.AnchorPoints[a].transform.position,
                    body.AnchorPoints[overallAnchorIndex].transform.position }
                );
            leftStickStrings.Add(anchorRender);

            overallAnchorIndex++;
        }

        // right stick
        for (int a = 0; a < rightStick.AnchorPoints.Length; a++) {
            AnchorPair newPair = new AnchorPair(rightStick.AnchorPoints[a], body.AnchorPoints[overallAnchorIndex], body.AnchorPoints[overallAnchorIndex].transform.localPosition.y);
            anchorPairs.Add(newPair);

            //strings
            LineRenderer anchorRender = rightStick.AnchorPoints[a].GetComponent<LineRenderer>();
            anchorRender.SetPositions(
                new Vector3[] {
                    rightStick.AnchorPoints[a].transform.position,
                    body.AnchorPoints[overallAnchorIndex].transform.position }
                );
            rightStickStrings.Add(anchorRender);

            overallAnchorIndex++;
        }
    }

    public Puppet GetCurrPuppet() {
        return currPuppet;
    }

    public void ClearPuppet() {
        // destroy puppet gameObjects (hands, body, strings, etc.)
        if (currParts.LeftStick != null) {
            Destroy(currParts.LeftStick.gameObject);
        }
        if (currParts.RightStick != null) {
            Destroy(currParts.RightStick.gameObject);
        }
        if (currParts.Body != null) {
            Destroy(currParts.Body.gameObject);
        }

        currParts = new PuppetParts(null, null, null);
        anchorPairs.Clear();

        leftStickStrings.Clear();
        rightStickStrings.Clear();

        // remove reference
        currPuppet = null;
    }

    #region Event handlers

    private void HandleTheaterStateAdvanced(TheaterManager.State state) {
        switch (state) {
            case TheaterManager.State.AdLib:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Puppet Manager] Enabling lines"); }
                SetLinesEnabled(true);
                break;
            default:
                break;
        }
    }

    private void HandleChestChoiceCompleted() {
        SetLinesEnabled(false);
    }

    #endregion // Event Handlers

    #region Helper Functions

    private void SetLinesEnabled(bool isEnabled) {
        foreach (XRInteractorLineVisual line in interactorLines) {
            line.enabled = isEnabled;
        }
    }

    #endregion // Helper Functions
}
