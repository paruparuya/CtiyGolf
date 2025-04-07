using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [Header("スコア関連")]
    public TMP_Text scoreText;
    private int score = 0;

    [Header("UIボタン制御")]
    public GameObject uiButton; // Canvas上のボタン
    public BollController bollController; // SpawnNewCanを呼ぶため

    private CanCollisionHandler currentHandler;
    public UnityChanController unityChanController;

    void Start()
    {
        UpdateScoreText();

        if (uiButton != null)
        {
            uiButton.SetActive(false);
            uiButton.GetComponent<Button>().onClick.AddListener(OnUIButtonClicked);
        }
    }

    void Update()
    {
        if (bollController == null) return;

        GameObject currentCan = GameObject.FindWithTag("Can"); // ← タグで取得
        if (currentCan != null)
        {
            var handler = currentCan.GetComponent<CanCollisionHandler>();
            if (handler != null && handler != currentHandler)
            {
                if (currentHandler != null)
                    currentHandler.OnCanTriggered -= HandleCanTriggered;

                currentHandler = handler;
                currentHandler.OnCanTriggered += HandleCanTriggered;
            }
        }
    }

    void HandleCanTriggered(string tag)
    {
        Debug.Log("缶が" + tag + "に当たりました。UIボタンを表示します");
        if (uiButton != null)
            uiButton.SetActive(true);
    }

    void OnUIButtonClicked()
    {
        if (uiButton != null)
            uiButton.SetActive(false);

        if (bollController != null)
            bollController.SendMessage("SpawnNewCan");

        if (unityChanController != null)
            unityChanController.ResetAnimationState();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void SubtractScore(int points)
    {
        score -= points;
        if (score < 0) score = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}