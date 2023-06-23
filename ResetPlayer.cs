using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPlayer : MonoBehaviour
{
    List<Transform> resetPoints; //リセット地点
    Transform myTransform;
    PlayerController playerController;
    [SerializeField] Slider slider; //コースアウト時に溜まるゲージ
    [SerializeField] GameObject panel;
    int collisionCounter;
    float gainSpeed = 0.5f; //ゲージが溜まる速さ
    bool isCourseOut = false;

    void Start()
    {
        resetPoints = new List<Transform>();
        myTransform = GetComponent<Transform>();
        playerController = GetComponent<PlayerController>();
        panel.SetActive(false);
        collisionCounter = 0;
    }

    void Update()
    {
        if (isCourseOut == true)
        {
            panel.SetActive(true);
            slider.value += gainSpeed * Time.deltaTime; //ゲージの値を増やす
            if (1.0f <= slider.value)
            {
                // プレイヤーの位置と向きをリセット
                myTransform.position = resetPoints[resetPoints.Count - 1].position;
                playerController.ResetCondition(resetPoints[resetPoints.Count - 1].eulerAngles.y);
                isCourseOut = false;
            }
        }
        else
        {
            panel.SetActive(false);
            slider.value = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // リセット地点更新
        if (other.gameObject.tag == "Waypoint")
        {
            collisionCounter++;
            if (collisionCounter % 3 == 0) resetPoints.Add(other.GetComponent<Transform>());
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Terrain") isCourseOut = true; //Terrain上に行くとコースアウト
        if (other.gameObject.tag == "Course") isCourseOut = false;
    }
}
