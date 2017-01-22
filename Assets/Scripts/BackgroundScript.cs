using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour {

    public bool DirectionRight = false;
    public float speed = 0.01f;

    public Sprite[] Good;
    public Sprite[] Bad;
    public Sprite[] Streak;

    public int frame = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        var newPosition = (transform.localPosition.x + (speed * (DirectionRight ? 1 : -1))) % 15f;
        Debug.Log(newPosition);
        transform.localPosition = new Vector3(newPosition, transform.localPosition.y, 0);        
    }

    public void Pulse(bool damaged, bool harmony)
    {
        Sprite[] array;
        if (damaged)
        {
            array = Bad;
        }
        else if (harmony)
        {
            array = Streak;
        }
        else
        {
            array = Good;
        }

        frame++;
        if (frame >= array.Length)
        {
            frame = 0;
        }

        GetComponent<SpriteRenderer>().sprite = array[frame];
    }
}
