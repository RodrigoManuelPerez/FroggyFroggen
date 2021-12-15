using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    float actualTime = 0;
    float timeToFish = 0;

    [Range(2.0f, 4.0f)]
    public float minRange;

    [Range(4.0f, 7.0f)]
    public float maxRange;
    

    public Animator anim;

    private void Start()
    {
        timeToFish = Random.Range(minRange, maxRange);
    }

    // Update is called once per frame
    void Update()
    {
        actualTime += Time.deltaTime;

        if(actualTime >= timeToFish)
        {
            actualTime = 0;
            timeToFish = Random.Range(minRange, maxRange);
            anim.SetTrigger("FishIn");
        }
    }
}
