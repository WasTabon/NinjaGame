using UnityEngine;

public class LoseZone : MonoBehaviour
{
    public ArcJumpCurve2D arcJumpCurve2D;
    
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

        if (viewPos.y < 0f || viewPos.y > 1f)
        {
            arcJumpCurve2D.Death();
        }
    }
}