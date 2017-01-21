using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class SongInfo
{
    public static SongInfo ParseSong(string filePath)
    {
        SongInfo song = new SongInfo();

        using (var sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var designation = line.Substring(0, 2);
                switch (designation)
                {
                    case "M:":
                        song.MelodyTrack = TrackInfo.ParseTrack(line.Substring(2));
                        break;
                    case "1:":
                        song.Harmony1Track = TrackInfo.ParseTrack(line.Substring(2));
                        break;
                    case "2:":
                        song.Harmony2Track = TrackInfo.ParseTrack(line.Substring(2));
                        break;
                    case "//":
                        break;
                    default:
                        throw new InvalidOperationException("Unrecognised start of string: " + designation);
                }
            }
        }

        return song;
    }

    public TrackInfo MelodyTrack
    {
        get;
        private set;
    }

    public TrackInfo Harmony1Track
    {
        get;
        private set;
    }

    public TrackInfo Harmony2Track
    {
        get;
        private set;
    }
}
