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
    // Instantiate a new card from the prefab at the deck position
    GameObject newCard = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity);

    // Set the card as a child of the target parent without changing its world position
    newCard.transform.SetParent(targetParent, worldPositionStays: true);

    // Animate the card moving from the deck to its target position (slot)
    Vector3 finalPosition = targetParent.position;
    newCard.transform.DOMove(finalPosition, 0.8f).OnComplete(() =>
    {
        // Set the card's local position relative to the new parent (to be zeroed)
        newCard.transform.localPosition = Vector3.zero;

        // Get the Card script component from the new card
        Card newCardScript = newCard.GetComponent<Card>();

        // Set up references for the new card
        newCardScript.discardZoneRectTransform = discardZoneRectTransform;
        newCardScript.deckManager = this; // Assign the deck manager reference

        // Re-enable the card interactions after the movement is done
        newCardScript.enabled = true;
    });

    // Reset the local scale of the new card to ensure it appears at the correct size
    newCard.transform.localScale = Vector3.one;

    // Temporarily disable interaction on the card until the movement completes
    Card cardScript = newCard.GetComponent<Card>();
    cardScript.enabled = false;
}

}
