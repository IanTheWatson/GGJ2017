using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour {

    public BarrierInfo barrierInfo;

    public Sprite[] normalSprites;
    public Sprite[] damagedSprites;
    public Sprite[] brokenSprites;

    private SpriteRenderer sprite
    {
        get
        {
            return line.GetComponent<SpriteRenderer>();
        }
    }

    public StaveLineScript line;

    int state = -1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator RektNess()
    {
        var limit = 50;

        for (int i = 0; i <= limit; i++)
        {
            if (i >= limit)
            {
                Destroy(gameObject);
            }
            else
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1 - ((1 / 50f) * (i + 1)));
            }
            
            yield return null;
        }
    }

    void Rekt()
    {
        StartCoroutine(RektNess());
    }

    void FixedUpdate()
    {
        var percentStrength = barrierInfo.PercentStrength;
        if (barrierInfo.Destroyed)
        {
            Rekt();
        }
        else if (percentStrength <= 0.50f)
        {
            if (state != 3)
            {
                line.Frames = brokenSprites;
                state = 3;
            }            
        }
        else if (percentStrength <= 0.75f)
        {
            if (state != 2)
            {
                line.Frames = damagedSprites;
                state = 2;
            }
        }
        else
        {
            if (state != 1)
            {
                line.Frames = normalSprites;
                state = 1;
            }
        }
    }
}
