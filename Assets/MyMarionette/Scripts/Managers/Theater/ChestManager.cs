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
        public string SequenceID;
        public string ChoiceTitle; // Protagonist, Antagonist, etc.
        // public Puppet[] AvailablePuppets // uncomment if we move away from pool implementation
        [HideInInspector]
        public Puppet SelectedPuppet;
    }

    #region Editor

    // Puppet Choices
    [SerializeField] private Puppet[] allPuppets;
    [SerializeField] private PuppetChoice[] allChoices;
    [SerializeField] private UIPuppetPicker puppetPicker;
    [SerializeField] private GameObject PuppetChest;

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<string, PuppetChoice> choiceMap;

    #endregion

    #region Puppet Picker

    private void StartPuppetPicker() {
        OpenChest();
        puppetPicker.OnChoiceConfirmed.AddListener(HandleChoiceConfirmed);
        puppetPicker.Open();
    }

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

    #endregion // Puppet Picker

    #region Events

    [HideInInspector]
    public UnityEvent OnChoiceCompleted;

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
    }

    private void Start() {
        TheaterManager.Instance.OnStateAdvanced.AddListener(HandleTheaterStateAdvanced);
    }

    #endregion // Unity Callbacks

    #region Get & Set

    public void SetAvailablePuppets(Puppet[] puppets) {
        availablePuppets = new List<Puppet>();
        foreach(Puppet puppet in puppets) {
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

    public Puppet[] GetPuppetOptions(PuppetChoice choice) {
        // TODO: return a subset of puppets based on choice

        return allPuppets;
    }

    public void SetPuppetChoice(PuppetChoice choice, Puppet chosenPuppet) {
        int choiceIndex = Array.IndexOf(allChoices, choice);
        allChoices[choiceIndex].SelectedPuppet = chosenPuppet;
    }

    #endregion

    #region Data Retrieval

    public PuppetChoice GetPuppetChoice(string sequenceID) {
        // initialize the map if it does not exist
        if (choiceMap == null) {
            choiceMap = new Dictionary<string, PuppetChoice>();
            foreach (PuppetChoice choice in allChoices) {
                choiceMap.Add(choice.SequenceID, choice);
            }
        }
        if (choiceMap.ContainsKey(sequenceID)) {
            return choiceMap[sequenceID];
        }
        else {
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

        // return control to Theater Manager
        OnChoiceCompleted.Invoke();
    }

    #endregion // Event Handlers
}
