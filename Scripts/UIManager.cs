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

    // ����̐e�ɓ����
    [SerializeField] GameObject RankingFadePanel;
    [SerializeField] Transform rankingListPearent;

    [SerializeField] Text rankingNameText;
    [SerializeField] SuperTextMesh rankinInputNameText;
    [SerializeField] GameObject rankingTextBoxPrefab; // �����L���O�ɓ����v�f
    List<GameObject> rankingTextBox = new List<GameObject>(); // �����L���O�ɓ����v�f


    [SerializeField] CustomButton rankingSubmitButton;
    [SerializeField] CustomButton NextButton;
    [SerializeField] CustomButton BackButton;
    [SerializeField] CustomButton upDateButton;

    [SerializeField] GameObject InputFadePanel;
    [SerializeField] GameObject ButtonFadePanel;

    // �C���X�^���X���쐬
    public static UIManager instance;

    bool isMoveScore;
    bool isChangeScene;

    //�t�F�[�h�p
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

        // �����L���O�{�b�N�X�𐶐����Ă���
        for (int i = 0; i < 10; i++)
        {
            rankingTextBox.Add(Instantiate(rankingTextBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            // �ʒu���꒲��
            rankingTextBox[i].transform.SetParent(rankingListPearent);
            rankingTextBox[i].transform.localPosition = new Vector3(0, 0, 0);
            rankingTextBox[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            rankingTextBox[i].transform.localScale = new Vector3(1, 1, 1);

            rankingTextBox[i].SetActive(false);
        }
        rankingTextBox[0].SetActive(true);


        // �����ʒu��ۑ����Ă���
        scoreTextPosition = scoreText.transform.localPosition;
        scoreTextScale = scoreText.transform.localScale;

        // �{�^���̃A�N�V�����ݒ�
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
                dangerousColorText.text = "<w><c=white>���F</c>�ɋC������!</w>";
                break;
            case "Blue":
                dangerousColorText.text = "<w><c=blue>�F</c>�ɋC������!</w>";
                break;
            case "Yellow":
                dangerousColorText.text = "<w><c=yellow>���F</c>�ɋC������!</w>";
                break;
            case "Red":
                dangerousColorText.text = "<w><c=red>�ԐF</c>�ɋC������!</w>";
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
        // �����L���O���擾����
        GetRankingBoard();
        yield return new WaitForSeconds(1.0f);

        ClaimScore();// �X�R�A�̈ړ����o
        yield return new WaitForSeconds(2.0f);
        ReturnScore();// �X�R�A�̈ʒu��߂�
        yield return new WaitForSeconds(1.5f);
        ExpansionCanvasFadePanel(RankingFadePanel);//�p�l���̉��o
        yield return new WaitForSeconds(0.1f);
        ExpansionCanvasFadePanel(InputFadePanel);//�p�l���̉��o2
        yield return new WaitForSeconds(0.1f);
        ExpansionCanvasFadePanel(ButtonFadePanel);//�p�l���̉��o3

    }

    void ReloadRankingBoard()
    {
        //�����L���O�Ď擾
        GetRankingBoard();
        ExpansionCanvasFadePanel(RankingFadePanel);//�p�l���̉��o
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  ���o�A�ړ�����
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
    /// TRUE�Ȃ瑱�s�AFALSE�Ȃ�^�C�g���ɖ߂�
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
    /// PlayFab�֘A
    /// </summary>
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// �����L���O���擾����
    /// </summary>
    public void GetRankingBoard()
    {
        // �����L���O���X�V�A�擾
        PlayfabManager.instance.GetLeaderboard();

        // �擾�����烉���L���O��\��������
        Invoke("ViewRanking", 0.1f);
    }


    /// <summary>
    /// �����L���O�ɓo�^����ۂ̖��O�𑗐M
    /// </summary>
    public void Submit()
    {
        if (rankingNameText.text.Length <= 2)
        {
            rankinInputNameText.text = "2�����ȏ�œ��͂��ĉ�����";
            return;
        }
        else if (rankingNameText.text.Length <= 25 && rankingNameText.text.Length > 2)
        {
            rankinInputNameText.text = "OK!";
        }
        else if (rankingNameText.text.Length <= 25)
        {
            rankinInputNameText.text = "25�����ȓ��œ��͂��ĉ�����!";
            return;
        }

        // ���O�X�V
        if (rankingNameText == null)
        {
            PlayfabManager.instance.UpdateUserName("����������");
        }
        else
        {
            PlayfabManager.instance.UpdateUserName(rankingNameText.text);
        }
        // �X�R�A�X�V
        PlayfabManager.instance.UpdatePlayerStatistics(GameManager.instance.GetScore());

        //�����L���O�Ď擾
        ReloadRankingBoard();
    }

    /// <summary>
    /// �����L���O��\��������
    /// </summary>
    void ViewRanking()
    {
        // �����L���O���擾
        var rankingData = PlayfabManager.instance.GetRankingBoard();
        if(rankingData == null){return;}


        for (int i = 0; i < rankingData.Count; i++)
        {
            rankingTextBox[i].SetActive(true);
            // �q�̃e�L�X�g���擾
            GameObject child = rankingTextBox[i].transform.GetChild(0).gameObject;

            // �e�L�X�g���X�V
            child.GetComponent<SuperTextMesh>().text =
                rankingData[i].Position + 1 + "��\n" + rankingData[i].DisplayName + " " + rankingData[i].StatValue + "P";
        }
    }
}
