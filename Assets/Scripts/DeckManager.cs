using DG.Tweening;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab; // Reference to the card prefab
    public Transform cardParent; // The parent where the card should be placed
    public RectTransform discardZoneRectTransform; // Reference to the discard zone
    public DeckManager deckManager; // Reference to itself for deck interactions
    public Transform deckPosition; // The transform where the deck is located

    // Method to draw a new card and place it in the specified parent
    public void DrawNewCard(Transform targetParent)
    {
        // Instantiate a new card from the prefab
        GameObject newCard = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity);

        // Set the card as a child of the target parent
        newCard.transform.SetParent(targetParent);
        

        // Animate the card moving from the deck to its target position (slot)
        newCard.transform.DOMove(targetParent.position, 0.2f).OnComplete(() =>
        {
            // Ensure the card's local position is correct after animation
            newCard.transform.localPosition = Vector3.zero; // Reset to local zero in the target parent
        });

        // Reset the local scale of the new card to ensure it appears at the correct size
        newCard.transform.localScale = Vector3.one;

        // Get the Card script component from the new card
        Card newCardScript = newCard.GetComponent<Card>();

        // Set up references for the new card
        newCardScript.discardZoneRectTransform = discardZoneRectTransform;
        newCardScript.deckManager = this; // Assign the deck manager reference

        // Use the MoveCardToPosition method to move the card and apply oscillation during the move
        newCardScript.MoveCardToPosition(targetParent.position);
    }
}
