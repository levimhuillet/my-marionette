using System.Collections;
using System.Collections.Generic;
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
            Puppet[] availablePuppets = ChestManager.Instance.GetPuppetOptions(choice);
            int numChoices = availablePuppets.Length;

            GameObject newTitle = Instantiate(choiceTitlePrefab, m_buttonHolder.transform);
            newTitle.GetComponent<Text>().text = choice.ChoiceTitle;

            float vertSpacing = rowIndex * -m_rowSpacing;
            newTitle.transform.position += new Vector3(0, vertSpacing, 0);

            foreach (Puppet puppet in availablePuppets) {
                GameObject newButton = Instantiate(puppetPickerButtonPrefab, m_buttonHolder.transform);
                newButton.GetComponent<PuppetPickerButton>().SetPuppet(puppet);
                generatedButtons.Add(newButton);

                // set spacing
                float horizSpacing = colIndex * m_colSpacing;
                newButton.gameObject.transform.position += new Vector3(horizSpacing, vertSpacing, 0);

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

    private void ConfirmChoice() {
        // return control to the chest manager
        OnChoiceConfirmed.Invoke();
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
