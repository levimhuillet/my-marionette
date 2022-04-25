using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNarrationData", menuName = "MyMarionette/Data/NarrationData")]
public class NarrationAudioData : AudioData
{
    public string Subtitle {
        get { return subtitle; }
        set { subtitle = value; }
    }
    public bool RequiresAction {
        get { return requiresAction; }
    }

    [SerializeField] private string subtitle;
    [SerializeField] private bool requiresAction;
}
