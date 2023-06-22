using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    //関節
    [SerializeField] Transform armature; //骨格全体
    [SerializeField] Transform leftUpLeg; //左股関節
    [SerializeField] Transform leftLeg; //左膝
    [SerializeField] Transform leftFoot; //左足首
    [SerializeField] Transform rightUpLeg; //右股関節
    [SerializeField] Transform rightLeg; //右膝
    [SerializeField] Transform rightFoot; //右足首
    [SerializeField] Transform waist; //腰

    PlayerController playerController;
    AgentController agentController;

    // 共通
    float leanValue; //体の左右の傾きの度合い
    float leanValueTerget; //leanValueの変化値
    float leanSpeed; //傾く速さ
    float maxleanValue = 0.4f;
    float armatureY; //y軸の回転角度
    float armatureRangeZ = 30.0f; //z軸の回転範囲
    float legDefaultX = -45.0f; //両膝の回転角度の初期値
    float legRangeX = 40.0f; //両膝の回転可能範囲
    float upLegDefaultX = -40.0f;
    float footDefaultX = 90.0f;
    float footRangeX = 2.0f;
    // プレイヤー用
    float maxStopTurn = 90.0f; //急停止時のy軸回転角度
    float stopTurnSpeed = 180.0f;
    float waistX;
    float dashWaistLean = 30.0f; //加速時の腰の傾き
    float waistLeanSpeed = 60.0f;

    void Start()
    {
        if (gameObject.tag == "Player") playerController = GetComponent<PlayerController>();
        if (gameObject.tag == "Npc") agentController = GetComponent<AgentController>();
        leanValue = 0;
        leanValueTerget = 0;
        leanSpeed = 0;
        armatureY = 0;
        waistX = 0;
    }

    void Update()
    {
        // 左右に姿勢を傾かせる方法
        if (gameObject.tag == "Player")
        {
            if (playerController.isStopping == false) leanValueTerget = 0;
            if (playerController.isTurning == true)
            {
                if (Input.GetKey(KeyCode.D)) leanValueTerget = maxleanValue;
                if (Input.GetKey(KeyCode.A)) leanValueTerget = -maxleanValue;
            }
            leanSpeed = Time.deltaTime;
        }
        else if (gameObject.tag == "Npc")
        {
            // カーブする角度が大きいほど大きく傾く
            leanValueTerget = agentController.turnAngle;
            leanSpeed = Mathf.Abs(agentController.turnAngle) * Time.deltaTime;
            maxleanValue = agentController.maxTurnAngle;
        }
        leanValue = Mathf.MoveTowards(leanValue, leanValueTerget, leanSpeed);
        float leanRate = leanValue / maxleanValue; //傾きの割合

        // 各関節の制御
        float armatureZ = -1 * leanRate * armatureRangeZ;
        float legX = legDefaultX - Mathf.Abs(leanRate * legRangeX);
        float upLegX = upLegDefaultX - Mathf.Abs(leanRate * legRangeX);
        float footX = footDefaultX + Mathf.Abs(leanRate * footRangeX);
        armature.localEulerAngles = new Vector3(armature.localEulerAngles.x, armatureY, armatureZ);
        if (0 <= leanRate)
        {
            // 右足の制御
            rightUpLeg.localEulerAngles = new Vector3(upLegX, -2.5f, -177.0f);
            rightLeg.localEulerAngles = new Vector3(legX, 0, -2.7f);
            rightFoot.localEulerAngles = new Vector3(footX, 15.0f, 14.0f);
        }
        else
        {
            // 左足の制御
            leftUpLeg.localEulerAngles = new Vector3(upLegX, 2.5f, 177.0f);
            leftLeg.localEulerAngles = new Vector3(legX, 0, 2.7f);
            leftFoot.localEulerAngles = new Vector3(footX, -15.0f, -14.0f);
        }


        // プレイヤーの減速・加速時の姿勢制御
        if (gameObject.tag == "Player")
        {
            float stopTurn = 0;
            float waistLean = 0;
            if (playerController.isStopping == true)
            {
                // 急停止時
                stopTurn = maxStopTurn;
                leanValueTerget = maxleanValue;
            }
            else
            {
                // 加速時
                if (Input.GetKey(KeyCode.W)) waistLean = dashWaistLean;
            }
            armatureY = Mathf.MoveTowards(armatureY, stopTurn, stopTurnSpeed * Time.deltaTime);
            waistX = Mathf.MoveTowards(waistX, waistLean, waistLeanSpeed * Time.deltaTime);
            waist.localEulerAngles = new Vector3(waistX, 0, 0);
        }
    }
}
