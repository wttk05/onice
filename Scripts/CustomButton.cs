using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using KanKikuchi.AudioManager;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    // 01�̏���
    public System.Action onClickCallback;
    [SerializeField] private CanvasGroup canvasGroup;


    void Start()
    {
        // �N���b�N�������̃A�N�V������ۑ�
       //onClickCallback = () => GameManager.instance.GetCommand(transform.GetComponentInChildren<SuperTextMesh>().text);
    }

    // �^�b�v
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        SEManager.Instance.Play(SEPath.CLICK);
        onClickCallback?.Invoke();
    }

     // �^�b�v�_�E��  
     public virtual void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject);
        canvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject);
    }

    // �^�b�v�A�b�v  
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject); ;
        canvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject); ;
    }
}
