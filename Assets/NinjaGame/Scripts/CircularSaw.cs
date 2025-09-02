using UnityEngine;

public class CircularSaw : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 360f;
    public bool clockwise = true;

    void Update()
    {
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime, Space.Self);
    }
}