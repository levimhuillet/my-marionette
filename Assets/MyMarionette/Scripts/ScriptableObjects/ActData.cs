using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EffectsManager;

[CreateAssetMenu(fileName = "NewActData", menuName = "MyMarionette/Data/ActData")]
public class ActData : ScriptableObject
{
    public List<EffectAction> StartActions {
        get { return startActions; }
    }
    public int Num {
        get { return num; }
    }
    public string FirstSequenceID {
        get { return firstSequenceID; }
    }
    public List<EffectAction> EndActions {
        get { return endActions; }
    }

    [SerializeField] private List<EffectAction> startActions;
    [SerializeField] private int num;
    [SerializeField] private string firstSequenceID;
    [SerializeField] private List<EffectAction> endActions;
}