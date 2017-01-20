using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class NoteInfo
{
    readonly static float scale = Mathf.Pow(2f, 1.0f / 12f);
    public const int MajorBaseNote = 6;
    public const int MinorBaseNote = 8;

    public static readonly int[] MajorScale = new int[]
    {
        0,  //C
        2,  //D
        4,  //E
        5,  //F
        7,  //G
        9,  //A
        11, //B
    };

    public static readonly int[] MinorScale = new int[]
    {
        0,  //A
        2,  //B
        3,  //C
        5,  //D
        7,  //E
        8,  //F
        10, //G
    };

    public static float GetPitchFromPosition(int position, bool minorScale = false, int mod = 0)
    {
        var normalisedPosition = position + (minorScale ? MinorBaseNote : MajorBaseNote);
        var absPosition = Mathf.Abs(normalisedPosition);

        int octave = normalisedPosition / 7;
        if (normalisedPosition < 0)
        {
            octave -= 1;
        }


        int scaleNote = absPosition % 7;

        int offset = (minorScale ? MinorScale[scaleNote] : MajorScale[scaleNote]) + (octave * 12) - (minorScale ? MinorBaseNote - MajorBaseNote + 1 : 0) + mod;
        Debug.Log("Offset:" + offset);
        Debug.Log("Math:" + Mathf.Pow(scale, offset));
        return (Mathf.Pow(scale, offset));
    }
}
