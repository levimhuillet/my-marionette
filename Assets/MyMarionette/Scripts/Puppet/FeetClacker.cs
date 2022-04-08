using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetClacker : MonoBehaviour
{
    [SerializeField] private string clackSoundID;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "stage") {
            AudioManager.Instance.PlayOneShot(clackSoundID);
        }
    }
}
