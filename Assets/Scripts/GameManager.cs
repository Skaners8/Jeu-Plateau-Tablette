using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentTurn;
    public int currentRound;
    public int maxActionsPerTurn = 2;
    public List<Player> players; // Liste des joueurs
    public int currentPlayerIndex = 0;
    private int actionsTaken = 0;
    public bool isPlayerTurn = true;
    public List<Planet> planets; // Liste des planètes disponibles dans la scène

    public Text turnText;
    public Text roundText;
    public Button endTurnButton;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public ResourceHUD resourceHUD; // Référence au ResourceHUD

=======
=======
>>>>>>> Stashed changes
    //Fin du jeu
    public GameObject endGamePanel;
    public Text winnerText;
    public int winningPoints = 25;
    public Button mainMenuButton;
    public Button quitButton;

    // Menu de pause
    public GameObject pausePanel;
    public Text pauseText;
    public Button resumeButton;
    public Button mainMenuPauseButton;
    public Button quitPauseButton;
    public Button pauseButton;
    private bool isPaused = false;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (resourceHUD == null)
        {
            Debug.LogError("ResourceHUD is not assigned in GameManager.");
            return;
        }

    }

    void Start()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        if (resourceHUD == null)
        {
            resourceHUD = FindObjectOfType<ResourceHUD>();
            if (resourceHUD == null)
            {
                Debug.LogError("ResourceHUD introuvable dans la scène.");
            }
        }
=======
        endGamePanel.SetActive(false);
        pausePanel.SetActive(false);
>>>>>>> Stashed changes
=======
        endGamePanel.SetActive(false);
        pausePanel.SetActive(false);
