using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.InputSystem;
using DigitalSalmon.Fade;





public class UIManager : MonoBehaviour
{
    [SerializeField] SuperTextMesh scoreText;
    Vector3 scoreTextPosition;
    Vector3 scoreTextScale;


    [SerializeField] SuperTextMesh dangerousColorText;

    // これの親に入れる
    [SerializeField] GameObject RankingFadePanel;
    [SerializeField] Transform rankingListPearent;

    [SerializeField] Text rankingNameText;
    [SerializeField] SuperTextMesh rankinInputNameText;
    [SerializeField] GameObject rankingTextBoxPrefab; // ランキングに入れる要素
    List<GameObject> rankingTextBox = new List<GameObject>(); // ランキングに入れる要素


    [SerializeField] CustomButton rankingSubmitButton;
    [SerializeField] CustomButton NextButton;
    [SerializeField] CustomButton BackButton;
    [SerializeField] CustomButton upDateButton;

    [SerializeField] GameObject InputFadePanel;
    [SerializeField] GameObject ButtonFadePanel;

    // インスタンスを作成
    public static UIManager instance;

    bool isMoveScore;
    bool isChangeScene;

    //フェード用
    [SerializeField]
    protected FadePostProcess fadePostProcess;

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
    void Start()
    {
        isMoveScore = false;
        scoreText.gameObject.SetActive(false);
        dangerousColorText.gameObject.SetActive(false);
        RankingFadePanel.SetActive(false);
        InputFadePanel.SetActive(false);
        ButtonFadePanel.SetActive(false);
        isChangeScene = false;

        // ランキングボックスを生成しておく
        for (int i = 0; i < 10; i++)
        {
            rankingTextBox.Add(Instantiate(rankingTextBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            // 位置ずれ調整
            rankingTextBox[i].transform.SetParent(rankingListPearent);
            rankingTextBox[i].transform.localPosition = new Vector3(0, 0, 0);
            rankingTextBox[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            rankingTextBox[i].transform.localScale = new Vector3(1, 1, 1);

            rankingTextBox[i].SetActive(false);
        }
        rankingTextBox[0].SetActive(true);


        // 初期位置を保存しておく
        scoreTextPosition = scoreText.transform.localPosition;
        scoreTextScale = scoreText.transform.localScale;

        // ボタンのアクション設定
        rankingSubmitButton.onClickCallback = () => Submit();
        NextButton.onClickCallback = () => SceneChange("main");
        BackButton.onClickCallback = () => SceneChange("title");
        upDateButton.onClickCallback = () => ReloadRankingBoard();
    }

    void Update()
    {
        if (GameManager.instance.isGameStart)
        {
            scoreText.text = "Score:<c=rev>" + GameManager.instance.GetScore() + "</c>";

            if (GameManager.instance.isGameSet && !isMoveScore)
            {
                isMoveScore = true;
                dangerousColorText.gameObject.SetActive(false);

                StartCoroutine(GameSet());

            }
        }

        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            Submit();
        }

        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            GetRankingBoard();
        }
    }

    public void GetDengerousColorTypeText(string s)
    {
        switch (s)
        {
            case "White":
                dangerousColorText.text = "<w><c=white>白色</c>に気をつけろ!</w>";
                break;
            case "Blue":
                dangerousColorText.text = "<w><c=blue>青色</c>に気をつけろ!</w>";
                break;
            case "Yellow":
                dangerousColorText.text = "<w><c=yellow>黄色</c>に気をつけろ!</w>";
                break;
            case "Red":
                dangerousColorText.text = "<w><c=red>赤色</c>に気をつけろ!</w>";
                break;
        }
    }

    public void GameStart()
    {
        scoreText.gameObject.SetActive(true);
        dangerousColorText.gameObject.SetActive(true);
    }

    IEnumerator GameSet()
    {
        // ランキングを取得する
        GetRankingBoard();
        yield return new WaitForSeconds(1.0f);

        ClaimScore();// スコアの移動演出
        yield return new WaitForSeconds(2.0f);
        ReturnScore();// スコアの位置を戻す
        yield return new WaitForSeconds(1.5f);
        ExpansionCanvasFadePanel(RankingFadePanel);//パネルの演出
        yield return new WaitForSeconds(0.1f);
        ExpansionCanvasFadePanel(InputFadePanel);//パネルの演出2
        yield return new WaitForSeconds(0.1f);
        ExpansionCanvasFadePanel(ButtonFadePanel);//パネルの演出3

    }

    void ReloadRankingBoard()
    {
        //ランキング再取得
        GetRankingBoard();
        ExpansionCanvasFadePanel(RankingFadePanel);//パネルの演出
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  演出、移動処理
    /// </summary>
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void ClaimScore()
    {
        scoreText.transform.DOLocalMove(new Vector3(0, 0, 0), 1.0f).SetEase(Ease.OutSine).SetLink(gameObject)
           .OnComplete(() => scoreText.transform.DOScale(new Vector3(3, 3, 0), 1.0f).SetEase(Ease.OutSine).SetLink(gameObject));
    }

    void ReturnScore()
    {
        scoreText.transform.DOLocalMove(scoreTextPosition, 1.0f).SetEase(Ease.OutSine).SetLink(gameObject);
        scoreText.transform.DOScale(scoreTextScale, 1.0f).SetEase(Ease.OutSine).SetLink(gameObject);
    }

    void ExpansionCanvasFadePanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.transform.localScale = new Vector3(0, 0, 0);
        panel.transform.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutSine).SetLink(gameObject);
    }

