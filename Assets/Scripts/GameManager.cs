using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    }

    void Start()
    {
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
        isPlayerTurn = true; // Activer les actions pour le nouveau joueur
        UpdateTurnText();
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
}