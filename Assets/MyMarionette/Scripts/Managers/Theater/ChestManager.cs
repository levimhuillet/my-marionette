using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestManager : MonoBehaviour
{
    #region Editor

    [SerializeField] private Puppet[] allPuppets;

    #endregion // Editor

    #region Events

    public UnityEvent OnChoiceMade;

    #endregion // Events

    #region Member Variables 

    private List<Puppet> availablePuppets;

    #endregion // Member Varibales

    #region Unity Callbacks

    private void OnEnable() {
        OnChoiceMade = new UnityEvent();
    }

    #endregion // Unity Callbacks

    #region Get & Set

    public void SetAvailablePuppets(List<Puppet> puppets) {
        availablePuppets = puppets;
    }

    #endregion
}
