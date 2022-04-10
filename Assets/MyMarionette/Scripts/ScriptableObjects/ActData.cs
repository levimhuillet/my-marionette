using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActData", menuName = "MyMarionette/Data/ActData")]
public class ActData : ScriptableObject
{
    public int Num {
        get { return num; }
    }
    public string FirstSequenceID {
        get { return firstSequenceID; }
    }

    [SerializeField] private int num;
    [SerializeField] private string firstSequenceID;
}