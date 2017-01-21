using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NoteInfo : TrackPartInfo
{
    readonly static float scale = Mathf.Pow(2f, 1.0f / 12f);
    public const int BaseNote = 6;

    public const int MiddleC = 3;
    public const int BaseOctave = 3;

    public NoteScript noteScript;

    public NoteEnum Note
    {
        get;
        set;
    }

    public int Octave
    {
        get;
        set;
    }

    public static readonly int[] CScale = new int[]
    {
        0,  //C
        2,  //D
        4,  //E
        5,  //F
        7,  //G
        9,  //A
        11, //B
    };

    public int GetPosition()
    {
        return ((int)Note - BaseNote) + ((Octave - BaseOctave) * 7);
    }
    
    public Color GetNoteColour()
    {
        switch (Note)
        {
            case NoteEnum.A:
                return new Color(0x3c / 255f, 0x00 / 255f, 0xff / 255f);
            case NoteEnum.B:
                return new Color(0xff / 255f, 0x46 / 255f, 0xfe / 255f);
            case NoteEnum.C:
                return new Color(0xff / 255f, 0x2d / 255f, 0x2d / 255f);
            case NoteEnum.D:
                return new Color(0xfd / 255f, 0x8e / 255f, 0x00 / 255f);
            case NoteEnum.E:
                return new Color(0xff / 255f, 0xf6 / 255f, 0x00 / 255f);
            case NoteEnum.F:
                return new Color(0x2e / 255f, 0xd2 / 255f, 0x02 / 255f);
            case NoteEnum.G:
                return new Color(0x00 / 255f, 0xff / 255f, 0xea / 255f);
            default:
                return new Color(0xff / 255f, 0xff / 255f, 0xff / 255f);
        }
    }

    public static float GetPitchFromPosition(int position, int mod = 0)
    {
        var normalisedPosition = position + BaseNote;

        int octave = 0;
        if (normalisedPosition < 0)
        {
            while (normalisedPosition < 0)
            {
                octave -= 1;
                normalisedPosition += 7;
            }
        }
        else
        {
            octave = normalisedPosition / 7;
        }

        int scaleNote = normalisedPosition % 7;

        int offset = CScale[scaleNote < 0 ? CScale.Length - (scaleNote + 1) : scaleNote] + (octave * 12) + mod;
        //Debug.Log("Offset:" + offset);
        //Debug.Log("Math:" + Mathf.Pow(scale, offset));
        return (Mathf.Pow(scale, offset));
    }

    public override string ToString()
    {
        return Note.ToString() + Octave.ToString();
    }
}
