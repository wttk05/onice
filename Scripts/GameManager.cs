using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utage;
using KanKikuchi.AudioManager;
using DG.Tweening;




public class GameManager : MonoBehaviour
{
    // ADVエンジン
    public AdvEngine AdvEngine { get { return advEngine; } }
    [SerializeField]
    protected AdvEngine advEngine;

    [SerializeField] Camera mainCamera;
    [SerializeField] CreateObjects createObjects;


    //Utageのキー入力用
    AdvUguiManager UguiManager { get { return uguiManager ?? (uguiManager = FindObjectOfType<AdvUguiManager>()); } }
    public AdvUguiManager uguiManager;

    public ColorType dangerousColorType;

    int score;
    float timer;
    public bool isGameSet;

    public bool isDeathTime;

    public bool isGameStart;

    // 死の宣告
    float END_TIMER = 75f;

    // インスタンスを作成
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
     * UtageのSendMessageから送られてきた情報を処理するところ
     *</summary> */
    private void Start()
    {
        BGMManager.Instance.Stop();
        //DOTween
        DOTween.SetTweensCapacity(1250, 50);

        /////////////////////////////////////////////////
        // 固有のシナリオを読ませる
        StartCoroutine(StartScenario());

        // 値の初期化
        Init();
   
        // プレイヤーに危険色の設定をする
         SelectDangerousBall();   //カラータイプ

         // playerの色をStringに変換しておく
         var dct = PlayerManager.instance.ConvertColorType(dangerousColorType);// Stringに変換

         //  危険色のテキストを取得
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

        // テキストの文字送り拡張機能（Utage）
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
        // STATEに変換
    }

    void SelectDangerousBall()
    {
        // ランダムにカラータイプを一つ取得
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
