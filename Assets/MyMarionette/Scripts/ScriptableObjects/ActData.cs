using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActData", menuName = "MyMarionette/Data/ActData")]
public class ActData : ScriptableObject
{
    public int Num {
        get { return num; }
    }
    public SequenceData[] Sequences {
        get { return sequences; }
    }

    [SerializeField] private int num;
    [SerializeField] private SequenceData[] sequences;
}