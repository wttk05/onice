using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettingSlider : MonoBehaviour
{
    [SerializeField]Slider MasterVolumeSlider;
    void Start()
    {
        // �X���C�_�[�ɒl��K��
        if (ES3.KeyExists("MasterVolume"))
        {
            MasterVolumeSlider.value = ES3.Load<float>("MasterVolume");
        }

        
    }


}
