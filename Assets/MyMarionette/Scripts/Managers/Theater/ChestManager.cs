using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    #region Editor

    [SerializeField] private Puppet[] allPuppets;

    #endregion // Editor

    #region Placeholder

    // Placeholder
    [SerializeField] private GameObject choiceUI;
    [SerializeField] private GameObject PuppetChest;
    [SerializeField] private Button choiceButton;
    // [SerializeField] private Animation OpenAnim;

    private void OpenChest() {
        Animation[] anims;
        anims = PuppetChest.GetComponents<Animation>();
        anims[0].Play(); // first animation is OpenChest
        // OpenAnim = anims[0];
        // OpenAnim.Play();
    }
    private void CloseChest() {
        Animation[] anims;
        anims = PuppetChest.GetComponents<Animation>();
        anims[1].Play(); // second animation is CloseChest
        // OpenAnim = anims[1];
        // OpenAnim.Play();
    }
    
    private void StartPuppetPicker() {
        OpenChest();
        choiceUI.SetActive(true);
        choiceButton.onClick.AddListener(HandleChoice);
    }

    #endregion // Placeholder

    #region Events

    [HideInInspector]
    public UnityEvent OnChoiceCompleted;

    #endregion // Events

    #region Member Variables 

    private List<Puppet> availablePuppets;

    #endregion // Member Varibales

    #region Unity Callbacks

    private void OnEnable() {
        OnChoiceCompleted = new UnityEvent();
    }

    private void Start() {
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
    }

    #endregion // Unity Callbacks

    #region Get & Set

    public void SetAvailablePuppets(List<Puppet> puppets) {
        availablePuppets = puppets;
    }

    #endregion

    #region Event Handlers

    private void HandleTheaterStateAdvanced(TheaterManager.State state) {
        switch (state) {
            case TheaterManager.State.AdLib:
                if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Chest Manager] Beginning " + state); }
                StartPuppetPicker();
                break;
            default:
                break;
        }
    }

    private void HandleChoice() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Chest Manager] Choice was made."); }

        // hide UI
        choiceUI.SetActive(false);
        choiceButton.onClick.RemoveAllListeners();

        // return control to Theater Manager
        OnChoiceCompleted.Invoke();
    }

    #endregion // Event Handlers
}
