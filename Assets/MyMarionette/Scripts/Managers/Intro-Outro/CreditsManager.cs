using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] GameObject mainColumn;
    [SerializeField] GameObject finalPanel;
    [SerializeField] float timeMod;

    private static float MUSIC_LENGTH_SECS = 105; // how long the credits music lasts in seconds

    private float scrollSpeed;

    private void Start() {
        scrollSpeed = (-finalPanel.transform.position.y - 100) / (MUSIC_LENGTH_SECS - timeMod);
    }

    // Update is called once per frame
    void Update()
    {
        while(finalPanel.transform.position.y < 100) {
            mainColumn.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            return;
        }

        // end game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
