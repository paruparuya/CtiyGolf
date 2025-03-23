using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Animator animator;
    private bool hasStarted = false; // �A�j���[�V�������J�n�ς݂����Ǘ�
    private bool hasEnd = false;
    


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0; // ������ԂŒ�~
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
           if (!hasStarted || hasEnd)
            {
                animator.speed = 1;
                animator.Play("GolfAnimator", 0, 0f); //�ŏ�����Đ�
                hasStarted = true;
                hasEnd = false;�@//���Z�b�g
            }
        }

        // �A�j���[�V�������I�����������`�F�b�N
        if (hasStarted && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.speed = 0; // �A�j���[�V�������~
            hasStarted = false; //���Z�b�g���Ď��̃N���b�N��ҋ@
            hasEnd = true;
            
        }
    }

   
}

