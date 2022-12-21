using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using KanKikuchi.AudioManager;

public class TitlePlayerManager : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    [SerializeField] GameObject Field;
    [SerializeField] GameObject friendBall;


    void Start()
    {
        // �R���|�[�l���g�擾
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        StartCoroutine(TitlePlayerMove());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TitlePlayerMove()
    {
        BGMManager.Instance.Play(BGMPath.TITLE);

        // �������ɑ���
        transform.transform.DOMove(new Vector3(0, 5.7f, 0), 3f).SetEase(Ease.Linear).SetLink(gameObject).OnComplete(() => animator.SetFloat("Speed", 0));
        transform.LookAt(new Vector3(0, 5.7f, 0));
        animator.SetFloat("Speed", 1);

        yield return new WaitForSeconds(2.7f);

        // �t�B�[���h���W�J�����
        Field.transform.DOMove(new Vector3(-5, 6, -5), 0.4f).SetEase(Ease.OutQuint).SetLink(gameObject);

        yield return new WaitForSeconds(0.2f);

        // �r�b�N���}�[�N���o������
        yield return new WaitForSeconds(0.4f);

        // �A�N�V�������N����
        animator.SetTrigger("ULT");

        //
        yield return new WaitForSeconds(0.4f);

        var friendBallPos = transform.position + new Vector3(2f, 6f, -2f);
        var fB = Instantiate(friendBall, friendBallPos, Quaternion.identity);
        fB.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        fB.GetComponent<Rigidbody>().AddForce(new Vector3(-10000f, 0f, 3000f));
    }
}
