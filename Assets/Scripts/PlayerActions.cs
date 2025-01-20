using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    private Player player;
    public Button largeToTowerUpgradeButton;
    public Button normalToLargeUpgradeButton;
    public Button createShipButton;
    public GameObject normalShipPrefab;
    public GameObject largeShipPrefab;
    public GameObject towerPrefab;
    private List<GameObject> smallShips = new List<GameObject>();
    private List<GameObject> largeShips = new List<GameObject>();
    public ShipSelection shipSelection;
    private GameManager gameManager;

    public Text testMinerais;
    public Text testFood;

    // Nouveau : Dictionnaire pour suivre les slots libres sur chaque planète
    private Dictionary<int, List<Vector3>> planetSlots = new Dictionary<int, List<Vector3>>();

    void Start()
    {
        normalToLargeUpgradeButton.gameObject.SetActive(false);
        largeToTowerUpgradeButton.gameObject.SetActive(false);
        createShipButton.gameObject.SetActive(false);
        createShipButton.onClick.AddListener(() => CreateNormalShip());
        normalToLargeUpgradeButton.onClick.AddListener(() => UpgradeToLargeShip());
        largeToTowerUpgradeButton.onClick.AddListener(() => UpgradeToTower());
        gameManager = GameManager.Instance;
        // Initialisation des slots pour chaque planète (exemple avec 4 emplacements autour de chaque planète)
        InitPlanetSlots();
    }

    // Initialisation des emplacements (slots) pour les planètes
    void InitPlanetSlots()
    {
        foreach (var planet in gameManager.planets) // Utilisation directe de la liste planets dans GameManager
        {
            List<Vector3> slots = new List<Vector3>()
            {
                new Vector3(0, 1, -1), // Position relative au centre de la planète
                new Vector3(1, 0, -1),
                new Vector3(0, -1, -1),
                new Vector3(-1, 0, -1)
            };
            planetSlots.Add(planet.GetInstanceID(), slots); // Utiliser GetInstanceID pour associer les slots à chaque planète par son ID unique
        }
    }

    void Update()
    {
        // Obtenir le joueur actuel + mettre à jour ses informations (utile pour le HUD des ressources d'Axel)
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer(); // récupère le joueur actuel
        testMinerais.text = "Minerals: " + currentPlayer.minerals.ToString(); //Met à jour un texte avec la valeur des variables de chaque joueur
        testFood.text = "Nourriture: " + currentPlayer.food.ToString();


        player = gameManager.GetCurrentPlayer(); // On obtient le joueur actuel via GameManager

        if (gameManager.isPlayerTurn)
        {
            createShipButton.gameObject.SetActive(player.minerals >= 3 && player.food >= 3);
            UpdateUpgradeButtons();
        }
        else
        {
            createShipButton.gameObject.SetActive(false);
            normalToLargeUpgradeButton.gameObject.SetActive(false);
            largeToTowerUpgradeButton.gameObject.SetActive(false);
        }
    }

    public void CreateNormalShip()
    {
        // Obtenir le joueur actuel
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer();

        if (currentPlayer.minerals < 3 && currentPlayer.food < 3)
        {
            Debug.Log("Pas assez de ressources pour créer un vaisseau.");
            return;
        }

        // Créer le vaisseau à la position définie
        GameObject ship = Instantiate(normalShipPrefab);

        // Assigner le vaisseau au joueur et le placer sur la planète de départ
        Planet startingPlanet = GameManager.Instance.GetPlayerStartingPlanet(currentPlayer);

        if (startingPlanet != null)
        {
            startingPlanet.AddShipToSlot(ship); // Ajouter le vaisseau dans un slot libre
            ship.GetComponent<Ship>().owner = currentPlayer; // Lier le vaisseau au joueur actif
            smallShips.Add(ship);
            currentPlayer.shipCount++;
            currentPlayer.minerals -= 3;
            currentPlayer.food -= 3;

            // Action terminée, donc informer le GameManager
            GameManager.Instance.ActionTaken();
        }
        else
        {
            Debug.Log("Planète de départ introuvable pour le joueur.");
        }
    }

    // Méthode pour récupérer le prochain slot libre sur la planète
    private Vector3 GetNextFreeSlot(int planetId)
    {
        if (planetSlots.ContainsKey(planetId) && planetSlots[planetId].Count > 0)
        {
            Vector3 slotPosition = planetSlots[planetId][0];
            planetSlots[planetId].RemoveAt(0); // Enlève le slot utilisé
            return slotPosition;
        }
        return Vector3.zero; // Si pas de slot disponible
    }

    public void UpgradeToLargeShip()
    {
        // Vérifier si le joueur a assez de petits vaisseaux pour l'amélioration
        if (smallShips.Count >= 2)
        {
            // Vérifier que tous les vaisseaux appartiennent au joueur actif
            Player currentPlayer = GameManager.Instance.GetCurrentPlayer();
            bool allShipsBelongToPlayer = true;

            foreach (GameObject ship in smallShips)
            {
                Ship shipComponent = ship.GetComponent<Ship>();
                if (shipComponent.owner != currentPlayer)
                {
                    allShipsBelongToPlayer = false;
                    break;
                }
            }

            if (allShipsBelongToPlayer)
            {
                // Vérifier que les vaisseaux sont bien sur la même planète
                GameObject planet = smallShips[0].GetComponent<Ship>().shipSelection.currentPlanet;
                bool samePlanet = true;

                foreach (GameObject ship in smallShips)
                {
                    if (ship.GetComponent<Ship>().shipSelection.currentPlanet != planet)
                    {
                        samePlanet = false;
                        break;
                    }
                }

                if (samePlanet)
                {
                    // Effectuer l'amélioration si toutes les conditions sont remplies
                    currentPlayer.shipCount -= 2;
                    currentPlayer.largeShipCount++;

                    Destroy(smallShips[0]);
                    smallShips.RemoveAt(0);
                    Destroy(smallShips[0]);
                    smallShips.RemoveAt(0);

                    Vector3 spawnPosition = GetPlanetPosition(currentPlayer.startingPlanetId) + new Vector3(0, 0, -1);
                    GameObject newLargeShip = Instantiate(largeShipPrefab, spawnPosition, Quaternion.identity);
                    newLargeShip.GetComponent<Ship>().owner = currentPlayer; // Lier le grand vaisseau au joueur actif
                    largeShips.Add(newLargeShip);
                }
                else
                {
                    Debug.Log("Les vaisseaux doivent être sur la même planète pour être améliorés.");
                }
            }
            else
            {
                Debug.Log("Tous les vaisseaux à améliorer doivent appartenir au joueur.");
            }
        }
        else
        {
            Debug.Log("Pas assez de vaisseaux normaux.");
        }
    }

    public void UpdateUpgradeButtons()
    {
        normalToLargeUpgradeButton.gameObject.SetActive(smallShips.Count >= 2);
        largeToTowerUpgradeButton.gameObject.SetActive(player.largeShipCount >= 2);
    }

    public void UpgradeToTower()
    {
        if (player.largeShipCount >= 2)
        {
            player.largeShipCount -= 2;
            Destroy(largeShips[0]);
            largeShips.RemoveAt(0);
            Destroy(largeShips[0]);
            largeShips.RemoveAt(0);
            Instantiate(towerPrefab, GetPlanetPosition(player.startingPlanetId) + new Vector3(0, 0, -1), Quaternion.identity);
        }
        else
        {
            Debug.Log("Pas assez de gros vaisseaux.");
        }
    }

    private Vector3 GetPlanetPosition(int planetId)
    {
        return new Vector3(planetId * 2, 0, 0);
    }

    public void EndTurn()
    {
        gameManager.EndTurn();
    }
}