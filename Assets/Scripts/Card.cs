using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Pour utiliser Image
using DG.Tweening;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 offset;
    private Transform originalParent;
    private Vector3 originalPosition;
    public bool isSelected = false;
    public bool isDragging = false;
    public bool isFlipped = false; // Indique si la carte est retournée
    public float clickTime = 0f; // Temps de début du clic
    public float clickDurationThreshold = 0.2f; // Durée pour distinguer le clic soutenu
    private bool clickValid = true; // Variable pour vérifier si le clic est valide (clic rapide)

    // Référence au GameObject enfant pour la représentation visuelle
    public GameObject visualChild;

    // Ajouter les sprites pour le recto et le verso
    public Sprite rectoSprite;  // Image du recto
    public Sprite versoSprite;  // Image du verso
    private Image imageComponent; // Le composant Image de l'enfant visuel

    private void Start()
    {
        // Sauvegarder la position locale d'origine
        originalParent = transform.parent;
        originalPosition = transform.localPosition;

        // Récupérer le composant Image sur l'enfant visuel
        imageComponent = visualChild.GetComponent<Image>();

        // Vérification pour s'assurer que l'image est bien assignée
        if (imageComponent == null)
        {
            Debug.LogError("L'Image est manquante sur l'enfant visuel: " + visualChild.name);
            return;
        }

        // Assurer que l'image du recto est visible au départ
        imageComponent.sprite = rectoSprite;

        // S'assurer que les raycasts sont activés pour permettre l'interaction
        imageComponent.raycastTarget = true;
    }

    // Gère le déplacement de la carte quand on commence à la faire glisser
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!clickValid) return; // Vérifie si le clic est valide

        isDragging = true; // Commence le drag
        offset = transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
    }

    // Gère le déplacement pendant que l'on fait glisser la carte
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return; // Empêche le drag si la carte n'est pas en cours de glissement
        Vector3 newPos = Camera.main.ScreenToWorldPoint(eventData.position) + offset;
        newPos.z = 0; // Garder la position Z à 0 pour rester en 2D
        transform.position = newPos;
    }

    // Gère la fin du déplacement quand on relâche la carte
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false; // Fin du drag

        if (!isSelected)
        {
            transform.DOLocalMove(Vector3.zero, 0.5f); // Retourner à (0,0) en 0.5 seconde
        }
        else
        {
            transform.DOLocalMove(originalPosition + new Vector3(0, 0.75f, 0), 0.5f); // Retourner à la position avec la montée
        }
    }

    // Gère la sélection, désélection et le retournement de la carte lors du clic
    public void OnPointerDown(PointerEventData eventData)
    {
        clickTime = Time.time;
        clickValid = true;
    }

    // Gère ce qu'il se passe quand on relâche la souris après un clic ou un drag
    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - clickTime < clickDurationThreshold && !isDragging)
        {
            if (!isSelected)
            {
                transform.DOLocalMoveY(originalPosition.y + 0.75f, 0.2f);
                isSelected = true;
            }
            else
            {
                transform.DOLocalMove(originalPosition, 0.2f);
                isSelected = false;
            }

            // Gérer le retournement de l'enfant visuel uniquement
            if (!isFlipped)
            {
                visualChild.transform.DORotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() =>
                {
                    imageComponent.sprite = versoSprite;
                    visualChild.transform.DORotate(new Vector3(0, 180, 0), 0.25f);
                });
                isFlipped = true;
            }
            else
            {
                visualChild.transform.DORotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() =>
                {
                    imageComponent.sprite = rectoSprite;
                    visualChild.transform.DORotate(new Vector3(0, 0, 0), 0.25f);
                });
                isFlipped = false;
            }
        }
        else
        {
            clickValid = false; // Invalide le clic si c'était un drag
        }

        isDragging = false;
    }
}