>>>>>>> Stashed changes
        currentTurn = 0;
        currentRound = 1;

        // Initialiser la liste des joueurs avec les GameObjects dans la scène
        players = new List<Player>();
        foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player")) // Assurez-vous que les objets joueurs sont tagués comme "Player"
        {
            Player playerScript = playerObject.GetComponent<Player>();
            if (playerScript != null)
            {
                players.Add(playerScript);
            }
            else
            {
                Debug.LogError("Le GameObject " + playerObject.name + " n'a pas de script Player attaché.");
            }
        }

        // Tri des joueurs par ID pour garantir l'ordre
        players.Sort((p1, p2) => p1.playerID.CompareTo(p2.playerID));

        // Vérification de la bonne initialisation des joueurs
        if (players == null || players.Count == 0)
        {
            Debug.LogError("La liste des joueurs n'a pas été initialisée correctement.");
        }
        else
        {
            Debug.Log("Joueurs initialisés : " + players.Count);
        }

        // Assigner une planète aléatoire à chaque joueur
        AssignPlanetsToPlayers();

        endTurnButton.onClick.AddListener(EndTurn);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        quitButton.onClick.AddListener(QuitGame);
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuPauseButton.onClick.AddListener(ReturnToMainMenu);
        quitPauseButton.onClick.AddListener(QuitGame);
        pauseButton.onClick.AddListener(PauseGame);

        StartNewRound();
    }

    public void StartNewRound()
    {
        // Distribution des ressources aux joueurs depuis les planètes colonisées
        foreach (Planet planet in planets)
        {
            if (planet.isColonized)
            {
                Player player = planet.player;
                planet.GiveResourcesToPlayer(player); // Distribuer les ressources via la méthode AddResource de Player
            }
        }
        StartNewTurn();
    }

    public void StartNewTurn()
    {
        actionsTaken = 0;
        isPlayerTurn = true;
        UpdateTurnText();

        // Ensure ResourceHUD is initialized and update bars
        if (resourceHUD == null)
        {
            resourceHUD = FindObjectOfType<ResourceHUD>();
            if (resourceHUD == null)
            {
                Debug.LogError("ResourceHUD is not assigned or found in the scene.");
                return;
            }
        }

        resourceHUD.UpdateBarsForCurrentPlayer();
    }


    public void EndTurn()
    {
        // Fin du tour actuel, désactiver les actions
        isPlayerTurn = false;

        if (currentPlayerIndex == players.Count - 1)
        {
            currentPlayerIndex = 0;
            currentRound++;
            DisplayRoundEndText();
            StartNewRound();
        }
        else
        {
            currentPlayerIndex++; // Passer au joueur suivant
            StartNewTurn();
        }
    }

    public void ActionTaken()
    {
        actionsTaken++;
        if (actionsTaken >= maxActionsPerTurn)
        {
            EndTurn();
        }
    }

    private void UpdateTurnText()
    {
        if (players != null && players.Count > 0 && currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            turnText.text = players[currentPlayerIndex].playerName;
        }
        else
        {
            Debug.LogError("Impossible de mettre à jour le texte du tour : Index ou liste des joueurs invalide.");
        }
    }

    private void DisplayRoundEndText()
    {
        roundText.text = "Manche " + currentRound;
    }

    public void AssignPlanetsToPlayers()
    {
        if (planets.Count < players.Count)
        {
            Debug.LogError("Il n'y a pas assez de planètes pour tous les joueurs!");
            return;
        }

        List<Planet> availablePlanets = new List<Planet>(planets);

        foreach (Player player in players)
        {
            if (availablePlanets.Count == 0)
            {
                Debug.LogError("Plus de planètes disponibles pour assigner à un joueur!");
                break;
            }

            int randomIndex = Random.Range(0, availablePlanets.Count);
            Planet selectedPlanet = availablePlanets[randomIndex];

            selectedPlanet.player = player;  // Assigner le joueur à la planète
            selectedPlanet.isDiscovered = true;
            selectedPlanet.isColonized = true;
            player.startingPlanetId = selectedPlanet.GetInstanceID();  // Assigner l'ID de la planète au joueur

            // Empêcher l'ajout de points pour la planète de départ
            selectedPlanet.hasAddedPoints = true;  // Empêche l'ajout des points pour la planète de départ

            // Afficher dans la console quel joueur a reçu quelle planète
            Debug.Log($"{player.playerName} a été assigné à la planète {selectedPlanet.name}");

            availablePlanets.RemoveAt(randomIndex);
        }
    }

    public Player GetCurrentPlayer()
    {
        // Vérification que la liste des joueurs est valide et que l'index est bien initialisé
        if (players == null || players.Count == 0)
        {
            Debug.LogError("La liste des joueurs est vide ou non initialisée.");
            return null;
        }

        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            if (players[currentPlayerIndex] != null)
            {
                return players[currentPlayerIndex];
            }
            else
            {
                Debug.LogError("Le joueur à l'index " + currentPlayerIndex + " est null.");
                return null;
            }
        }
        else
        {
            Debug.LogError("Index du joueur actuel invalide : " + currentPlayerIndex);
            return null;
        }
    }

    public Planet GetPlayerStartingPlanet(Player player)
    {
        foreach (Planet planet in planets)
        {
            if (planet.GetInstanceID() == player.startingPlanetId)
            {
                return planet;
            }
        }
        return null;
    }

    // Nouvelle méthode : Vérification des points pour déterminer le gagnant
    public void CheckForWinner()
    {
        foreach (Player player in players)
        {
            if (player.points >= winningPoints)
            {
                EndGame(player);
                break;
            }
        }
    }

    // Nouvelle méthode : Afficher le menu de fin de partie avec le vainqueur
    public void EndGame(Player winner)
    {
        isPlayerTurn = false;  // Désactiver les actions supplémentaires
        endGamePanel.SetActive(true);  // Afficher le panneau de fin
        winnerText.text = winner.playerName + " a gagné la partie !";  // Annonce du gagnant
        Time.timeScale = 0;  // Arrêter le temps
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.enabled = false;  // Désactiver le script de la caméra pour arrêter le mouvement
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;  // Mettre le temps en pause
        Camera.main.GetComponent<CameraController>().enabled = false;  // Arrêter la caméra si elle est en mouvement
    }

    private void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;  // Reprendre le temps
        Camera.main.GetComponent<CameraController>().enabled = true;  // Reprendre la caméra
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1;  // Remettre le temps en marche si le jeu était en pause
        SceneManager.LoadScene("StartMenu");  // Charger la scène du menu principal
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}