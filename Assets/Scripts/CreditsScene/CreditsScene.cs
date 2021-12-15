using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScene : MonoBehaviour
{
    public float timeToMenu = 3.0f;

    void Start()
    {
        Invoke("menuScene", timeToMenu);
        Invoke("startSound", 0.25f);
    }

    private void startSound()
    {
        StartCoroutine(MusicManager.instance.PlaySound(Sounds.credits));
    }

    private void menuScene()
    {
        StartCoroutine(SceneLoader.instance.LoadScene(2, 1.0f));
    }
}
