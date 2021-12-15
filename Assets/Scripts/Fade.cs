using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float speed = 1.0f;
    SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color c)
    {
        rend.color = c;
    }

    public float GetAlpha()
    {
        return rend.color.a;
    }

    public void FadeIn()
    {
        StartCoroutine(fadeIn());
    }

    private IEnumerator fadeIn()
    {
        Color c;
        c = rend.color;

        while(c.a < 1.0f)
        {
            c.a += Time.deltaTime * speed;
            rend.color = c;

            yield return 0;
        }
    }

    public void FadeOut()
    {
        StartCoroutine(fadeOut());
    }

    private IEnumerator fadeOut()
    {
        Color c;
        c = rend.color;

        while (c.a > 0.0f)
        {
            c.a -= Time.deltaTime * speed;
            rend.color = c;

            yield return 0;
        }
    }

}
