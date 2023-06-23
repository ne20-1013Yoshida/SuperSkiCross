using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IRankingDecider
{
    Transform[] waypoints; //コースの屈折点をスタートから順に格納
    Rigidbody rigidbody;
    Transform myTransform;
    JointController jointController;
    Vector3 beginningPosition; //スタート地点
    int waypointIndex;
    int collisionCounter;
    int myRanking; //順位
    int finalWaypointIndex;
    float rotationX; //x軸の回転角度
    float rotationY; //y軸の回転角度
    float turnSpeed = 50.0f; //曲がる速さ
    float turnFriction = 1.5f; //曲がる時の摩擦
    float moveForce = 20.0f; //前進させる力
    float maxMoveSpeed; //最高前進速度
    float normalSpeed = 19.0f; //最高前進速度(通常時)
    float dashSpeed = 23.0f; //最高前進速度(加速時)
    float canMoveAngle = 90.0f; //コースの進行方向に対してこの角度異常曲がると加速できない
    float jumpBoardForce = 50.0f; //ジャンプ台での前進させる力
    public bool isStopping; //急停止しているかどうか
    public bool canTurn; //曲がっているかどうか
    bool onCourse; //コース上にいるかどうか
    bool onJumpBoard; //ジャンプ台に乗っているかどうか

    void Start()
    {
        waypoints = GameObject.Find("GameDirector").GetComponent<GameDirector>().waypoints;
        rigidbody = GetComponent<Rigidbody>();
        myTransform = GetComponent<Transform>();
        jointController = GetComponent<JointController>();
        beginningPosition = myTransform.position;
        waypointIndex = 0;
        collisionCounter = 0;
        myRanking = 0;
        finalWaypointIndex = waypoints.Length - 1;
        rotationX = 0;
        rotationY = 0;
        onCourse = false;
        onJumpBoard = false;
    }

    void Update()
    {
        if (onCourse == true)
        {
            rotationX = myTransform.eulerAngles.x;
            canTurn = false;
            if (isStopping == false)
            {
                // 左右のカーブ
                rigidbody.drag = 0;
                if (maxMoveSpeed == normalSpeed) //加速していない時
                {
                    int key = 0; //どちらに曲がるか
                    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
                    {
                        canTurn = true;
                        rigidbody.drag = turnFriction;
                        if (Input.GetKey(KeyCode.D)) key = 1;
                        if (Input.GetKey(KeyCode.A)) key = -1;
                    }
                    rotationY += key * Time.deltaTime * turnSpeed;
                }
            }
            else
            {
                // 減速する
                rigidbody.drag = 2;
            }
            if (Input.GetKey(KeyCode.S) || finalWaypointIndex <= waypointIndex) isStopping = true;
            else isStopping = false;

            // 加速時はスピード上限を変更
            if (Input.GetKey(KeyCode.W)) maxMoveSpeed = dashSpeed;
            else maxMoveSpeed = normalSpeed;
        }
        // 回転制限
        myTransform.eulerAngles = new Vector3(rotationX, rotationY, 0);
    }

    void FixedUpdate()
    {
        if (onCourse == true)
        {
            if (isStopping == false)
            {
                // コースの進行方向を計算
                Vector3 waypointDelta = myTransform.forward;
                if (1 <= waypointIndex) waypointDelta = waypoints[waypointIndex].position - waypoints[waypointIndex - 1].position;
                Vector3 currentWaypointVector = new Vector3(waypointDelta.x, 0, waypointDelta.z);
                float myWayAngle = Vector3.SignedAngle(myTransform.forward, currentWaypointVector, Vector3.up);
                // 逆走していなければ加速
                if (Mathf.Abs(myWayAngle) <= canMoveAngle)
                {
                    float currentSpeed = rigidbody.velocity.magnitude;
                    if (currentSpeed < maxMoveSpeed) rigidbody.AddForce(myTransform.forward * moveForce);
                }
            }

            if (onJumpBoard == true)
            {
                // ジャンプ台で加速させる
                rigidbody.AddForce(myTransform.forward * jumpBoardForce);
            }
        }
    }

    // コースアウトしたときに位置と向きをリセットする
    public void ResetCondition(float resetY)
    {
        waypointIndex--;
        rigidbody.velocity = Vector3.zero;
        rotationX = 0;
        rotationY = resetY;
    }

    public float ReturnWaypointIndex()
    {
        return waypointIndex;
    }

    //次のwaypointまでの距離を返す
    public float MeasureWaypointDistance()
    {
        float distance = Vector3.Distance(myTransform.position, waypoints[waypointIndex].position);
        return distance;
    }

    public void DecideRanking(int ranking)
    {
        myRanking = ranking;
    }

    // 順位を表示するためにGameDirector.csから呼び出される
    public int ReturnRanking()
    {
        return myRanking;
    }
    
    // スタート時の位置と向きに戻す
    public void OnClickRetryButton()
    {
        myTransform.position = beginningPosition;
        myTransform.eulerAngles = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Waypoint" && waypointIndex < finalWaypointIndex)
        {
            // コライダーを3つ持つため、3回の衝突判定に1回のみwaypointを更新する
            collisionCounter++;
            if (collisionCounter % 3 == 0) waypointIndex++;
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Course") onCourse = true;
        if (other.gameObject.tag == "Terrain") onCourse = false; //terrainに触れたらコースアウト
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "JumpBoard") onJumpBoard = true;
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Course") onCourse = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "JumpBoard") onJumpBoard = false;
    }
}
