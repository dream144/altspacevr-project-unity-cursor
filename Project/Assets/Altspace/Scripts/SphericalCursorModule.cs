using UnityEngine;

public class SphericalCursorModule : MonoBehaviour {
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


    void Awake() {
        Cursor = transform.Find("Cursor").gameObject;
        CursorMeshRenderer = Cursor.transform.GetComponentInChildren<MeshRenderer>();
        CursorMeshRenderer.GetComponent<Renderer>().material.color = new Color(0.0f, 0.8f, 1.0f);

        if (Cursor) {
            ScreenCoordinate = Cursor.transform.localPosition;
        }
    }

    void Update() {
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
        RaycastHit cursorHit = new RaycastHit();/* Your cursor hit code should set this properly. */;
        if (Physics.Raycast(transform.position, lookDir, out cursorHit, MaxDistance, ColliderMask)) {
            // Hit Selectable Target
            float distance = (cursorHit.point - transform.position).magnitude;
            Vector3 scale = Vector3.one * (distance * DistanceScaleFactor + 1.0f) / 2.0f;
            // Assign cursor to contact position and update scale
            Cursor.transform.position = cursorHit.point;
            Cursor.transform.localScale = scale;

        }
        else {
            // Reset cursor to virtual sphere and update scale
            Cursor.transform.localPosition = ScreenCoordinate;
            Cursor.transform.localScale = DefaultCursorScale;
        }


        // TODO: Update cursor transform.


        // Update highlighted object based upon the raycast.
        if (cursorHit.collider != null) {
            Selectable.CurrentSelection = cursorHit.collider.gameObject;
        }
        else {
            Selectable.CurrentSelection = null;
        }
    }
}
