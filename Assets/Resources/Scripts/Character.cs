using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string name;
    public string type;
    public Effect wantedEffect;
    public int bargainingTimes;
    public bool likelyToCompromise;

    public Character(string name, string type, Effect wantedEffect, int bargainingTimes, bool likelyToCompromise)
    {
        this.name = name;
        this.type = type;
        this.wantedEffect = wantedEffect;
        this.bargainingTimes = bargainingTimes;
        this.likelyToCompromise = likelyToCompromise;
    }
}
