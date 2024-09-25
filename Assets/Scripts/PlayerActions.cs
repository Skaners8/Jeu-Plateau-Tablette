using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public int resources;  // Ressources du joueur
    public int shipCount;  // Nombre de vaisseaux
    public int planetCount; // Nombre de planètes colonisées
    public int turnCount = 0; // Nombre de tours écoulés

    // Récupération des ressources des planètes colonisées
    public void CollectResourcesFromPlanets() {
        int resourcesCollected = planetCount * 10; // Exemple: 10 ressources par planète
        resources += resourcesCollected;
        Debug.Log("Ressources collectées des planètes: " + resourcesCollected);
    }

    // Déplacer un vaisseau vers une nouvelle position
    public void MoveShip(Vector3 newPosition) {
        // Exemple simple de déplacement
        if (shipCount > 0) {
            // Code de déplacement ici
            Debug.Log("Vaisseau déplacé vers " + newPosition);
        } else {
            Debug.Log("Pas de vaisseau disponible.");
        }
    }

    // Améliorer les vaisseaux
    public void UpgradeShips() {
        int upgradeCost = 50; // Exemple coût d'amélioration
        if (resources >= upgradeCost) {
            resources -= upgradeCost;
            Debug.Log("Vaisseaux améliorés.");
        } else {
            Debug.Log("Pas assez de ressources pour améliorer les vaisseaux.");
        }
    }

    // Récupérer des ressources via un type de vaisseau spécifique
    public void CollectResourcesFromShip() {
        if (shipCount > 0) {
            int resourcesFromShip = 5; // Exemple: chaque vaisseau collecte 5 ressources
            resources += resourcesFromShip;
            Debug.Log("Ressources collectées par un vaisseau: " + resourcesFromShip);
        } else {
            Debug.Log("Pas de vaisseau disponible pour collecter des ressources.");
        }
    }

    // Fonction ajoutée pour gérer les tours de jeu
    public void EndTurn() {
        turnCount++;
        Debug.Log("Tour terminé. Nombre de tours: " + turnCount);
    }

    // Appel pour chaque tour
    public void StartTurn() {
        CollectResourcesFromPlanets();
        // Actions possibles après le début du tour :
        // - MoveShip()
        // - UpgradeShips()
        // - CollectResourcesFromShip()
        Debug.Log("Début du tour " + turnCount);
    }
}