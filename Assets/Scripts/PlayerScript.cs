using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public MusicMasterScript master;
    public Camera playerCamera;
    public ParticleSystem playerParticle;
    public GameObject stave;

    public RuntimeAnimatorController normalAnimation;
    public RuntimeAnimatorController damagedAnimation;

    bool _moving = false;

    bool damaged = false;

    bool dead = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if (!dead)
        {
            var movementDistance = master.MovementSpeedPerTick;
            transform.Translate(Vector3.right * movementDistance);
            playerCamera.transform.position = new Vector3(transform.position.x + 3, playerCamera.transform.position.y, playerCamera.transform.position.z);
            playerParticle.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            stave.transform.position = new Vector3(transform.position.x + 3, stave.transform.position.y, stave.transform.position.z);
            HandleInputs();

            master.CurrentPlayerRow = Mathf.RoundToInt(transform.position.y * 2);
        }
        else
        {

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
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (!_moving)
            {
                //Debug.Log("Moving set to true");
                _moving = true;
                transform.Translate(Vector3.down * 0.5f);
            }
        }
        else if (_moving)
        {
            //Debug.Log("Moving set to false");
            _moving = false;
        }
    }

    public bool TakeDamage()
    {
        StartCoroutine(ShowDamageTaken());

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

    IEnumerator ShowDamageTaken()
    {
        var time = damaged ? 200 : 20;
        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {
                playerCamera.backgroundColor = new Color(0, 0, 0);
            }
            else
            {
                playerCamera.backgroundColor = new Color(1 - ((1f / time) * (i + 1f)), 0, 0);
            }
            
            yield return null;
        }
    }

    public void Pulse()
    {
        if (damaged && !dead)
        {
            StartCoroutine(FlashDamage());
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
        var time = 60;
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
}
