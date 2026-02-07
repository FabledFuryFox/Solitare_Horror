using UnityEngine;
using UnityEngine.InputSystem;

public class ControlledView : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalRotationLimit = 80f;
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private float smoothing = 10f; // Smoothing factor for camera rotation
    [SerializeField] private bool useSmoothing = true; // Toggle smoothing on/off

    [Header("Selection Settings")]
    [SerializeField] private float selectionRange = 10f;
    [SerializeField] private LayerMask selectableLayers = -1; // All layers by default

    [Header("Crosshair Settings")]
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float crosshairSize = 3f;
    [SerializeField] private bool showCrosshair = true;

    [Header("Card Engine Reference")]
    [SerializeField] private CardEngine cardEngine;

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    private float targetVerticalRotation = 0f;
    private float targetHorizontalRotation = 0f;
    private Camera cam;
    private bool isCursorLocked = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get camera component
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (lockCursor)
        {
            LockCursor();
        }

        // Initialize target rotations to match current rotation
        Vector3 currentEuler = transform.localEulerAngles;
        targetHorizontalRotation = currentEuler.y;
        targetVerticalRotation = currentEuler.x;
        
        // Normalize vertical rotation to -180 to 180 range
        if (targetVerticalRotation > 180f)
        {
            targetVerticalRotation -= 360f;
        }
        
        horizontalRotation = targetHorizontalRotation;
        verticalRotation = targetVerticalRotation;


    }

    // Update is called once per frame
    void Update()
    {
        // Keep cursor locked and centered when it should be locked
        if (lockCursor && isCursorLocked && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }

        // Get mouse input using new Input System (only when cursor is locked)
        if (Mouse.current != null && isCursorLocked)
        {
            // Get mouse delta and apply frame-rate independent sensitivity
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime * 60f; // Normalized to 60fps
            float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime * 60f;

            // Update target rotations
            targetHorizontalRotation += mouseX;
            targetVerticalRotation -= mouseY;
            targetVerticalRotation = Mathf.Clamp(targetVerticalRotation, -verticalRotationLimit, verticalRotationLimit);

            // Apply smoothing or instant rotation
            if (useSmoothing && smoothing > 0f)
            {
                // Smoothly interpolate towards target rotation
                horizontalRotation = Mathf.LerpAngle(horizontalRotation, targetHorizontalRotation, smoothing * Time.deltaTime);
                verticalRotation = Mathf.LerpAngle(verticalRotation, targetVerticalRotation, smoothing * Time.deltaTime);
            }
            else
            {
                // Instant rotation (no smoothing)
                horizontalRotation = targetHorizontalRotation;
                verticalRotation = targetVerticalRotation;
            }
            
            // Apply both rotations to the transform
            transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);

            // Handle left mouse click for selection
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleSelection();
            }
        }

        // Toggle cursor lock with Escape key
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isCursorLocked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }

    /// <summary>
    /// Draws the crosshair dot in the center of the screen
    /// </summary>
    void OnGUI()
    {
        // Only show crosshair when cursor is locked
        if (showCrosshair && isCursorLocked)
        {
            // Calculate center of screen
            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;
            float halfSize = crosshairSize / 2f;

            // Draw the crosshair dot
            Rect crosshairRect = new Rect(centerX - halfSize, centerY - halfSize, crosshairSize, crosshairSize);
            
            // Create a texture for the dot
            Texture2D crosshairTexture = new Texture2D(1, 1);
            crosshairTexture.SetPixel(0, 0, crosshairColor);
            crosshairTexture.Apply();

            // Set GUI style
            GUIStyle style = new GUIStyle();
            style.normal.background = crosshairTexture;

            // Draw the crosshair
            GUI.Box(crosshairRect, "", style);
        }
    }

    /// <summary>
    /// Locks the cursor and hides it
    /// </summary>
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    /// <summary>
    /// Unlocks the cursor and shows it
    /// </summary>
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }

    /// <summary>
    /// Handles object selection when left mouse button is clicked.
    /// Performs a raycast from the camera center and calls OnObjectSelected if something is hit.
    /// </summary>
    private void HandleSelection()
    {
        if (cam == null) return;

        // Raycast from camera center (since cursor is locked)
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, selectionRange, selectableLayers))
        {
            
           GameObject selectedObject = hit.collider.gameObject;
           Debug.Log("Hit object: " + selectedObject.name);
           if (selectedObject.transform.parent != null)
           {
                if (cardEngine.cardList.Contains(selectedObject.transform.parent.gameObject))
                {
           // Debug.Log("Selected card: " + selectedObject.name);
                    cardEngine.GivePlayerCard();
                }
           }
        }
    }

    /// <summary>
    /// Called when an object is selected via left mouse click.
    /// Override this method or subscribe to handle selection logic.
    /// </summary>
    /// <param name="selectedObject">The GameObject that was selected</param>
    /// <param name="hitPoint">The world position where the raycast hit</param>
    /// <param name="hitNormal">The normal vector of the surface hit</param>
    protected virtual void OnObjectSelected(GameObject selectedObject, Vector3 hitPoint, Vector3 hitNormal)
    {
        // Override this method in a derived class or add event system here
        Debug.Log($"Selected: {selectedObject.name} at position {hitPoint}");
    }
}
