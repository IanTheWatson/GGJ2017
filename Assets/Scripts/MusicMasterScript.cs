using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMasterScript : MonoBehaviour {

    public int BPM;

    public float BPS
    {
     get
        {
            return BPM / 60;
        }
    }

    public float MovementSpeedPerTick
    {
        get
        {
            return BPS * Time.fixedDeltaTime;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
