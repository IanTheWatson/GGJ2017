using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public MusicMasterScript master;
    public Camera playerCamera;

    bool _moving = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        transform.Translate(Vector3.right * master.MovementSpeedPerTick);
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!_moving)
            {
                _moving = true;
                transform.Translate(Vector3.up * 0.5f);
            }
        }
        else
        {
            _moving = false;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!_moving)
            {
                _moving = true;
                transform.Translate(Vector3.down * 0.5f);
            }
        }
        else
        {
            _moving = false;
        }
    }
}
