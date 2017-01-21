using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : MonoBehaviour {

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            return GetComponent<SpriteRenderer>();
        }
    }

    public SpriteRenderer Glow
    {
        get
        {
            return transform.Find("Glow").GetComponent<SpriteRenderer>();
        }
    }

    public void GlowNote()
    {
        Glow.enabled = true;
        Glow.color = new Color(Glow.color.r, Glow.color.g, Glow.color.b, 1);
    }

    public void FadeNote()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
    }

    bool exploding = false;
    public void Explode()
    {
        if (!exploding)
        {
            exploding = true;
            var particles = transform.FindChild("Particles").GetComponent<ParticleSystem>();
            particles.startColor = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b);
            particles.Play();
            SpriteRenderer.color = new Color(1, 1, 1, 1);
        }        
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if (Glow.enabled)
        {
            Glow.color = new Color(Glow.color.r, Glow.color.g, Glow.color.b, Glow.color.a - 0.05f);
            if (Glow.color.a <= 0)
            {
                Glow.enabled = false;
            }
        }
    }
}
