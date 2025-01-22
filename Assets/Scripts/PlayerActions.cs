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
    public Text testShip;
    public Text testBigShip;
    public Text testScore;

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
        // Obtenir le joueur actuel + mettre à jour ses informations
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer(); // récupère le joueur actuel
        testMinerais.text = "Minerals: " + currentPlayer.minerals.ToString(); // Met à jour un texte avec la valeur des variables de chaque joueur
        testFood.text = "Nourriture: " + currentPlayer.food.ToString();
        testShip.text = "Vaisseaux: " + currentPlayer.shipCount.ToString();
        testBigShip.text = "Gros Vaisseaux: " + currentPlayer.largeShipCount.ToString();
        testScore.text = "Score: " + currentPlayer.points.ToString();

        if (currentPlayer.shipCount < 2)
        {
            UpdateUpgradeButtons();
        } 
        else if (currentPlayer.shipCount >= 2)
        {
            UpdateUpgradeButtons();
        }

        player = gameManager.GetCurrentPlayer(); // On obtient le joueur actuel via GameManager

        if (gameManager.isPlayerTurn)
        {
            // Vérifier si le joueur peut créer un vaisseau
            createShipButton.gameObject.SetActive(player.minerals >= 3 && player.food >= 3);

            // Mettre à jour les boutons d'amélioration
            UpdateUpgradeButtons();
        }
        else
        {
            // Désactiver tous les boutons quand ce n'est pas le tour du joueur
            createShipButton.gameObject.SetActive(false);
            normalToLargeUpgradeButton.gameObject.SetActive(false);
            largeToTowerUpgradeButton.gameObject.SetActive(false);
        }
    }

    public void CreateNormalShip()
    {
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

    public void UpgradeToLargeShip()
    {
        if (smallShips.Count >= 2)
        {
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
                    currentPlayer.shipCount -= 2;
                    currentPlayer.largeShipCount++;

                    Destroy(smallShips[0]);
                    smallShips.RemoveAt(0);

                    Destroy(smallShips[0]);
                    smallShips.RemoveAt(0);

                    Planet startingPlanet = GameManager.Instance.GetPlayerStartingPlanet(currentPlayer);
                    if (startingPlanet != null)
                    {
                        Vector3 spawnPosition = startingPlanet.GetNextFreeSlot();
                        GameObject newLargeShip = Instantiate(largeShipPrefab, spawnPosition, Quaternion.identity);
                        newLargeShip.GetComponent<Ship>().owner = currentPlayer;
                        largeShips.Add(newLargeShip);

                        startingPlanet.AddShipToSlot(newLargeShip); // Ajouter dans le slot disponible
                        GameManager.Instance.ActionTaken();
                    }
                    else
                    {
                        Debug.Log("Planète de départ introuvable pour le joueur.");
                    }
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

    public void UpdateUpgradeButtons()
    {
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer();
        // Désactiver le bouton d'amélioration du vaisseau normal au vaisseau grand vaisseau si le joueur ne peut pas
        normalToLargeUpgradeButton.gameObject.SetActive(currentPlayer.shipCount >= 2);  // Ajout de condition de ressources si nécessaire

        // Désactiver le bouton d'amélioration du vaisseau grand vaisseau à tour si le joueur ne possède pas assez de vaisseaux
        largeToTowerUpgradeButton.gameObject.SetActive(currentPlayer.largeShipCount >= 2);
    }

    public void UpgradeToTower()
    {
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer(); // Récupérer le joueur actif

        if (currentPlayer.largeShipCount >= 2)
        {
            if (largeShips.Count < 2)
            {
                Debug.Log("Pas assez de gros vaisseaux pour coloniser.");
                return;
            }

            // Vérifier que les deux vaisseaux sont sur la même planète
            GameObject firstShipPlanet = largeShips[0].GetComponent<Ship>().shipSelection.currentPlanet;
            bool samePlanet = true;

            foreach (GameObject ship in largeShips)
            {
                if (ship.GetComponent<Ship>().shipSelection.currentPlanet != firstShipPlanet)
                {
                    samePlanet = false;
                    break;
                }
            }

            if (samePlanet)
            {
                // Réduire le nombre de gros vaisseaux du joueur
                currentPlayer.largeShipCount -= 2;

                // Détruire les deux vaisseaux utilisés pour la colonisation
                Destroy(largeShips[0]);
                largeShips.RemoveAt(0);
                Destroy(largeShips[0]);
                largeShips.RemoveAt(0);

                // Récupérer la planète sur laquelle la colonisation est effectuée
                Planet currentPlanet = firstShipPlanet != null ? firstShipPlanet.GetComponent<Planet>() : null;

                if (currentPlanet != null)
                {
                    // Coloniser la planète
                    currentPlanet.ColonizePlanet();

                    // Associer la planète au joueur actif
                    currentPlanet.player = currentPlayer;
                    currentPlanet.isColonized = true;

                    // Ajouter les points de colonisation au joueur
                    currentPlayer.AddPoints(currentPlanet.colonizationPoints);

                    Debug.Log("La planète a été colonisée avec succès par le joueur " + currentPlayer.name);
                    GameManager.Instance.ActionTaken();
                }
                else
                {
                    Debug.Log("Planète non trouvée pour la colonisation.");
                }
            }
            else
            {
                Debug.Log("Les deux gros vaisseaux doivent être sur la même planète pour coloniser.");
            }
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