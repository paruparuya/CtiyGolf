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
        // すでに反応していたら何もしない
        if (hasTriggered) return;

        // 特定のタグに当たった場合に反応
        if (other.CompareTag("Trash") || other.CompareTag("Window") || other.CompareTag("Box") || other.CompareTag("Ground"))
        {
            Debug.Log($"缶が {other.tag} に当たりました！");

            // 反応済みとしてフラグを設定
            hasTriggered = true;

            OnCanTriggered?.Invoke(other.tag); // ← ここで通知
        }
    }
   

    // 新しい缶を生成したらボタンを非表示にする
    public void ResetTrigger()
    {
        hasTriggered = false; // 反応フラグをリセット

        
    }
}