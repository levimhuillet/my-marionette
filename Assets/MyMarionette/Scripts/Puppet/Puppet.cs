using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Puppet : MonoBehaviour
{
    public PuppetSticks Sticks;
    public GameObject Body;
    public GameObject RopeConnection;
    public GameObject[] AnchorPoints; // points on the puppet to which the sticks will be anchored

    public string Name;
}
