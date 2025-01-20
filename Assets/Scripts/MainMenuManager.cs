using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // Singleton Instance
    public static MainMenuManager Instance { get; private set; }

    public Button choosePlayersButton;
    public Button newGameButton;
    public Button loadGameButton;
    public Button quitGameButton;
    public InputField playerNameInputFieldPrefab;
    public Transform inputFieldContainer;
    public Button addButton;
    public Button removeButton;
    public Button mainMenuButton;
    public Button leftArrowButton;
    public Button rightArrowButton;

    public int playerCount = 0; // Nombre de joueurs
    public string[] playerNames = new string[4]; // Tableau des noms des joueurs
    private InputField[] playerNameInputFields = new InputField[4]; // Champs de texte pour les noms
    private int currentPlayerIndex = -1; // Index du joueur courant

    public GameObject mainMenu;
    public GameObject choosePlayersMenu;

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
        // Ajoutez des listeners pour les boutons
        choosePlayersButton.onClick.AddListener(ChoosePlayers);
        newGameButton.onClick.AddListener(StartNewGame);
        loadGameButton.onClick.AddListener(LoadGame);
        quitGameButton.onClick.AddListener(QuitGame);
        addButton.onClick.AddListener(AddPlayer);
        removeButton.onClick.AddListener(RemovePlayer);
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        leftArrowButton.onClick.AddListener(NavigateLeft);
        rightArrowButton.onClick.AddListener(NavigateRight);

        mainMenu.SetActive(true);
        choosePlayersMenu.SetActive(false);
    }

    public void ChoosePlayers()
    {
        mainMenu.SetActive(false);
        choosePlayersMenu.SetActive(true);
        playerCount = 0; // Réinitialiser le nombre de joueurs

        // Détruire les champs existants
        for (int i = 0; i < playerNameInputFields.Length; i++)
        {
            if (playerNameInputFields[i] != null)
            {
                Destroy(playerNameInputFields[i].gameObject);
                playerNameInputFields[i] = null; // Réinitialiser la référence
            }
        }

        currentPlayerIndex = -1; // Réinitialiser l'index
        for (int i = 0; i < playerNames.Length; i++)
        {
            playerNames[i] = string.Empty; // Réinitialiser les noms
        }
    }

    public void AddPlayer()
    {
        if (playerCount < 4) // Vérifier si le nombre maximum n'est pas atteint
        {
            // Créer un nouveau champ de texte
            InputField newInputField = Instantiate(playerNameInputFieldPrefab, inputFieldContainer);

            // Supprimer tous les anciens listeners
            newInputField.onEndEdit.RemoveAllListeners();

            // Enregistrer le nom uniquement lorsque l'utilisateur termine l'édition
            newInputField.onEndEdit.AddListener(delegate { OnNameEntered(newInputField.text); });

            playerNameInputFields[playerCount] = newInputField; // Ajouter le champ au tableau

            newInputField.gameObject.SetActive(true); // Afficher le champ
            newInputField.Select(); // Sélectionner le champ de saisie

            // Mettre à jour le tableau des noms de joueurs avec le texte par défaut
            playerNames[playerCount] = string.Empty;

            playerCount++; // Incrémenter le nombre de joueurs
            currentPlayerIndex = playerCount - 1; // Mettre à jour l'index du joueur courant
            UpdateNavigation(); // Mettre à jour la navigation
        }
    }

    public void RemovePlayer()
    {
        if (currentPlayerIndex >= 0) // Vérifier qu'un joueur est sélectionné
        {
            Destroy(playerNameInputFields[currentPlayerIndex].gameObject); // Détruire le champ de texte
            playerNameInputFields[currentPlayerIndex] = null; // Réinitialiser la référence
            playerCount--; // Décrémenter le nombre de joueurs

            // Réajuster l'index courant
            if (currentPlayerIndex >= playerCount) currentPlayerIndex = playerCount - 1;

            UpdateNavigation(); // Mettre à jour la navigation
        }
    }

    public void NavigateLeft()
    {
        if (currentPlayerIndex > 0) // Vérifier qu'on peut naviguer vers la gauche
        {
            currentPlayerIndex--;
            UpdateNavigation(); // Mettre à jour la navigation
        }
    }

    public void NavigateRight()
    {
        if (currentPlayerIndex < playerCount - 1) // Vérifier qu'on peut naviguer vers la droite
        {
            currentPlayerIndex++;
            UpdateNavigation(); // Mettre à jour la navigation
        }
    }

    private void UpdateNavigation()
    {
        // Masquer tous les champs de texte
        for (int i = 0; i < playerNameInputFields.Length; i++)
        {
            if (playerNameInputFields[i] != null)
                playerNameInputFields[i].gameObject.SetActive(false);
        }

        // Afficher le champ de texte correspondant à l'index courant
        if (currentPlayerIndex >= 0 && currentPlayerIndex < playerNameInputFields.Length)
        {
            playerNameInputFields[currentPlayerIndex].gameObject.SetActive(true);
            playerNameInputFields[currentPlayerIndex].Select(); // Sélectionner le champ courant
        }
    }

    void OnNameEntered(string name)
    {
        Debug.Log("Nom entré : " + name);

        // Vérifie si le nom n'est pas déjà présent dans playerNames
        if (currentPlayerIndex >= 0 && currentPlayerIndex < playerNames.Length && !string.IsNullOrWhiteSpace(name))
        {
            if (!System.Array.Exists(playerNames, playerName => playerName == name))
            {
                playerNames[currentPlayerIndex] = name; // Stocker le nom uniquement s'il est différent
            }
            else
            {
                Debug.Log("Ce nom est déjà utilisé.");
            }
        }
    }

    public void BackToMainMenu()
    {
        // Réinitialiser l'état avant de retourner au menu principal
        mainMenu.SetActive(true);
        choosePlayersMenu.SetActive(false);
    }

    public void StartNewGame()
    {
        for (int i = 0; i < playerCount; i++)
        {
            Debug.Log("Joueur " + (i + 1) + ": " + playerNames[i]);
        }
        SceneManager.LoadScene("JeuPrincipal");
    }

    void LoadGame() { }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}