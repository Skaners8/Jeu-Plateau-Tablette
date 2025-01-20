using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planet : MonoBehaviour
{
    public enum ResourceType { None, Food, Minerals }
    public bool isDiscovered = false;
    public bool _isColonized = false;
    public int colonizationPoints = 10;
    // Liste des types de ressources
    public List<ResourceType> resourceTypes = new List<ResourceType>();
    public int resourcesProduced = 0;
    // Liste des vaisseaux sur la planète
    public List<GameObject> shipsOnPlanet = new List<GameObject>();
    // Liste des positions des slots pour placer les vaisseaux
    public Transform[] shipSlots;
    public bool[] slotOccupied; // Statut pour savoir si un slot est occupé
    public Player player;  // Remplacer playerActions par player
    public PlayerActions playerActions;
    public ShipSelection shipSelection;
    public bool hasAddedPoints = false;
    // Gestion des sprites
    public Sprite undiscoveredSprite;
    public Sprite discoveredSprite;
    public Sprite colonizedSprite;
    private SpriteRenderer spriteRenderer;
    public bool isColonized
    {
        get { return _isColonized; }
        set
        {
            if (_isColonized != value)
            {
                _isColonized = value;
                UpdateSprite();
            }
        }
    }

    private void Awake()
    {
        shipSelection = FindObjectOfType<ShipSelection>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
        if (isDiscovered)
        {
            GenerateResources();
        }
        slotOccupied = new bool[shipSlots.Length];
    }

    public void AddShipToSlot(GameObject ship)
    {
        for (int i = 0; i < shipSlots.Length; i++)
        {
            if (!slotOccupied[i])
            {
                slotOccupied[i] = true;
                ship.transform.position = shipSlots[i].position;
                shipsOnPlanet.Add(ship);
                if (!isDiscovered)
                {
                    DiscoverPlanet();
                }
                return;
            }
        }
        Debug.Log("Aucun slot disponible pour ce vaisseau.");
    }

    public delegate void PlanetClicked(GameObject planet);
    private void OnMouseDown()
    {
        if (!isColonized)
        {
            FindObjectOfType<ShipSelection>().SelectPlanet(this.gameObject);
        }
    }

    private void Update()
    {
        if (isDiscovered && resourceTypes.Count == 0)
        {
            GenerateResources();
        }
        // Cette condition vérifie l'état de colonisation et l'ajout de points
        if (_isColonized && !hasAddedPoints && player.startingPlanetId != this.GetInstanceID())
        {
            player.AddPoints(colonizationPoints);  // Ajouter des points seulement pour les planètes non de départ
            hasAddedPoints = true;
        }
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null) { spriteRenderer = GetComponent<SpriteRenderer>(); }
        if (_isColonized)
        {
            spriteRenderer.sprite = colonizedSprite;
        }
        else if (isDiscovered)
        {
            spriteRenderer.sprite = discoveredSprite;
        }
        else
        {
            spriteRenderer.sprite = undiscoveredSprite;
        }
    }

    public void VisitPlanet(GameObject ship)
    {
        if (!isDiscovered)
        {
            DiscoverPlanet();
        }
        if (!shipsOnPlanet.Contains(ship))
        {
            AddShipToSlot(ship);
        }
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
        if (CanColonize() && !hasAddedPoints)
        {
            isColonized = true;
            if (player.startingPlanetId != this.GetInstanceID())
            {
                player.AddPoints(colonizationPoints);  // Ajouter des points seulement pour les planètes non de départ
                hasAddedPoints = true;
            }
        }
    }

    public void GiveResourcesToPlayer(Player player)
    {
        if (isColonized)
        {
            foreach (ResourceType resource in resourceTypes)
            {
                player.AddResource(resource);  // Appeler AddResource sur Player
            }
        }
    }
}