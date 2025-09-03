using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 2f;
    public bool startFromA = true;

    private Transform target;

    void Start()
    {
        // Определяем начальную цель
        target = startFromA ? pointB : pointA;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        // Двигаемся к цели
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Проверяем достижение точки
        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            // Меняем цель
            target = target == pointA ? pointB : pointA;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
#endif
}