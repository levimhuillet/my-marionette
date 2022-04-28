using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EffectsManager;
using static SequenceManager;

[CreateAssetMenu(fileName = "NewSequenceData", menuName = "MyMarionette/Data/SequenceData")]
public class SequenceData : ScriptableObject
{
    public List<EffectAction> StartActions {
        get { return startActions; }
    }
    public string ID {
        get { return id; }
    }
    public string[] NarrationDataIDs {
        get { return narrationDataIDs; }
    }
    public bool TriggersGameplay {
        get { return triggersGameplay; }
    }
    public bool PuppetSwap {
        get { return puppetSwap; }
    }
    public List<Branch> Branches {
        get { return branches; }
    }
    public List<EffectAction> EndActions {
        get { return endActions; }
    }

    [SerializeField] private List<EffectAction> startActions;
    [SerializeField] private string id;
    [SerializeField] private string[] narrationDataIDs;
    [SerializeField] private bool triggersGameplay;
    [SerializeField] private bool puppetSwap;
    [SerializeField] private List<Branch> branches;
    [SerializeField] private List<EffectAction> endActions;
}
