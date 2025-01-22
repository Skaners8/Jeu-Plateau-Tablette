using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planet : MonoBehaviour
{
    public enum ResourceType { None, Food, Minerals }
    public bool isDiscovered = false;
    public bool _isColonized = false;
    public int colonizationPoints = 10;
    public List<ResourceType> resourceTypes = new List<ResourceType>();
    public int resourcesProduced = 0;
    public List<GameObject> shipsOnPlanet = new List<GameObject>();
    public Transform[] shipSlots;
    public bool[] slotOccupied;
    public Player player;
    public ShipSelection shipSelection;
    public bool hasAddedPoints = false;
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
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer n'a pas été trouvé sur la planète.");
        }
        UpdateSprite();
        if (isDiscovered)
        {
            GenerateResources();
        }
        if (shipSlots.Length > 0)
        {
            slotOccupied = new bool[shipSlots.Length];  // Initialisation correcte des slots
        }
        else
        {
            Debug.LogWarning("Aucun slot de vaisseau défini pour la planète.");
        }
    }

    // Ajout d'un vaisseau au slot libre
    public void AddShipToSlot(GameObject ship)
    {
        // Vérifier si le vaisseau est déjà sur la planète avant de l'ajouter
        if (shipsOnPlanet.Contains(ship))
        {
            Debug.Log("Le vaisseau est déjà sur cette planète.");
            return;  // Ne pas ajouter le vaisseau à nouveau
        }

        // Vérifier s'il y a un slot disponible avant d'ajouter un vaisseau
        bool slotAvailable = false;
        for (int i = 0; i < shipSlots.Length; i++)
        {
            if (!slotOccupied[i])  // Si le slot n'est pas occupé
            {
                slotAvailable = true;
                break;
            }
        }

        if (!slotAvailable)
        {
            Debug.Log("Aucun slot disponible pour ce vaisseau.");
            return;
        }

        // Ajouter le vaisseau au premier slot libre
        for (int i = 0; i < shipSlots.Length; i++)
        {
            if (!slotOccupied[i])  // Si le slot n'est pas occupé
            {
                slotOccupied[i] = true;  // Marque le slot comme occupé
                ship.transform.position = shipSlots[i].position;  // Place le vaisseau dans le slot
                shipsOnPlanet.Add(ship);  // Ajoute le vaisseau à la liste des vaisseaux
                if (!isDiscovered)
                {
                    DiscoverPlanet();
                }
                return;
            }
        }
    }

    // Méthode pour récupérer le prochain slot libre
    public Vector3 GetNextFreeSlot()
    {
        for (int i = 0; i < shipSlots.Length; i++)
        {
            if (!slotOccupied[i])
            {
                return shipSlots[i].position;
            }
        }
        return Vector3.zero; // Si aucun slot n'est disponible
    }

    // Méthode pour libérer un slot occupé lorsque le vaisseau est déplacé
    public void FreeSlot(GameObject ship)
    {
        int index = shipsOnPlanet.IndexOf(ship);
        if (index >= 0)
        {
            shipsOnPlanet.RemoveAt(index);  // Retire le vaisseau de la liste

            // Libérer le bon slot correspondant à ce vaisseau
            for (int i = 0; i < shipSlots.Length; i++)
            {
                if (shipSlots[i].position == ship.transform.position)
                {
                    slotOccupied[i] = false;  // Libère le slot correspondant
                    break;
                }
            }
        }
    }

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
        if (_isColonized && !hasAddedPoints)
        {
            Player currentPlayer = GameManager.Instance.GetCurrentPlayer(); 
            currentPlayer.AddPoints(colonizationPoints);
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
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer(); 
        currentPlayer.AddPoints(25);
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