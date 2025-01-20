using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 0.5f;  // Vitesse de déplacement
    public float zoomSpeed = 0.05f;  // Vitesse de zoom
    public float minZoom = 5f;  // Zoom minimum
    public float maxZoom = 20f;  // Zoom maximum
    private Vector3 touchStart;  // Position de départ pour le déplacement

    void Update()
    {
        // Déplacement par glissement (pan)
        if (Input.touchCount == 1)  // Si un seul doigt est utilisé
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = Camera.main.ScreenToWorldPoint(touch.position);  // Définir la position de départ
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(touch.position);
                transform.position += direction * panSpeed;  // Déplacer la caméra dans la direction du glissement
            }
        }

        // Zoom/Dézoom par pincement
        if (Input.touchCount == 2)  // Si deux doigts sont utilisés
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomSpeed);  // Appliquer le zoom en fonction de la différence de distance
        }

        // Déplacement avec les touches Z, Q, S, D (temporaire)
        if (Input.GetKey(KeyCode.W))  // Aller vers le haut
        {
            transform.Translate(Vector3.up * panSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.S))  // Aller vers le bas
        {
            transform.Translate(Vector3.down * panSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.A))  // Aller vers la gauche
        {
            transform.Translate(Vector3.left * panSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.D))  // Aller vers la droite
        {
            transform.Translate(Vector3.right * panSpeed, Space.World);
        }
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, minZoom, maxZoom);  // Limiter le zoom
    }
}