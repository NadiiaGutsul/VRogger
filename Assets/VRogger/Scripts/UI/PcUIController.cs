using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class PcUIController : MonoBehaviour
{
    [Header("UI Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pcPlayerHUD;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject score;
    [SerializeField] private GameObject lives;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject endScore;


    private bool pauseMenuActive;
    private bool mainMenuActive;
    private TextMeshPro scoreText;
    private TextMeshPro livesText;
    private TextMeshPro timerText;
    private TextMeshProUGUI endScoreText;

    void Start()
    {
        pauseMenuActive = false;
        mainMenuActive = true;
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        
        scoreText = score.GetComponent<TextMeshPro>();
        livesText = lives.GetComponent<TextMeshPro>();
        timerText = timer.GetComponent<TextMeshPro>();
        endScoreText = endScore.GetComponent<TextMeshProUGUI>();
        

        GameManager.instance.GameOver.AddListener(GameOver);
        GameManager.instance.LifeLost.AddListener(UpdateLives);
        

        UpdateLives();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !mainMenuActive) 
        {
            TogglePause();
        }

        UpdateTime();
        UpdateScore();
    }
    
    private void OnDestroy()
    {
        if (GameManager.instance)
        {
            GameManager.instance.GameOver.RemoveListener(GameOver);
            GameManager.instance.LifeLost.RemoveListener(UpdateLives);
        }
    }
    
    private void UpdateScore() 
    {
        if (!GameManager.instance || !scoreText) return;
        
        scoreText.text = "Score:\n" + GameManager.instance.score;
    }
    
    private void UpdateLives() 
    {
        if (!GameManager.instance || !livesText) return;
        
        livesText.text = "Lives:\n" + GameManager.instance.currentLifes;
    }
    
    private void UpdateTime() 
    {
        if (!GameManager.instance || !timerText) return;
        
        timerText.text = "Time:\n" + GameManager.instance.remainingTime.ToString("F0");
        
    }
    
    private void UpdateEndScore()
    {
        if (!GameManager.instance || !endScoreText) return;
        
       
        endScoreText.text = "Score: " + GameManager.instance.score;
    }
    
    
    public void TogglePause() 
    {
        if (!gameOverMenu.activeSelf)
        {
            pauseMenu.SetActive(!pauseMenuActive);
            GameManager.instance.isPaused = !pauseMenuActive;
            pauseMenuActive = !pauseMenuActive;
        }
    }

    public void RestartGame() 
    {
        GameManager.instance.StartGame();
    }

    public void StartGame()
    {
        mainMenuActive = false;
        startMenu.SetActive(false);
        GameManager.instance.isPaused = false;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        pauseMenuActive = false;
        GameManager.instance.isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        mainMenuActive = true;
        gameOverMenu.SetActive(true);
        pcPlayerHUD.SetActive(false);
       
        UpdateEndScore();
    }
}
