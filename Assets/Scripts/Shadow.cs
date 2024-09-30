using UnityEngine;
using UnityEngine.UI; // Pour accéder au composant Image

public class Shadow : MonoBehaviour
{
    public Transform cardTransform; // La carte dont l'ombre dépend
    public Transform shadowTransform; // L'ombre à manipuler
    public float maxShadowOffset = 5f; // Distance maximale de l'ombre par rapport à la carte
    public float maxShadowScale = 1.2f; // Échelle maximale de l'ombre
    public float minShadowScale = 0.8f; // Échelle minimale de l'ombre
    public float shadowYPositionOffset = -0.2f; // Décalage Y pour placer l'ombre sous la carte
    public float shadowZOffset = -0.01f; // Petit décalage sur l'axe Z pour que l'ombre reste sous la carte
    public Card cardScript; // Référence au script Card pour vérifier isDragging et isSelected

    private Image shadowImage; // Composant Image utilisé pour l'ombre

    void Start()
    {
        // Récupérer le composant Image de l'ombre
        shadowImage = shadowTransform.GetComponent<Image>();

        if (shadowImage == null)
        {
            Debug.LogError("L'Image est manquante sur l'objet Shadow.");
            return;
        }

        // Mettre l'ombre sur le layer "Shadow" si vous avez besoin
        // Cela s'applique uniquement si l'Image est utilisée dans un Canvas avec des layers spécifiques
    }

    void Update()
    {
        // Rendre l'ombre visible seulement si la carte est sélectionnée ou en cours de drag
        if (cardScript.isSelected || cardScript.isDragging)
        {
            shadowImage.enabled = true; // Activer l'ombre
            UpdateShadowPositionAndScale(); // Mettre à jour la position et l'échelle
        }
        else
        {
            shadowImage.enabled = false; // Désactiver l'ombre
        }
    }

    void UpdateShadowPositionAndScale()
    {
        // Obtenir la position de la carte dans les coordonnées du monde
        Vector3 cardWorldPosition = cardTransform.position;

        // Calculer l'échelle de l'ombre en fonction de la hauteur de la carte
        float cardHeightFactor = Mathf.Clamp(cardTransform.position.y * 0.1f, minShadowScale, maxShadowScale);

        // Appliquer l'échelle calculée à l'ombre
        shadowTransform.localScale = new Vector3(cardHeightFactor, cardHeightFactor, 1f);

        // Calculer le décalage de l'ombre proportionnel à la distance en X par rapport à la carte
        float shadowOffsetX = Mathf.Clamp(cardTransform.position.x * 0.2f, -maxShadowOffset, maxShadowOffset);

        // Mettre à jour la position de l'ombre
        Vector3 shadowPosition = cardWorldPosition;
        shadowPosition.x = cardWorldPosition.x + shadowOffsetX;   // Éloigner l'ombre en X en fonction de la distance de la carte
        shadowPosition.y += shadowYPositionOffset;   // Appliquer un léger décalage en Y
        shadowPosition.z = cardWorldPosition.z + shadowZOffset; // Positionner l'ombre derrière en Z
        shadowTransform.position = shadowPosition;
    }
}
