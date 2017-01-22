using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public MusicMasterScript master;
    public Camera playerCamera;
    public ParticleSystem playerParticle;
    public ParticleSystem victoryParticles;
    public GameObject stave;
    public SpriteRenderer motionBlur;
    public GameObject background;

    public RuntimeAnimatorController normalAnimation;
    public RuntimeAnimatorController damagedAnimation;

    bool _moving = false;

    public bool damaged = false;

    bool dead = false;
    bool win = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Win()
    {
        victoryParticles.Play();
        win = true;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (!dead)
        {
            if (!win)
            {
                var movementDistance = master.MovementSpeedPerTick;
                transform.Translate(Vector3.right * movementDistance);
                playerCamera.transform.position = new Vector3(transform.position.x + 3, playerCamera.transform.position.y, playerCamera.transform.position.z);
                playerParticle.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                background.transform.position = new Vector3(transform.position.x + 3, 0, -2);
                stave.transform.position = new Vector3(transform.position.x + 3, stave.transform.position.y, stave.transform.position.z);
            }
            
            HandleInputs();

            master.CurrentPlayerRow = Mathf.RoundToInt(transform.position.y * 2);
        }
        else
        {

        }
        
    }

    public void ReactToNote(Color noteColour)
    {
        StartCoroutine(ReactingToNote(noteColour));
    }

    IEnumerator ReactingToNote(Color newColour)
    {
        var time = 20;
        var minScale = 0.6f;
        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1, 1, 1, 1), newColour, 1f - (1f / time) * (i + 1f));
                transform.localScale = new Vector3(minScale + ((1f - minScale) * (1f / time) * i), minScale + ((1f - minScale) * (1f / time) * i), 1);
            }

            yield return null;
        }
    }

    private void HandleInputs()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (!_moving)
            {
                //Debug.Log("Moving set to true");
                _moving = true;
                transform.Translate(Vector3.up * 0.5f);
                StartCoroutine(MotionBlur(true));
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (!_moving)
            {
                //Debug.Log("Moving set to true");
                _moving = true;
                transform.Translate(Vector3.down * 0.5f);
                StartCoroutine(MotionBlur(false));
            }
        }
        else if (_moving)
        {
            //Debug.Log("Moving set to false");
            _moving = false;
        }
    }

    IEnumerator MotionBlur(bool up)
    {
        var time = 10;
        var maxScale = 1.5f;
        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {
                motionBlur.enabled = false;
            }
            else
            {
                motionBlur.enabled = true;
                var difference = (maxScale - 1f) * (1f / time) * (i + 1);
                motionBlur.transform.localScale = new Vector3(1.1f, maxScale - difference, 1);
                motionBlur.transform.localPosition = new Vector3(0, (maxScale - 1f - difference) * (up ? -1 : 1), 1);
            }

            yield return null;
        }
    }

    public bool TakeDamage()
    {
        FlashScreenColour(Color.red, damaged ? 200 : 20);

        if (!damaged)
        {
            damaged = true;
            GetComponent<Animator>().runtimeAnimatorController = damagedAnimation;
            return false;
        }

        dead = true;
        playerParticle.Stop();
        GetComponent<Animator>().Stop();
        StartCoroutine(Die());
        return true;
    }

    public void Repair()
    {
        damaged = false;
        GetComponent<Animator>().runtimeAnimatorController = normalAnimation;
    }

    public void FlashScreenColour(Color flashColour, int time)
    {
        StartCoroutine(FlashingScreenColour(flashColour, time));
    }

    IEnumerator FlashingScreenColour(Color flashColour, int time)
    {
        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {
                playerCamera.backgroundColor = new Color(0, 0, 0);
            }
            else
            {
                playerCamera.backgroundColor = Color.Lerp(new Color(0, 0, 0), flashColour, 1 - ((1f / time) * (i + 1f)));
            }
            
            yield return null;
        }
    }

    public void Pulse()
    {
        if (!dead)
        {
            if (damaged)
            {
                StartCoroutine(FlashDamage());
            }

            ZoomCamera(10, 4.14f);
        }
        
    }

    IEnumerator FlashDamage()
    {
        var time = 10;
        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 0.5f + ((0.5f / time) * (i + 1f)), 0.5f + ((0.5f / time) * (i + 1f)), 1);
            }

            yield return null;
        }
    }

    IEnumerator Die()
    {
        var time = 30;
        var maxScale = 3f;
        var minScale = 0.1f;

        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {

            }
            else if (i < time / 2)
            {
                var scale = 1f + ((maxScale - 1f) * (1f / (time / 2) * i));
                transform.localScale = new Vector3(scale, scale, 1);
            }
            else
            {
                var scale = maxScale - ((maxScale - minScale) / (time / 2) * (i - (time / 2)));
                transform.localScale = new Vector3(scale, scale, 1);
            }
            yield return null;
        }
    }

    public void ZoomCamera(int time, float zoomIn)
    {
        StartCoroutine(ZoomingCamera(time, zoomIn));
    }

    IEnumerator ZoomingCamera(int time, float zoomIn)
    {
        float standardCameraZoom = 4.219f;

        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {
                playerCamera.orthographicSize = standardCameraZoom;
            }
            else
            {
                playerCamera.orthographicSize = zoomIn + ((1f / time) * (i + 1f) * (standardCameraZoom - zoomIn));
            }

            yield return null;
        }
    }
}
