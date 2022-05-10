using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] GameObject mainColumn;
    [SerializeField] GameObject finalPanel;
    [SerializeField] float timeMod;
    [SerializeField] Transform[] images;
    [SerializeField] float imageTime;

    private static float MUSIC_LENGTH_SECS = 105; // how long the credits music lasts in seconds

    private float scrollSpeed;
    private float imageTimer;

    private void Start() {
        scrollSpeed = (-finalPanel.transform.position.y - 100) / (MUSIC_LENGTH_SECS - timeMod);

        imageTimer = imageTime;
    }

    // Update is called once per frame
    void Update()
    {
        while(finalPanel.transform.position.y < 100) {
            mainColumn.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

            // flip pictures
            imageTime -= Time.deltaTime;
            if (imageTime <= 0) {
                foreach(Transform image in images) {
                    image.localScale = new Vector3(-image.localScale.x, image.localScale.y, image.localScale.z);
                }

                imageTime = imageTimer;
            }

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
