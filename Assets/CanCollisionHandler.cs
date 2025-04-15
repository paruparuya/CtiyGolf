using UnityEngine;
using System;

public class CanCollisionHandler : MonoBehaviour
{
    private BollController controller;
    private Score scoreManager;
    private bool hasTriggered = false;
    public event Action<string> OnCanTriggered;

    public AudioClip trashSound;
    public AudioClip windowSound;
    public AudioClip groundSound;
    public AudioClip boxSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = FindObjectOfType<BollController>();
        scoreManager = FindObjectOfType<Score>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        int points = 0;

        if (other.CompareTag("Trash") || other.CompareTag("Window") || other.CompareTag("Box") || other.CompareTag("Ground"))
        {
            // š scoreManager ‚ª null ‚©ƒ`ƒFƒbƒN
            if (scoreManager == null)
            {
                Debug.LogError("yƒGƒ‰[zscoreManager ‚ª null ‚Å‚·I");
            }
            else
            {
                Debug.Log("scoreManager ‚ÍŒ©‚Â‚©‚Á‚Ä‚¢‚Ü‚·I");
            }

            // š OnCanTriggered ƒCƒxƒ“ƒg‚ª“o˜^‚³‚ê‚Ä‚¢‚é‚©Šm”F
            if (OnCanTriggered == null)
            {
                Debug.LogWarning("yŒxzOnCanTriggered ‚ª“o˜^‚³‚ê‚Ä‚¢‚Ü‚¹‚ñI");
            }
            else
            {
                Debug.Log("OnCanTriggered ‚ª“o˜^‚³‚ê‚Ä‚¢‚Ü‚·BƒCƒxƒ“ƒg”­‰Î‚µ‚Ü‚·I");
            }

            switch (other.tag)
            {
                case "Trash":
                    points = 10;
                    if (trashSound != null && audioSource != null) audioSource.PlayOneShot(trashSound);
                    break;
                case "Window":
                    points = -20;
                    if (windowSound != null && audioSource != null) audioSource.PlayOneShot(windowSound);
                    break;
                case "Box":
                    points = 50;
                    if (boxSound != null && audioSource != null) audioSource.PlayOneShot(boxSound);
                    break;
                case "Ground":
                    points = -5;
                    if (groundSound != null && audioSource != null) audioSource.PlayOneShot(groundSound);
                    break;
            }

            hasTriggered = true;
            OnCanTriggered?.Invoke(other.tag);
        }
    }

    public void RegisterScoreHandler(Score score)
    {
        scoreManager = score;
        OnCanTriggered += score.HandleCanTriggered;
        Debug.Log("yè“®“o˜^zƒXƒRƒAƒnƒ“ƒhƒ‰[“o˜^¬Œ÷I");
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
