using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private bool inTrash = false; //�S�~���ɓ�������
    private bool inBox = false;  //���ɓ�������
    private bool hitWindow = false;  //���ɓ���������
    private bool missHit = false;  //�~�X������
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("�����ɓ��������I: " + other.gameObject.name); // �Փ˂����I�u�W�F�N�g����\��

        if (other.gameObject.tag == "Trash")
        {
            this.inTrash = true;
            Debug.Log("�S�~���ɓ���܂���");
        }
        else if (other.gameObject.tag == "Window")
        {
            this.hitWindow = true;
            Debug.Log("��������܂���");
        }
        else if (other.gameObject.tag == "Box")
        {
            this.inBox = true;
            Debug.Log("���ɓ���܂���");
        }
        else
        {
            this.missHit = true;
            Debug.Log("�~�X�ł�");
        }
    }
}