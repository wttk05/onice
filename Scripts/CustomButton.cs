using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using KanKikuchi.AudioManager;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    // 01の処理
    public System.Action onClickCallback;
    [SerializeField] private CanvasGroup canvasGroup;


    void Start()
    {
        // クリックした時のアクションを保存
       //onClickCallback = () => GameManager.instance.GetCommand(transform.GetComponentInChildren<SuperTextMesh>().text);
    }

    // タップ
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        SEManager.Instance.Play(SEPath.CLICK);
        onClickCallback?.Invoke();
    }

     // タップダウン  
     public virtual void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject);
        canvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject);
    }

    // タップアップ  
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject); ;
        canvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Late, true).SetLink(gameObject); ;
    }
}
