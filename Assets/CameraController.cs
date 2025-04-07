using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform initialPosition; // カメラの初期位置（インスペクターで設定）
    public Transform[] targetPositions; // カメラの移動先（最大7箇所）
    private Vector3 startPos; // 初期位置を保持
    private Quaternion startRot; // 初期回転を保持
    private bool isMoving = false; // カメラが移動中かどうかのフラグ

    void Start()
    {
        if (initialPosition != null)
        {
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
            Debug.Log($"Camera set to initialPosition: Position={initialPosition.position}, Rotation={initialPosition.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogWarning("initialPosition is null! Camera position remains unchanged.");
        }
        startPos = transform.position;
        startRot = transform.rotation;
        Debug.Log($"startPos={startPos}, startRot={startRot.eulerAngles}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCameraPosition();
        }
    }

    public void MoveCamera(int index)
    {
        if (index >= 0 && index < targetPositions.Length)
        {
            transform.position = targetPositions[index].position;
            transform.rotation = targetPositions[index].rotation;
            Debug.Log($"カメラを {index} の位置に瞬間移動しました！");
        }
    }

    public void ResetCameraPosition()
    {
        if (!isMoving)
        {
            Debug.Log($"ResetCameraPosition called. Moving to startPos={startPos}, startRot={startRot.eulerAngles}");
            StartCoroutine(MoveToTarget(startPos, startRot));
        }
    }

    private IEnumerator MoveToTarget(Vector3 targetPos, Quaternion targetRot)
    {
        isMoving = true;
        float duration = 1.0f;
        float elapsedTime = 0;

        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startingPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startingRot, targetRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        isMoving = false;
    }
}