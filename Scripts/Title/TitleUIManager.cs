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
    // ����̐e�ɓ����
    [SerializeField] GameObject RankingFadePanel;
    [SerializeField] Transform rankingListPearent;

    [SerializeField] GameObject rankingTextBoxPrefab; // �����L���O�ɓ����v�f
    List<GameObject> rankingTextBox = new List<GameObject>(); // �����L���O�ɓ����v�f

    [SerializeField] CustomButton PlayButton;
    [SerializeField] CustomButton rankingButton;

    //�t�F�[�h�p
    [SerializeField]
    protected FadePostProcess fadePostProcess;

    void Start()
    {
        RankingFadePanel.SetActive(false);

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



        // �{�^���̃A�N�V�����ݒ�
        PlayButton.onClickCallback = () => GameStart();
        rankingButton.onClickCallback = () => ViewRankingBoard();
    }

    void GameStart()
    {
        // �t�F�[�h�Ƃ�����Ȃ���Q�[���V�[���ֈڍs����
        BGMManager.Instance.Stop();
        fadePostProcess.FadeDown(false, () => SceneManager.LoadScene("main"));
    }

    void ViewRankingBoard()
    {
        // �����L���O���X�V�A�擾
        PlayfabManager.instance.GetLeaderboard();
        Invoke("ViewRanking", 0.5f);


        ExpansionCanvasFadePanel(RankingFadePanel);
    }

    /// <summary>
    /// �����L���O��\��������
    /// </summary>
    void ViewRanking()
    {
        // �����L���O���擾
        var rankingData = PlayfabManager.instance.GetRankingBoard();
        if (rankingData == null) { return; }

        

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

    //�����L���O�p�l���̉��o
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
