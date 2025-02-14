using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelection : MonoBehaviour
{
    public GameObject selectedShip;
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

            // Mettre à jour la planète actuelle du vaisseau
            currentPlanet = shipComponent.shipSelection.currentPlanet;
        }
        else
        {
            errorMessageText.text = "Ce vaisseau ne vous appartient pas";
            StartCoroutine(DisplayErrorMessage());
        }
    }

    public void SelectPlanet(GameObject planet)
    {
        // Si un vaisseau est déjà sélectionné et que la planète n'est pas la planète actuelle
        if (selectedShip != null)
        {
            Planet planetComponent = planet.GetComponent<Planet>();
            
            // Vérifie si la planète est la même que celle où se trouve le vaisseau
            if (planet == currentPlanet)
            {
                return;
            }
            
            // Planète sélectionnée pour déplacer le vaisseau
            targetPlanet = planet;
            confirmationPanel.SetActive(true);
            confirmationText.text = $"Envoyer {selectedShip.name} vers {planet.name} ?";
        }
    }

    public void OnConfirmMove()
    {
        if (selectedShip != null && targetPlanet != null)
        {
            // Libérer le slot de la planète actuelle avant de déplacer le vaisseau
            if (currentPlanet != null)
            {
                currentPlanet.GetComponent<Planet>().FreeSlot(selectedShip);
            }

            // Ajouter le vaisseau à la nouvelle planète
            targetPlanet.GetComponent<Planet>().AddShipToSlot(selectedShip);

            // Mise à jour de la planète actuelle
            currentPlanet = targetPlanet;

            // Réinitialisation des sélections
            selectedShip = null;
            targetPlanet = null;
            confirmationPanel.SetActive(false);
            selectionPanel.SetActive(false);

            GameManager.Instance.ActionTaken();
        }
    }

    private void OnCancelMove()
    {
        confirmationPanel.SetActive(false);
        selectedShip = null;
        targetPlanet = null;
    }

    public void CancelSelection()
    {
        selectedShip = null;
        selectionPanel.SetActive(false);
        confirmationPanel.SetActive(false);
    }

    private IEnumerator DisplayErrorMessage()
    {
        errorMessageText.gameObject.SetActive(true);

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
}