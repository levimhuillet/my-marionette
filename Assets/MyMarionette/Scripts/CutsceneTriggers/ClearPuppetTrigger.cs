using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPuppetTrigger : MonoBehaviour
{
    private void OnEnable() {
        PuppetManager.Instance.ClearPuppet();
    }
}
