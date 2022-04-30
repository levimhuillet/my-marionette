using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPuppetPicker : MenuBase
{
    #region Editor

    [SerializeField] private GameObject choiceTitlePrefab;
    [SerializeField] private GameObject puppetPickerButtonPrefab;
    [SerializeField] private GameObject m_buttonHolder; // holds the buttons generated at run-time

    [SerializeField] private float m_colSpacing; // horizontal spacing between buttons
    [SerializeField] private float m_rowSpacing; // vertical spacing between buttons

    [SerializeField] private Button m_confirmButton;

    #endregion // Editor

    #region Member Variables

    private List<GameObject> generatedButtons;
    private List<GameObject> generatedTitles;

    #endregion

    #region Events

    [HideInInspector]
    public UnityEvent OnChoiceConfirmed;

    #endregion // Events

    #region Unity Callbacks

    private void OnEnable() {
        m_confirmButton.onClick.AddListener(ConfirmChoice);
        generatedButtons = new List<GameObject>();

        ChestManager.PuppetChoice[] allChoices = ChestManager.Instance.GetAllPuppetChoices();

        int colIndex = 0;
        int rowIndex = 0;
        foreach (ChestManager.PuppetChoice choice in allChoices) {
            Puppet[] availablePuppets = choice.AvailablePuppets; //ChestManager.Instance.GetPuppetOptions(choice);
            int numChoices = availablePuppets.Length;

            // create a new title for the choice category
            GameObject newTitle = Instantiate(choiceTitlePrefab, m_buttonHolder.transform);
            newTitle.GetComponent<TMP_Text>().text = choice.ChoiceTitle;

            // recaulculate row spacing
            float vertSpacing = rowIndex * -m_rowSpacing;
            newTitle.transform.localPosition += new Vector3(0, vertSpacing, 0);

            // generate a new choice button for each puppet you can choose from
            foreach (Puppet puppet in availablePuppets) {
                // set up puppet button
                GameObject newButtonObj = Instantiate(puppetPickerButtonPrefab, m_buttonHolder.transform);
                generatedButtons.Add(newButtonObj);

                PuppetPickerButton newButton = newButtonObj.GetComponent<PuppetPickerButton>();
                newButton.SetPuppet(puppet);

                // Set button actions
                // newButton.onClick.AddListener(delegate { UpdateSelectColor(newButton); }); // incomplete
                newButton.onClick.AddListener(delegate { ChoosePuppet(choice, puppet); });

                // set spacing
                float horizSpacing = colIndex * m_colSpacing;
                newButton.gameObject.transform.localPosition += new Vector3(horizSpacing, vertSpacing, -.01f);

                // move to next column
                colIndex++;
            }

            // move to next row
            rowIndex++;

            // reset column
            colIndex = 0;
        }
    }

    private void OnDisable() {
        foreach (GameObject button in generatedButtons) {
            Destroy(button);
        }

        generatedButtons.Clear();
    }

    #endregion // Unity Callbacks

    #region Member Functions

    private void ChoosePuppet(ChestManager.PuppetChoice choice, Puppet chosenPuppet) {
        ChestManager.Instance.SetPuppetChoice(choice, chosenPuppet);
    }

    void UpdateSelectColor(Button b) {
        Image bImage = b.GetComponent<Image>();

        // TODO: specify and load these colors externally
        if (bImage.color == Color.green) {
            bImage.color = Color.white;
        }
        else {
            bImage.color = Color.green;
        }
    }

    private void ConfirmChoice() {
        bool choosingComplete = true;

        ChestManager.PuppetChoice[] allChoices = ChestManager.Instance.GetAllPuppetChoices();

        // check if any roles still need to be chosen
        foreach (ChestManager.PuppetChoice choice in allChoices) {
            if (choice.SelectedPuppet == null) {
                choosingComplete = false;
            }
        }

        if (choosingComplete) {
            // return control to the chest manager
            OnChoiceConfirmed.Invoke();
        }
        else {
            if (TheaterManager.Instance.DEBUGGING) { Debug.Log("[UI Puppet Picker] Not all roles have been chosen!"); }
        }
    }

    #endregion // Member Functions

    #region MenuBase

    public void Open() {
        base.OpenMenu();
    }

    public void Close() {
        base.CloseMenu();
    }

    #endregion // MenuBase
}
