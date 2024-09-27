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
    public Button createShipButton;

    // Préfabriqués pour les vaisseaux et les tours
    public GameObject normalShipPrefab;
    public GameObject largeShipPrefab;
    public GameObject towerPrefab;

    // Listes pour garder une trace des vaisseaux créés
    private List<GameObject> smallShips = new List<GameObject>();
    private List<GameObject> largeShips = new List<GameObject>();

    // Localisation des vaisseaux sur les planètes
    public Dictionary<int, int> smallShipLocations = new Dictionary<int, int>();
    public Dictionary<int, int> largeShipLocations = new Dictionary<int, int>();

    // Planète de départ du joueur
    public int startingPlanetId = 0;

    void Start()
    {
        // Désactiver les boutons d'amélioration au départ
        normalToLargeUpgradeButton.gameObject.SetActive(false);
        largeToTowerUpgradeButton.gameObject.SetActive(false);
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
            if (!smallShipLocations.ContainsKey(planetId))
                smallShipLocations[planetId] = 0;
            smallShipLocations[planetId]++;
            // Instancier le vaisseau sur la planète de départ
            GameObject newShip = Instantiate(normalShipPrefab, GetPlanetPosition(planetId), Quaternion.identity);
            smallShips.Add(newShip); // Ajouter à la liste des petits vaisseaux
            Debug.Log("Vaisseau normal créé à la planète de départ.");
        }
        else
        {
            Debug.Log("Pas assez de minerais ou de nourriture.");
        }
    }

    // Mise à jour des boutons d'amélioration
    public void UpdateUpgradeButtons(int planetId)
    {
        int totalSmallShips = 0;
        foreach (int shipCount in smallShipLocations.Values)
        {
            totalSmallShips += shipCount;
        }
        normalToLargeUpgradeButton.gameObject.SetActive(totalSmallShips >= 2);
        largeToTowerUpgradeButton.gameObject.SetActive(largeShipCount >= 2);
    }

    public void UpgradeToLargeShip()
    {
        int totalSmallShips = 0;
        foreach (int shipCount in smallShipLocations.Values)
        {
            totalSmallShips += shipCount;
        }
        if (totalSmallShips >= 2)
        {
            int shipsToRemove = 2;
            foreach (int planetId in new List<int>(smallShipLocations.Keys))
            {
                if (smallShipLocations[planetId] > 0)
                {
                    int availableShips = smallShipLocations[planetId];
                    if (availableShips >= shipsToRemove)
                    {
                        smallShipLocations[planetId] -= shipsToRemove;
                        shipCount -= shipsToRemove; // Corrected here
                        largeShipCount++;

                        // Détruire les vaisseaux normaux
                        for (int i = 0; i < shipsToRemove; i++)
                        {
                            if (smallShips.Count > 0)
                            {
                                Destroy(smallShips[0]);
                                smallShips.RemoveAt(0);
                            }
                        }
                        // Créer un nouveau vaisseau gros
                        GameObject newLargeShip = Instantiate(largeShipPrefab, GetPlanetPosition(startingPlanetId), Quaternion.identity);
                        largeShips.Add(newLargeShip);
                        Debug.Log("Amélioration effectuée : Gros vaisseau créé.");
                        UpdateUpgradeButtons(startingPlanetId);
                        return; // On sort de la méthode après avoir créé le gros vaisseau
                    }
                    else
                    {
                        // Si on n'a pas assez de vaisseaux, on les retire tous
                        shipsToRemove -= availableShips;
                        shipCount -= availableShips; // Corrected here
                        smallShipLocations[planetId] = 0;
                        for (int i = 0; i < availableShips; i++)
                        {
                            if (smallShips.Count > 0)
                            {
                                Destroy(smallShips[0]);
                                smallShips.RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Pas assez de vaisseaux normaux pour l'amélioration.");
        }
    }

    public void UpgradeToTower()
    {
        // Vérifie qu'il y a assez de gros vaisseaux
        if (largeShipCount >= 2)
        {
            int shipsToRemove = 2;
            // Déduction des gros vaisseaux
            largeShipCount -= shipsToRemove;

            // Détruire les gros vaisseaux
            for (int i = 0; i < shipsToRemove; i++)
            {
                if (largeShips.Count > 0)
                {
                    Destroy(largeShips[0]);
                    largeShips.RemoveAt(0);
                }
            }
            // Créer une nouvelle tour sur la planète du joueur
            Instantiate(towerPrefab, GetPlanetPosition(startingPlanetId), Quaternion.identity);
            Debug.Log("Amélioration effectuée : Tour créée.");
            UpdateUpgradeButtons(startingPlanetId);
        }
        else
        {
            Debug.Log("Pas assez de gros vaisseaux pour l'amélioration en tour.");
        }
    }

    // Fonction pour obtenir la position de la planète
    private Vector3 GetPlanetPosition(int planetId)
    {
        return new Vector3(planetId * 2, 0, 0);
    }

    // Vérifier si une planète est colonisée
    private bool IsPlanetColonized(int planetId)
    {
        return planetCount > planetId;
    }
}