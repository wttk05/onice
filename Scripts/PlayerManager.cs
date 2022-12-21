using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KanKikuchi.AudioManager;


public class PlayerManager : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    //�@�R���C�_������z��
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

    // ��~���Ă��邩
    public bool isStop;

    // �v���C���[�̏��
    public bool isJump;
    public bool isGround;
    public bool isULT;

    public bool isDead;
    public bool isGameStart;

    // ��b����
    public bool isTalking;

    //
    public string StringDangerousColorType;

    // �C���X�^���X���쐬
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
        // �ϐ��̏������{�R���|�[�l���g�擾
        Init();

        // �������ď����Ă���
        hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        hitEffect.gameObject.SetActive(false);

        // �F�B������
        frendBall = Instantiate(frendBall);
    }

    void Init()
    {
        // �ϐ�������
        isStop = false;
        isTalking = false;
        isJump = false;
        isDead = false;
        isGameStart = false;
        //moveSpeed = 3;

        // ���g�ɐݒ肳��Ă���Collider�R���|�[�l���g��S�Ď擾
        //colliders = GetComponentsInChildren<Collider>();

        // �R���|�[�l���g�擾
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
            // �A�N�V��������
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

            //���A
            ResetPosition(transform.position);
        }
        else
        {
            // ���S����
            StartCoroutine(Dead());
        }


    }

    /// <summary>
    /// �͂ݏo�����ۂɈʒu�𕜋A������
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
        // �W���C�X�e�B�b�N�̕���
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
            var v = transform.forward * 1000;// �������擾
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

            // ��������
            frendBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.None;

            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(0.2f);
            Time.timeScale = 1;

            //����
            Instantiate(exEffectPrefab, transform.position, Quaternion.identity);
            SEManager.Instance.Play(SEPath.EXPROSITON);
            Exprosion();

            yield return new WaitForSeconds(1f);
            // Destroy(gameObject); // ��ԍŌ�
        }
    }

    void Exprosion()
    {
        float radius = 100f;
        float power = 100f;

        // ���e�_�𔚐S�n�ɂ���
        Vector3 explosionPos = transform.position;

        // ���S�n����w�w�肵�����a���x�ɂ���I�u�W�F�N�g��collider���擾����B
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb)
            {
                // �����̔���
                rb.AddExplosionForce(power, transform.position, radius, 1.0f, ForceMode.VelocityChange);
            }
        }
    }

    public void GameStart()
    {
        isGameStart = true;
    }
}
