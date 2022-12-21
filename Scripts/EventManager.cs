using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using UnityEngine.InputSystem;
using System;

public class EventManager : MonoBehaviour
{
    // ADVエンジン
    public AdvEngine AdvEngine { get { return advEngine; } }
    [SerializeField]
    protected AdvEngine advEngine;

    public bool IsPlaying { private set; get; }
    float defaultSpeed = -1;

    // インスタンスを作成
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



    //指定のラベルのシナリオを再生する
    public void JumpScenario(string label)
    {
        Debug.Log("シナリオ再生");
        StartCoroutine(JumpScenarioAsync(label, null));

    }

    //指定のラベルのシナリオを再生する
    //終了した時にonCompleteが呼ばれる
    public void JumpScenario(string label, Action onComplete)
    {
        Debug.Log("シナリオ再生2");

        if (ES3.KeyExists("MasterVolume"))
        {
            //シナリオ再生前に音を調整
            Setting.instance.SettingMasterVolume(ES3.Load<float>("MasterVolume"));// ここに入れないとしなりお内部の音声が反映されなかった
        }

        StartCoroutine(JumpScenarioAsync(label, onComplete));

    }

    //指定のラベルのシナリオを再生する
    //ラベルがなかった場合を想定
    public void JumpScenario(string label, Action onComplete, Action onFailed)
    {
        JumpScenario(label, null, onComplete, onFailed);

    }

    //指定のラベルのシナリオを再生する
    //ラベルがなかった場合を想定
    public void JumpScenario(string label, Action onStart, Action onComplete, Action onFailed)
    {
        if (string.IsNullOrEmpty(label))
        {
            if (onFailed != null) onFailed();
            Debug.LogErrorFormat("シナリオラベルが空です");
            return;
        }
        if (label[0] == '*')
        {
            label = label.Substring(1);
        }
        if (AdvEngine.DataManager.FindScenarioData(label) == null)
        {
            if (onFailed != null) onFailed();
            Debug.LogErrorFormat("{0}はまだロードされていないか、存在しないシナリオです", label);
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

    //シナリオの呼び出し以外に、
    //AdvEngineを操作する処理をまとめておくと、便利
    //何が必要かはプロジェクトによるので、場合によって増やしていく

    //以下、メッセージウィンドのテキスト表示速度を操作する処理のサンプル

    //メッセージウィンドのテキスト表示の速度を指定のスピードに
    public void ChangeMessageSpeed(float speed)
    {
        if (defaultSpeed < 0)
        {
            defaultSpeed = AdvEngine.Config.MessageSpeed;
        }
        AdvEngine.Config.MessageSpeed = speed;
    }
    //メッセージウィンドのテキスト表示の速度を元に戻す
    public void ResetMessageSpeed()
    {
        if (defaultSpeed >= 0)
        {
            AdvEngine.Config.MessageSpeed = defaultSpeed;
        }
    }


}