    /// <summary>
    /// TRUEなら続行、FALSEならタイトルに戻る
    /// </summary>
    /// <param name="bool"></param>
    void SceneChange(string sceneName)
    {
        if (!isChangeScene)
        {
            isChangeScene = true;
            fadePostProcess.FadeDown(false, () => SceneManager.LoadScene(sceneName));
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// PlayFab関連
    /// </summary>
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// ランキングを取得する
    /// </summary>
    public void GetRankingBoard()
    {
        // ランキングを更新、取得
        PlayfabManager.instance.GetLeaderboard();

        // 取得したらランキングを表示させる
        Invoke("ViewRanking", 0.1f);
    }


    /// <summary>
    /// ランキングに登録する際の名前を送信
    /// </summary>
    public void Submit()
    {
        if (rankingNameText.text.Length <= 2)
        {
            rankinInputNameText.text = "2文字以上で入力して下さい";
            return;
        }
        else if (rankingNameText.text.Length <= 25 && rankingNameText.text.Length > 2)
        {
            rankinInputNameText.text = "OK!";
        }
        else if (rankingNameText.text.Length <= 25)
        {
            rankinInputNameText.text = "25文字以内で入力して下さい!";
            return;
        }

        // 名前更新
        if (rankingNameText == null)
        {
            PlayfabManager.instance.UpdateUserName("名無しさん");
        }
        else
        {
            PlayfabManager.instance.UpdateUserName(rankingNameText.text);
        }
        // スコア更新
        PlayfabManager.instance.UpdatePlayerStatistics(GameManager.instance.GetScore());

        //ランキング再取得
        ReloadRankingBoard();
    }

    /// <summary>
    /// ランキングを表示させる
    /// </summary>
    void ViewRanking()
    {
        // ランキングを取得
        var rankingData = PlayfabManager.instance.GetRankingBoard();
        if(rankingData == null){return;}


        for (int i = 0; i < rankingData.Count; i++)
        {
            rankingTextBox[i].SetActive(true);
            // 子のテキストを取得
            GameObject child = rankingTextBox[i].transform.GetChild(0).gameObject;

            // テキストを更新
            child.GetComponent<SuperTextMesh>().text =
                rankingData[i].Position + 1 + "位\n" + rankingData[i].DisplayName + " " + rankingData[i].StatValue + "P";
        }
    }
}
