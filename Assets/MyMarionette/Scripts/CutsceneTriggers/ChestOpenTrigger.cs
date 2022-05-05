using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpenTrigger : MonoBehaviour
{
    private void OnEnable() {
        ChestManager.Instance.OpenChest();
    }
}
