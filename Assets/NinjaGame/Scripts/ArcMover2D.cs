using System;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcJumpCurve2D : MonoBehaviour
{
    public bool startGame;
    
    [SerializeField] private MMF_Player _jumpFeedback;
    [SerializeField] private MMF_Player _collisionFeedback;

    [SerializeField] private GameObject _collisionParticle;
    [SerializeField] private GameObject _deathParticle;   // 🔥 партикл смерти
    [SerializeField] private int _poolSize = 5;

    [Header("Arc Settings")]
    public float jumpDistance = 3f;
    public float jumpHeight = 2f;
    public float duration = 0.5f;
    public bool mirror = false; // true = левая стена, false = правая
    public AnimationCurve arcCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sliding Settings")]
    public float slideSpeed = 1f;
    private float defaultSlideSpeed; // 🔥 чтобы восстанавливать
    public Transform slideSpawnPointLeft;
    public Transform slideSpawnPointRight;
    public GameObject slidePrefab;

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.green;

    private Rigidbody2D rb;
    private Vector3 startPos;
    private float elapsed;
    private bool isJumping;
    private float direction;
    private float invDuration;

    private Queue<PooledParticle> particlePool;
    private List<PooledParticle> activeParticles = new List<PooledParticle>();

    private GameObject slideParticleLeft;
    private GameObject slideParticleRight;
    private GameObject currentSlideParticle;

    private class PooledParticle
    {
        public GameObject obj;
        public ParticleSystem ps;
        public float lifeTime;
        public float timer;
    }

    public void StartGame()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        defaultSlideSpeed = slideSpeed;

        // Пул для коллизий
        particlePool = new Queue<PooledParticle>();
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_collisionParticle);
            obj.SetActive(false);
            var ps = obj.GetComponent<ParticleSystem>();
            particlePool.Enqueue(new PooledParticle
            {
                obj = obj,
                ps = ps,
                lifeTime = ps.main.duration,
                timer = 0f
            });
        }

        if (slidePrefab != null)
        {
            if (slideSpawnPointLeft != null)
            {
                slideParticleLeft = Instantiate(slidePrefab, slideSpawnPointLeft);
                slideParticleLeft.SetActive(false);
            }
            if (slideSpawnPointRight != null)
            {
                slideParticleRight = Instantiate(slidePrefab, slideSpawnPointRight);
                slideParticleRight.SetActive(false);
            }
        }
        
        SwipeParticles.Instance.OnSwipeLeft += HandleJumpLeft;
        SwipeParticles.Instance.OnSwipeRight += HandleJumpRight;
        SwipeParticles.Instance.OnComboSwipe += HandleJumpCombo;

        startGame = true;
    }

    private void Update()
    {
        if (!startGame) return;
        
        // Движение вниз и включение слайдов
        if (!isJumping)
        {
            transform.position += Vector3.down * slideSpeed * Time.deltaTime;

            GameObject target = mirror ? slideParticleLeft : slideParticleRight;
            if (currentSlideParticle != target)
            {
                if (currentSlideParticle != null) currentSlideParticle.SetActive(false);
                if (target != null) target.SetActive(true);
                currentSlideParticle = target;
            }
        }
        else
        {
            if (currentSlideParticle != null)
            {
                currentSlideParticle.SetActive(false);
                currentSlideParticle = null;
            }
        }

        // Обновление активных частиц
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            var p = activeParticles[i];
            p.timer += Time.deltaTime;
            if (p.timer >= p.lifeTime)
            {
                p.obj.SetActive(false);
                particlePool.Enqueue(p);
                activeParticles.RemoveAt(i);
            }
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

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Spike") || coll.gameObject.CompareTag("Fire"))
        {
            Death();
        }
        else if (coll.gameObject.CompareTag("Ice"))
        {
            slideSpeed = defaultSlideSpeed * 2f;
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Ice"))
        {
            slideSpeed = defaultSlideSpeed;
        }
    }

    private void SpawnParticle(Vector3 position)
    {
        if (particlePool.Count == 0) return;

        var p = particlePool.Dequeue();
        p.obj.transform.position = position;
        p.obj.SetActive(true);
        p.ps.Play();
        p.timer = 0f;

        activeParticles.Add(p);
    }

    private void HandleJumpLeft()
    {
        if (isJumping || mirror) return;
        mirror = true;
        _jumpFeedback.PlayFeedbacks();
        DoArcJump();
    }

    private void HandleJumpRight()
    {
        if (isJumping || !mirror) return;
        mirror = false;
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
        invDuration = 1f / duration;
    }

    private void FixedUpdate()
    {
        if (!startGame) return;
        
        if (!isJumping) return;

        elapsed += Time.fixedDeltaTime;
        float tNorm = elapsed * invDuration;
        if (tNorm > 1f) tNorm = 1f;

        float x = jumpDistance * tNorm * direction;
        float y = arcCurve.Evaluate(tNorm) * jumpHeight;

        rb.MovePosition(startPos + new Vector3(x, y, 0f));

        if (tNorm >= 1f)
            isJumping = false;
    }
    
    public void Death()
    {
        if (_deathParticle != null)
        {
            Transform spawnPoint = mirror ? slideSpawnPointLeft : slideSpawnPointRight;
            
            if (spawnPoint != null)
            {
                Quaternion rot = Quaternion.Euler(0f, mirror ? 90f : -90f, 0f);

                GameObject particle = Instantiate(_deathParticle, spawnPoint.position, rot);
            }
            else
            {
                GameObject particle = Instantiate(_deathParticle, transform.position, Quaternion.identity);
            }
        }

        SwipeParticles.Instance.OnSwipeLeft -= HandleJumpLeft;
        SwipeParticles.Instance.OnSwipeRight -= HandleJumpRight;
        SwipeParticles.Instance.OnComboSwipe -= HandleJumpCombo;
        
        gameObject.SetActive(false);
        
        GameStartController.Instance.LoseGameController();
    }


#if UNITY_EDITOR
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
#endif
}
