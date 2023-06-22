using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    public Transform[] waypoints; //コースの屈折点をスタートから順に格納
    [SerializeField] GameObject player;
    [SerializeField] GameObject npc1;
    [SerializeField] GameObject npc2;
    [SerializeField] GameObject npc3;
    [SerializeField] GameObject beginningPanel; //3カウントを表示するパネル
    [SerializeField] GameObject rankingPanel; //順位を表示するパネル
    [SerializeField] GameObject goalPanel; //ゴール時に表示するパネル
    [SerializeField] Text countText;
    [SerializeField] Text rankingText;
    [SerializeField] Text finalRankingText;
    List<IRankingDecider> rankingDeciderList = new List<IRankingDecider>(); //インターフェースのリスト
    PlayerController playerController;
    int finalWaypointIndex;
    float beginningTime; //ゲーム開始時の時刻
    float countTime;
    float delayTime = 3.0f;
    
    void Start()
    {
        rankingDeciderList.Add(player.GetComponent<IRankingDecider>());
        rankingDeciderList.Add(npc1.GetComponent<IRankingDecider>());
        rankingDeciderList.Add(npc2.GetComponent<IRankingDecider>());
        rankingDeciderList.Add(npc3.GetComponent<IRankingDecider>());
        beginningPanel.SetActive(true);
        rankingPanel.SetActive(true);
        goalPanel.SetActive(false);
        playerController = player.GetComponent<PlayerController>();
        finalWaypointIndex = waypoints.Length - 1;
        beginningTime = Time.realtimeSinceStartup; //ゲーム外の時間
        countTime = 3.0f;
        Time.timeScale = 0; //ゲーム内時間を停止
    }

    void Update()
    {
        //３秒のカウントダウン後にゲームスタート
        if (0 <= countTime)
        {
            float seconds = Time.realtimeSinceStartup - beginningTime - delayTime;
            if (0 <= seconds) countTime = 3.0f - seconds;
            countText.text = ((int)countTime).ToString();
        }
        else
        {
            beginningPanel.SetActive(false);
            Time.timeScale = 1;
        }

        if (playerController.ReturnWaypointIndex() < finalWaypointIndex)
        {
            // 通り過ぎたwaypointの数が多い順にする
            // waypointの数が同じ場合は次のwaypointまでの距離が近い順にする
            var currentOrder = rankingDeciderList.OrderByDescending(x => x.ReturnWaypointIndex()).ThenBy(x => x.MeasureWaypointDistance());
            int ranking = 0;
            foreach (var rankingDecider in currentOrder)
            {
                ranking++;
                rankingDecider.DecideRanking(ranking);
            }
            rankingText.text = playerController.ReturnRanking().ToString() + "/4";
        }
        else
        {
            // ゴールした時
            rankingPanel.SetActive(false);
            goalPanel.SetActive(true);
            finalRankingText.text = playerController.ReturnRanking().ToString() + "位";
        }
    }

    public void OnClickRetryButton()
    {
        SceneManager.LoadScene("gameScene");
    }

    public void OnClickTitleButton()
    {
        SceneManager.LoadScene("startScene");
    }
}
