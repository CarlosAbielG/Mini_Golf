using UnityEngine;

public class GoalHole : MonoBehaviour
{
    public LevelManager levelManager;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            levelManager.LoadNextLevel();
        }
    }
}