using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SequenceManager;

[CreateAssetMenu(fileName = "NewSequenceData", menuName = "MyMarionette/Data/SequenceData")]
public class SequenceData : ScriptableObject
{
    
    public enum LightAction
    {
        None,
        TurnOn,
        TurnOff,
        ChangeColor
    }

    // TODO: make this more intuitive and logical
    [Serializable]
    public struct SequenceAction
    {
        public LightAction LightActionType; // whether lights turn on or off
        public int LightColorIndex; // if light action is change color, this specifies color
    }

    public List<SequenceAction> StartActions {
        get { return startActions; }
    }
    public string ID {
        get { return id; }
    }
    public string[] NarrationDataIDs {
        get { return narrationDataIDs; }
    }
    public List<Branch> Branches {
        get { return branches; }
    }
    public List<SequenceAction> EndActions {
        get { return endActions; }
    }

    [SerializeField] private List<SequenceAction> startActions;
    [SerializeField] private string id;
    [SerializeField] private string[] narrationDataIDs;
    [SerializeField] private List<Branch> branches;
    [SerializeField] private List<SequenceAction> endActions;
}
