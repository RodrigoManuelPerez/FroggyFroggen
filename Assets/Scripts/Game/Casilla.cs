using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour
{
    public Box box;
    public SpriteRenderer rend;
    public Sprite[] sprites;

    public Animator anim;
    public RuntimeAnimatorController Cross;
    public RuntimeAnimatorController Piercer;
    public RuntimeAnimatorController Fly;

    private bool change = false;
    public float timeToChange = 5.0f;
    private float actualTime = 0f;

    public float timeToFlickOn = 0.3f;
    public float timeToFlickOff = 0.1f;
    private float actualFlickTime = 0f;
    public int flickers = 5;
    private int flickContAct = 0;

    public bool paused = false;

    public Casilla(Box b = Box.empty)
    {
        box = b;        
    }

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        SetBox(box);
    }

    private void Update()
    {
        if (change && !paused)
        {
            actualTime += Time.deltaTime;

            if (actualTime >= timeToChange)
            {
                if (flickContAct < flickers) 
                {
                    actualFlickTime += Time.deltaTime;
                    
                    if(rend.enabled && actualFlickTime >= timeToFlickOn)
                    {
                        flickContAct++;
                        actualFlickTime = 0;
                        rend.enabled = !rend.enabled;
                    }
                    else if (!rend.enabled && actualFlickTime >= timeToFlickOff)
                    {
                        flickContAct++;
                        actualFlickTime = 0;
                        rend.enabled = !rend.enabled;
                    }
                }
                else if(flickContAct == flickers)
                {
                    flickContAct = 0;
                    actualFlickTime = 0;
                    change = false;
                    Box aux = box;
                    SetBox(Box.empty);
                    GameManager.instance.SpawnBuff(aux);
                }
            }
        }
    }


    public void SetBox(Box b, int piercer = 0)
    {
        box = b;

        anim.runtimeAnimatorController = null;
        change = false;

        rend.enabled = true;

        switch (b)
        {
            case Box.empty:
                rend.sprite = null;
                break;
            case Box.locked:
                rend.sprite = null;
                break;
            case Box.cursor:    // Hay que ajustar para mostrar el pierce
                rend.sprite = sprites[0];
                break;
            case Box.upLeft:
                rend.sprite = sprites[1];
                break;
            case Box.downLeft:
                rend.sprite = sprites[2];
                break;
            case Box.upRight:
                rend.sprite = sprites[3];
                break;
            case Box.downRight:
                rend.sprite = sprites[4];
                break;
            case Box.vertical:
                rend.sprite = sprites[5];
                break;
            case Box.horizontal:
                rend.sprite = sprites[6];
                break;
            case Box.coin:
                change = true;                
                anim.runtimeAnimatorController = Fly;                
                break;
            case Box.cross:
                change = true;
                anim.runtimeAnimatorController = Cross;
                break;
            case Box.piercer:
                change = true;
                anim.runtimeAnimatorController = Piercer;
                break;
            case Box.piercerCursor:                
                rend.sprite = sprites[7 + piercer];
                break;
            
        }
    }
}
