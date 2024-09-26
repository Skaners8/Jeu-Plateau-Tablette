using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalPosition;
    private bool isSelected = false;

    private void Start()
    {
        // Sauvegarder la position locale d'origine
        originalPosition = transform.localPosition;
    }

    // Appelée quand on clique sur la carte
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSelected)
        {
            // Si la carte n'est pas déjà sélectionnée, elle monte légèrement
            transform.DOLocalMoveY(originalPosition.y + 1f, 0.2f); // Monte de 50 unités sur l'axe Y
            isSelected = true;
        }
        else
        {
            // Si la carte est déjà sélectionnée, elle redescend
            transform.DOLocalMove(originalPosition, 0.2f); // Retour à la position d'origine
            isSelected = false;
        }
    }

    // Appelée quand on relâche le clic (désélection)
    public void OnPointerUp(PointerEventData eventData)
    {
        // Si tu veux que la carte redescende après avoir relâché le clic, active ce code.
        // transform.DOLocalMove(originalPosition, 0.2f);
    }
}
