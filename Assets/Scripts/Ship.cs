using UnityEngine;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{
    public Player owner; // Propriétaire du vaisseau
    public bool isLargeShip = false;
    public int actionPoints = 1;
    public delegate void ShipClicked(GameObject ship);
    public event ShipClicked OnShipClicked;
    public ShipSelection shipSelection { get; set; }

    private void Start()
    {
        shipSelection = FindObjectOfType<ShipSelection>();
    }

    private void OnMouseDown()
    {
        // On récupère le joueur actuellement actif
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer(); // Récupère le joueur dont c'est le tour
        
        // Vérification si c'est le tour du joueur et s'il est le propriétaire du vaisseau
        if (owner == currentPlayer && GameManager.Instance.isPlayerTurn)
        {
            OnShipClicked?.Invoke(gameObject);
            shipSelection.SelectShip(gameObject);
        }
        else
        {
            shipSelection.SelectShip(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Planet planet = other.gameObject.GetComponent<Planet>();
        if (planet != null)
        {
            VisitPlanet(planet);
        }
    }

    // Méthode pour visiter une planète.
    public void VisitPlanet(Planet planet)
    {
        // Ajoute le vaisseau à la liste des visiteurs de la planète et la découvre si elle ne l'est pas déjà.
        planet.VisitPlanet(this.gameObject);

        // Vérifie si des ressources peuvent être collectées et si le vaisseau dispose encore de points d'action.
        if (planet.CanCollectResources(this.gameObject) && actionPoints > 0)
        {
            CollectResources(planet); // Appelle la méthode de collecte de ressources.
        }
    }

    // Méthode privée pour collecter les ressources d'une planète.
    private void CollectResources(Planet planet)
    {
        // Vérifie si le vaisseau est grand (seuls les grands vaisseaux collectent les ressources).
        if (isLargeShip == true)
        {
            actionPoints--; // Réduit le nombre de points d'action du vaisseau après la collecte.
            Debug.Log("Resources collected from planet."); // Affiche un message de débogage indiquant la collecte de ressources.
        }
    }

    // Méthode pour tenter de coloniser une planète.
    public void AttemptColonization(Planet planet)
    {
        // Vérifie si la planète est colonisable.
        if (planet.CanColonize())
        {
            planet.ColonizePlanet(); // Colonise la planète.
            Debug.Log("Planet colonized!"); // Affiche un message de débogage indiquant la colonisation.
        }
    }
}