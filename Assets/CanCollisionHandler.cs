using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanCollisionHandler : MonoBehaviour
{
    private BollController controller;

    void Start()
    {
        // BollControllerÇÉVÅ[ÉìÇ©ÇÁåüçı
        controller = FindObjectOfType<BollController>();
        if (controller == null)
        {
            Debug.LogError("BollController not found in the scene!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered! Collided with: " + other.gameObject.name + ", Tag: " + other.gameObject.tag);
        if (controller != null)
        {
            if (other.CompareTag("Trash"))
            {
                controller.inTrash = true;
            }
            else if (other.CompareTag("Window"))
            {
                controller.hitWindow = true;
            }
            else if (other.CompareTag("Box"))
            {
                controller.inBox = true;
            }
            else
            {
                controller.missHit = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Entered! Collided with: " + collision.gameObject.name);
    }
}
