using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using UnityEngine.InputSystem;
using System;

public class EventManager : MonoBehaviour
{
    // ADV�G���W��
    public AdvEngine AdvEngine { get { return advEngine; } }
    [SerializeField]
    protected AdvEngine advEngine;

    public bool IsPlaying { private set; get; }
    float defaultSpeed = -1;

    // �C���X�^���X���쐬
    public static EventManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        IsPlaying = false;
        ResetMessageSpeed();
    }



    //�w��̃��x���̃V�i���I���Đ�����
    public void JumpScenario(string label)
    {
        Debug.Log("�V�i���I�Đ�");
        StartCoroutine(JumpScenarioAsync(label, null));

    }

    //�w��̃��x���̃V�i���I���Đ�����
    //�I����������onComplete���Ă΂��
    public void JumpScenario(string label, Action onComplete)
    {
        Debug.Log("�V�i���I�Đ�2");

        if (ES3.KeyExists("MasterVolume"))
        {
            //�V�i���I�Đ��O�ɉ��𒲐�
            Setting.instance.SettingMasterVolume(ES3.Load<float>("MasterVolume"));// �����ɓ���Ȃ��Ƃ��Ȃ肨�����̉��������f����Ȃ�����
        }

        StartCoroutine(JumpScenarioAsync(label, onComplete));

    }

    //�w��̃��x���̃V�i���I���Đ�����
    //���x�����Ȃ������ꍇ��z��
    public void JumpScenario(string label, Action onComplete, Action onFailed)
    {
        JumpScenario(label, null, onComplete, onFailed);

    }

    //�w��̃��x���̃V�i���I���Đ�����
    //���x�����Ȃ������ꍇ��z��
    public void JumpScenario(string label, Action onStart, Action onComplete, Action onFailed)
    {
        if (string.IsNullOrEmpty(label))
        {
            if (onFailed != null) onFailed();
            Debug.LogErrorFormat("�V�i���I���x������ł�");
            return;
        }
        if (label[0] == '*')
        {
            label = label.Substring(1);
        }
        if (AdvEngine.DataManager.FindScenarioData(label) == null)
        {
            if (onFailed != null) onFailed();
            Debug.LogErrorFormat("{0}�͂܂����[�h����Ă��Ȃ����A���݂��Ȃ��V�i���I�ł�", label);
            return;
        }

        if (onStart != null) onStart();
        JumpScenario(
            label,
            onComplete);

    }

    IEnumerator JumpScenarioAsync(string label, Action onComplete)
    {
        IsPlaying = true;
        AdvEngine.JumpScenario(label);
        while (!AdvEngine.IsEndScenario)
        {
            yield return null;

        }
        IsPlaying = false;
        if (onComplete != null) onComplete();
    }

    //�V�i���I�̌Ăяo���ȊO�ɁA
    //AdvEngine�𑀍삷�鏈�����܂Ƃ߂Ă����ƁA�֗�
    //�����K�v���̓v���W�F�N�g�ɂ��̂ŁA�ꍇ�ɂ���đ��₵�Ă���

    //�ȉ��A���b�Z�[�W�E�B���h�̃e�L�X�g�\�����x�𑀍삷�鏈���̃T���v��

    //���b�Z�[�W�E�B���h�̃e�L�X�g�\���̑��x���w��̃X�s�[�h��
    public void ChangeMessageSpeed(float speed)
    {
        if (defaultSpeed < 0)
        {
            defaultSpeed = AdvEngine.Config.MessageSpeed;
        }
        AdvEngine.Config.MessageSpeed = speed;
    }
    //���b�Z�[�W�E�B���h�̃e�L�X�g�\���̑��x�����ɖ߂�
    public void ResetMessageSpeed()
    {
        if (defaultSpeed >= 0)
        {
            AdvEngine.Config.MessageSpeed = defaultSpeed;
        }
    }


}
