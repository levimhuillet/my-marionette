using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuppetPickerButton : Button
{
    [SerializeField] private TMP_Text puppetName;
    //private Puppet puppet;

    #region Unity Callbacks

    override protected void OnEnable() {
        base.OnEnable();

        onClick.AddListener(HandleClick);
    }

    #endregion // Unity Callbacks

    #region Member Functions

    public void SetPuppet(Puppet puppet) {
        this.puppetName.text = puppet.Name;
        //this.puppet = puppet;
    }

    #endregion // Member Functions

    #region Button Handlers

    private void HandleClick() {

    }

    #endregion // Button Handlers
}
