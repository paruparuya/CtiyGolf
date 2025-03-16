using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Animator animator;
    private bool hasStarted = false; // �A�j���[�V�������J�n�ς݂����Ǘ�


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0; // ������ԂŒ�~
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasStarted) // ���N���b�N�����J�n�̏ꍇ
        {
            animator.speed = 1;
            animator.Play("Motion");
            hasStarted = true; // �J�n�ς݂ɐݒ�
        }
    }
}
