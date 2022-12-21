using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjects : MonoBehaviour
{
    [SerializeField] GameObject BallPrafab;

    IEnumerator CreateBall()
    {
        while(!GameManager.instance.isGameSet)
        {
            var r = Random.Range(-4.5f,5.5f);
            var r2 = Random.Range(-4.5f, 5.5f);
            int r3 = (int)Random.Range(1, 101f);

            var ball = Instantiate(BallPrafab, new Vector3(r, 12.0f, r2),Quaternion.identity);
            ball.GetComponent<Renderer>().material.color = SelectColor(r3,ball);
            ball.GetComponent<Rigidbody>().mass = r3;


            var r4 = Random.Range(0.5f, 1.5f);
            yield return new WaitForSeconds(r4);
        }
    }

    public void StartCreateBall()
    {
        StartCoroutine(CreateBall());
    }

    Color SelectColor(float r,GameObject ball)
    {
        // ì‡ïîèIóπéûä‘Ç™óàÇΩÇÁéÄÇ êFÇÃÇ›ç~ÇÁÇπÇÈ
        if (GameManager.instance.isDeathTime)
        {
            ball.gameObject.tag = PlayerManager.instance.StringDangerousColorType;
            switch (PlayerManager.instance.StringDangerousColorType)
            {
                case "White":
                    return Color.white;
                case "Blue":
                    return Color.blue;
                case "Yellow":
                    return Color.yellow;
                case "Red":
                    return Color.red;
            }
        }

        if (r <= 25)
        {
            ball.gameObject.tag = "White";
            return Color.white;
        }
        else if(r >= 26 && r <= 49)
        {
            ball.gameObject.tag = "Blue";
            return Color.blue;
        }
        else if(r >= 50 && r<= 74)
        {
            ball.gameObject.tag = "Yellow";
            return Color.yellow;
        }
        else if (r >= 75 && r <= 100)
        {
            ball.gameObject.tag = "Red";
            return Color.red;
        }

        return Color.magenta;
    }
}
