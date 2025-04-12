using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CanCollisionHandler : MonoBehaviour
{
    private BollController controller; // BollControllerへの参照
    private Score scoreManager; // Scoreへの参照
    private bool hasTriggered = false; // トリガーがすでに反応しているかどうか
    public event Action<string> OnCanTriggered; // ← 追加：タグ名を通知

    // 初期化処理
    void Start()
    {
        // BollControllerとScoreManagerをシーンから探して取得
        controller = FindObjectOfType<BollController>();
        scoreManager = FindObjectOfType<Score>();

        
    }

    // トリガーに入った時の処理
    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Trash") || other.CompareTag("Window") || other.CompareTag("Box") || other.CompareTag("Ground"))
        {
            // タグに応じた点数を設定
            int points = 0;
            switch (other.tag)
            {
                case "Trash":
                    points = 10;  //ゴミ箱に入ったら
                    break;
                case "Window":
                    points = -20;  //窓に当たったら
                    break;
                case "Box":
                    points = +50;   //箱に入ったら
                    break;
                case "Ground":
                    points = -5;  //地面に当たったら
                    break;
                default:
                    points = 0;    //例外
                    break;
            }

            Debug.Log($"缶が {other.tag} に当たりました！ 点数: {points}");

            hasTriggered = true;
            OnCanTriggered?.Invoke(other.tag);

            // Score スクリプトを使ってスコアを更新（scoreManager は Start() で Find している前提）
            if (scoreManager != null)
            {
                Debug.Log("スコアに加算: " + points); // ★ここ追加！
                if (points >= 0)
                scoreManager.AddScore(points);
                else
                   
                    scoreManager.SubtractScore(-points);
            }
        }
    }


    // 新しい缶を生成したらボタンを非表示にする
    public void ResetTrigger()
    {
        hasTriggered = false; // 反応フラグをリセット

        
    }
}