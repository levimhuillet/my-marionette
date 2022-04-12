using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPuppetPicker : MenuBase
{
    #region Editor

    [SerializeField] private GameObject puppetPickerButtonPrefab;

    #endregion // Editor

    #region Member Variables

    private List<GameObject> generatedButtons;

    #endregion

    #region Unity Callbacks

    private void OnEnable() {
        generatedButtons = new List<GameObject>();

        ChestManager.PuppetChoice[] allChoices = ChestManager.Instance.GetAllPuppetChoices();

        foreach (ChestManager.PuppetChoice choice in allChoices) {
            Puppet[] availablePuppets = ChestManager.Instance.GetPuppetOptions(choice);

            foreach(Puppet puppet in availablePuppets) {
                GameObject newButton = Instantiate(puppetPickerButtonPrefab, this.transform);
                newButton.GetComponent<PuppetPickerButton>().SetPuppet(puppet);
                generatedButtons.Add(newButton);
            }
        }
    }

    private void OnDisable() {
        foreach (GameObject button in generatedButtons) {
            Destroy(button);
        }

        generatedButtons.Clear();
    }

    #endregion // Unity Callbacks

    #region MenuBase

    public void Open() {
        base.OpenMenu();
    }

    private void Close() {
        base.CloseMenu();
    }

    #endregion // MenuBase
}
