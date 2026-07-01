using TMPro;
using UnityEngine;

public class VRUIController : MonoBehaviour
{
    private GameObject gamePaused;
    void Start()
    {
        gamePaused = transform.Find("GamePaused").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused)
        {
            gamePaused.SetActive(true);
        }
        else if(gamePaused.activeSelf)
        {
            gamePaused.SetActive(false);
        }
    }
}

