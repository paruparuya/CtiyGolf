using UnityEngine;

public class BollController : MonoBehaviour
{
    public GameObject canPrefab; // 缶のプレハブ
    public GameObject[] targets; // ターゲットの配列
    private Rigidbody rb; // 現在の缶のRigidbody
    private GameObject currentCan; // 現在の缶オブジェクト
    private int targetCount = 7; // ターゲットの数
    private int selectedTarget = 0; // 選択中のターゲット
    private bool isReadyToHit = false; // 缶を飛ばす準備ができているか
    private float timer = 0f; // ターゲット切り替え用のタイマー
    private int direction = 1; // 移動方向（1: 0→6, -1: 6→0）

    void Start()
    {
        // ターゲットの設定確認
        if (targets == null || targets.Length != targetCount)
        {
            Debug.LogError($"Target count must be {targetCount}, but found {(targets == null ? 0 : targets.Length)}. Please assign targets in the inspector.");
            return;
        }
        UpdateTargetVisibility();
        SpawnNewCan(); // ゲーム開始時に最初の缶を生成
        isReadyToHit = true; // 最初は飛ばす準備ができている状態
    }

    void Update()
    {
        // ターゲットが正しく設定されていない場合は処理をスキップ
        if (targets == null || targets.Length != targetCount)
        {
            Debug.LogWarning("Update skipped: Targets not properly set.");
            return;
        }

        // タイマーを更新し、0.1秒ごとにターゲットを切り替え
        timer += Time.deltaTime;
        if (timer >= 0.1f)
        {
            timer = 0f; // タイマーリセット
            selectedTarget += direction; // 次のターゲットへ移動

            // 境界に達したら方向を反転
            if (selectedTarget >= targetCount || selectedTarget < 0)
            {
                direction *= -1; // 方向反転
                selectedTarget += direction * 2; // 端での移動を自然に調整
            }

            UpdateTargetVisibility(); // ターゲットの表示を更新
            Debug.Log($"Selected Target: {selectedTarget}");
        }

        // マウスクリック時の処理
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked");
            if (isReadyToHit)
            {
                HitBall(); // 缶を飛ばす
                isReadyToHit = false; // 次は生成フェーズへ
            }
            else
            {
                SpawnNewCan(); // 新しい缶を生成
                isReadyToHit = true; // 次は飛ばすフェーズへ
            }
        }
    }

    // 新しい缶を生成する関数
    void SpawnNewCan()
    {
       

        // 新しい缶を生成し、Rigidbodyを取得
        currentCan = Instantiate(canPrefab);
        rb = currentCan.GetComponent<Rigidbody>();
        Debug.Log("Can spawned: " + (currentCan != null) + ", Rigidbody: " + (rb != null));
    }

    // ターゲットの表示を更新する関数
    void UpdateTargetVisibility()
    {
        Debug.Log("Updating target visibility");
        for (int i = 0; i < targetCount; i++)
        {
            targets[i].SetActive(i == selectedTarget); // 選択中のターゲットのみ表示
        }
    }

    // 缶を飛ばす関数
    void HitBall()
    {
        // 選択したターゲットのTransformを取得
        Transform targetTransform = targets[selectedTarget].transform;
        Vector3 targetPosition;

        // 子オブジェクトがあるか確認
        if (targetTransform.childCount > 0)
        {
            targetPosition = targetTransform.GetChild(0).position; // 最初の子オブジェクトの位置を使用
        }
        else
        {
            Debug.LogError("Target has no children! Using target position as fallback.");
            targetPosition = targetTransform.position; // 子がない場合はターゲット自体の位置を使用
        }

        // ボールの現在位置
        Vector3 startPosition = currentCan.transform.position;

        // 水平方向の距離
        Vector3 horizontalDistance = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z);
        float distance = horizontalDistance.magnitude;

        // ターゲット方向（水平）
        Vector3 direction = horizontalDistance.normalized;

        // 仮定する飛行時間（調整可能）
        float flightTime = 1.5f; // 例えば1.5秒で到達するように設定

        // 水平速度 = 距離 / 時間
        float horizontalSpeed = distance / flightTime;

        // 上方向速度（放物線用）
        float gravity = Mathf.Abs(Physics.gravity.y); // 重力の大きさ（通常9.81）
        float upwardSpeed = gravity * flightTime / 2f;

        // 高さの差を考慮
        float heightDifference = targetPosition.y - startPosition.y;
        upwardSpeed += heightDifference / flightTime; // 高さ補正

        // 最終的な速度ベクトル
        Vector3 velocity = direction * horizontalSpeed + Vector3.up * upwardSpeed;

        // ボールに速度を適用
        rb.velocity = Vector3.zero;
        rb.AddForce(velocity, ForceMode.VelocityChange);

        Debug.Log($"Velocity: {velocity}, Flight Time: {flightTime}, Target Pos: {targetPosition}");
    }
}