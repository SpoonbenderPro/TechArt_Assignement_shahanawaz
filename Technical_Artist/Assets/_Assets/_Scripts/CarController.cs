using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Lane Setup")]
    [SerializeField] private float leftX = -5f;
    [SerializeField] private float rightX = 5f;
    [SerializeField] private int laneCount = 3;

    [Header("Movement")]
    [SerializeField] private float laneChangeSpeed = 12f;
    [SerializeField] private float laneReachThreshold = 0.02f;

    [Header("Tilt")]
    [SerializeField] private float maxTiltZ = 12f;
    [SerializeField] private float tiltSpeed = 10f;
    [SerializeField] private float returnTiltSpeed = 8f;

    [Header("Input")]
    [SerializeField] private float swipeThreshold = 50f;

    [Header("Optional VFX")]
    [SerializeField] private ParticleSystem laneSwitchDust;
    [SerializeField] private ParticleSystem crashEffect;

    [Header("Crash")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private float gameOverDelay = 0.15f;

    private int currentLane = 1;
    private Vector3 targetPosition;
    private bool isMoving;

    private float currentTiltZ = 0f;
    private float targetTiltZ = 0f;

    private Vector2 swipeStartPosMouse;
    private bool isSwipingMouse = false;

    private Vector2 swipeStartPosTouch;
    private bool isSwipingTouch = false;

    private bool isCrashed = false;
    private bool gameOverShown = false;

    private void Start()
    {
        laneCount = Mathf.Max(2, laneCount);
        currentLane = Mathf.Clamp(currentLane, 0, laneCount - 1);

        float startX = GetLaneX(currentLane);
        targetPosition = new Vector3(startX, transform.position.y, transform.position.z);
        transform.position = targetPosition;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isCrashed)
        {
            return;
        }

        HandleMouseSwipe();
        HandleTouchSwipe();
        UpdateLaneMovement();
        UpdateTilt();
    }

    private void UpdateLaneMovement()
    {
        Vector3 currentPosition = transform.position;
        Vector3 desiredPosition = new Vector3(targetPosition.x, currentPosition.y, currentPosition.z);

        float newX = Mathf.Lerp(currentPosition.x, desiredPosition.x, Time.deltaTime * laneChangeSpeed);
        transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);

        float distance = Mathf.Abs(transform.position.x - desiredPosition.x);

        if (distance <= laneReachThreshold)
        {
            transform.position = desiredPosition;
            isMoving = false;
            targetTiltZ = 0f;
        }
        else
        {
            isMoving = true;
        }
    }

    private void UpdateTilt()
    {
        float tiltLerpSpeed = isMoving ? tiltSpeed : returnTiltSpeed;
        currentTiltZ = Mathf.Lerp(currentTiltZ, targetTiltZ, Time.deltaTime * tiltLerpSpeed);

        Vector3 currentEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentTiltZ);
    }

    private void HandleMouseSwipe()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isSwipingMouse = true;
            swipeStartPosMouse = Mouse.current.position.ReadValue();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && isSwipingMouse)
        {
            Vector2 endPos = Mouse.current.position.ReadValue();
            Vector2 delta = endPos - swipeStartPosMouse;
            isSwipingMouse = false;

            ProcessSwipe(delta);
        }
    }

    private void HandleTouchSwipe()
    {
        if (Touchscreen.current == null)
        {
            return;
        }

        var primaryTouch = Touchscreen.current.primaryTouch;

        if (primaryTouch.press.wasPressedThisFrame)
        {
            isSwipingTouch = true;
            swipeStartPosTouch = primaryTouch.position.ReadValue();
        }
        else if (primaryTouch.press.wasReleasedThisFrame && isSwipingTouch)
        {
            Vector2 endPos = primaryTouch.position.ReadValue();
            Vector2 delta = endPos - swipeStartPosTouch;
            isSwipingTouch = false;

            ProcessSwipe(delta);
        }
    }

    private void ProcessSwipe(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) <= swipeThreshold || Mathf.Abs(delta.x) <= Mathf.Abs(delta.y))
        {
            return;
        }

        if (delta.x < 0f)
        {
            MoveHorizontal(1);
        }
        else
        {
            MoveHorizontal(-1);
        }
    }

    private void MoveHorizontal(int direction)
    {
        int newLane = Mathf.Clamp(currentLane + direction, 0, laneCount - 1);

        if (newLane == currentLane)
        {
            return;
        }

        currentLane = newLane;

        float newX = GetLaneX(currentLane);
        targetPosition = new Vector3(newX, transform.position.y, transform.position.z);

        if (direction > 0)
        {
            targetTiltZ = -maxTiltZ;
        }
        else
        {
            targetTiltZ = maxTiltZ;
        }

        if (laneSwitchDust != null)
        {
            laneSwitchDust.Play();
        }

        if (gameaudio.instance != null)
        {
            gameaudio.instance.PlayLaneSwitch();
        }
    }

    public void ForceCrash()
    {
        if (isCrashed)
        {
            return;
        }

        isCrashed = true;
        isMoving = false;
        targetTiltZ = 0f;

        if (crashEffect != null)
        {
            crashEffect.transform.SetParent(null);
            crashEffect.Play();
            Destroy(crashEffect.gameObject, 2f);
        }

        if (gameaudio.instance != null)
        {
            gameaudio.instance.PlayCrash();
            gameaudio.instance.StopMusic();
        }

        Time.timeScale = 1f;
        Invoke(nameof(ShowGameOver), gameOverDelay);
    }

    private void ShowGameOver()
    {
        if (gameOverShown)
        {
            return;
        }

        gameOverShown = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogWarning("CarController: Game Over Panel is not assigned.");
        }

        Time.timeScale = 0f;
    }

    private float GetLaneX(int laneIndex)
    {
        if (laneCount == 1)
        {
            return 0f;
        }

        float t = laneIndex / (float)(laneCount - 1);
        return Mathf.Lerp(leftX, rightX, t);
    }
}