using UnityEngine;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{
    public bool isLargeShip = false; // Assume large ships can colonize
    public int actionPoints = 1;

    private void OnCollisionEnter(Collision collision)
    {
        Planet planet = collision.gameObject.GetComponent<Planet>();
        if (planet != null)
        {
            // Appeler la méthode pour visiter la planète
            VisitPlanet(planet);
            Debug.Log("TEST");
        }
    }

    public void VisitPlanet(Planet planet)
    {
        // Visite la planète et la découvre si nécessaire
        planet.VisitPlanet(this.gameObject);

        // Vérifier si des ressources peuvent être collectées
        if (planet.CanCollectResources(this.gameObject) && actionPoints > 0)
        {
            CollectResources(planet);
        }
    }

    private void CollectResources(Planet planet)
    {
        // Simuler la collecte de ressources
        if (isLargeShip == true){
            actionPoints--;
            Debug.Log("Resources collected from planet.");
        }
    }

    public void AttemptColonization(Planet planet)
    {
        if (planet.CanColonize())
        {
            planet.ColonizePlanet();
            Debug.Log("Planet colonized!");
        }
    }
}