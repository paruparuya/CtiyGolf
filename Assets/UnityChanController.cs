using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Animator animator;
    private bool hasStarted = false; // アニメーションが開始済みかを管理


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0; // 初期状態で停止
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasStarted) // 左クリックかつ未開始の場合
        {
            animator.speed = 1;
            animator.Play("Motion");
            hasStarted = true; // 開始済みに設定
        }
    }
}
