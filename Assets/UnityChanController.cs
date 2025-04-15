using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Animator animator;
    private bool hasStarted = false; // アニメーションが開始済みかを管理
    private bool hasEnd = false;
    public BollController bollController;
    public GameObject shootButton;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0; // 初期状態で停止

        if (bollController == null)
        {
            Debug.LogError("bollController が割り当てられていません！");
        }
        else
        {
            Debug.Log("bollController は正常に割り当てられています！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.speed = 0;
            hasStarted = false;
            hasEnd = true;
        }


    }

    public void PlayGolfAndHitCan()
    {
        if (!hasStarted || hasEnd)
        {
            animator.speed = 1;
            animator.Play("GolfAnimator", 0, 0f);
            hasStarted = true;
            hasEnd = false;

            // 缶を飛ばす処理（準備できてたら）
            if (bollController != null)
            {
                bollController.isTargetmoving = false;
                StartCoroutine(DelayHitCan(1.3f)); // ← 1.3秒後にヒット
            }

            // ボタンを非表示
            if (shootButton != null)
            {
                
                shootButton.SetActive(false);
            }
            if (shootButton != null && shootButton.activeSelf)
            {
                shootButton.SetActive(false); // 念のための二重非表示！
            }

        }
    }

    public void OnGolfAnimationEnd()
    {
        animator.speed = 0;
        hasStarted = false;
        hasEnd = true;
        
    }

    public void ResetAnimationState()
    {
        animator.Play("GolfAnimator", 0, 0f); // ← 最初のフレームに強制再生
        animator.speed = 0;                   // ← 再生を止める
        hasStarted = false;
        hasEnd = true;
       
    }

    private IEnumerator DelayHitCan(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bollController != null)
        {
            bollController.ManualHitBall();
        }
    }

}

