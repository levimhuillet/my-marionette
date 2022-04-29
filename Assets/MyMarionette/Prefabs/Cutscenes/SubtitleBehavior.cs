using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleBehavior : PlayableBehaviour
{
    public string subtitleText;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {

        TMPro.TextMeshProUGUI text = playerData as TMPro.TextMeshProUGUI;

        text.text = subtitleText;
        text.color = new Color(1, 1, 1, info.weight); // fade text out
    }
    
}
