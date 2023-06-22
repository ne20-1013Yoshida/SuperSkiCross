using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour, IRankingDecider
{
    Transform[] waypoints;
    NavMeshAgent agent; //NavMeshAgentで動かす
    Rigidbody rigidbody;
    Transform myTransform;
    Transform armature; //子オブジェクトである骨格
    Vector3 beginningPosition;
    int waypointIndex;
    int myRanking;
    int finalWaypointIndex;
    public float turnAngle; //次のwaypointへのy軸回転角度
    public float maxTurnAngle;
    float jumpBoardForce = 60.0f; //ジャンプ台での前進させる力
    float rotationY; //y軸の回転角度
    float moveForce = 40.0f; //前進させる力
    float maxMoveSpeed = 25.0f; //最高前進速度
    bool agentSwitch; //agentの有効・無効
    bool onJumpBoard; //ジャンプ台に乗っているかどうか
 
    void Start()
    {
        waypoints = GameObject.Find("GameDirector").GetComponent<GameDirector>().waypoints;
        agent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        myTransform = GetComponent<Transform>();
        armature = myTransform.Find("Armature").GetComponent<Transform>();
        beginningPosition = myTransform.position;
        waypointIndex = 0;
        myRanking = 0;
        finalWaypointIndex = waypoints.Length - 1;
        maxTurnAngle = 50.0f;
        agentSwitch = true;
        onJumpBoard = false;
    }
 
    void Update()
    {
        // waypointの更新
        if (agent.enabled == true)
        {
            if (MeasureWaypointDistance() <= agent.stoppingDistance)
            {
                if (waypointIndex < finalWaypointIndex)
                {
                    waypointIndex++;
                }
                else agent.autoBraking = true;
            }
            agent.SetDestination(waypoints[waypointIndex].position);
        }
        agent.enabled = agentSwitch;
    }

    void FixedUpdate()
    {
        if (agent.enabled == false)
        {
            // 真っ直ぐ進む
            float currentSpeed = rigidbody.velocity.magnitude;
            if (currentSpeed <= maxMoveSpeed) rigidbody.AddForce(myTransform.forward * moveForce);
            myTransform.localEulerAngles = new Vector3(myTransform.eulerAngles.x, rotationY, 0);
        }

        if (onJumpBoard == true)
        {
            rigidbody.AddForce(myTransform.forward * jumpBoardForce);
        }
    }

    public float ReturnWaypointIndex()
    {
        return waypointIndex;
    }

    //次のwaypoinyまでの距離を返す
    public float MeasureWaypointDistance()
    {
        float distance = Vector3.Distance(myTransform.position, waypoints[waypointIndex].position);
        return distance;
    }

    public void DecideRanking(int ranking)
    {
        myRanking = ranking;
    }

    // スタート時の位置と向きに戻す
    public void OnClickRetryButton()
    {
        myTransform.position = beginningPosition;
        myTransform.eulerAngles = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        // waypointを通ったときの処理
        if (other.gameObject.tag == "Waypoint")
        {
            if (1 <= waypointIndex)
            {
                // 傾斜角度を計算して体の傾きを合わせる
                Vector3 waypointDelta = waypoints[waypointIndex].position - waypoints[waypointIndex - 1].position;
                Vector3 currentWaypointVector = new Vector3(waypointDelta.x, 0, waypointDelta.z);
                float rad = Mathf.Atan2(waypointDelta.y, currentWaypointVector.magnitude);
                float slopeAngle = rad * Mathf.Rad2Deg;
                armature.eulerAngles = new Vector3(-slopeAngle, armature.eulerAngles.y, armature.eulerAngles.z);
                // 次のwaypointへの回転角度を計算
                turnAngle = Vector3.SignedAngle(waypoints[waypointIndex - 1].forward, currentWaypointVector, Vector3.up);
            }
        }

        if (other.gameObject.tag == "AgentSwitch")
        {
            // NavMeshAgentをジャンプ台前で無効、後で有効化
            rotationY = myTransform.eulerAngles.y;
            agentSwitch = !agentSwitch;
        }

        if (other.gameObject.tag == "JumpBoard") onJumpBoard = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "JumpBoard") onJumpBoard = false;
    }
}
