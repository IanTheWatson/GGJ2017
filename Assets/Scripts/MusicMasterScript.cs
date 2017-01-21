using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MusicMasterScript : MonoBehaviour {

    public int BPM;

    public NoteScript notePrefab;

    public AudioSource drumBeat1;

    public AudioSource drumBeat2;

    public AudioSource noteSound;

    public NoteScript activeNote;

    public Text StreakUI;

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
            return (int)(1 / (MovementSpeedPerTick));
        }
    }

	// Use this for initialization
	void Start () {
        Song = SongInfo.ParseSong(Path.Combine(Application.dataPath, SongFilePath));
        BuildNotes(Song.MelodyTrack);
        CurrentStreak = 0;
    }

    public bool?[] Scoring;
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool beatAlt = false;
    private bool offBeat = false;
    private int _ticksUntilNextBeat = 1;
    private int currentBeat = 0;
    
    public int CurrentScore
    {
        get;
        private set;
    }

    public int MaxScore
    {
        get;
        private set;
    }

    public int CurrentStreak
    {
        get;
        private set;
    }

    void FixedUpdate ()
    {
        _ticksUntilNextBeat -= 1;

        if (_ticksUntilNextBeat == 0)
        {
            foreach (var note in Song.MelodyTrack.Notes.Skip(currentBeat))
            {
                var noteIndex = Song.MelodyTrack.Notes.IndexOf(note);
                var noteToBeat = GetNoteForNote(noteIndex, Song.MelodyTrack);
                if (noteToBeat != null)
                {
                    noteToBeat.GlowNote();
                }
            }                   

            _ticksUntilNextBeat += TicksPerBeat;
            if (!offBeat)
            {
                if (beatAlt)
                {
                    drumBeat2.Play();
                }
                else
                {
                    drumBeat1.Play();
                }

                beatAlt = !beatAlt;
            }

            offBeat = !offBeat;
            

            var currentNote = Song.MelodyTrack.Notes[currentBeat];
            int position;
            if (currentNote != null)
            {
                var noteScript = GetNoteForNote(currentBeat, Song.MelodyTrack);
                if ((position = currentNote.GetPosition()) == CurrentPlayerRow)
                {

                    noteSound.pitch = NoteInfo.GetPitchFromPosition(position, true);
                    noteSound.Play();
                    Scoring[currentBeat] = true;
                    CurrentStreak++;

                    if (noteScript != null)
                    {
                        noteScript.Explode();
                    }
                }
                else
                {
                    Scoring[currentBeat] = false;
                    CurrentStreak = 0;
                    if (noteScript != null)
                    {
                        noteScript.FadeNote();
                    }                    
                }
            }

            StreakUI.text = "Current Streak: " + CurrentStreak.ToString();
            

            
            currentBeat++;
        }
    }

    NoteScript GetNoteForNote(int position, TrackInfo track)
    {
        var noteInfo = track.Notes[position];
        if (noteInfo == null)
        {
            return null;
        }

        return FindObjectsOfType<NoteScript>().Where(n => Mathf.RoundToInt(n.transform.position.y * 2) == noteInfo.GetPosition() && Mathf.RoundToInt(n.transform.position.x) == position).FirstOrDefault();
    }

    void BuildNotes(TrackInfo track)
    {
        Scoring = new bool?[track.Notes.Count];

        for (int note = 0; note < track.Notes.Count; note++)
        {
            var thisNoteInfo = track.Notes[note];
            if (thisNoteInfo != null)
            {
                Scoring[note] = null;
                var thisNote = Instantiate(notePrefab);
                thisNote.name = thisNoteInfo.ToString();
                thisNote.GetComponent<SpriteRenderer>().color = thisNoteInfo.GetNoteColour();
                thisNote.Glow.color = thisNoteInfo.GetNoteColour();
                thisNote.transform.position = new Vector3
                (
                    note,
                    thisNoteInfo.GetPosition() / 2f,
                    0
                );
            }
            else
            {
                Scoring[note] = true;
            }
        }
    }

    public SongInfo Song
    {
        get;
        private set;
    }
}
