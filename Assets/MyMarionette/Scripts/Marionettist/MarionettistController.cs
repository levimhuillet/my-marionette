using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyMarionette {
    public class MarionettistController : MonoBehaviour {

        #region Editor

        [SerializeField]
        private GameObject m_sticks; // Marionette sticks

        private GameObject m_currPuppet;

        // RTemporary for mockup
        [SerializeField]
        private GameObject m_puppetPrefab;
        [SerializeField]
        private GameObject m_stage;

        #endregion

        #region Unity Callbacks

        private void Awake() {
            
        }

        private void Start() {
            // sticks start hidden
            m_sticks.gameObject.SetActive(false);

            //ActivatePuppet();
        }

        private void OnDestroy() {
            StashPuppet();
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

        #endregion

        #region InputSystem

        public void OnToggleSticks() {
            HandleToggleSticks();
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

        #endregion
    }
}
