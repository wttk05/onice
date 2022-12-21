using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;
using KanKikuchi.AudioManager;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DigitalSalmon.Fade;



public class Setting : MonoBehaviour
{
    [SerializeField] GameObject SettingCanvas;

    [SerializeField] GameObject SettingPanel;

    [SerializeField]  CustomButton settingButton;
    [SerializeField] CustomButton ResetButton;

    FadePostProcess fadePostProcess;

    // �C���X�^���X���쐬
    public static Setting instance;

    bool isFade;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.activeSceneChanged += OnActiveSceneChanged;


        // �O��̐ݒ�f�[�^�𔽉f
        if (ES3.KeyExists("MasterVolume"))
        {
            instance.SettingMasterVolume(ES3.Load<float>("MasterVolume"));
        }
    }

    private void Start()
    {
        // �{�^���N����
        settingButton.onClickCallback = () => ClickSettingImage();
        ResetButton.onClickCallback = () => ResetGame();

        // ��\���ɂ��Ă���
        SettingPanel.gameObject.SetActive(false);

        isFade = false;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClickSettingImage();
        }
    }

    public void ClickSettingImage()
    {
        if(!SettingPanel.gameObject.activeSelf)
        {
            SettingPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            SettingPanel.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void SettingMasterVolume(float volume)
    {
        if (SoundManager.GetInstance() != null)
        {
            SoundManager.GetInstance().MasterVolume = volume;
            SoundManager.GetInstance().SetTaggedMasterVolume("�킽��������", volume);
        }

        BGMManager.Instance.ChangeBaseVolume(volume);
        SEManager.Instance.ChangeBaseVolume(volume);

        // �ݒ��ۑ�
        ES3.Save<float>("MasterVolume", volume);
    }

    public bool GetActiveSettingPanel()
    {
        return SettingPanel.gameObject.activeSelf;
    }

    void ResetGame()
    {
        if(!isFade)
        {
            isFade = true;

            // ��\���ɖ߂�
            SettingPanel.gameObject.SetActive(false);

            fadePostProcess.FadeDown(false, () => SceneReload());
        }

    }

    void SceneReload()
    {
        isFade = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnActiveSceneChanged(Scene i_preChangedScene, Scene i_postChangedScene)
    {
        fadePostProcess = GameObject.Find("Main Camera").GetComponent<FadePostProcess>();
    }
}

