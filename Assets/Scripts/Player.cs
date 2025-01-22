using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int playerID;
    public int minerals;            // Quantité de minerais
    public int food;                // Quantité de nourriture
    public int shipCount;           // Nombre de petits vaisseaux
    public int largeShipCount;      // Nombre de gros vaisseaux
    public int points;              // Points du joueur
    public int startingPlanetId;    // ID de la planète de départ

    public Player(string name, int id)
    {
        playerName = name;
        playerID = id;
        minerals = 12;
        food = 12;
        shipCount = 0;
        largeShipCount = 0;
        points = 0;
        startingPlanetId = -1; // Valeur par défaut si pas encore assignée
    }

    // Méthode pour ajouter des ressources
    public void AddResource(Planet.ResourceType resource)
    {
        switch (resource)
        {
            case Planet.ResourceType.Food:
                food += 1;
                break;
            case Planet.ResourceType.Minerals:
                minerals += 1;
                break;
        }
    }

    // Méthode pour ajouter des points
    public void AddPoints(int amount)
    {
        points += amount;
    }
}