using UnityEngine;
using UnityEngine.InputSystem;

public class GridMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;
    /// <summary>Y step size. When 0, falls back to gridSize.</summary>
    public float gridSizeY = 0f;

    [Header("Bounds")]
    /// <summary>When true, the min/max values below are used directly instead of being calculated from the camera.</summary>
    public bool useManualBounds = false;
    public int minX = -8, maxX = 8;
    public int minY = -4, maxY = 4;

    [Header("Dash Settings")]
    public float holdThreshold = 1f;
    public int dashDistance = 3;
    public float dashCooldown = 2f;

    private Vector2Int gridPos;
    private SpriteRenderer sr;

    // Input
    private PlayerInput playerInput;
    private InputAction moveAction;

    // Hold tracking
    private Vector2Int heldDirection = Vector2Int.zero;
    private Vector2Int lastDirection = Vector2Int.zero;
    private float holdTimer = 0f;
    private bool dashConsumed = false;

    // Cooldown
    private float cooldownTimer = 0f;
    private bool onCooldown = false;

    // Colors
    private readonly Color readyColor = Color.white;
    private readonly Color dashColor = Color.red;
    private readonly Color chargeColor = Color.green;

    /// <summary>Returns the effective Y step, falling back to gridSize when gridSizeY is not set.</summary>
    private float EffectiveGridSizeY => gridSizeY > 0f ? gridSizeY : gridSize;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = readyColor;

        gridPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / gridSize),
            Mathf.RoundToInt(transform.position.y / EffectiveGridSizeY)
        );

        if (!useManualBounds)
        {
            Camera cam = Camera.main;
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            minX = Mathf.RoundToInt(-halfW / gridSize);
            maxX = Mathf.RoundToInt( halfW / gridSize);
            minY = Mathf.RoundToInt(-halfH / EffectiveGridSizeY);
            maxY = Mathf.RoundToInt( halfH / EffectiveGridSizeY);
        }
    }

    void Update()
    {
        HandleCooldownColor();
        HandleMovement();
    }

    void HandleMovement()
{
    Vector2 raw = moveAction.ReadValue<Vector2>();
    Vector2Int input = Vector2Int.zero;

    if (raw.x > 0.5f)       input = Vector2Int.right;
    else if (raw.x < -0.5f) input = Vector2Int.left;
    else if (raw.y > 0.5f)  input = Vector2Int.up;
    else if (raw.y < -0.5f) input = Vector2Int.down;

    if (input != Vector2Int.zero)
    {
        if (input != heldDirection)
        {
            heldDirection = input;
    holdTimer = 0f;
    dashConsumed = onCooldown; // instead of false
    TryMove(input);
        }
        else
        {
            holdTimer += Time.deltaTime;

            // Lerp to green while charging (only if dash is off cooldown)
            if (!onCooldown && !dashConsumed)
            {
                float chargeT = Mathf.Clamp01(holdTimer / holdThreshold);
                sr.color = Color.Lerp(readyColor, chargeColor, chargeT);
            }

            if (holdTimer >= holdThreshold && !dashConsumed && !onCooldown)
            {
                TryDash(input);
                dashConsumed = true;
            }
        }
    }
    else
    {
        // Reset color if player lets go before dash fires
        if (!onCooldown) sr.color = readyColor;

    heldDirection = Vector2Int.zero;
    holdTimer = 0f;
    dashConsumed = onCooldown; //  stays true (locked) while cooldown is active
    }
}

    void TryMove(Vector2Int direction)
    {
        Vector2Int newPos = gridPos + direction;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        gridPos = newPos;
        transform.position = new Vector3(gridPos.x * gridSize, gridPos.y * EffectiveGridSizeY, 0f);
    }

    void TryDash(Vector2Int direction)
    {
        Vector2Int newPos = gridPos + direction * dashDistance;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        gridPos = newPos;
        transform.position = new Vector3(gridPos.x * gridSize, gridPos.y * EffectiveGridSizeY, 0f);

        onCooldown = true;
        cooldownTimer = 0f;
        sr.color = dashColor;
    }

    void HandleCooldownColor()
    {
        if (!onCooldown) return;

        cooldownTimer += Time.deltaTime;
        float t = Mathf.Clamp01(cooldownTimer / dashCooldown);
        sr.color = Color.Lerp(dashColor, readyColor, t);

        if (cooldownTimer >= dashCooldown)
        {
            onCooldown = false;
            sr.color = readyColor;
        }
    }
}