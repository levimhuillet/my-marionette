using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestCloseTrigger : MonoBehaviour
{
    private void OnEnable() {
        ChestManager.Instance.CloseChest();
    }
}
