using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int playerPoints = 0;

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

    public void AddPoints(int points)
    {
        playerPoints += points;
        Debug.Log("Points added: " + points);
    }

    public void StartNewTurn(List<Planet> planets)
    {
        foreach (Planet planet in planets)
        {
            List<Planet.ResourceType> resources = planet.GetResourcesAtTurnStart(); // Récupère la liste des ressources
            int resourceCount = resources.Count; // Compte le nombre de ressources

            if (resourceCount > 0)
            {
                Debug.Log("Player receives " + resourceCount + " resources from planet.");
                // Mise à jour des ressources du joueur ici
            }
        }
    }
}