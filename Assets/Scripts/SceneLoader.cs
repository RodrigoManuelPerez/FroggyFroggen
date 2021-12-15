using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Fade transitionFade;

    public Color Dark;
    public Color Light;

    public static SceneLoader instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator LoadScene(int scene, float delay = 0.0f, bool dark = false)
    {
        if (dark)
            transitionFade.SetColor(Dark);
        else
            transitionFade.SetColor(Light);

        transitionFade.FadeIn();

        while (transitionFade.GetAlpha() < 1.0f)
        {
            yield return 0;
        }

        yield return new WaitForSeconds(0.25f);

        if (delay >= 0)
            yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(scene);

        transitionFade.FadeOut();

        while (transitionFade.GetAlpha() > 0.0f)
        {
            yield return 0;
        }
    }
}
