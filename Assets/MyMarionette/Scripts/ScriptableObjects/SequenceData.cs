using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SequenceManager;

[CreateAssetMenu(fileName = "NewSequenceData", menuName = "MyMarionette/Data/SequenceData")]
public class SequenceData : ScriptableObject
{
    public string ID {
        get { return id; }
    }
    public string[] NarrationClipIDs {
        get { return narrationClipIDs; }
    }
    public List<Branch> Branches {
        get { return branches; }
    }

    [SerializeField] private string id;
    [SerializeField] private string[] narrationClipIDs;
    [SerializeField] private List<Branch> branches;
}
