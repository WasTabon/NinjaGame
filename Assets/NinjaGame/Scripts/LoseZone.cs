using UnityEngine;

public class LoseZone : MonoBehaviour
{
    public Transform player;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (player == null || cam == null) return;

        Vector3 viewPos = cam.WorldToViewportPoint(player.position);

        if (viewPos.y < 0f || viewPos.y > 1f) // за границами по Y
        {
            Debug.Log("Поражение! Игрок вылетел за пределы камеры.");
        }
    }
}