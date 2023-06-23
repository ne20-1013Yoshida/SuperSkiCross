using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    // 関節
    [SerializeField] Transform armature; //骨格全体
    [SerializeField] Transform leftUpLeg; //左股関節
    [SerializeField] Transform leftLeg; //左膝
    [SerializeField] Transform leftFoot; //左足首
    [SerializeField] Transform rightUpLeg; //右股関節
    [SerializeField] Transform rightLeg; //右膝
    [SerializeField] Transform rightFoot; //右足首
    [SerializeField] Transform waist; //腰

    // スクリプト
    PlayerController playerController;
    AgentController agentController;

    // 共通
    float leanValue; //体の左右の傾きの度合い
    float leanValueTarget; //leanValueの変化値
    float leanSpeed; //傾く速さ
    float maxleanValue = 0.4f;
    float armatureY; //y軸の回転角度
    float armatureRangeZ = 30.0f; //z軸の回転範囲
    float legDefaultX = -45.0f; //両膝の回転角度の初期値
    float legRangeX = 40.0f; //両膝の回転可能範囲
    float upLegDefaultX = -40.0f; //両股関節の回転角度の初期値
    float footDefaultX = 90.0f; //両足首の回転角度の初期値
    float footRangeX = 2.0f; //両足首の回転可能範囲

    // プレイヤー用
    float maxStopTurn = 90.0f; //急停止時のy軸回転角度
    float stopTurnSpeed = 180.0f; //急停止時のy軸回転スピード
    float waistX; //腰のx軸回転角度
    float dashWaistLean = 30.0f; //加速時の腰の傾き
    float waistLeanSpeed = 60.0f; //腰が傾く速さ

    void Start()
    {
        if (gameObject.tag == "Player") playerController = GetComponent<PlayerController>();
        if (gameObject.tag == "Npc") agentController = GetComponent<AgentController>();
        leanValue = 0;
        leanValueTarget = 0;
        leanSpeed = 0;
        armatureY = 0;
        waistX = 0;
    }

    void Update()
    {
        // プレイヤーとNPCそれぞれの姿勢を左右に傾かせる方法
        if (gameObject.tag == "Player")
        {
            // leanValueTargetの値によってどちらに傾くかを決める
            if (playerController.isStopping == false) leanValueTarget = 0;
            if (playerController.canTurn == true)
            {
                if (Input.GetKey(KeyCode.D)) leanValueTarget = maxleanValue;
                if (Input.GetKey(KeyCode.A)) leanValueTarget = -maxleanValue;
            }
            leanSpeed = Time.deltaTime;
        }
        else if (gameObject.tag == "Npc")
        {
            // カーブする角度が大きいほど大きく傾く
            leanValueTarget = agentController.turnAngle;
            leanSpeed = Mathf.Abs(agentController.turnAngle) * Time.deltaTime;
            maxleanValue = agentController.maxTurnAngle;
        }
        leanValue = Mathf.MoveTowards(leanValue, leanValueTarget, leanSpeed); //なめらかに傾かせる
        float leanRate = leanValue / maxleanValue;
        // leanRateの値が正のときは右に、負のときは左に傾く
        // 値が大きいほど体は大きく傾き、足も大きく曲がる
        float armatureZ = -1 * leanRate * armatureRangeZ; //体の左右の傾き
        float legX = legDefaultX - Mathf.Abs(leanRate * legRangeX); //膝の回転
        float upLegX = upLegDefaultX - Mathf.Abs(leanRate * legRangeX); //股関節のx軸回転
        float footX = footDefaultX + Mathf.Abs(leanRate * footRangeX); //足首のx軸回転
        armature.localEulerAngles = new Vector3(armature.localEulerAngles.x, armatureY, armatureZ); //体の向きの制御
        if (0 <= leanRate)
        {
            // 右に傾いているため右足が曲がる
            rightUpLeg.localEulerAngles = new Vector3(upLegX, -2.5f, -177.0f);
            rightLeg.localEulerAngles = new Vector3(legX, 0, -2.7f);
            rightFoot.localEulerAngles = new Vector3(footX, 15.0f, 14.0f);
        }
        else
        {
            // 左に傾いているため左足が曲がる
            leftUpLeg.localEulerAngles = new Vector3(upLegX, 2.5f, 177.0f);
            leftLeg.localEulerAngles = new Vector3(legX, 0, 2.7f);
            leftFoot.localEulerAngles = new Vector3(footX, -15.0f, -14.0f);
        }

        // プレイヤーの減速・加速時の姿勢制御
        if (gameObject.tag == "Player")
        {
            float armatureTarget = 0; //体のy軸回転の変化値
            float waistLean = 0; //腰の傾き
            if (playerController.isStopping == true)
            {
                // 急停止時に体を回転させると同時に傾ける
                armatureTarget = maxStopTurn;
                leanValueTarget = maxleanValue;
            }
            else
            {
                // 加速時に腰を曲げる
                if (Input.GetKey(KeyCode.W)) waistLean = dashWaistLean;
            }
            armatureY = Mathf.MoveTowards(armatureY, armatureTarget, stopTurnSpeed * Time.deltaTime);
            waistX = Mathf.MoveTowards(waistX, waistLean, waistLeanSpeed * Time.deltaTime);
            waist.localEulerAngles = new Vector3(waistX, 0, 0);
        }
    }
}
