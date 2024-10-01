using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    // Variables pour les ressources et les vaisseaux
    public int minerals;
    public int food;
    public int shipCount;
    public int largeShipCount;
    // Nouvelle variable pour les points
    public int points = 0;
    // Déclaration de la variable startingPlanetId
    public int startingPlanetId; // Ajoutez cette ligne
    // Boutons UI pour les actions
    public Button largeToTowerUpgradeButton;
    public Button normalToLargeUpgradeButton;
    public Button createShipButton;
    // Préfabriqués pour les vaisseaux et les tours
    public GameObject normalShipPrefab;
    public GameObject largeShipPrefab;
    public GameObject towerPrefab;
    // Listes pour garder une trace des vaisseaux créés
    private List<GameObject> smallShips = new List<GameObject>();
    private List<GameObject> largeShips = new List<GameObject>();

    void Start()
    {
        // Désactiver les boutons d'amélioration au départ
        normalToLargeUpgradeButton.gameObject.SetActive(false);
        largeToTowerUpgradeButton.gameObject.SetActive(false);
        createShipButton.gameObject.SetActive(false);
        // Ajouter les listeners pour les boutons
        createShipButton.onClick.AddListener(() => CreateNormalShip(0)); // Planète ID par défaut
        normalToLargeUpgradeButton.onClick.AddListener(() => UpgradeToLargeShip());
        largeToTowerUpgradeButton.onClick.AddListener(() => UpgradeToTower());
    }

    void Update()
    {
        // Activer le bouton de création de vaisseau si assez de ressources
        createShipButton.gameObject.SetActive(minerals >= 3 && food >= 3);
        UpdateUpgradeButtons();
    }

    // Méthode pour ajouter des points
    public void AddPoints(int amount)
    {
        points += amount; // Ajoute des points
        Debug.Log("Points ajoutés : " + amount + ". Total : " + points);
    }

    // Créer un vaisseau normal
    public void CreateNormalShip(int planetId)
    {
        if (minerals >= 3 && food >= 3)
        {
            minerals -= 3;
            food -= 3;
            shipCount++;
            Vector3 spawnPosition = GetPlanetPosition(planetId) + new Vector3(0, 0, -1);
            spawnPosition = AdjustSpawnPosition(spawnPosition, true); // Ajuster la position pour les vaisseaux normaux
            // Créer un vaisseau normal
            GameObject newShip = Instantiate(normalShipPrefab, spawnPosition, Quaternion.identity);
            smallShips.Add(newShip);
        }
        else
        {
            Debug.Log("Pas assez de ressources.");
        }
    }

    // Créer un gros vaisseau
    public void UpgradeToLargeShip()
    {
        if (smallShips.Count >= 2)
        {
            shipCount -= 2;
            largeShipCount++;
            // Détruire deux petits vaisseaux
            Destroy(smallShips[0]);
            smallShips.RemoveAt(0);
            Destroy(smallShips[0]);
            smallShips.RemoveAt(0);
            // Créer un gros vaisseau
            Vector3 spawnPosition = GetPlanetPosition(startingPlanetId) + new Vector3(0, 0, -1);
            spawnPosition = AdjustSpawnPosition(spawnPosition, false); // Ajuster la position pour les gros vaisseaux
            GameObject newLargeShip = Instantiate(largeShipPrefab, spawnPosition, Quaternion.identity);
            largeShips.Add(newLargeShip);
        }
        else
        {
            Debug.Log("Pas assez de vaisseaux normaux.");
        }
    }

    // Ajuste la position de création si un vaisseau ou une tour existe déjà
    private Vector3 AdjustSpawnPosition(Vector3 originalPosition, bool isNormalShip)
    {
        float offset = 1.0f; // Décalage en unités
        int attempts = 0;
        // Tenter de décaler sur l'axe X d'abord, puis Y
        while (CheckIfPositionOccupied(originalPosition) && attempts < 10)
        {
            if (isNormalShip)
            {
                // Tenter de décaler en X
                if (!CheckIfPositionOccupied(originalPosition + new Vector3(offset, 0, 0)))
                {
                    originalPosition.x += offset; // Décalage sur X
                }
                else
                {
                    originalPosition.y += offset; // Sinon, décaler sur Y
                }
            }
            else
            {
                // Tenter de décaler en X pour les gros vaisseaux
                if (!CheckIfPositionOccupied(originalPosition + new Vector3(offset, 0, 0)))
                {
                    originalPosition.x += offset; // Décalage sur X
                }
                else
                {
                    originalPosition.y += offset; // Sinon, décaler sur Y
                }
            }
            attempts++;
        }
        return originalPosition;
    }

    // Vérifie si la position est occupée par un vaisseau ou une tour
    private bool CheckIfPositionOccupied(Vector3 position)
    {
        // Vérifier tous les vaisseaux créés
        foreach (GameObject ship in smallShips)
        {
            if (Vector3.Distance(ship.transform.position, position) < 0.5f) // Ajustez la distance selon vos besoins
                return true;
        }
        foreach (GameObject largeShip in largeShips)
        {
            if (Vector3.Distance(largeShip.transform.position, position) < 0.5f) // Ajustez la distance selon vos besoins
                return true;
        }
        // Vous pouvez ajouter une vérification pour les tours ici si nécessaire
        return false;
    }

    // Mise à jour des boutons d'amélioration
    public void UpdateUpgradeButtons()
    {
        normalToLargeUpgradeButton.gameObject.SetActive(smallShips.Count >= 2);
        largeToTowerUpgradeButton.gameObject.SetActive(largeShipCount >= 2);
    }

    // Améliorer en tour
    public void UpgradeToTower()
    {
        if (largeShipCount >= 2)
        {
            largeShipCount -= 2;
            // Détruire deux gros vaisseaux
            Destroy(largeShips[0]);
            largeShips.RemoveAt(0);
            Destroy(largeShips[0]);
            largeShips.RemoveAt(0);
            // Créer une tour
            Instantiate(towerPrefab, GetPlanetPosition(startingPlanetId) + new Vector3(0, 0, -1), Quaternion.identity);
        }
        else
        {
            Debug.Log("Pas assez de gros vaisseaux.");
        }
    }

    // Fonction pour obtenir la position de la planète
    private Vector3 GetPlanetPosition(int planetId)
    {
        return new Vector3(planetId * 2, 0, 0);
    }
}