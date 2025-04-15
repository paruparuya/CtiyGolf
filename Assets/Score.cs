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
    public GameObject uiButton;
    public BollController bollController;
    public UnityChanController unityChanController;

    public TMP_Text finalScoreText;
    private bool isGameOver = false;

    [Header("一時スコア表示用")]
    public TMP_Text temporaryScoreText;

    [Header("リセットボタン")]
    public GameObject resetButton;

    [SerializeField] private float finalScoreDelay = 1.0f;
    private CanCollisionHandler currentHandler;

    void Start()
    {
        Debug.Log("現在のシーン名は: " + SceneManager.GetActiveScene().name);
        score = GameManager.Instance.totalScore;
        UpdateScoreText();
        if (uiButton != null)
        {
            uiButton.SetActive(false);
            uiButton.GetComponent<Button>().onClick.AddListener(OnUIButtonClicked);
        }

        InvokeRepeating("TryRegisterHandler", 0.5f, 1f); // 追加

        if (SceneManager.GetActiveScene().name == "SampleScene 2" ||
            SceneManager.GetActiveScene().name == "SampleScene 1")
        {
            if (scoreText != null)
            {
                Debug.Log("scoreText は設定されています");
                scoreText.color = Color.white;
            }
            else
            {
                Debug.LogWarning("scoreText が設定されていません！");
            }

            if (finalScoreText != null)
            {
                Debug.Log("finalText は設定されています");
                finalScoreText.color = Color.white;
            }
            else
            {
                Debug.LogWarning("finalText が設定されていません！");
            }

            if (temporaryScoreText != null)
            {
                Debug.Log("temporaryText は設定されています");
                temporaryScoreText.color = Color.white;
            }
            else
            {
                Debug.LogWarning("temporaryText が設定されていません！");
            }
        }

        if (uiButton != null)
        {
            uiButton.SetActive(false);
            uiButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnUIButtonClicked);
        }
    }

    void Update()
    {
        if (bollController == null) return;

        GameObject currentCan = GameObject.FindWithTag("Can");
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

    public void AddScore(int points)
    {
        score += points; // 自分のスコアも加算
        GameManager.Instance.AddScore(points); // GameManager にも記録
        UpdateScoreText(); // UI更新！
    }

    public void HandleCanTriggered(string tag)
    {
        Debug.Log("缶が" + tag + "に当たりました。UIボタンを表示します");
        int points = GetPointsFromTag(tag);
        AddScore(points);
        StartCoroutine(ShowTemporaryScore(points));
        GameManager.Instance.AddHitCountOnly();

        if (GameManager.Instance.IsGameOver())
        {
            // ★スポーンボタン非表示にする（安全のため）
            if (uiButton != null)
            {
                uiButton.SetActive(false);
            }

            StartCoroutine(DelayedShowFinalScore()); // ← 最終スコアとリセットボタン表示
            return;
        }


        StartCoroutine(ShowUIButtonAfterDelay(2f));
    }

    void OnUIButtonClicked()
    {
        if (temporaryScoreText != null)
        {
            temporaryScoreText.gameObject.SetActive(false);
        }

        

        // ★ この時点で15回なら終了処理
        if (GameManager.Instance.totalHitCount >= 15)
        {
            ShowFinalScore();
            return;
        }

        // ★ 5回 or 10回ならシーンを移行（ここがメイン修正）
        if (GameManager.Instance.IsTransitionPoint())
        {
            GameManager.Instance.MoveToNextSceneIfNeeded();
            return; // 次のシーンへ移動するので以降の処理不要
        }

        // 通常時の処理（新しい缶を出す）
        if (uiButton != null)
        {
            uiButton.SetActive(false);
            uiButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }

        if (bollController != null)
            bollController.SpawnNewCan();

        if (unityChanController != null)
            unityChanController.ResetAnimationState();
    }

    void ShowFinalScore()
    {
        if (scoreText != null) scoreText.gameObject.SetActive(false);

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score " + GameManager.Instance.totalScore;
            finalScoreText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("finalScoreText がアサインされていません！");
        }
    }

    private IEnumerator DelayedShowFinalScore()
    {
        yield return new WaitForSeconds(finalScoreDelay);
        ShowFinalScore();

        yield return new WaitForSeconds(1f);
        if (resetButton != null)
        {
            resetButton.SetActive(true);
        }
    }

    private IEnumerator ShowTemporaryScore(int points)
    {
        yield return new WaitForSeconds(1f);

        if (temporaryScoreText != null)
        {
            temporaryScoreText.text = points >= 0 ? $"+{points}" : points.ToString();
            temporaryScoreText.gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);
            temporaryScoreText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowUIButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (uiButton != null)
        {
            uiButton.SetActive(true);
            uiButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + GameManager.Instance.totalScore;
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
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene(0); // 最初のシーンに戻す
    }

    void TryRegisterHandler()
    {
        GameObject currentCan = GameObject.FindWithTag("Can");
        if (currentCan == null)
        {
            Debug.LogWarning("【Handler登録】Can タグ付きオブジェクトが見つかりません！");
            return;
        }

        var handler = currentCan.GetComponent<CanCollisionHandler>();
        if (handler != null && handler != currentHandler)
        {
            if (currentHandler != null)
                currentHandler.OnCanTriggered -= HandleCanTriggered;

            currentHandler = handler;
            currentHandler.OnCanTriggered += HandleCanTriggered;
            Debug.Log("【登録成功】Handler 登録されました！");

            CancelInvoke("TryRegisterHandler"); // 登録できたら停止
        }
    }

}
