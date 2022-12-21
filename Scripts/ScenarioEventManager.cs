using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utage;

public class ScenarioEventManager : MonoBehaviour
{
    // ADVƒGƒ“ƒWƒ“
    public AdvEngine AdvEngine { get { return advEngine; } }
    [SerializeField]
    protected AdvEngine advEngine;

    private void Start()
    {
    }

    /*<summary>
 * Utage‚ÌSendMessage‚©‚ç‘—‚ç‚ê‚Ä‚«‚½î•ñ‚ğˆ—‚·‚é‚Æ‚±‚ë
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
