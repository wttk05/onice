using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;
using DigitalSalmon.Fade;


public class TitleUIManager : MonoBehaviour
{
    // これの親に入れる
    [SerializeField] GameObject RankingFadePanel;
    [SerializeField] Transform rankingListPearent;

    [SerializeField] GameObject rankingTextBoxPrefab; // ランキングに入れる要素
    List<GameObject> rankingTextBox = new List<GameObject>(); // ランキングに入れる要素

    [SerializeField] CustomButton PlayButton;
    [SerializeField] CustomButton rankingButton;

    //フェード用
    [SerializeField]
    protected FadePostProcess fadePostProcess;

    void Start()
    {
        RankingFadePanel.SetActive(false);

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



        // ボタンのアクション設定
        PlayButton.onClickCallback = () => GameStart();
        rankingButton.onClickCallback = () => ViewRankingBoard();
    }

    void GameStart()
    {
        // フェードとか入れながらゲームシーンへ移行する
        BGMManager.Instance.Stop();
        fadePostProcess.FadeDown(false, () => SceneManager.LoadScene("main"));
    }

    void ViewRankingBoard()
    {
        // ランキングを更新、取得
        PlayfabManager.instance.GetLeaderboard();
        Invoke("ViewRanking", 0.5f);


        ExpansionCanvasFadePanel(RankingFadePanel);
    }

    /// <summary>
    /// ランキングを表示させる
    /// </summary>
    void ViewRanking()
    {
        // ランキングを取得
        var rankingData = PlayfabManager.instance.GetRankingBoard();
        if (rankingData == null) { return; }

        

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

    //ランキングパネルの演出
    void ExpansionCanvasFadePanel(GameObject panel)
    {
        if(!panel.activeSelf)
        {
            panel.SetActive(true);
            panel.transform.transform.localScale = new Vector3(0, 0, 0);
            panel.transform.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutSine).SetLink(gameObject);
        }
        else
        {
            panel.transform.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutSine).SetLink(gameObject).OnComplete(() => panel.SetActive(false));
        }

    }
}
