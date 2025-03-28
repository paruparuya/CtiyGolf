using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private bool inTrash = false; //ゴミ箱に入ったか
    private bool inBox = false;  //箱に入ったか
    private bool hitWindow = false;  //窓に当たったか
    private bool missHit = false;  //ミスしたか
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
        Debug.Log("何かに当たった！: " + other.gameObject.name); // 衝突したオブジェクト名を表示

        if (other.gameObject.tag == "Trash")
        {
            this.inTrash = true;
            Debug.Log("ゴミ箱に入りました");
        }
        else if (other.gameObject.tag == "Window")
        {
            this.hitWindow = true;
            Debug.Log("窓が割れました");
        }
        else if (other.gameObject.tag == "Box")
        {
            this.inBox = true;
            Debug.Log("箱に入りました");
        }
        else
        {
            this.missHit = true;
            Debug.Log("ミスです");
        }
    }
}