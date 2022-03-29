using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyMarionette {
    public class MarionettistController : MonoBehaviour {

        #region Editor

        [SerializeField]
        private MarionetteSticks m_sticks; // Marionette sticks
        [SerializeField]
        private float m_rotationSpeed;

        // RTemporary for mockup
        [SerializeField]
        private GameObject m_puppetPrefab;
        [SerializeField]
        private GameObject m_stage;

        #endregion // Editor

        #region Member Variables

        private GameObject m_currPuppet;

        #endregion // Member Variables

        #region Rotate Input

        struct StickRotation
        {
            public bool RotateLeft;
            public bool RotateRight;
        }
        struct SticksRotateInput
        {
            public StickRotation Left;
            public StickRotation Right;
        }

        private SticksRotateInput m_sticksRotateInput;

        #endregion // Rotate Input

        #region Unity Callbacks
        private void Start() {
            // sticks start hidden
            m_sticks.gameObject.SetActive(false);

            //ActivatePuppet();
        }

        private void OnDestroy() {
            StashPuppet();
        }
        private void Update() {
            ApplyInputs();
        }

        #endregion

        #region Member Functions

        private void ActivatePuppet() {
            if (m_currPuppet != null) {
                Debug.Log("Error: Attempting to activate a new puppet when previous puppet still assigned");
                return;
            }

            // pull out marionette sticks
            m_sticks.gameObject.SetActive(true);

            // instantiate the new puppet
            m_currPuppet = Instantiate(m_puppetPrefab);

            // place the puppet on the stage
            m_currPuppet.transform.position = m_stage.gameObject.transform.position + new Vector3(0f, 2.8f, -2.4f);
        }

        private void StashPuppet() {
            if (m_currPuppet == null) {
                Debug.Log("Error: no puppet to stash");
                return;
            }

            // destroy the physical representation of the puppet
            Destroy(m_currPuppet.gameObject);

            // remove the reference to the puppet
            m_currPuppet = null;

            // put away sticks
            m_sticks.gameObject.SetActive(false);
        }
        void ApplyInputs() {
            if (m_sticksRotateInput.Left.RotateLeft) {
                RotateStick(m_sticks.Left, "left");
            }
            if (m_sticksRotateInput.Left.RotateRight) {
                RotateStick(m_sticks.Left, "right");
            }
            if (m_sticksRotateInput.Right.RotateLeft) {
                RotateStick(m_sticks.Right, "left");
            }
            if (m_sticksRotateInput.Right.RotateRight) {
                RotateStick(m_sticks.Right, "right");
            }
        }
        void RotateStick(GameObject stick, string dir) {
            float rotateVal = m_rotationSpeed * Time.deltaTime;
            if (dir == "right") {
                rotateVal *= -1;
            }

            stick.transform.RotateAround(stick.transform.position, stick.transform.forward, rotateVal);
        }

        #endregion

        #region InputSystem

        public void OnToggleSticks() {
            HandleToggleSticks();
        }

        public void OnRotateLStickLeft(InputValue val) {
            HandleRotateLStickLeft(val);
        }

        public void OnRotateLStickRight(InputValue val) {
            HandleRotateLStickRight(val);
        }

        public void OnRotateRStickLeft(InputValue val) {
            HandleRotateRStickLeft(val);
        }

        public void OnRotateRStickRight(InputValue val) {
            HandleRotateRStickRight(val);
        }

        #endregion

        #region InputSystemHandlers

        private void HandleToggleSticks() {
            if (m_currPuppet == null) {
                ActivatePuppet();
            }
            else {
                StashPuppet();
            }
        }
        private void HandleRotateLStickLeft(InputValue val) {
            m_sticksRotateInput.Left.RotateLeft = val.isPressed;
        }
        private void HandleRotateLStickRight(InputValue val) {
            m_sticksRotateInput.Left.RotateRight = val.isPressed;
        }
        private void HandleRotateRStickLeft(InputValue val) {
            m_sticksRotateInput.Right.RotateLeft = val.isPressed;
        }
        private void HandleRotateRStickRight(InputValue val) {
            m_sticksRotateInput.Right.RotateRight = val.isPressed;
        }

        #endregion
    }
}
