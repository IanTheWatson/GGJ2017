using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BarrierInfo : TrackPartInfo
{
    public BarrierScript barrierScript;

    public int FirstNote
    {
        get;
        set;
    }

    public int LastNote
    {
        get;
        set;
    }

    public bool Destroyed
    {
        get
        {
            return PercentStrength <= 0.25f;
        }
    }

    public float PercentStrength
    {
        get
        {
            return Strength / (float)MaxStrength;
        }
    }

    public int MaxStrength
    {
        get;
        set;
    }

    public int Strength
    {
        get;
        set;
    }

    public override string ToString()
    {
        return "||";
    }
}
