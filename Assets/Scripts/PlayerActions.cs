using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Assuming you're using Unity's UI system

public class PlayerActions : MonoBehaviour
{
    public int resources;
    public int food;
    public int minerals;
    public int shipCount;    // Number of normal ships
    public int largeShipCount; // Number of large ships
    public int planetCount;  // Number of colonized planets
    public int turnCount = 0; // Number of turns elapsed

    public Button normalToLargeUpgradeButton;  // UI Button to upgrade normal ships to large ships
    public Button largeToTowerUpgradeButton;   // UI Button to upgrade large ships to tower for colonization

    // Call this method each time ship count changes to update UI buttons
    public void UpdateUpgradeButtons()
    {
        // Check if player has at least 2 normal ships to enable upgrade to large ship
        normalToLargeUpgradeButton.interactable = shipCount >= 2;

        // Check if player has at least 2 large ships to enable upgrade to tower
        largeToTowerUpgradeButton.interactable = largeShipCount >= 2;
    }

    // Collect resources from colonized planets
    public void CollectResourcesFromPlanets()
    {
        int resourcesCollected = planetCount * 10; // Example: 10 resources per planet
        resources += resourcesCollected;
        minerals += resourcesCollected / 2;  // Assume 50% minerals
        food += resourcesCollected / 2;      // Assume 50% food
        Debug.Log("Resources collected from planets: " + resourcesCollected);
        Debug.Log("Food: " + food + ", Minerals: " + minerals);
    }

    // Create a normal ship for 3 minerals and 3 food
    public void CreateNormalShip()
    {
        if (minerals >= 3 && food >= 3)
        {
            minerals -= 3;
            food -= 3;
            shipCount++;
            Debug.Log("Normal ship created.");
            Debug.Log("Remaining food: " + food + ", Remaining minerals: " + minerals);

            // Update UI buttons after creating a ship
            UpdateUpgradeButtons();
        }
        else
        {
            Debug.Log("Not enough minerals or food to create a ship.");
        }
    }

    // Upgrade 2 normal ships to create a large ship
    public void UpgradeToLargeShip()
    {
        if (shipCount >= 2)
        {
            shipCount -= 2;
            largeShipCount++;
            Debug.Log("Large ship created.");

            // Update UI buttons after upgrading
            UpdateUpgradeButtons();
        }
        else
        {
            Debug.Log("Not enough normal ships to create a large ship.");
        }
    }

    // Upgrade 2 large ships to create a tower and colonize the planet
    public void CreateTowerToColonize()
    {
        if (largeShipCount >= 2)
        {
            largeShipCount -= 2;
            planetCount++;
            Debug.Log("Tower created, planet colonized.");

            // Update UI buttons after colonization
            UpdateUpgradeButtons();
        }
        else
        {
            Debug.Log("Not enough large ships to create a tower.");
        }
    }

    // Move a ship to a new position
    public void MoveShip(Vector3 newPosition)
    {
        if (shipCount > 0)
        {
            // Ship movement code here
            Debug.Log("Ship moved to " + newPosition);
        }
        else
        {
            Debug.Log("No available ships to move.");
        }
    }

    // Function to handle end of turn
    public void EndTurn()
    {
        turnCount++;
        Debug.Log("Turn ended. Number of turns: " + turnCount);
    }

    // Call at the start of each turn
    public void StartTurn()
    {
        CollectResourcesFromPlanets();
        Debug.Log("Start of turn " + turnCount);
    }
}