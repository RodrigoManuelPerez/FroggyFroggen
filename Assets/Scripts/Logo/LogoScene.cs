using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    public Fade dark;
    public float nextSceneTime = 7.5f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DarkOut", 1.0f);
        Invoke("CreditsScene", nextSceneTime);
    }

    private void DarkOut()
    {
        dark.FadeOut();
        StartCoroutine(MusicManager.instance.PlaySound(Sounds.logo));
    }

    private void CreditsScene()
    {
        StartCoroutine(SceneLoader.instance.LoadScene(1,1.0f, true));
    }
}
