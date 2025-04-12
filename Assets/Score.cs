using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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

    public TMP_Text finalScoreText; // 最終スコア表示用UI（別の位置に配置）
    private bool isGameOver = false;
    private int currentActionCount = 0; // アクションカウント
    [Header("ゲーム終了演出")]
    [SerializeField] private float finalScoreDelay = 1.0f;
    [Header("一時スコア表示用")]
    public TMP_Text temporaryScoreText;
    [Header("リセットボタン")]
    public GameObject resetButton; //リセットボタン

    private int triggerCount = 0; // 缶が当たった回数
    private const int MAX_TRIGGERS = 5;

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
        if (bollController == null)
        {
            return;
        }

        GameObject currentCan = GameObject.FindWithTag("Can"); // ← タグで取得
        if (currentCan != null)
        {
            var handler = currentCan.GetComponent<CanCollisionHandler>();
            if (handler != null && handler != currentHandler)
            {
                if (currentHandler != null)
                {
                    currentHandler.OnCanTriggered -= HandleCanTriggered;
                }

                currentHandler = handler;
                currentHandler.OnCanTriggered += HandleCanTriggered;
            }
        }
    }

    void HandleCanTriggered(string tag)
    {
        Debug.Log("缶が" + tag + "に当たりました。UIボタンを表示します");
        int points = GetPointsFromTag(tag); // タグから点数取得
        StartCoroutine(ShowTemporaryScore(points)); // 1秒後に表示！

        triggerCount++;

        if (triggerCount >= MAX_TRIGGERS)
        {
            isGameOver = true;
            StartCoroutine(DelayedShowFinalScore());
            return;
        }

        StartCoroutine(ShowUIButtonAfterDelay(2f));
    }

    void OnUIButtonClicked()
    {
        if (temporaryScoreText != null)
        {
            temporaryScoreText.gameObject.SetActive(false); // ← ここで非表示に！
        }

        if (currentActionCount >= 5)
        {
            isGameOver = true;
            ShowFinalScore();
            return;
        }

        if (uiButton != null)
        {
            uiButton.SetActive(false);
            uiButton.GetComponent<Button>().interactable = false;
        }

        currentActionCount++; // ← 新しく自分で用意したカウント変数
        Debug.Log("現在のアクション数: " + currentActionCount);

        // 5回終わったら終了フラグを立てる
        if (currentActionCount >= 5)
        {

            isGameOver = true;
            Debug.Log("ゲーム終了フラグを立てました");
            ShowFinalScore(); // ★ この時点でスコアを表示！
            return; // もうSpawnしない
        }

        if (bollController != null)
        { 

            bollController.SpawnNewCan();

        }
        if (unityChanController != null)
        {
            unityChanController.ResetAnimationState();
        }
        
    }

    void ShowFinalScore()
    {
        Debug.Log("最終スコアを表示します！");
        Debug.Log("score の値: " + score); // ←★スコアの実数値を確認

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score " + score;
            finalScoreText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("finalScoreText がアサインされていません！");
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void SubtractScore(int points)
    {
        score -= points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    private IEnumerator DelayedShowFinalScore()
    {
        yield return new WaitForSeconds(finalScoreDelay);
        ShowFinalScore();

        yield return new WaitForSeconds(2f);
        if (resetButton != null)
        {
            resetButton.SetActive(true);
        }
    }

    private IEnumerator ShowUIButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (uiButton != null)
        {
            uiButton.SetActive(true);
            uiButton.GetComponent<Button>().interactable = true;
        }
    }

    private IEnumerator ShowTemporaryScore(int points)
    {
        yield return new WaitForSeconds(1f); // ← 1秒待ってから表示

        if (temporaryScoreText != null)
        {
            temporaryScoreText.text = points >= 0 ? $"+{points}" : points.ToString();
            temporaryScoreText.gameObject.SetActive(true);
        }
    }

    private int GetPointsFromTag(string tag)
    {
        switch (tag)
        {
            case "Trash": return 10;
            case "Box": return 50;
            case "Window": return -20;
            case "Ground": return -5;
            default: return 0;
        }
    }

    public void OnResetButtonClicked()
    {
        Scene currentScene = SceneManager.GetActiveScene(); // 現在のシーン取得
        SceneManager.LoadScene(currentScene.name); // 同じシーンを再読み込み！
    }
}