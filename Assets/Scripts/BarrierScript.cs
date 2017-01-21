using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour {

    public BarrierInfo barrierInfo;

    public Sprite[] normalSprites;
    public Sprite[] damagedSprites;
    public Sprite[] brokenSprites;

    public StaveLineScript line;

    int state = -1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        var percentStrength = barrierInfo.PercentStrength;
        if (percentStrength < 0.33f)
        {
            if (state != 3)
            {
                Debug.Log(percentStrength);
                line.Frames = brokenSprites;
                state = 3;
            }            
        }
        else if (percentStrength < 0.67f)
        {
            if (state != 2)
            {
                Debug.Log(percentStrength);
                line.Frames = damagedSprites;
                state = 2;
            }
        }
        else
        {
            if (state != 1)
            {
                Debug.Log(percentStrength);
                line.Frames = normalSprites;
                state = 1;
            }
        }
    }
}
