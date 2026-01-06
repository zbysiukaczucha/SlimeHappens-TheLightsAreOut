using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character
{
    public string name;
    public string type;
    public int bargainingTimes;
    public bool easilyThreatened;

    public Character(string name, string type, int bargainingTimes, bool easilyThreatened)
    {
        this.name = name;
        this.type = type;
        this.bargainingTimes = bargainingTimes;
        this.easilyThreatened = easilyThreatened;
    }
}
