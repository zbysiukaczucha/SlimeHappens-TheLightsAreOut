using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Slimeborne
{
    public class UltimateBar : MonoBehaviour
    {
        public Slider slider;

        public void SetMaxUltMeter(int maxUltMeter)
        {
            slider.maxValue = maxUltMeter;
        }
        
        public void SetCurrentUltMeter(int currentUltMeter)
        {
            slider.value = currentUltMeter;
        }
    }
}