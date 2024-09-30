using UnityEngine;
using UnityEngine.UI; // For Image component

public class Shadow : MonoBehaviour
{
    public Transform cardTransform; // The card's transform
    public Transform shadowTransform; // The shadow's transform
    public float maxShadowOffset = 5f; // Maximum distance of the shadow from the card
    public float maxShadowScale = 1.2f; // Maximum shadow scale
    public float minShadowScale = 0.8f; // Minimum shadow scale
    public float shadowYPositionOffset = -0.2f; // Y offset to place the shadow under the card
    public float shadowZOffset = -0.01f; // Small Z offset to keep the shadow behind the card
    public Card cardScript; // Reference to the Card script for checking isDragging and isSelected

    private Image shadowImage;

    void Start()
    {
        shadowImage = shadowTransform.GetComponent<Image>();

        if (shadowImage == null)
        {
            Debug.LogError("The Image component is missing on the shadow object.");
            return;
        }
    }

    void Update()
    {
        if (cardScript.isSelected || cardScript.isDragging)
        {
            shadowImage.enabled = true; // Enable the shadow
            UpdateShadowPositionAndScale(); // Update the shadow's position and scale
        }
        else
        {
            shadowImage.enabled = false; // Disable the shadow
        }
    }

    void UpdateShadowPositionAndScale()
    {
        // Get the card's world position
        Vector3 cardWorldPosition = cardTransform.position;

        // Calculate the shadow's scale based on the card's Y position
        float cardHeightFactor = Mathf.Clamp(cardTransform.position.y * 0.1f, minShadowScale, maxShadowScale);
        shadowTransform.localScale = new Vector3(cardHeightFactor, cardHeightFactor, 1f);

        // Normalize the card's position relative to the screen width
        float screenHalfWidth = Screen.width / 2f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(cardTransform.position);
        float normalizedX = (screenPos.x - screenHalfWidth) / screenHalfWidth;

        // Calculate shadow offset based on the normalized X position, clamping it within the maxShadowOffset
        float shadowOffsetX = Mathf.Clamp(normalizedX * maxShadowOffset, -maxShadowOffset, maxShadowOffset);

        // Update the shadow's position
        Vector3 shadowPosition = cardWorldPosition;
        shadowPosition.x = cardWorldPosition.x + shadowOffsetX;   // Apply X offset
        shadowPosition.y += shadowYPositionOffset;   // Apply Y offset
        shadowPosition.z = cardWorldPosition.z + shadowZOffset; // Ensure shadow stays behind the card
        shadowTransform.position = shadowPosition;
    }
}
