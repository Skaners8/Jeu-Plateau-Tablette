using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // Singleton Instance
    public static MainMenuManager Instance { get; private set; }

    public Button newGameButton;
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
        // Add listener for the New Game button
        newGameButton.onClick.AddListener(StartNewGame);

        mainMenu.SetActive(true);
    }

    public void StartNewGame()
    {
        Debug.Log("Starting a new game...");
        SceneManager.LoadScene("JeuPrincipal");
    }
}
