using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcJumpCurve2D : MonoBehaviour
{
    [SerializeField] private MMF_Player _jumpFeedback;
    [SerializeField] private MMF_Player _collisionFeedback;

    [SerializeField] private GameObject _collisionParticle;
    [SerializeField] private int _poolSize = 5;

    [Header("Arc Settings")]
    public float jumpDistance = 3f;
    public float jumpHeight = 2f;
    public float duration = 0.5f;
    public bool mirror = false;
    public AnimationCurve arcCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sliding Settings")]
    public float slideSpeed = 1f;                   
    public Transform slideSpawnPointLeft;           // точка для левой стены
    public Transform slideSpawnPointRight;          // точка для правой стены
    public GameObject slidePrefab;                  
    public float spawnInterval = 0.3f;              

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.green;

    private Rigidbody2D rb;
    private Vector3 startPos;
    private float elapsed;
    private bool isJumping;
    private float direction;

    private Queue<GameObject> particlePool;
    private float slideTimer;

    // Активный слайд-партикл
    private GameObject activeSlideParticle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // Пул для коллизий
        particlePool = new Queue<GameObject>();
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_collisionParticle);
            obj.SetActive(false);
            particlePool.Enqueue(obj);
        }

        // Создаём один партикл для скольжения
        if (slidePrefab != null)
        {
            activeSlideParticle = Instantiate(slidePrefab);
            activeSlideParticle.SetActive(false);
        }
    }

    private void Start()
    {
        SwipeParticles.Instance.OnSwipeLeft += HandleJumpLeft;
        SwipeParticles.Instance.OnSwipeRight += HandleJumpRight;
        SwipeParticles.Instance.OnComboSwipe += HandleJumpCombo;
    }

    private void Update()
    {
        if (!isJumping)
        {
            // движение вниз
            transform.position += Vector3.down * slideSpeed * Time.deltaTime;

            // включаем партикл на правильной стороне
            if (activeSlideParticle != null)
            {
                Transform spawnPoint = mirror ? slideSpawnPointLeft : slideSpawnPointRight;
                if (spawnPoint != null)
                {
                    activeSlideParticle.transform.position = spawnPoint.position;
                    if (!activeSlideParticle.activeSelf)
                        activeSlideParticle.SetActive(true);
                }
            }
        }
        else
        {
            // В прыжке → отключаем партикл
            if (activeSlideParticle != null && activeSlideParticle.activeSelf)
                activeSlideParticle.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (!isJumping) return;

        if (coll.gameObject.CompareTag("Wall"))
        {
            _collisionFeedback.PlayFeedbacks();
            SpawnParticle(transform.position);

            if (Application.isMobilePlatform)
                Handheld.Vibrate();
        }
    }

    private void SpawnParticle(Vector3 position)
    {
        if (particlePool.Count == 0) return;

        GameObject particle = particlePool.Dequeue();
        particle.transform.position = position;
        particle.SetActive(true);

        var ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            StartCoroutine(ReturnToPoolAfterDelay(particle, ps.main.duration));
        }
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        particle.SetActive(false);
        particlePool.Enqueue(particle);
    }

    private void HandleJumpLeft()
    {
        if (isJumping || mirror) return;
        mirror = true; // левая стена
        _jumpFeedback.PlayFeedbacks();
        DoArcJump();
    }

    private void HandleJumpRight()
    {
        if (isJumping || !mirror) return;
        mirror = false; // правая стена
        _jumpFeedback.PlayFeedbacks();
        DoArcJump();
    }

    private void HandleJumpCombo()
    {
        if (!isJumping) return;

        Debug.Log("Combo Jump");

        isJumping = false;
        mirror = !mirror;

        _jumpFeedback.PlayFeedbacks();
        DoArcJump();
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
