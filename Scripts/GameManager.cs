using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utage;
using KanKikuchi.AudioManager;
using DG.Tweening;




public class GameManager : MonoBehaviour
{
    // ADV�G���W��
    public AdvEngine AdvEngine { get { return advEngine; } }
    [SerializeField]
    protected AdvEngine advEngine;

    [SerializeField] Camera mainCamera;
    [SerializeField] CreateObjects createObjects;


    //Utage�̃L�[���͗p
    AdvUguiManager UguiManager { get { return uguiManager ?? (uguiManager = FindObjectOfType<AdvUguiManager>()); } }
    public AdvUguiManager uguiManager;

    public ColorType dangerousColorType;

    int score;
    float timer;
    public bool isGameSet;

    public bool isDeathTime;

    public bool isGameStart;

    // ���̐鍐
    float END_TIMER = 75f;

    // �C���X�^���X���쐬
    public static GameManager instance;
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

    /*<summary>
     * Utage��SendMessage���瑗���Ă���������������Ƃ���
     *</summary> */
    private void Start()
    {
        BGMManager.Instance.Stop();
        //DOTween
        DOTween.SetTweensCapacity(1250, 50);

        /////////////////////////////////////////////////
        // �ŗL�̃V�i���I��ǂ܂���
        StartCoroutine(StartScenario());

        // �l�̏�����
        Init();
   
        // �v���C���[�Ɋ댯�F�̐ݒ������
         SelectDangerousBall();   //�J���[�^�C�v

         // player�̐F��String�ɕϊ����Ă���
         var dct = PlayerManager.instance.ConvertColorType(dangerousColorType);// String�ɕϊ�

         //  �댯�F�̃e�L�X�g���擾
         UIManager.instance.GetDengerousColorTypeText(dct);
    }

    void Init()
    {
        score = 0;
        dangerousColorType = ColorType.MAX;
        isGameSet =false;
        timer = 0;
        isDeathTime = false;
        isGameStart = false;
    }

    private void Update()
    {
        if (isGameStart)
        {
            timer += Time.deltaTime;
        }

        // �e�L�X�g�̕�������g���@�\�iUtage�j
        if (!Setting.instance.GetActiveSettingPanel())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                uguiManager.OnInput();
            }
        }

        if (isGameSet)
        {
            if(BGMManager.Instance.IsPlaying())
                BGMManager.Instance.Stop();
        }

        if(timer > END_TIMER)
        {
            isDeathTime = true;
        }
    }

    public void AddScore()
    {
        if(!isGameSet)
        {
            score++;
            GetScore();
        }

    }

    public int GetScore()
    {
        return score;
    }

    public void GetCommand(int n)
    {
        Debug.Log(n);
        // STATE�ɕϊ�
    }

    void SelectDangerousBall()
    {
        // �����_���ɃJ���[�^�C�v����擾
        ColorType r = (ColorType)Random.Range(0, (int)ColorType.MAX);
        dangerousColorType = r;
    }

    public enum ColorType
    {
        WHITE,
        BLUE,
        YELLOW,
        RED,
        MAX
    }    

    public void MoveCamera()
    {
        Vector3 cameraPos = new Vector3(0, 14.7f, -7.8f);
        Vector3 cameraRotate = new Vector3(50f, 0, 0);

        mainCamera.transform.DOMove(cameraPos, 1.0f).SetLink(gameObject);
        mainCamera.transform.DORotate(cameraRotate, 1.0f).SetLink(gameObject).OnComplete(() => GameStart());
    }

    void GameStart()
    {
        isGameStart = true;
        createObjects.StartCreateBall();
        PlayerManager.instance.GameStart();
        UIManager.instance.GameStart();
        BGMManager.Instance.Play(BGMPath.GAME_BGM);
    }

    IEnumerator StartScenario()
    {
        yield return new WaitForSeconds(0.5f);
        EventManager.instance.JumpScenario("U1W_RE", () => MoveCamera());
    }
}
