using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class SwipeParticles : MonoBehaviour
{
    public static SwipeParticles Instance;
    
    [Header("Particle Settings")]
    public ParticleSystem swipeParticles;
    public Camera mainCamera;
    public float zDistance = 10f;

    [Header("Swipe Settings")]
    public float swipeThreshold = 50f; // минимальная дистанция для свайпа
    public float comboTime = 0.15f;     // время для комбо свайпа

    public event Action OnSwipeLeft;
    public event Action OnSwipeRight;
    public event Action OnComboSwipe; // если два противоположных свайпа подряд

    private bool isDragging = false;
    private ParticleSystem.EmissionModule emission;

    private Vector2 startTouch;
    private bool swipeDetected = false;

    private string lastSwipe = "";
    private Coroutine comboRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (swipeParticles != null)
        {
            emission = swipeParticles.emission;
            emission.enabled = false;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            StartTouch(Input.mousePosition);
            StartParticles(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            MoveParticles(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndTouch(Input.mousePosition);
            EndParticles();
        }
#endif

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !IsPointerOverUI())
            {
                StartTouch(touch.position);
                StartParticles(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                MoveParticles(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                EndTouch(touch.position);
                EndParticles();
            }
        }
    }

    private void StartTouch(Vector2 screenPos)
    {
        isDragging = true;
        swipeDetected = false;
        startTouch = screenPos;
    }

    private void EndTouch(Vector2 endPos)
    {
        isDragging = false;

        Vector2 delta = endPos - startTouch;

        if (!swipeDetected && delta.magnitude > swipeThreshold)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) // свайпы по X
            {
                if (delta.x > 0)
                    HandleSwipe("right");
                else
                    HandleSwipe("left");
            }
        }
    }

    private void HandleSwipe(string direction)
    {
        swipeDetected = true;

        if (direction == "left")
        {
            OnSwipeLeft?.Invoke();
            Debug.Log("Swipe Left");
        }
        else if (direction == "right")
        {
            OnSwipeRight?.Invoke();
            Debug.Log("Swipe Right");
        }

        // проверка на комбо свайп
        if (lastSwipe != "" && lastSwipe != direction)
        {
            OnComboSwipe?.Invoke();
            Debug.Log("Combo Swipe!");
            lastSwipe = "";
            if (comboRoutine != null)
            {
                StopCoroutine(comboRoutine);
                comboRoutine = null;
            }
        }
        else
        {
            lastSwipe = direction;
            if (comboRoutine != null)
                StopCoroutine(comboRoutine);

            comboRoutine = StartCoroutine(ResetLastSwipe());
        }
    }

    private IEnumerator ResetLastSwipe()
    {
        yield return new WaitForSeconds(comboTime);
        lastSwipe = "";
        comboRoutine = null;
    }

    private void StartParticles(Vector2 screenPos)
    {
        if (swipeParticles != null)
        {
            emission.enabled = true;
            swipeParticles.transform.position = ScreenToWorld(screenPos);
        }
    }

    private void MoveParticles(Vector2 screenPos)
    {
        if (swipeParticles != null)
            swipeParticles.transform.position = ScreenToWorld(screenPos);
    }

    private void EndParticles()
    {
        if (swipeParticles != null)
            emission.enabled = false;
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 pos = new Vector3(screenPos.x, screenPos.y, zDistance);
        return mainCamera.ScreenToWorldPoint(pos);
    }

    private bool IsPointerOverUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
        if (EventSystem.current == null) return false;
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return false;
#else
        return false;
#endif
    }
}
