using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject titlePanel;
    [SerializeField] GameObject describePanel;

    void Start()
    {
        titlePanel.SetActive(true);
        describePanel.SetActive(false);
    }

    public void OnClickStartButton()
    {
        //ゲームを始める
        SceneManager.LoadScene("gameScene");
    }

    public void OnClickDescribeButton()
    {
        //ゲーム説明を見る
        titlePanel.SetActive(false);
        describePanel.SetActive(true);
    }

    public void OnClickBackToTitleButton()
    {
        // ゲーム説明を閉じる
        titlePanel.SetActive(true);
        describePanel.SetActive(false);
    }
}
