using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver, Victory }
    public GameState CurrentState { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Game States")]
    public bool voldemortDefeated = false;

    // Static flag to start directly in gameplay after reloading the scene
    private static bool startDirectlyInGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CurrentState = GameState.MainMenu;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Check if we should skip the main menu (e.g., after Restarting)
        if (startDirectlyInGame)
        {
            startDirectlyInGame = false;
            StartGame();
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (CurrentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void ShowMainMenu()
    {
        CurrentState = GameState.MainMenu;
        Time.timeScale = 0f;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TriggerGameOver()
    {
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;

        if (hudPanel != null) hudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TriggerVictory()
    {
        CurrentState = GameState.Victory;
        Time.timeScale = 0f;

        if (hudPanel != null) hudPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RegisterVoldemortDefeated()
    {
        voldemortDefeated = true;
        Debug.Log("GameManager: Voldemort has been defeated!");
    }

    public void RestartGame()
    {
        startDirectlyInGame = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        startDirectlyInGame = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
