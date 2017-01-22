using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MusicMasterScript : MonoBehaviour {

    public int BPM;

    public NoteScript notePrefab;

    public PlayerScript playerRef;

    public BarrierScript barrierPrefab;

    public AudioSource drumBeat1;

    public AudioSource drumBeat2;

    public AudioSource noteSound;

    public AudioSource harmony1Sound;

    public AudioSource harmony2Sound;

    public AudioSource damageSound;

    public NoteScript activeNote;

    public BackgroundScript[] backgrounds;

    public Text StreakUI;

    public Text ScoreUI;

    public Text RestartUI;

    public Text VictoryUI;

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

    public bool dead = false;

    bool started = false;

    bool victory = false;

	// Use this for initialization
	void Start () {
        Song = SongInfo.ParseSong(Path.Combine(Application.streamingAssetsPath, SongFilePath));
        BuildNotes(Song.MelodyTrack);
        BuildHarmony(Song.Harmony1Track, 1);
        BuildHarmony(Song.Harmony2Track, 2);
        CurrentStreak = 0;
        CurrentScore = 0;
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

    void FixedUpdate()
    {
        if (!started)
        {
            playerRef.FlashScreenColour(Color.white, 30);
            started = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }        

        _ticksUntilNextBeat -= 1;

        if (!dead && _ticksUntilNextBeat == 0)
        {               

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

                playerRef.Pulse();
                foreach (var background in backgrounds)
                {
                    background.Pulse(playerRef.damaged, HarmonyLevel >= 2);
                }

                foreach (var note in Song.MelodyTrack.Notes.Skip(currentBeat))
                {
                    if (note != null && note is NoteInfo)
                    {
                        (note as NoteInfo).noteScript.GlowNote();
                    }
                }

                beatAlt = !beatAlt;
            }

            offBeat = !offBeat;


            if (currentBeat < Song.MelodyTrack.Notes.Count)
            {
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
                            CurrentStreak++;
                            CurrentScore++;

                            if (CurrentStreak >= 10)
                            {
                                CurrentScore++;
                                if (HarmonyLevel < 1)
                                {
                                    playerRef.Repair();
                                    HarmonyLevel = 1;
                                    playerRef.FlashScreenColour(new Color(0x42 / 255f, 0xb6 / 255f, 0xff / 255f), 10);
                                }
                            }

                            if (CurrentStreak >= 20)
                            {
                                CurrentScore++;
                                if (HarmonyLevel < 2)
                                {
                                    HarmonyLevel = 2;
                                    playerRef.FlashScreenColour(Color.white, 10);
                                }
                            }


                            playerRef.ReactToNote(currentNote.GetNoteColour());

                            noteSound.pitch = NoteInfo.GetPitchFromPosition(position);
                            noteSound.Play();

                            if (HarmonyLevel > 0)
                            {
                                var hTrackInfo = Song.Harmony1Track.Notes[currentBeat];
                                if (hTrackInfo != null && hTrackInfo is NoteInfo)
                                {
                                    var hNoteInfo = hTrackInfo as NoteInfo;
                                    hNoteInfo.noteScript.Explode();
                                    harmony1Sound.pitch = NoteInfo.GetPitchFromPosition(hNoteInfo.GetPosition());
                                    harmony1Sound.Play();
                                }
                            }

                            if (HarmonyLevel > 1)
                            {
                                var hTrackInfo = Song.Harmony2Track.Notes[currentBeat];
                                if (hTrackInfo != null && hTrackInfo is NoteInfo)
                                {
                                    var hNoteInfo = hTrackInfo as NoteInfo;
                                    hNoteInfo.noteScript.Explode();
                                    harmony2Sound.pitch = NoteInfo.GetPitchFromPosition(hNoteInfo.GetPosition());
                                    harmony2Sound.Play();
                                }
                            }

                            if (nextBarrier != null)
                            {
                                nextBarrier.Strength -= 1;
                            }
                            Scoring[currentBeat] = true;

                            if (noteScript != null)
                            {
                                noteScript.Explode();
                            }
                        }
                        else
                        {
                            Scoring[currentBeat] = false;
                            if (HarmonyLevel > 0)
                            {
                                playerRef.FlashScreenColour(Color.gray, 10);
                            }
                            CurrentStreak = 0;
                            HarmonyLevel = 0;
                            noteSound.Stop();
                            harmony1Sound.Stop();
                            harmony2Sound.Stop();
                            if (noteScript != null)
                            {
                                noteScript.FadeNote();
                            }
                        }
                    }
                    else if (currentTrackInfo is BarrierInfo)
                    {
                        var currentBarrier = currentTrackInfo as BarrierInfo;
                        if (!currentBarrier.Destroyed)
                        {
                            CurrentStreak = 0;
                            damageSound.Play();
                            if (playerRef.TakeDamage())
                            {
                                dead = true;
                                RestartUI.enabled = true;
                            }
                        }


                        nextBarrier = Song.MelodyTrack.Notes.Skip(currentBeat + 1).Where(t => t is BarrierInfo).Select(t => t as BarrierInfo).FirstOrDefault();
                    }
                }
            }
            else
            {
                if (!victory && currentBeat > Song.MelodyTrack.Notes.Count + 10)
                {
                    playerRef.Win();
                    playerRef.FlashScreenColour(Color.white, 30);
                    foreach(var note in FindObjectsOfType<NoteScript>())
                    {
                        note.SpriteRenderer.enabled = false;
                    }

                    StartCoroutine(FadeOutNotes(100));
                    victory = true;
                    if (Scoring.Any(s => s.HasValue && !s.Value))
                    {
                        VictoryUI.text = "GOOD WORK! BUT CAN YOU GET 100%?";
                    }
                    else
                    {
                        VictoryUI.text = "FLAWLESS VICTORY!";
                    }

                    VictoryUI.enabled = true;
                }               

            }          

            ScoreUI.text = (CurrentScore * 17).ToString();
            StreakUI.text = HarmonyLevel > 0 ? "X" + (HarmonyLevel + 1).ToString() : "";
            

            
            currentBeat++;
        }
    }

    IEnumerator FadeOutNotes(int time)
    {
        var currentVolume = noteSound.volume;

        for (int i = 0; i <= time; i++)
        {
            if (i == time)
            {
                noteSound.volume = 0f;
                harmony1Sound.volume = 0f;
                harmony2Sound.volume = 0f;
            }
            else
            {
                noteSound.volume = currentVolume - (((currentVolume / time) * (i + 1)));
                harmony1Sound.volume = currentVolume - (((currentVolume / time) * (i + 1)));
                harmony2Sound.volume = currentVolume - (((currentVolume / time) * (i + 1)));
            }

            yield return null;         
        }
    }

    void BuildHarmony(TrackInfo track, int level)
    {
        for (int note = 0; note < track.Notes.Count; note++)
        {
            var trackPart = track.Notes[note];
            if (trackPart != null && trackPart is NoteInfo)
            {
                var noteInfo = trackPart as NoteInfo;
                var thisNote = Instantiate(notePrefab);
                thisNote.noteInfo = noteInfo;
                noteInfo.noteScript = thisNote;
                thisNote.masterScript = this;

                thisNote.name = "H" + level.ToString() + ":" + trackPart.ToString();
                thisNote.HarmonyLevel = level;
                thisNote.GetComponent<SpriteRenderer>().color = noteInfo.GetNoteColour();
                thisNote.Glow.color = noteInfo.GetNoteColour();
                thisNote.transform.position = new Vector3
                (
                    note,
                    noteInfo.GetPosition() / 2f,
                    0
                );
            }
        }
    }

    public int HarmonyLevel = 0;

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
                thisNote.masterScript = this;

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
