using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Pour utiliser Image
using DG.Tweening;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 offset;
    private Vector3 originalPosition;
    public bool isSelected = false;
    public bool isDragging = false;
    public bool isFlipped = false; // Indique si la carte est retournée
    public float clickTime = 0f; // Temps de début du clic
    public float clickDurationThreshold = 0.2f; // Durée pour distinguer le clic soutenu
    private bool clickValid = true; // Variable pour vérifier si le clic est valide (clic rapide)

    // Référence au GameObject visible et invisible
    public GameObject invisibleCard; // La carte qui gère les interactions (invisible)
    public GameObject visibleCard;   // La carte visible avec les images et animations

    // Ajouter les `Image` pour le recto et le verso
    public Image rectoImage;  // Image du recto
    public Image versoImage;  // Image du verso
    private Image visibleImage; // Le composant Image de la carte visible
    private Vector3 lastPosition;
    public float tiltAmount = 40f; // Angle maximum de tangage
    private float originalZPosition;

    // Inertie pour le tangage
    private float currentTiltAngle = 0f; // Angle de tangage actuel
    private float tiltVelocity = 0f;     // Vitesse du tangage
    public float tiltDamping = 5f;       // Taux de ralentissement de l'inertie

    // Pour stocker la vitesse de la carte
    private float cardSpeedX = 0f;

    // Courbe pour fluidifier le mouvement de tangage
    public AnimationCurve tiltCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Courbe d'animation par défaut

    private void Start()
    {
        // Sauvegarder la position locale d'origine du parent
        originalPosition = visibleCard.transform.localPosition;

        // Récupérer le composant Image sur l'enfant visible
        visibleImage = visibleCard.GetComponent<Image>();

        // Vérification pour s'assurer que l'image est bien assignée
        if (visibleImage == null)
        {
            Debug.LogError("L'Image est manquante sur l'enfant visible: " + visibleCard.name);
            return;
        }

        // Assurer que l'image du recto est visible au départ
        visibleImage.sprite = rectoImage.sprite;

        // Sauvegarder la position Z originale de la carte visible
        originalZPosition = visibleCard.transform.position.z;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!clickValid) return;

        isDragging = true;
        offset = visibleCard.transform.position - Camera.main.ScreenToWorldPoint(eventData.position);

        // Faire passer la carte visible au-dessus en augmentant l'axe Z
        Vector3 newPosition = visibleCard.transform.position;
        newPosition.z = 10; // Amène la carte visible devant les autres
        visibleCard.transform.position = newPosition;

        lastPosition = visibleCard.transform.position; // Initialiser la position pour le calcul du tangage
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 newPos = Camera.main.ScreenToWorldPoint(eventData.position) + offset;
        newPos.z = 10; // Garder la carte visible au premier plan pendant le drag
        visibleCard.transform.position = newPos;

        // Calculer la vitesse de déplacement sur l'axe X
        cardSpeedX = (visibleCard.transform.position.x - lastPosition.x) / Time.deltaTime;

        // Appliquer un tangage inversé basé sur la vitesse de la carte avec courbe
        float t = Mathf.Clamp01(Mathf.Abs(cardSpeedX) / tiltAmount); // Normaliser la vitesse pour l'utiliser dans la courbe
        float rotationZ = Mathf.Clamp(-cardSpeedX * tiltCurve.Evaluate(t) * 0.1f, -tiltAmount, tiltAmount); // Utiliser la courbe pour fluidifier le tangage
        visibleCard.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        lastPosition = visibleCard.transform.position; // Mettre à jour la dernière position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (!isSelected)
        {
            visibleCard.transform.DOLocalMove(Vector3.zero, 0.5f); // Retourner à (0,0) sur le parent invisible
        }
        else
        {
            visibleCard.transform.DOLocalMove(originalPosition + new Vector3(0, 0.75f, 0), 0.5f); // Retourner à la position avec la montée
        }

        // Remettre la carte visible à sa position Z originale
        Vector3 newPosition = visibleCard.transform.position;
        newPosition.z = originalZPosition;
        visibleCard.transform.position = newPosition;
    }

    private void Update()
    {
        // Appliquer l'inertie au tangage après le drag
        if (!isDragging && Mathf.Abs(cardSpeedX) > 0.01f)
        {
            // Lissage de la rotation avec une inertie
            cardSpeedX = Mathf.SmoothDamp(cardSpeedX, 0, ref tiltVelocity, tiltDamping * Time.deltaTime);
            float t = Mathf.Clamp01(Mathf.Abs(cardSpeedX) / tiltAmount); // Normaliser la vitesse pour la courbe
            float rotationZ = Mathf.Clamp(-cardSpeedX * tiltCurve.Evaluate(t) * 0.1f, -tiltAmount, tiltAmount); // Appliquer l'inertie avec la courbe
            visibleCard.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
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
                visibleCard.transform.DOLocalMoveY(originalPosition.y + 0.75f, 0.2f);
                isSelected = true;
            }
            else
            {
                visibleCard.transform.DOLocalMove(originalPosition, 0.2f);
                isSelected = false;
            }

            // Gérer le retournement avec l'animation de mise à l'échelle (tangage inclus)
            if (!isFlipped)
            {
                visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 0.1f, 0.25f).OnComplete(() =>
                {
                    visibleImage.sprite = versoImage.sprite; // Changer vers l'image du verso
                    visibleImage.raycastTarget = true; // S'assurer que le raycast reste actif
                    visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 10f, 0.25f); // Revenir à la taille normale
                });
                isFlipped = true;
            }
            else
            {
                visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 0.1f, 0.25f).OnComplete(() =>
                {
                    visibleImage.sprite = rectoImage.sprite; // Changer vers l'image du recto
                    visibleImage.raycastTarget = true; // S'assurer que le raycast reste actif
                    visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 10f, 0.25f); // Revenir à la taille normale
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
