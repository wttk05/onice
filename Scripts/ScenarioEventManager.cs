using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utage;

public class ScenarioEventManager : MonoBehaviour
{
    // ADVエンジン
    public AdvEngine AdvEngine { get { return advEngine; } }
    [SerializeField]
    protected AdvEngine advEngine;

    private void Start()
    {
    }

    /*<summary>
 * UtageのSendMessageから送られてきた情報を処理するところ
 *</summary> */
    void OnDoCommand(AdvCommandSendMessage command)
    {
        switch (command.Name)
        {
            case "MoveCamera":
                Debug.Log(command);
                GameManager.instance.MoveCamera();
                break;
        }
    }
}
