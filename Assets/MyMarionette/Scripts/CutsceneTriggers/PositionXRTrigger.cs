using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionXRTrigger : MonoBehaviour
{
    [SerializeField] private Transform xrOrigin, newPosTransform;

    private void OnEnable() {
        xrOrigin.position = newPosTransform.position;
    }
}
