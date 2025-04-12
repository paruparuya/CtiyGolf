using UnityEngine;
using System.Collections;

public class BollController : MonoBehaviour
{
    // 缶関連
    public GameObject canPrefab; // 缶のプレハブ
    private Rigidbody rb; // 現在の缶のRigidbody
    private GameObject currentCan; // 現在の缶オブジェクト
    private bool isReadyToHit = false; // 缶を飛ばす準備ができているか
    [HideInInspector] public bool inTrash = false; // ゴミ箱に入ったか
    [HideInInspector] public bool inBox = false;  // 箱に入ったか
    [HideInInspector] public bool hitWindow = false;  // 窓に当たったか
    [HideInInspector] public bool missHit = false;  // ミスしたか

    // CanCollisionHandlerへの参照を追加
    private CanCollisionHandler collisionHandler;

    public GameObject shootButton; //シュートボタンを配置

    // ターゲット関連
    public GameObject[] targets; // ターゲットの配列
    private int targetCount = 7; // ターゲットの数
    private int selectedTarget = 0; // 選択中のターゲット
    private float timer = 0f; // ターゲット切り替え用のタイマー
    private int direction = 1; // 移動方向（1: 0→6, -1: 6→0）
    public bool isTargetmoving = false;
    public CameraController cameraController; //カメラコントローラーの参照

    // かご関連
    public GameObject basket; // かごのオブジェクト
    public Vector3[] basketPositions = new Vector3[5]; // かごの5つの固定位置

    public GameObject boxLid; // 蓋のGameObject（Inspectorで設定）
    private int actionCount = 0; // アクション回数のカウンター
    private const int MAX_ACTIONS = 5; // 5回で1サイクル
    private int openLidTiming; // 蓋が開くタイミング（0～4のランダム）
    [SerializeField] private Vector3 closedPosition = new Vector3(-0.00814029f, 1.149f, -0.392f); // 閉じた状態のローカル位置
    [SerializeField] private Vector3 openPosition = new Vector3(-0.00814029f, 1.133f, -0.37f);   // 開いた状態のローカル位置
    [SerializeField] private Quaternion closedRotation = Quaternion.Euler(0, 0, 0); // 閉じた状態の回転
    [SerializeField] private Quaternion openRotation = Quaternion.Euler(-96.665f, 0, 0); // 開いた状態の回転



    void Start()
    {
        if (targets == null || targets.Length != targetCount)
        {
            Debug.LogError($"Target count must be {targetCount}, but found {(targets == null ? 0 : targets.Length)}. Please assign targets in the inspector.");
            return;
        }

        if (basketPositions.Length == 0)
        {
            basketPositions = new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(1f, 0f, 0f)
            };
        }

        // 蓋の初期設定
        if (boxLid == null)
        {
            Debug.LogError("BoxLid GameObject is not assigned in the inspector!");
        }
        else
        {
            boxLid.transform.rotation = closedRotation; // 最初は閉じた状態
        }



