using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public CameraController cameraController; // カメラコントローラへの参照
    public int cameraIndex = 0; // カメラ移動先のインデックス（インスペクターで設定）

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Can")) // 缶がトリガーを通過したとき
        {
            cameraController.MoveCamera(cameraIndex);
            Debug.Log($"カメラが位置 {cameraIndex} に移動しました！");
        }
    }
}