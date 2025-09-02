using UnityEngine;

[ExecuteAlways]
public class CameraFollowY : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    private float lastCameraY;

    void Start()
    {
        lastCameraY = transform.position.y;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);

        // Камера двигается только вверх
        if (targetPosition.y > lastCameraY)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            lastCameraY = transform.position.y;
        }
    }

    void OnValidate()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);
        transform.position = targetPosition;
        lastCameraY = transform.position.y;
    }
}