        UpdateTargetVisibility();
        SpawnNewCan();
        isReadyToHit = true;
        SetRandomOpenTiming();
    }

    void Update()
    {
        if (targets == null || targets.Length != targetCount)
        {
            Debug.LogWarning("Update skipped: Targets not properly set.");
            return;
        }

        if (isTargetmoving)
        {
            timer += Time.deltaTime;
            if (timer >= 0.1f)
            {
                timer = 0f;
                selectedTarget += direction;
                if (selectedTarget >= targetCount || selectedTarget < 0)
                {
                    direction *= -1;
                    selectedTarget += direction * 2;
                }
                UpdateTargetVisibility();
            }
        }


        if (this.inTrash) { Debug.Log("ゴミ箱に入りました"); this.inTrash = false; }
        if (this.inBox) { Debug.Log("箱に入りました"); this.inBox = false; }
        if (this.hitWindow) { Debug.Log("窓が割れました"); this.hitWindow = false; }
        if (this.missHit) { Debug.Log("ミスです"); this.missHit = false; }
    }

    public void SpawnNewCan()
    {
        if (currentCan != null)
        {
            Destroy(currentCan);
            Debug.Log("Previous can destroyed.");
        }

        if (canPrefab == null)
        {
            Debug.LogError("CanPrefab is not assigned in the inspector!");
            return;
        }

        currentCan = Instantiate(canPrefab);
        rb = currentCan.GetComponent<Rigidbody>();
        collisionHandler = currentCan.GetComponent<CanCollisionHandler>();

        if (rb == null || collisionHandler == null)
        {
            Debug.LogError("Missing components on new can.");
            return;
        }

        collisionHandler.ResetTrigger();

        MoveBasketToRandomPosition();
        actionCount++;
        if (actionCount - 1 == openLidTiming)
        {
            OpenBoxLid();
        }
        else
        {
            CloseBoxLid();
        }

        if (actionCount >= MAX_ACTIONS)
        {
            ResetGame();
        }

        isTargetmoving = true;

        if (cameraController != null)
        {
            cameraController.ResetCameraPosition();
        }

        // ★ ここで1秒後にシュートボタン表示
        StartCoroutine(ShowShootButtonAfterDelay());
    }

    void UpdateTargetVisibility()
    {
        for (int i = 0; i < targetCount; i++)
        {
            targets[i].SetActive(i == selectedTarget);
        }
    }

    void HitBall()
    {
        Transform targetTransform = targets[selectedTarget].transform;
        Vector3 targetPosition;

        if (targetTransform.childCount > 0)
        {
            targetPosition = targetTransform.GetChild(0).position;
        }
        else
        {
            Debug.LogError("Target has no children! Using target position as fallback.");
            targetPosition = targetTransform.position;
        }

        Vector3 startPosition = currentCan.transform.position;
        Vector3 horizontalDistance = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z);
        float distance = horizontalDistance.magnitude;
        Vector3 direction = horizontalDistance.normalized;

        float flightTime = 1.5f;
        float horizontalSpeed = distance / flightTime;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float upwardSpeed = gravity * flightTime / 2f;
        float heightDifference = targetPosition.y - startPosition.y;
        upwardSpeed += heightDifference / flightTime;

        Vector3 velocity = direction * horizontalSpeed + Vector3.up * upwardSpeed;
        rb.velocity = Vector3.zero;
        rb.AddForce(velocity, ForceMode.VelocityChange);
    }

    void MoveBasketToRandomPosition()
    {
        int randomIndex = Random.Range(0, basketPositions.Length);
        basket.transform.position = basketPositions[randomIndex];
        
    }

    void OpenBoxLid()
    {
        if (boxLid != null)
        {
            boxLid.transform.localPosition = openPosition;
            boxLid.transform.localRotation = Quaternion.Euler(-96.665f, 0, 0);
            Debug.Log("蓋が開きました！");
        }
    }

    void CloseBoxLid()
    {
        if (boxLid != null)
        {
            boxLid.transform.localPosition = closedPosition;
            boxLid.transform.localRotation = closedRotation;
            Debug.Log("蓋が閉じています。");
        }
    }

    void SetRandomOpenTiming()
    {
        openLidTiming = Random.Range(0, MAX_ACTIONS);
        Debug.Log($"蓋が開くタイミング: {openLidTiming + 1}回目");
    }

    void ResetGame()
    {
        actionCount = 0;
        CloseBoxLid();
        SetRandomOpenTiming();
    }

    public void ManualHitBall()
    {
        if (isReadyToHit)
        {
            HitBall();
            isReadyToHit = false;
        }
    }

    private IEnumerator ShowShootButtonAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        isReadyToHit = true; 

        if (shootButton != null)
        {
            shootButton.SetActive(true);
        }
    }

    public int GetActionCount()
    {
        return actionCount;
    }
}