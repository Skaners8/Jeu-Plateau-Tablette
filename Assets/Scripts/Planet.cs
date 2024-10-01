using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public enum ResourceType { None, Food, Minerals }
    public bool isDiscovered = false;

    [SerializeField] // Rendre la variable visible dans l'éditeur
    public bool _isColonized = false; // Variable privée pour la colonisation
    public int colonizationPoints = 10;
    public List<ResourceType> resourceTypes = new List<ResourceType>(); // Liste des ressources produites par la planète
    public int resourcesProduced = 0; // 0, 1, or 2
    public List<GameObject> shipsOnPlanet = new List<GameObject>();
    public PlayerActions playerActions; // Référence au script PlayerActions
    private bool hasAddedPoints = false; // Variable pour s'assurer que les points ne sont ajoutés qu'une seule fois

    // Propriété pour la colonisation
    public bool isColonized
    {
        get { return _isColonized; }
        set
        {
            if (_isColonized != value)
            {
                _isColonized = value;
            }
        }
    }

    private void Start()
    {
        playerActions = FindObjectOfType<PlayerActions>();
        if (isDiscovered)
        {
            GenerateResources();
        }
    }

    private void Update()
    {
        if (isDiscovered && resourceTypes.Count == 0)
        {
            GenerateResources();
        }

        if (_isColonized == true && hasAddedPoints == false){
            Debug.Log("TEST");
            playerActions.AddPoints(colonizationPoints);
            hasAddedPoints = true;
        }
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
        GenerateResources();
    }

    private void GenerateResources()
    {
        resourcesProduced = Random.Range(0, 3);
        resourceTypes.Clear();
        for (int i = 0; i < resourcesProduced; i++)
        {
            if (Random.value > 0.5f)
            {
                resourceTypes.Add(ResourceType.Food);
            }
            else
            {
                resourceTypes.Add(ResourceType.Minerals);
            }
        }
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
        // La ligne ci-dessous a été modifiée pour ajouter des points directement dans PlayerActions
        playerActions.AddPoints(colonizationPoints);
    }

    public void Colonize()
    {
        if (!_isColonized)
        {
            _isColonized = true;
            playerActions.AddPoints(10); // Ajoutez des points lorsque la planète est colonisée
        }
    }

    public List<ResourceType> GetResourcesAtTurnStart()
    {
        if (isColonized)
        {
            return resourceTypes;
        }
        return new List<ResourceType>();
    }
}