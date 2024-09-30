using UnityEngine;
using UnityEngine.UI; // Pour acc�der au composant Image

public class Shadow : MonoBehaviour
{
    public Transform cardTransform; // La carte dont l'ombre d�pend
    public Transform shadowTransform; // L'ombre � manipuler
    public float maxShadowOffset = 5f; // Distance maximale de l'ombre par rapport � la carte
    public float maxShadowScale = 1.2f; // �chelle maximale de l'ombre
    public float minShadowScale = 0.8f; // �chelle minimale de l'ombre
    public float shadowYPositionOffset = -0.2f; // D�calage Y pour placer l'ombre sous la carte
    public float shadowZOffset = -0.01f; // Petit d�calage sur l'axe Z pour que l'ombre reste sous la carte
    public Card cardScript; // R�f�rence au script Card pour v�rifier isDragging et isSelected

    private Image shadowImage; // Composant Image utilis� pour l'ombre

    void Start()
    {
        // R�cup�rer le composant Image de l'ombre
        shadowImage = shadowTransform.GetComponent<Image>();

        if (shadowImage == null)
        {
            Debug.LogError("L'Image est manquante sur l'objet Shadow.");
            return;
        }

        // Mettre l'ombre sur le layer "Shadow" si vous avez besoin
        // Cela s'applique uniquement si l'Image est utilis�e dans un Canvas avec des layers sp�cifiques
    }

    void Update()
    {
        // Rendre l'ombre visible seulement si la carte est s�lectionn�e ou en cours de drag
        if (cardScript.isSelected || cardScript.isDragging)
        {
            shadowImage.enabled = true; // Activer l'ombre
            UpdateShadowPositionAndScale(); // Mettre � jour la position et l'�chelle
        }
        else
        {
            shadowImage.enabled = false; // D�sactiver l'ombre
        }
    }

    void UpdateShadowPositionAndScale()
    {
        // Obtenir la position de la carte dans les coordonn�es du monde
        Vector3 cardWorldPosition = cardTransform.position;

        // Calculer l'�chelle de l'ombre en fonction de la hauteur de la carte
        float cardHeightFactor = Mathf.Clamp(cardTransform.position.y * 0.1f, minShadowScale, maxShadowScale);

        // Appliquer l'�chelle calcul�e � l'ombre
        shadowTransform.localScale = new Vector3(cardHeightFactor, cardHeightFactor, 1f);

        // Calculer le d�calage de l'ombre proportionnel � la distance en X par rapport � la carte
        float shadowOffsetX = Mathf.Clamp(cardTransform.position.x * 0.2f, -maxShadowOffset, maxShadowOffset);

        // Mettre � jour la position de l'ombre
        Vector3 shadowPosition = cardWorldPosition;
        shadowPosition.x = cardWorldPosition.x + shadowOffsetX;   // �loigner l'ombre en X en fonction de la distance de la carte
        shadowPosition.y += shadowYPositionOffset;   // Appliquer un l�ger d�calage en Y
        shadowPosition.z = cardWorldPosition.z + shadowZOffset; // Positionner l'ombre derri�re en Z
        shadowTransform.position = shadowPosition;
    }
}
