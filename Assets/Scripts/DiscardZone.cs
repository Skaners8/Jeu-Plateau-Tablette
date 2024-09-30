using UnityEngine;

public class DiscardZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Card card = collision.GetComponent<Card>();
        if (card != null)
        {
            card.isInDiscardZone = true;  // La carte est dans la zone de discard
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Card card = collision.GetComponent<Card>();
        if (card != null)
        {
            card.isInDiscardZone = false; // La carte sort de la zone de discard
        }
    }
}
