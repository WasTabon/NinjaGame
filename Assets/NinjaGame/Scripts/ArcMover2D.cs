using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcJumpCurve2D : MonoBehaviour
{
    [Header("Arc Settings")]
    public float jumpDistance = 3f;       // длина по X
    public float jumpHeight = 2f;         // масштаб высоты
    public float duration = 0.5f;         // время прыжка
    public bool mirror = false;           // зеркалить по X
    public AnimationCurve arcCurve =      // форма дуги
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.green;

    private Rigidbody2D rb;
    private Vector3 startPos;
    private float elapsed;
    private bool isJumping;
    private float direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    [ContextMenu("Jump")]
    public void HandleJump()
    {
        DoArcJump();
        mirror = !mirror;
    }

    public void DoArcJump()
    {
        startPos = transform.position;
        elapsed = 0f;
        isJumping = true;
        direction = mirror ? -1f : 1f;
    }

    private void FixedUpdate()
    {
        if (!isJumping) return;

        elapsed += Time.fixedDeltaTime;
        float tNorm = Mathf.Clamp01(elapsed / duration);

        float x = jumpDistance * tNorm * direction;
        float y = arcCurve.Evaluate(tNorm) * jumpHeight;

        Vector3 newPos = startPos + new Vector3(x, y, 0f);
        rb.MovePosition(newPos);

        if (tNorm >= 1f)
            isJumping = false;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;

        Vector3 from = Application.isPlaying ? startPos : transform.position;
        float dir = mirror ? -1f : 1f;

        int segments = 30;
        Vector3 prevPoint = from;

        for (int i = 1; i <= segments; i++)
        {
            float tNorm = i / (float)segments;
            float x = jumpDistance * tNorm * dir;
            float y = arcCurve.Evaluate(tNorm) * jumpHeight;
            Vector3 nextPoint = from + new Vector3(x, y, 0f);

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
