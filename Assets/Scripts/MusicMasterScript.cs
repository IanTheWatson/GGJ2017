using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MusicMasterScript : MonoBehaviour {

    public int BPM;

    public NoteScript notePrefab;

    public BarrierScript barrierPrefab;

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

    private BarrierInfo nextBarrier = null;
    
    public int CurrentScore
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
                if (note != null && note is NoteInfo)
                {
                    (note as NoteInfo).noteScript.GlowNote();
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
            

            var currentTrackInfo = Song.MelodyTrack.Notes[currentBeat];
            int position;
            if (currentTrackInfo != null)
            {
                if (currentTrackInfo is NoteInfo)
                {
                    var currentNote = currentTrackInfo as NoteInfo;

                    var noteScript = currentNote.noteScript;
                    if ((position = currentNote.GetPosition()) == CurrentPlayerRow)
                    {

                        noteSound.pitch = NoteInfo.GetPitchFromPosition(position, true);
                        noteSound.Play();
                        if (nextBarrier != null)
                        {
                            nextBarrier.Strength -= 1;
                        }
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
                else if (currentTrackInfo is BarrierInfo)
                {
                    var currentBarrier = currentTrackInfo as BarrierInfo;


                    nextBarrier = Song.MelodyTrack.Notes.Skip(currentBeat + 1).Where(t => t is BarrierInfo).Select(t => t as BarrierInfo).FirstOrDefault();
                }
            }

            StreakUI.text = "Current Streak: " + CurrentStreak.ToString();
            

            
            currentBeat++;
        }
    }

    void BuildNotes(TrackInfo track)
    {
        Scoring = new bool?[track.Notes.Count];

        var firstNoteInSection = 0;

        for (int note = 0; note < track.Notes.Count; note++)
        {
            var trackPart = track.Notes[note];
            if (trackPart == null)
            {
                Scoring[note] = true;
            }
            else if (trackPart is NoteInfo)
            {
                var noteInfo = trackPart as NoteInfo;
                Scoring[note] = null;

                var thisNote = Instantiate(notePrefab);
                thisNote.noteInfo = noteInfo;
                noteInfo.noteScript = thisNote;

                thisNote.name = trackPart.ToString();
                thisNote.GetComponent<SpriteRenderer>().color = noteInfo.GetNoteColour();
                thisNote.Glow.color = noteInfo.GetNoteColour();
                thisNote.transform.position = new Vector3
                (
                    note,
                    noteInfo.GetPosition() / 2f,
                    0
                );
            }
            else if(trackPart is BarrierInfo)
            {
                var barrierInfo = trackPart as BarrierInfo;
                Scoring[note] = true;
                barrierInfo.FirstNote = firstNoteInSection;
                barrierInfo.LastNote = note - 1;
                barrierInfo.MaxStrength = Song.MelodyTrack.Notes.Skip(barrierInfo.FirstNote).Take(barrierInfo.LastNote - barrierInfo.FirstNote + 1).Where(t => t is NoteInfo).Count();
                barrierInfo.Strength = barrierInfo.MaxStrength;

                firstNoteInSection = note + 1;

                var thisBarrier = Instantiate(barrierPrefab);
                thisBarrier.barrierInfo = barrierInfo;
                barrierInfo.barrierScript = thisBarrier;

                thisBarrier.transform.position = new Vector3
                (
                    note,
                    0,
                    0
                );

                if (nextBarrier == null)
                {
                    nextBarrier = barrierInfo;
                }
            }
            else
            {
                //to do
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
