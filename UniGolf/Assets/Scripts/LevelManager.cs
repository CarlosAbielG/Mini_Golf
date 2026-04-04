using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform playerBall;
    public Transform[] startPoints;
    public GameObject[] levelRoots;

    private Rigidbody rb;
    private int currentLevel = 0;

    void Start()
    {
        rb = playerBall.GetComponent<Rigidbody>();
        LoadLevel(0);
    }

    public void LoadNextLevel()
    {
        currentLevel++;

        if (currentLevel >= startPoints.Length)
        {
            Debug.Log("You finished all levels!");
            currentLevel = startPoints.Length - 1;
            return;
        }

        LoadLevel(currentLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevel = levelIndex;

        for (int i = 0; i < levelRoots.Length; i++)
        {
            if (levelRoots[i] != null)
            {
                levelRoots[i].SetActive(i == levelIndex);
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (startPoints[levelIndex] != null)
        {
            playerBall.position = startPoints[levelIndex].position + Vector3.up * 0.2f;
            playerBall.rotation = startPoints[levelIndex].rotation;
        }
    }
}