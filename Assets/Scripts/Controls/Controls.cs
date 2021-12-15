using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public float timeToEnable = 3.0f;
    private bool canBack = false;

    private void Start()
    {
        Invoke("enableBack", timeToEnable);
    }

    private void Update()
    {
        if(canBack && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape)))
        {
            menuScene();
        }
    }

    private void menuScene()
    {
        StartCoroutine(SceneLoader.instance.LoadScene(2, 0.5f));
    }

    private void enableBack()
    {
        canBack = true;
}
}
