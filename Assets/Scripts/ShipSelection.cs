using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelection : MonoBehaviour
{
    private GameObject selectedShip;
    private GameObject targetPlanet;
    public GameObject currentPlanet { get; set; }
    public GameObject confirmationPanel; // Panel contenant la boîte de dialogue
    public Text confirmationText; // Texte dans la boîte de dialogue
    public Button confirmButton; // Bouton "Oui"
    public Button cancelButton; // Bouton d'annulation pour voyage
    public Button cancelButton2; // Bouton d'annulation pour sélection vaisseau
    public GameObject SelectedShip { get; set; }
    public GameObject selectionPanel; // Panel UI contenant le texte et le bouton d'annulation
    public Text selectionText; // Texte dans le panel

    public Text errorMessageText; // Texte pour afficher le message d'erreur (ajouté dans Canvas)

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmMove);
        cancelButton.onClick.AddListener(OnCancelMove);
        cancelButton2.onClick.AddListener(CancelSelection);
        confirmationPanel.SetActive(false);
        selectionPanel.SetActive(false);
        errorMessageText.gameObject.SetActive(false); // Cacher le message d'erreur au début

        GameObject[] ships = GameObject.FindGameObjectsWithTag("Ship");
        foreach (GameObject ship in ships)
        {
            Ship shipComponent = ship.GetComponent<Ship>();
            if (shipComponent != null)
            {
                shipComponent.OnShipClicked += SelectShip;
            }
        }
    }

    public void SelectShip(GameObject ship)
    {
        Ship shipComponent = ship.GetComponent<Ship>();

        // Vérifier que le vaisseau appartient au joueur actuel
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer(); // Récupère le joueur dont c'est le tour
        if (shipComponent.owner == currentPlayer)
        {
            selectedShip = ship;
            selectionPanel.SetActive(true); // Affiche le panel
            selectionText.text = "Choisissez la destination"; // Change le texte
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => CancelSelection());
        }
        else
        {
            StartCoroutine(DisplayErrorMessage());
        }
    }

    private IEnumerator DisplayErrorMessage()
    {
        // Affiche le message
        Debug.Log("DisplayErrorMessage Coroutine called"); // Vérifie que la coroutine est appelée
        errorMessageText.gameObject.SetActive(true);
        errorMessageText.text = "Ce vaisseau ne vous appartient pas";

        // Attendre 2 secondes avant de commencer la disparition
        yield return new WaitForSeconds(2f);

        // Début de la disparition avec effet de transparence
        float elapsedTime = 0f;
        Color initialColor = errorMessageText.color;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime); // Fade out progress
            errorMessageText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Cacher le message après la disparition
        errorMessageText.gameObject.SetActive(false);

        // Remettre la transparence à 100% (complètement opaque)
        errorMessageText.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
    }

    public void CancelSelection()
    {
        selectedShip = null;
        selectionPanel.SetActive(false);
        confirmationPanel.SetActive(false);
    }

    public void SelectPlanet(GameObject planet)
    {
        if (selectedShip != null && planet != currentPlanet && planet != selectedShip.GetComponent<Ship>().shipSelection.currentPlanet)
        {
            targetPlanet = planet;
            confirmationPanel.SetActive(true);
            confirmationText.text = $"Envoyer {selectedShip.name} vers {planet.name} ?";
        }
    }

    private void OnConfirmMove()
    {
        if (selectedShip != null && targetPlanet != null)
        {
            // Appelle la méthode AddShipToSlot de la planète cible pour gérer les slots
            targetPlanet.GetComponent<Planet>().AddShipToSlot(selectedShip);

            // Met à jour la référence de la planète actuelle et réinitialise les sélections
            currentPlanet = targetPlanet;
            selectedShip = null;
            confirmationPanel.SetActive(false);
            selectionPanel.SetActive(false);
        }
    }

    private void OnCancelMove()
    {
        confirmationPanel.SetActive(false);
        selectedShip = null;
        targetPlanet = null;
    }
}