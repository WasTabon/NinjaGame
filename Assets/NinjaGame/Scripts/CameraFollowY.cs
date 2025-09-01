using UnityEngine;

[ExecuteAlways] // Позволяет работать в редакторе
public class CameraFollowY : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform player;          // Игрок, за которым следует камера
    public Vector3 offset = new Vector3(0, 5, -10); // Настраиваемый оффсет
    public float smoothSpeed = 5f;    // Скорость сглаживания

    void Update()
    {
        if (player == null) return;

        // Целевая позиция камеры с оффсетом, только Y изменяем по игроку
        Vector3 targetPosition = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);

        // Плавное следование камеры
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    // В редакторе, чтобы камера сразу встала на нужную позицию
    void OnValidate()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);
        transform.position = targetPosition;
    }
}