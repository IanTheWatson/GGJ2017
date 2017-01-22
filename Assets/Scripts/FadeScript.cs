using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{

    public float Speed;
    bool directionUp = false;

    public SpriteRenderer sprite
    {
        get
        {
            return GetComponent<SpriteRenderer>();
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        var alpha = sprite.color.a + (Speed * (directionUp ? 1 : -1));
        if (directionUp && alpha > 1f)
        {
            directionUp = false;
            alpha = 1f;
        }
        else if (!directionUp && alpha < 0f)
        {
            directionUp = true;
            alpha = 0f;
        }

        sprite.color = new Color(1, 1, 1, alpha);
    }
}
