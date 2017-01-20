using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public MusicMasterScript master;
    public Camera playerCamera;
    public AudioSource noteSound;

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
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        HandleInputs();

        var notes = FindObjectsOfType<NoteScript>();
        var currentNote = notes.Where
        (
            n => n.transform.position.x >= transform.position.x && n.transform.position.x < transform.position.x + movementDistance &&
                 n.transform.position.y >= transform.position.y - 0.1f && n.transform.position.y <= transform.position.y + 0.1f
        ).FirstOrDefault();
        if (currentNote != null)
        {
            noteSound.pitch = NoteInfo.GetPitchFromPosition((int)(currentNote.transform.position.y * 2), true);
            noteSound.Play();
        }
    }

    private void HandleInputs()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (!_moving)
            {
                Debug.Log("Moving set to true");
                _moving = true;
                transform.Translate(Vector3.up * 0.5f);
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (!_moving)
            {
                Debug.Log("Moving set to true");
                _moving = true;
                transform.Translate(Vector3.down * 0.5f);
            }
        }
        else if (_moving)
        {
            Debug.Log("Moving set to false");
            _moving = false;
        }
    }
}
