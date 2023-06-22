using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform playerTransform;
    Transform myTransform;
    Vector3 beginningPosition;
    Vector3 offset; //プレイヤーとカメラの間の距離

    void Start()
    {
        playerTransform = GameObject.Find("player").GetComponent<Transform>();
        myTransform = GetComponent<Transform>();
        beginningPosition = myTransform.position;
        offset = myTransform.position - playerTransform.position;
    }

    void LateUpdate()
    {
        // 一定距離を保ってプレイヤーに追従
        myTransform.position = playerTransform.position + offset;
        myTransform.eulerAngles = new Vector3(0, playerTransform.eulerAngles.y, 0);
    }

    public void OnClickRetryButton()
    {
        myTransform.position = beginningPosition;
        myTransform.eulerAngles = Vector3.zero;
    }
}
