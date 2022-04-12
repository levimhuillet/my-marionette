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

    #endregion // Editor

    #region Member Variables

    // Maps
    private Dictionary<string, PuppetChoice> choiceMap;

    #endregion

    #region Puppet Picker

    private void StartPuppetPicker() {
        puppetPicker.Open();
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
            throw new KeyNotFoundException(string.Format("No Puppet Choice " +
                "with sequenceID `{0}' is in the database", sequenceID
            ));
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

    private void HandleChoice() {
        if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[Chest Manager] Choice was made."); }

        // return control to Theater Manager
        OnChoiceCompleted.Invoke();
    }

    #endregion // Event Handlers
}
