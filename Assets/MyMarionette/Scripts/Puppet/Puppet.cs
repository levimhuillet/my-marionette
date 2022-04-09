using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Puppet : MonoBehaviour
{
    public GameObject LeftStick;
    public GameObject RightStick;
    public GameObject Body;
    public GameObject RopeConnection;

    // void start() {
    //     float Horizontal = Input.GetAxis("Horizontal");
    //     float Vertical = Input.GetAxis("Vertical");

    //     RopeConnection.GetComponent<Rigidbody>().AddForce(transform.right * Horizontal, ForceMode.Acceleration);

    //     RopeConnection.GetComponent<Rigidbody>().AddForce(transform.forward * Vertical, ForceMode.Acceleration);
    // }
}
