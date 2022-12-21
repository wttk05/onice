using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject player;
    private Vector3 offset;

    /// <summary>
    /// x 0 y 2.5 z-2
    /// x30 y 0 z 0
    /// </summary>

    Vector3 cameraPos = new Vector3(0,14.7f,-7.8f);
    Quaternion cameraRotate = new Quaternion(50f,0,0,0);

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var x  = player.transform.position.x + offset.x;
        var z = player.transform.position.z + offset.z;

        transform.position = new Vector3(x, offset.y, z);
    }
}
