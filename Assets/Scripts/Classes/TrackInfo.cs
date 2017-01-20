using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TrackInfo
{
    private static readonly int aNote = 'A';
    private static readonly int gNote = 'G';

    public static TrackInfo ParseTrack(string text)
    {
        TrackInfo track = new TrackInfo()
        {
            Notes = new NoteInfo[text.Length / 2]
        };

        text = text.ToUpper();
        int currentIndex = 0;
        while (text.Length > 1)
        {
            var thisPart = text.Substring(0, 2);
            text = text.Substring(2);
            track.Notes[currentIndex] = ParseNote(thisPart);
            currentIndex++;
        }

        return track;
    }

    public static NoteInfo ParseNote(string text)
    {
        if (text == "--")
        {
            return null;
        }

        var note = text[0];
        if ((int)note >= aNote && (int)note <= gNote && char.IsDigit(text[1]))
        {
            return new NoteInfo()
            {
                Note = (NoteEnum)Enum.Parse(typeof(NoteEnum), text.Substring(0, 1)),
                Octave = int.Parse(text.Substring(1, 1))
            };
        }

        throw new InvalidOperationException("Unrecognised string: " + text);
    }

    public NoteInfo[] Notes
    {
        get;
        private set;
    }

    public override string ToString()
    {
        return string.Join("", Notes.Select(n => n == null ? "--" : n.ToString()).ToArray());
    }
}
