using UnityEngine;
using UnityEngine.EventSystems;
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

    private void Start()
    {
        // Sauvegarder la position locale d'origine
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
    }

    // Gère le déplacement de la carte quand on commence à la faire glisser
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!clickValid) return;

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

        // Si la carte n'est pas sélectionnée, elle retourne à la position (0,0) dans son parent
        if (!isSelected)
        {
            transform.DOLocalMove(Vector3.zero, 0.5f); // Retourner à (0,0) en 0.5 seconde
        }
        else
        {
            // Si la carte est sélectionnée, elle retourne à sa position actuelle avec la montée
            transform.DOLocalMove(originalPosition + new Vector3(0, 0.75f, 0), 0.5f); // Retourner à la position avec la montée
        }
    }

    // Gère la sélection, désélection et le retournement de la carte lors du clic
    public void OnPointerDown(PointerEventData eventData)
    {
        clickTime = Time.time; // Sauvegarder le moment où on a cliqué
        clickValid = true; // Par défaut, le clic est valide
    }

    // Gère ce qu'il se passe quand on relâche la souris après un clic ou un drag
    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - clickTime < clickDurationThreshold && !isDragging)
        {
            // Si c'est un clic rapide et pas un drag, on fait monter ou descendre la carte
            if (!isSelected)
            {
                // Sélectionne la carte (monte légèrement)
                transform.DOLocalMoveY(originalPosition.y + 0.75f, 0.2f); // Monte de 0.75 unités sur l'axe Y
                isSelected = true;
            }
            else
            {
                // Désélectionne la carte (retour à la position d'origine)
                transform.DOLocalMove(originalPosition, 0.2f);
                isSelected = false;
            }

            // Gérer le retournement de la carte
            if (!isFlipped)
            {
                // Si la carte est actuellement face recto, on la retourne vers le verso
                transform.DORotate(new Vector3(0, 180, 0), 0.5f); // Rotation sur l'axe Y en 0.5 seconde
                isFlipped = true; // Marquer la carte comme retournée
            }
            else
            {
                // Si la carte est face verso, on la retourne vers le recto
                transform.DORotate(new Vector3(0, 0, 0), 0.5f); // Retour à l'angle d'origine sur l'axe Y
                isFlipped = false; // Marquer la carte comme non retournée
            }
        }
        else
        {
            clickValid = false; // Invalide le clic si c'était un drag
        }

        isDragging = false; // Réinitialiser la variable de drag
    }
}
