using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance;

    [Serializable]
    public struct PuppetChoice
    {
        public string ChoiceTitle; // Protagonist, Antagonist, etc.
        public Puppet[] AvailablePuppets;
        [HideInInspector]
        public Puppet SelectedPuppet;
    }

    #region Editor

    // Puppet Choices
    [SerializeField] private PuppetChoice[] allChoices;
    [SerializeField] private UIPuppetPicker puppetPicker;
    [SerializeField] private GameObject PuppetChest;

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<string, PuppetChoice> choiceMap;

    #endregion

    #region Puppet Picker

    // Chest Animations
    [SerializeField] private AnimationClip Open;
    [SerializeField] private AnimationClip Close;

    public void OpenChest() {
        if (PuppetChest == null) {
            PuppetChest = GameObject.Find("Puppet Chest");
        }
        Animation[] anims;
        anims = PuppetChest.GetComponents<Animation>();
        PuppetChest.GetComponent<Animation>().clip = Open;
        anims[0].Play(); // first animation is OpenChest
    }
    public void CloseChest() {
        if (PuppetChest == null) {
            PuppetChest = GameObject.Find("Puppet Chest");
        }
        Animation[] anims;
        anims = PuppetChest.GetComponents<Animation>();
        PuppetChest.GetComponent<Animation>().clip = Close;
        anims[0].Play(); // second animation is CloseChest
    }
    private void StartPuppetPicker() {
        OpenChest();
        puppetPicker.OnChoiceConfirmed.AddListener(HandleChoiceConfirmed);
        puppetPicker.Open();
    }

    public IEnumerator BeginPuppetSwap(string swapRole) {
        // halts play until player opens chest, clicks on new puppet
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Chest Manager] Swapping puppets..."); }

        ChestManager.PuppetChoice currChoice = ChestManager.Instance.GetPuppetChoice(swapRole);
        PuppetManager.Instance.SetCurrPuppet(currChoice.SelectedPuppet);

        yield return null;

        OnSwapCompleted.Invoke();
    }

    #endregion // Puppet Picker

    #region Events

    [HideInInspector]
    public UnityEvent OnChoiceCompleted;
    [HideInInspector]
    public static UnityEvent OnSwapCompleted;

    #endregion // Events

    #region Member Variables 

    private List<Puppet> availablePuppets;

    #endregion // Member Varibales

    #region Unity Callbacks

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        OnChoiceCompleted = new UnityEvent();
        OnSwapCompleted = new UnityEvent();
    }

    private void Start() {
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
    }

    #endregion // Unity Callbacks

    #region Get & Set

    public void SetAvailablePuppets(Puppet[] puppets) {
        availablePuppets = new List<Puppet>();
        foreach (Puppet puppet in puppets) {
            availablePuppets.Add(puppet);
        }
    }

    public void ReleaseAvailablePuppet(Puppet puppet) {
        if (availablePuppets.Contains(puppet)) {
            Debug.Log("[Chest Manager] ERROR: tried to release a puppet that is already available");
            return;
        }

        availablePuppets.Add(puppet);
    }

    public Puppet ReserveAvailablePuppet(Puppet puppet) {
        if (!availablePuppets.Contains(puppet)) {
            Debug.Log("[Chest Manager] ERROR: tried to reserve a puppet that is not available");
            return null;
        }

        availablePuppets.Remove(puppet);

        return puppet;
    }

    public PuppetChoice[] GetAllPuppetChoices() {
        return allChoices;
    }

    public void SetPuppetChoice(PuppetChoice choice, Puppet chosenPuppet) {
        int foundIndex = -1;
        for (int i = 0; i < allChoices.Length; i++) {
            if (allChoices[i].ChoiceTitle == choice.ChoiceTitle) {
                foundIndex = i;
            }
        }

        if (foundIndex == -1) {
            Debug.Log("ERROR: choice not found: " + choice.ChoiceTitle);
            return;
        }

        allChoices[foundIndex].SelectedPuppet = chosenPuppet;
    }

    #endregion

    #region Data Retrieval

    public PuppetChoice GetPuppetChoice(string swapRole) {
        // initialize the map if it does not exist
        if (choiceMap == null) {
            choiceMap = new Dictionary<string, PuppetChoice>();
            foreach (PuppetChoice choice in allChoices) {
                choiceMap.Add(choice.ChoiceTitle, choice);
            }
        }
        if (choiceMap.ContainsKey(swapRole)) {
            return choiceMap[swapRole];
        }
        else {
            Debug.Log("no key for " + swapRole);
            return new PuppetChoice();
        }
    }

    #endregion // Data Retrieval

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

    private void HandleChoiceConfirmed() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Chest Manager] Choice was made."); }

        // close the puppet picker UI
        puppetPicker.Close();
        puppetPicker.OnChoiceConfirmed.RemoveAllListeners();
        CloseChest();

        // return control to Theater Manager
        OnChoiceCompleted.Invoke();
    }

    #endregion // Event Handlers
}
