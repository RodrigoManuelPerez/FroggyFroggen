using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public int menuOptions = 4;

    private int selected = 0;

    private bool canMove = false;

    public Fade darkFade;

    public SpriteRenderer difficultyRender;
    public Sprite hardMode;
    public Sprite normalMode;
    public Sprite easyMode;

    public GameObject Selector;
    public Vector3[] SelectorPositions;

    public Animator bigFrog;
    public Animator smallFrog;

    private void Start()
    {
        Invoke("StartGame", 0.5f);

        switch (DifficultyManager.instance.GetDifficulty())
        {
            case Difficulty.easy:
                difficultyRender.sprite = easyMode;                
                break;
            case Difficulty.hard:
                difficultyRender.sprite = hardMode;
                break;
        }
    }

    private void StartGame()
    {
        bigFrog.SetTrigger("PlayIn");
        StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuOver));
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {            
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                StartCoroutine(MoveSelector(false));

            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                StartCoroutine(MoveSelector(true));

            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
                ChooseSelection();            
        }
    }

    private IEnumerator MoveSelector(bool up)
    {
        canMove = false;

        yield return new WaitForSeconds(0.15f);      

        switch (selected)
        {
            case 0:
                bigFrog.SetTrigger("PlayOut");
                break;
            case 1:
                bigFrog.SetTrigger("ControlsOut");
                break;
            case 2:
                smallFrog.SetTrigger("ModeOut");
                break;
            case 3:
                smallFrog.SetTrigger("ExitOut");
                break;
        }

        if (up)
        {
            selected++;
            if (selected >= menuOptions)
                selected = 0;
        }
        else
        {
            selected--;
            if (selected < 0)
                selected = menuOptions - 1;
        }

        switch (selected)
        {
            case 0:
                bigFrog.SetTrigger("PlayIn");
                break;
            case 1:
                bigFrog.SetTrigger("ControlsIn");
                break;
            case 2:
                smallFrog.SetTrigger("ModeIn");
                break;
            case 3:
                smallFrog.SetTrigger("ExitIn");
                break;
        }

        Selector.transform.position = SelectorPositions[selected];

        StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuOver));

        yield return new WaitForSeconds(0.15f);

        canMove = true;
    }

    private void ChooseSelection()
    {
        canMove = false;

        switch (selected)
        {
            case 0:
                Play();
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuSelect));
                break;
            case 1:
                Controls();
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuSelect));
                break;
            case 2:
                switch (DifficultyManager.instance.RotateDifficultty())
                {
                    case Difficulty.easy:
                        difficultyRender.sprite = easyMode;
                        break;
                    case Difficulty.hard:
                        difficultyRender.sprite = hardMode;
                        break;
                }
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuSelect));
                canMove = true;

                break;
            case 3:
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.exit));
                StartCoroutine(Exit());
                break;
        }
    }

    private void Play()
    {
        StartCoroutine(SceneLoader.instance.LoadScene(3));
    }

    private void Controls()
    {
        StartCoroutine(SceneLoader.instance.LoadScene(4));
    }

    private IEnumerator Exit()
    {
        darkFade.FadeIn();

        while (darkFade.GetAlpha() < 1.0f)
        {
            yield return 0;
        }

        yield return new WaitForSeconds(1.0f);

        Application.Quit();
    }
}
