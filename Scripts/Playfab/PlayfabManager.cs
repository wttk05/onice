using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System.Text;
using PlayFab.ClientModels;

public class PlayfabManager : MonoBehaviour
{
    // �C���X�^���X���쐬
    public static PlayfabManager instance;

    List<PlayerLeaderboardEntry> ent = new List<PlayerLeaderboardEntry>();

    //�A�J�E���g���쐬���邩
    private bool _shouldCreateAccount;

    //���O�C�����Ɏg��ID
    private string _customID;



    private void Awake()
    {
        // �C���X�^���X����
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
        //���O�C���������s
        Login();
    }

    private void Login()
    {
        _customID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //���O�C������
    private void OnLoginSuccess(LoginResult result)
    {
        //�A�J�E���g���쐬���悤�Ƃ����̂ɁAID�����Ɏg���Ă��āA�o���Ȃ������ꍇ
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {_customID} �͊��Ɏg���Ă��܂��B");
            Login();//���O�C�����Ȃ���
            return;
        }

        //�A�J�E���g�쐬����ID��ۑ�
        if (result.NewlyCreated)
        {
            SaveCustomID();
        }
        Debug.Log($"PlayFab�̃��O�C���ɐ���\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\n�A�J�E���g���쐬������ : {result.NewlyCreated}");
    }

    //=================================================================================
    //�J�X�^��ID�̎擾
    //=================================================================================

    //ID��ۑ����鎞��KEY
    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";

    //ID���擾
    private string LoadCustomID()
    {
        //ID���擾
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //�ۑ�����Ă��Ȃ���ΐV�K����
        _shouldCreateAccount = string.IsNullOrEmpty(id);
        return _shouldCreateAccount ? GenerateCustomID() : id;
    }

    //ID�̕ۑ�
    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _customID);
    }

    //=================================================================================
    //�J�X�^��ID�̐���
    //=================================================================================

    //ID�Ɏg�p���镶��
    private static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyz";

    //ID�𐶐�����
    private string GenerateCustomID()
    {
        int idLength = 32;//ID�̒���
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        //�����_����ID�𐶐�
        for (int i = 0; i < idLength; i++)
        {
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        return stringBuilder.ToString();
    }

    //���O�C�����s
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    /// ----------------------------------------------------------------------------------------------------------------
    public void UpdateUserName(string name)
    {
        //���[�U�����w�肵�āAUpdateUserTitleDisplayNameRequest�̃C���X�^���X�𐶐�
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };

        //���[�U���̍X�V
        Debug.Log($"���[�U���̍X�V�J�n");
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateUserNameSuccess, OnUpdateUserNameFailure);
    }

    //���[�U���̍X�V����
    private void OnUpdateUserNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        //result.DisplayName�ɍX�V������̃��[�U���������Ă�
        Debug.Log($"���[�U���̍X�V���������܂��� : {result.DisplayName}");
    }

    //���[�U���̍X�V���s
    private void OnUpdateUserNameFailure(PlayFabError error)
    {
        Debug.LogError($"���[�U���̍X�V�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
    }
    /// ----------------------------------------------------------------------------------------------------------------
    public void UpdatePlayerStatistics(int score)
    {
        //UpdatePlayerStatisticsRequest�̃C���X�^���X�𐶐�
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>{
        new StatisticUpdate{
          StatisticName = "Ranking",   //�����L���O��(���v���)
          Value = score, //�X�R�A(int)
        }
      }
        };

        //���[�U���̍X�V
        Debug.Log($"�X�R�A(���v���)�̍X�V�J�n");
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdatePlayerStatisticsSuccess, OnUpdatePlayerStatisticsFailure);
    }

    //�X�R�A(���v���)�̍X�V����
    private void OnUpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log($"�X�R�A(���v���)�̍X�V���������܂���");
    }

    //�X�R�A(���v���)�̍X�V���s
    private void OnUpdatePlayerStatisticsFailure(PlayFabError error)
    {
        Debug.LogError($"�X�R�A(���v���)�X�V�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
    }



    public void GetLeaderboard()
    {
        //GetLeaderboardRequest�̃C���X�^���X�𐶐�
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Ranking", //�����L���O��(���v���)
            StartPosition = 0,                 //���ʈȍ~�̃����L���O���擾���邩
            MaxResultsCount = 10                  //�����L���O�f�[�^�������擾���邩(�ő�100)
        };

        //�����L���O(���[�_�[�{�[�h)���擾
        Debug.Log($"�����L���O(���[�_�[�{�[�h)�̎擾�J�n");
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);


    }

    //�����L���O(���[�_�[�{�[�h)�̎擾����
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log($"�����L���O(���[�_�[�{�[�h)�̎擾�ɐ������܂���");
        ent.Clear();

        //result.Leaderboard�Ɋe���ʂ̏��(PlayerLeaderboardEntry)�������Ă���
        // _rankingText.text = "";
        foreach (var entry in result.Leaderboard)
        {
            ent.Add(entry);
            Debug.Log("����" + entry.Position);
            Debug.Log("�X�R�A" + entry.StatValue);
            Debug.Log("���O" + entry.DisplayName);

            // _rankingText.text += $"\n���� : {entry.Position}, �X�R�A : {entry.StatValue}, ���O : {entry.DisplayName}, ID : {entry.PlayFabId}";
        }
    }


    public List<PlayerLeaderboardEntry> GetRankingBoard()
    {
        return ent;
    }

    //�����L���O(���[�_�[�{�[�h)�̎擾���s
    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError($"�����L���O(���[�_�[�{�[�h)�̎擾�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
    }


}
