using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Animator animator;
    private bool hasStarted = false; // アニメーションが開始済みかを管理
    private bool hasEnd = false;
    


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0; // 初期状態で停止
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
           if (!hasStarted || hasEnd)
            {
                animator.speed = 1;
                animator.Play("GolfAnimator", 0, 0f); //最初から再生
                hasStarted = true;
                hasEnd = false;　//リセット
            }
        }

        // アニメーションが終了したかをチェック
        if (hasStarted && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.speed = 0; // アニメーションを停止
            hasStarted = false; //リセットして次のクリックを待機
            hasEnd = true;
            
        }
    }

   
}

