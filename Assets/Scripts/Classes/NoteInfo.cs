using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class NoteInfo
{
    readonly static float scale = Mathf.Pow(2f, 1.0f / 12f);
    public const int BaseNote = 6;

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

    public static float GetPitchFromPosition(int position, bool minorScale = false, int mod = 0)
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
}
