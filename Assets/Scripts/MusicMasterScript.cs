using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class MusicMasterScript : MonoBehaviour {

    public int BPM;

    public NoteScript notePrefab;

    public AudioSource drumBeat1;

    public AudioSource drumBeat2;

    public AudioSource noteSound;

    public NoteScript activeNote;

    public int CurrentPlayerRow;

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

    public int TicksPerBeat
    {
        get
        {
            return (int)(1 / (BPS * Time.fixedDeltaTime));
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

    private bool beatAlt = false;
    private int _ticksUntilNextBeat = 1;
    private int currentBeat = 0;
    void FixedUpdate ()
    {
        _ticksUntilNextBeat -= 1;

        if (_ticksUntilNextBeat == 0)
        {
            _ticksUntilNextBeat += TicksPerBeat;
            if (beatAlt)
            {
                drumBeat2.Play();
            }
            else
            {
                drumBeat1.Play();
            }

            var currentNote = Song.MelodyTrack.Notes[currentBeat];
            int position;
            if (currentNote != null && (position = currentNote.GetPosition()) == CurrentPlayerRow)
            {
                noteSound.pitch = NoteInfo.GetPitchFromPosition(position, true);
                noteSound.Play();
            }

            beatAlt = !beatAlt;
            currentBeat++;
        }
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
