using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettingSlider : MonoBehaviour
{
    [SerializeField]Slider MasterVolumeSlider;
    void Start()
    {
        // スライダーに値を適応
        if (ES3.KeyExists("MasterVolume"))
        {
            MasterVolumeSlider.value = ES3.Load<float>("MasterVolume");
        }

        
    }


}
