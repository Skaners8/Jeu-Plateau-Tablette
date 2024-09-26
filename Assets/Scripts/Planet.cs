using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public enum ResourceType { None, Food, Minerals }
    public bool isDiscovered = false;
    public bool isColonized = false;
    public int colonizationPoints = 10;
    public List<ResourceType> resourceTypes = new List<ResourceType>(); // Liste des ressources produites par la planète
    public int resourcesProduced = 0; // 0, 1, or 2
    public List<GameObject> shipsOnPlanet = new List<GameObject>();

    private void Start()
    {
        // Le nombre de ressources est toujours tiré aléatoirement entre 0 et 2
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

        // Faire le tirage pour les ressources de la planète (une seule fois lors de la découverte)
        for (int i = 0; i < resourcesProduced; i++)
        {
            if (Random.value > 0.5f)
            {
                resourceTypes.Add(ResourceType.Food); // 50% de chance d'ajouter de la nourriture
            }
            else
            {
                resourceTypes.Add(ResourceType.Minerals); // 50% de chance d'ajouter des minerais
            }
        }

        // Trigger visuel pour montrer que la planète est découverte
    }

    public bool CanCollectResources(GameObject ship)
    {
        return isDiscovered && shipsOnPlanet.Contains(ship) && !isColonized;
    }

    public bool CanColonize()
    {
        return shipsOnPlanet.Count >= 2 && !isColonized;
    }

    public void ColonizePlanet()
    {
        isColonized = true;
        GameManager.Instance.AddPoints(colonizationPoints);
        // Mise à jour visuelle pour montrer la colonisation
    }

    public List<ResourceType> GetResourcesAtTurnStart()
    {
        if (isColonized)
        {
            return resourceTypes; // Le joueur récupère les ressources tirées à chaque tour
        }
        return new List<ResourceType>(); // Pas de ressources si la planète n'est pas colonisée
    }
}