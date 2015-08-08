using UnityEngine;

public class SphericalCursorModule : MonoBehaviour {
    // To trigger whether show View rotation tip on Left Bottom corner
    public bool showTip = false;

    // This is a sensitivity parameter that should adjust how sensitive the mouse control is.
    public float Sensitivity;

    // This is a scale factor that determines how much to scale down the cursor based on its collision distance.
    public float DistanceScaleFactor;

    // This is the layer mask to use when performing the ray cast for the objects.
    // The furniture in the room is in layer 8, everything else is not.
    private const int ColliderMask = (1 << 8);

    // This is the Cursor game object. Your job is to update its transform on each frame.
    private GameObject Cursor;

    // This is the Cursor mesh. (The sphere.)
    private MeshRenderer CursorMeshRenderer;

    // This is the scale to set the cursor to if no ray hit is found.
    private Vector3 DefaultCursorScale = new Vector3(2.0f, 2.0f, 2.0f);

    // Maximum distance to ray cast.
    private const float MaxDistance = 100.0f;

    // Sphere radius to project cursor onto if no raycast hit.
    private const float SphereRadius = 100.0f;

    // Extra paramters included by Xin
    // The screen coordinates of the cursor
    private Vector3 ScreenCoordinate;
    private Vector3 InitCoordiante;


    void Awake() {
        // Find Cursor Object
        Cursor = transform.Find("Cursor").gameObject;
        CursorMeshRenderer = Cursor.transform.GetComponentInChildren<MeshRenderer>();
        CursorMeshRenderer.GetComponent<Renderer>().material.color = new Color(0.0f, 0.8f, 1.0f);
        // Record initial coordinate for reset function
        if (Cursor) {
            ScreenCoordinate = Cursor.transform.localPosition;
            InitCoordiante = Cursor.transform.localPosition;
        }
    }

    void Update() {
        // In case of Cursor Move out of visual range and can not be found
        if (Input.GetKey(KeyCode.R)) {
            ResetCursor();
        }

        // If Cursor is enabled
        if (Cursor.activeSelf) {
            // Handle mouse movement to update cursor position.
            Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            mouseMovement *= Sensitivity;
            // Make sure coordinate in range
            if ((ScreenCoordinate + mouseMovement).magnitude <= SphereRadius) {
                ScreenCoordinate += mouseMovement;
            }
            // Update cursor position 
            Cursor.transform.localPosition = ScreenCoordinate;

            // Perform ray cast to find object cursor is pointing at.
            Vector3 lookDir = Cursor.transform.position - transform.position;
            RaycastHit cursorHit = new RaycastHit();
            if (Physics.Raycast(transform.position, lookDir, out cursorHit, MaxDistance, ColliderMask)) {
                // Hit Selectable Target
                float distance = (cursorHit.point - transform.position).magnitude;
                Vector3 scale = Vector3.one * (distance * DistanceScaleFactor + 1.0f) / 2.0f;
                // Assign cursor to contact position and update scale
                Cursor.transform.position = cursorHit.point;
                Cursor.transform.localScale = scale;

            }
            else {
                // If no raycast hit
                // Reset cursor to virtual sphere and update scale
                Cursor.transform.localPosition = ScreenCoordinate;
                Cursor.transform.localScale = DefaultCursorScale;
            }

            // Update highlighted object based upon the raycast.
            if (cursorHit.collider != null) {
                Selectable.CurrentSelection = cursorHit.collider.gameObject;
            }
            else {
                Selectable.CurrentSelection = null;
            }
        }
    }

    // Reset cursor back to screen center
    public void ResetCursor() {
        Cursor.SetActive(true);
        ScreenCoordinate = InitCoordiante;
    }

    // Lock cursor to center and hide it for object interaction
    public void LockAndHideCursor() {
        Cursor.SetActive(false);
        ScreenCoordinate = InitCoordiante;
    }

    // Show tip for SHIFT-Looking around
    void OnGUI() {
        if(showTip && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
            GUI.Box(new Rect(10, Screen.height - 40, 200, 30), "Hold SHIFT to Look Around");
        }
    }
}
