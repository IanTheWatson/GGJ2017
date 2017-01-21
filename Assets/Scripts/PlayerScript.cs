using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public MusicMasterScript master;
    public Camera playerCamera;
    public ParticleSystem playerParticle;
    public GameObject stave;

    bool _moving = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        var movementDistance = master.MovementSpeedPerTick;
        transform.Translate(Vector3.right * movementDistance);
        playerCamera.transform.position = new Vector3(transform.position.x + 3, playerCamera.transform.position.y, playerCamera.transform.position.z);
        playerParticle.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        stave.transform.position = new Vector3(transform.position.x + 3, stave.transform.position.y, stave.transform.position.z);
        HandleInputs();

        master.CurrentPlayerRow = Mathf.RoundToInt(transform.position.y * 2);
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
}
