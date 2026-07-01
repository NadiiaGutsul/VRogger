using UnityEngine;

public class GoalController : MonoBehaviour
{
    private PlayerMovement playerController;
    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("PCPlayer").GetComponent<PlayerMovement>();

        playerController.goalReached.AddListener(OnGoalReached);
    }


    private void OnGoalReached(GameObject goal) {
        
        // Only disable specific goal player interacts with, not all goals in the scene
        if (goal == gameObject)
        {
            gameObject.SetActive(false);
            
        }
    }
}
