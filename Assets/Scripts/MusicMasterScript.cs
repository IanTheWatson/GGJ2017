using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicMasterScript : MonoBehaviour {

    public int BPM;

    public NoteScript notePrefab;

    public float BPS
    {
     get
        {
            return BPM / 60;
        }
    }

    public string SongFilePath;

    public float MovementSpeedPerTick
    {
        get
        {
            return BPS * Time.fixedDeltaTime;
        }
    }

	// Use this for initialization
	void Start () {
        Song = SongInfo.ParseSong(Path.Combine(Application.dataPath, SongFilePath));
        BuildNotes(Song.MelodyTrack);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void BuildNotes(TrackInfo track)
    {
        for (int note = 0; note < track.Notes.Length; note++)
        {
            var thisNoteInfo = track.Notes[note];
            if (thisNoteInfo != null)
            {
                var thisNote = Instantiate(notePrefab);
                thisNote.name = thisNoteInfo.ToString();
                thisNote.transform.position = new Vector3
                (
                    note,
                    thisNoteInfo.GetPosition() / 2f,
                    0
                );
            }
        }
    }

    public SongInfo Song
    {
        get;
        private set;
    }
}
