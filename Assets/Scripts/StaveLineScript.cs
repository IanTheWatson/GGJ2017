using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaveLineScript : MonoBehaviour {

    public Sprite[] Frames;

    int currentFrame = 0;
    int delay = 0;
    const int maxDelay = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (delay <= 0)
        {
            var i = Random.Range(0, Frames.Length - 1);

            if (i == currentFrame)
            {
                i++;
            }

            if (i >= Frames.Length)
            {
                i = 0;
            }

            if (i == currentFrame)
            {
                i++;
            }

            GetComponent<SpriteRenderer>().sprite = Frames[i];
            currentFrame = i;
            delay = maxDelay;
        }
        else
        {
            delay--;
        }
        
	}
}
