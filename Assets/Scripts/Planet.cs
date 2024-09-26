using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public enum ResourceType { None, Resource1, Resource2 }
    public bool isDiscovered = false;
    public bool isColonized = false;
    public int colonizationPoints = 10;
    public ResourceType resourceType;
    public int resourcesProduced = 0; // 0, 1, or 2

    public List<GameObject> shipsOnPlanet = new List<GameObject>();

    private void Start()
    {
        // Randomize resource production (can be 0, 1, or 2)
        resourcesProduced = Random.Range(0, 3); 
    }

    public void VisitPlanet(GameObject ship)
    {
        if (!isDiscovered)
        {
            DiscoverPlanet();
        }
        shipsOnPlanet.Add(ship);
    }

    private void DiscoverPlanet()
    {
        isDiscovered = true;
        // Trigger visual update to show the planet
    }

    public bool CanCollectResources(GameObject ship)
    {
        // Resource collection requires a large ship (you can add a check for ship size here)
        return isDiscovered && shipsOnPlanet.Contains(ship) && !isColonized;
    }

    public bool CanColonize()
    {
        // Requires 2 large ships on the planet
        return shipsOnPlanet.Count >= 2 && !isColonized;
    }

    public void ColonizePlanet()
    {
        isColonized = true;
        // Award points for colonization
        GameManager.Instance.AddPoints(colonizationPoints);
        // Trigger visual update for the colonized planet (e.g., build a tower)
    }

    public int GetResourcesAtTurnStart()
    {
        if (isColonized)
        {
            return resourcesProduced; // Give resources at the start of the turn
        }
        return 0;
    }

    // Méthode publique pour vérifier si la planète est découverte
    public bool IsPlanetDiscovered()
    {
        return isDiscovered;
    }
}