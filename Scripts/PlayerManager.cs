using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KanKikuchi.AudioManager;


public class PlayerManager : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    //　コライダを入れる配列
    //private Collider[] colliders;

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;

    [SerializeField] GameObject exEffectPrefab;
    [SerializeField] GameObject hitEffectPrefab;

    [SerializeField] GameObject frendBall;

    ParticleSystem hitEffect;

    //Joystick
    public VariableJoystick variableJoystick;
    Vector3 joyDirection;

    private Vector3 movement;

    // 停止しているか
    public bool isStop;

    // プレイヤーの状態
    public bool isJump;
    public bool isGround;
    public bool isULT;

    public bool isDead;
    public bool isGameStart;

    // 会話中か
    public bool isTalking;

    //
    public string StringDangerousColorType;

    // インスタンスを作成
    public static PlayerManager instance;
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
        // 変数の初期化＋コンポーネント取得
        Init();

        // 生成して消しておく
        hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        hitEffect.gameObject.SetActive(false);

        // 友達も生成
        frendBall = Instantiate(frendBall);
    }

    void Init()
    {
        // 変数初期化
        isStop = false;
        isTalking = false;
        isJump = false;
        isDead = false;
        isGameStart = false;
        //moveSpeed = 3;

        // 自身に設定されているColliderコンポーネントを全て取得
        //colliders = GetComponentsInChildren<Collider>();

        // コンポーネント取得
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //

    }
    public void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movement = new Vector3(movementVector.x, 0.0f, movementVector.y);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            isDead = true;
        }
#endif

        if (!isDead)
        {
            // アクション処理
            if (Mouse.current.leftButton.wasPressedThisFrame && !Setting.instance.GetActiveSettingPanel() && isGameStart && !GameManager.instance.isGameSet)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ULT"))
                {
                    animator.SetTrigger("ULT");
                    isULT = true;
                }
                else
                {
                    animator.ResetTrigger("ULT");
                }
            }

            //復帰
            ResetPosition(transform.position);
        }
        else
        {
            // 死亡処理
            StartCoroutine(Dead());
        }


    }

    /// <summary>
    /// はみ出した際に位置を復帰させる
    /// </summary>
    /// <param name="t"></param>
    private void ResetPosition(Vector3 t)
    {
        if (t.x >= 8f || t.x <= -8f)
        {
            transform.position = new Vector3(0, 8f, 0);
            //   
        }
        if (t.z >= 6f || t.z <= -6f)
        {
            transform.position = new Vector3(0, 8f, 0);
        }
    }

    void FixedUpdate()
    {
        // ジョイスティックの方向
        //joyDirection = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        Debug.Log(movement);
        Vector3  speed = movement * moveSpeed;

        if (!isStop && !Setting.instance.GetActiveSettingPanel() && isGameStart && !GameManager.instance.isGameSet)
        {
            rb.AddForce(speed, ForceMode.Force);
            transform.LookAt(transform.position + speed);
            animator.SetFloat("Speed", speed.magnitude);
        }
        else
        {
            speed = Vector3.zero;
            rb.AddForce(speed);
            animator.SetFloat("Speed", speed.magnitude);
        }

        if (isULT)
        {
            var v = transform.forward * 1000;// 向きを取得
            rb.AddForce(new Vector3(0, 300f, 0) + v, ForceMode.Impulse);
            isULT = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground")
        {

            if (collision.gameObject.tag != StringDangerousColorType)
            {
                hitEffect.gameObject.SetActive(true);
                hitEffect.transform.position = transform.position;
                hitEffect.Play();
                SEManager.Instance.Play(SEPath.BOYON);
            }
            else
            {
                isDead = true;
            }
        }

        if (collision.gameObject.tag == StringDangerousColorType)
        {

        }
    }

    public string ConvertColorType(GameManager.ColorType dct)
    {
        switch (dct)
        {
            case GameManager.ColorType.WHITE:
                StringDangerousColorType = "White";
                break;
            case GameManager.ColorType.BLUE:
                StringDangerousColorType = "Blue";
                break;
            case GameManager.ColorType.YELLOW:
                StringDangerousColorType = "Yellow";
                break;
            case GameManager.ColorType.RED:
                StringDangerousColorType = "Red";
                break;
        }

        return StringDangerousColorType;

    }

    IEnumerator Dead()
    {
        if (!GameManager.instance.isGameSet)
        {
            GameManager.instance.isGameSet = true;

            // 制限解除
            frendBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.None;

            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(0.2f);
            Time.timeScale = 1;

            //爆発
            Instantiate(exEffectPrefab, transform.position, Quaternion.identity);
            SEManager.Instance.Play(SEPath.EXPROSITON);
            Exprosion();

            yield return new WaitForSeconds(1f);
            // Destroy(gameObject); // 一番最後
        }
    }

    void Exprosion()
    {
        float radius = 100f;
        float power = 100f;

        // 着弾点を爆心地にする
        Vector3 explosionPos = transform.position;

        // 爆心地から『指定した半径内』にあるオブジェクトのcolliderを取得する。
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb)
            {
                // 爆風の発生
                rb.AddExplosionForce(power, transform.position, radius, 1.0f, ForceMode.VelocityChange);
            }
        }
    }

    public void GameStart()
    {
        isGameStart = true;
    }
}
