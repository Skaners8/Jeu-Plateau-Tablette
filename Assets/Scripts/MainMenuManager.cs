using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // Singleton Instance
    public static MainMenuManager Instance { get; private set; }

    public Button newGameButton;
    public Button loadGameButton;
    public Button quitGameButton;

    public GameObject mainMenu;

    private void Awake()
    {
        // Singleton Pattern: Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Preserve the instance across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    void Start()
    {
        // Add listeners for buttons
        newGameButton.onClick.AddListener(StartNewGame);
        loadGameButton.onClick.AddListener(LoadGame);
        quitGameButton.onClick.AddListener(QuitGame);

        mainMenu.SetActive(true);
    }

    public void StartNewGame()
    {
        Debug.Log("Starting a new game...");
        SceneManager.LoadScene("JeuPrincipal");
    }

    void LoadGame()
    {
        Debug.Log("Loading a game...");
        // Add load game logic here
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
