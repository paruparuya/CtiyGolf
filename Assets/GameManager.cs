using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalHitCount = 0;
    public int totalScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        totalScore += points;
    }

    public void AddHitCountOnly()
    {
        totalHitCount++;
    }

    public bool IsTransitionPoint()
    {
        return totalHitCount == 5 || totalHitCount == 10;
    }

    public void MoveToNextSceneIfNeeded()
    {
        if (IsTransitionPoint())
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene + 1);
        }
    }

    public bool IsGameOver()
    {
        return totalHitCount >= 15;
    }

    public void ResetGame()
    {
        totalScore = 0;
        totalHitCount = 0;
    }
}