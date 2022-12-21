using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;


public class FallObjects : MonoBehaviour
{
    void Update()
    {
        if(!GameManager.instance.isGameSet)
        {
            if (transform.position.x >= 13f || transform.position.x <= -13f)
            {
                this.gameObject.SetActive(false);
                GameManager.instance.AddScore();
                SEManager.Instance.Play(SEPath.KATEN);
            }
            if (transform.position.z >= 10f || transform.position.z <= -10f)
            {
                this.gameObject.SetActive(false);
                GameManager.instance.AddScore();
                SEManager.Instance.Play(SEPath.KATEN);
            }
        }
    }
}
