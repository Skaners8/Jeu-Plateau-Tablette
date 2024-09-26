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
            int resources = planet.GetResourcesAtTurnStart();
            if (resources > 0)
            {
                Debug.Log("Player receives " + resources + " resources from planet.");
                // Update player resource count
            }
        }
    }
}