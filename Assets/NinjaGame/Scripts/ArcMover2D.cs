using UnityEngine;
using DG.Tweening;

public class ArcJump2D : MonoBehaviour
{
    [Header("Arc Settings")]
    public float jumpDistance = 3f;   // длина по X
    public float jumpHeight = 2f;     // высота дуги
    public float duration = 0.5f;     // время прыжка
    public bool mirror = false;       // зеркалить дугу (влево/вправо)

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.green;

    private Vector3 startPos;

    /// <summary>
    /// Запустить прыжок по дуге
    /// </summary>
    [ContextMenu("Jump")]
    public void DoArcJump()
    {
        startPos = transform.position;

        float direction = mirror ? -1f : 1f;
        Vector3 endPos = startPos + new Vector3(jumpDistance * direction, 0f, 0f);

        // Сбрасываем анимацию, если уже идёт
        transform.DOKill();

        // Двигаем по дуге
        transform.DOJump(endPos, jumpHeight, 1, duration)
            .SetEase(Ease.Linear);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;

        Vector3 from = Application.isPlaying ? startPos : transform.position;
        float direction = mirror ? -1f : 1f;
        Vector3 to = from + new Vector3(jumpDistance * direction, 0f, 0f);

        // Рисуем дугу сегментами
        int segments = 20;
        Vector3 prevPoint = from;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float x = Mathf.Lerp(from.x, to.x, t);
            float y = Mathf.Lerp(from.y, to.y, t) + Mathf.Sin(t * Mathf.PI) * jumpHeight;
            Vector3 nextPoint = new Vector3(x, y, from.z);

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}