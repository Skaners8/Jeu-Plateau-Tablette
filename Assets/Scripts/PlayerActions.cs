using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    // Variables pour les ressources, les vaisseaux, et les planètes
    public int resources;
    public int food;
    public int minerals;
    public int shipCount;
    public int largeShipCount;
    public int planetCount;
    public int turnCount = 0;

    // Boutons UI pour les améliorations
    public Button normalToLargeUpgradeButton;
    public Button largeToTowerUpgradeButton;
    public Button createShipButton; // Nouveau bouton pour créer un vaisseau

    // Préfabriqués pour les vaisseaux et les tours
    public GameObject normalShipPrefab;
    public GameObject largeShipPrefab;
    public GameObject towerPrefab;

    // Localisation des vaisseaux sur les planètes
    public Dictionary<int, int> smallShipLocations = new Dictionary<int, int>();
    public Dictionary<int, int> largeShipLocations = new Dictionary<int, int>();

    // Planète de départ du joueur
    public int startingPlanetId = 0; // Planète de départ par défaut

    void Start()
    {
        // Désactiver les boutons d'amélioration au départ
        normalToLargeUpgradeButton.gameObject.SetActive(false);
        largeToTowerUpgradeButton.gameObject.SetActive(false);
        // Désactiver le bouton de création de vaisseau au départ
        createShipButton.gameObject.SetActive(false);

        // Ajouter les listeners pour les boutons d'amélioration
        createShipButton.onClick.AddListener(() => CreateNormalShip(startingPlanetId));
        normalToLargeUpgradeButton.onClick.AddListener(() => UpgradeToLargeShip());
        largeToTowerUpgradeButton.onClick.AddListener(() => UpgradeToTower());
    }

    void Update()
    {
        // Activer le bouton de création de vaisseau si assez de ressources
        createShipButton.gameObject.SetActive(minerals >= 3 && food >= 3);
        // Mettre à jour les boutons d'amélioration
        UpdateUpgradeButtons(startingPlanetId);
    }

    // Création d'un vaisseau normal avec 3 minerais et 3 nourritures
    public void CreateNormalShip(int planetId)
    {
        if (minerals >= 3 && food >= 3)
        {
            minerals -= 3;
            food -= 3;
            shipCount++;
            // Ajouter le vaisseau à la localisation de la planète
            if (!smallShipLocations.ContainsKey(planetId))
                smallShipLocations[planetId] = 0;
            smallShipLocations[planetId]++;
            // Instancier le vaisseau sur la planète de départ
            Instantiate(normalShipPrefab, GetPlanetPosition(planetId), Quaternion.identity);
            Debug.Log("Vaisseau normal créé à la planète de départ.");
        }
        else
        {
            Debug.Log("Pas assez de minerais ou de nourriture.");
        }
    }

    // Mise à jour des boutons d'amélioration en fonction des vaisseaux présents sur la planète
    public void UpdateUpgradeButtons(int planetId)
    {
        // Activer le bouton si le joueur possède au moins 2 vaisseaux normaux au total
        int totalSmallShips = 0;
        foreach (int shipCount in smallShipLocations.Values)
        {
            totalSmallShips += shipCount;
        }
        normalToLargeUpgradeButton.gameObject.SetActive(totalSmallShips >= 2);
        largeToTowerUpgradeButton.gameObject.SetActive(largeShipCount >= 2); // Exemple pour activer le bouton de tour
    }

    public void UpgradeToLargeShip()
    {
        // Vérifier si le joueur a au moins 2 vaisseaux normaux
        int totalSmallShips = 0;
        foreach (int shipCount in smallShipLocations.Values)
        {
            totalSmallShips += shipCount;
        }
        if (totalSmallShips >= 2)
        {
            // Supprimer 2 vaisseaux normaux
            foreach (int planetId in new List<int>(smallShipLocations.Keys))
            {
                if (smallShipLocations[planetId] > 0)
                {
                    int shipsToRemove = Mathf.Min(2, smallShipLocations[planetId]);
                    smallShipLocations[planetId] -= shipsToRemove;
                    totalSmallShips -= shipsToRemove;
                    if (totalSmallShips < 2) break;
                }
            }
            shipCount -= 2;
            largeShipCount++;
            // Créer un gros vaisseau sur la planète de départ
            Instantiate(largeShipPrefab, GetPlanetPosition(startingPlanetId), Quaternion.identity);
            Debug.Log("Amélioration effectuée : Gros vaisseau créé.");
            // Mettre à jour les boutons d'amélioration
            UpdateUpgradeButtons(startingPlanetId);
        }
        else
        {
            Debug.Log("Pas assez de vaisseaux normaux pour l'amélioration.");
        }
    }

    public void UpgradeToTower()
    {
        // Logique pour améliorer un gros vaisseau en tour
        // Ajoute la logique ici selon tes besoins
        Debug.Log("Amélioration effectuée : Tour créée.");
    }

    // Fonction pour obtenir la position de la planète
    private Vector3 GetPlanetPosition(int planetId)
    {
        return new Vector3(planetId * 2, 0, 0); // Modifier selon ta logique
    }

    // Vérifier si une planète est colonisée
    private bool IsPlanetColonized(int planetId)
    {
        return planetCount > planetId; // Logique simplifiée
    }
}