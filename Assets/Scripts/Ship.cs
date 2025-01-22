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

        // Mise à jour de la planète actuelle du vaisseau
        shipSelection.currentPlanet = planet.gameObject;  // Met à jour currentPlanet dans ShipSelection
    }

    // Méthode pour tenter de coloniser une planète.
    public void AttemptColonization(Planet planet)
    {
        // Vérifie si la planète est colonisable.
        if (planet.CanColonize())
        {
            planet.ColonizePlanet(); // Colonise la planète.
            Debug.Log("Planet colonisée!"); // Affiche un message de débogage indiquant la colonisation.
        }
    }
}