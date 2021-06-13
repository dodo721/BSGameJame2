using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class BoatController : MonoBehaviour
{

    public static BoatController player;

    private Rigidbody rb;
    private LineRenderer lineRenderer;
    public List<Boat> boatsInTow = new List<Boat>();
    public float dragLength;
    public float speed;
    public float forceIncreaseFac;
    public float currentForce;
    public float maxSpeed;
    public float turnSpeed;
    public float maxRopeLength;
    public float minRopeLength;
    private Rigidbody draggingBoat;
    public Transform cameraTransform;
    public float dragStrength;
    public float zoomAmount;
    public PosOnlyParent cameraTarget;

    // Start is called before the first frame update
    void Start()
    {
        player = this;
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update () {

        foreach (Boat boat in FindObjectsOfType<Boat>()) {
            if (boat != null && boat.gameObject.activeInHierarchy) boat.DisableHighlight();
        }

        cameraTarget.zoom = 1 + (boatsInTow.Count * zoomAmount);

        RaycastHit hit;
        //int layerMask = LAYER_MASK_IGNORE_PLAYER;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))//, layerMask))
        {
            float distance = draggingBoat == null ? Mathf.Infinity : Vector3.Distance(hit.transform.position, draggingBoat.transform.position);
            bool isBoat = hit.collider.CompareTag("Boat") || hit.collider.CompareTag("Player");
            Boat hitBoat = hit.collider.GetComponent<Boat>();
            Boat ourBoat = draggingBoat == null ? null : draggingBoat.GetComponent<Boat>();
            if (draggingBoat == null) {
                // Show hover highlight if boat is draggable
                if (hit.collider.CompareTag("Boat") && hitBoat.draggable && hitBoat.attachments.Count + hitBoat.inTow.Count < 3) {
                    hitBoat.HoverHighlight();
                }
            } else {
                // Show hover highlight if boat is in range of currently dragged boat
                if (isBoat && distance <= maxRopeLength && distance >= minRopeLength &&
                    hitBoat.attachments.Count + hitBoat.inTow.Count < 3 &&
                    ourBoat.attachments.Count + ourBoat.inTow.Count < 3) {
                    hitBoat.HoverHighlight();
                }
            }
            if (Input.GetButtonDown("Fire1")) {
                if (hit.collider.CompareTag("Boat") && hitBoat.draggable) {
                    hitBoat.ClickHighlight();
                    if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                    {
                        draggingBoat = hit.rigidbody;
                        draggingBoat.transform.parent = null;
                    }
                }
            }
            if (Input.GetButtonUp("Fire1")) {
                if ((hit.collider.CompareTag("Player") || hit.collider.CompareTag("Boat")) && hit.rigidbody != draggingBoat) {
                    if (distance <= maxRopeLength && distance >= minRopeLength) {
                        // If the target boat is attached to the player and the target boat is not already accounted for, log it
                        if (hitBoat.IsConnectedToPlayer() && !boatsInTow.Contains(ourBoat)) {
                            boatsInTow.Add(ourBoat);
                            boatsInTow.AddRange(ourBoat.inTow);
                        }
                        ourBoat.Connect(hit.rigidbody);
                    }
                }
            }
        }

        if (Input.GetButtonUp("Fire1")) {
            draggingBoat = null;
        }

        currentForce = boatsInTow.Count == 0 ? speed : speed + (boatsInTow.Count * speed * forceIncreaseFac);

        if (Mathf.Abs(Input.GetAxisRaw("Forward")) > 0) {
            if (rb.velocity.magnitude < maxSpeed)
                rb.AddForce(transform.forward * currentForce * Time.deltaTime, ForceMode.Acceleration);
        }
        if (Mathf.Abs(Input.GetAxisRaw("Right")) > 0) {
            rb.AddTorque(transform.up * turnSpeed * Time.deltaTime, ForceMode.Acceleration);
        }
        if (Mathf.Abs(Input.GetAxisRaw("Backward")) > 0) {
            if (rb.velocity.magnitude < maxSpeed)
                rb.AddForce(-transform.forward * currentForce * Time.deltaTime, ForceMode.Acceleration);
        }
        if (Mathf.Abs(Input.GetAxisRaw("Left")) > 0) {
            rb.AddTorque(-transform.up * turnSpeed * Time.deltaTime, ForceMode.Acceleration);
        }

        // Apply forces and draw effects for a dragging object
        if (draggingBoat != null) {

            Vector3 objectScreenPos = Camera.main.WorldToViewportPoint(draggingBoat.transform.position) - new Vector3(0.5f, 0.5f, 0);
            Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) - 0.5f - objectScreenPos.x, (Input.mousePosition.y / Screen.height) - 0.5f - objectScreenPos.y);
            Vector3 directionWorldSpace = new Vector3(mousePos.x, 0, mousePos.y);
            Vector3 directionCameraSpace = cameraTransform.TransformDirection(directionWorldSpace);
            
            if (draggingBoat.GetComponent<Boat>().attachments.Count == 0) {
                // Force
                float forceMag = directionCameraSpace.magnitude * dragStrength;
                draggingBoat.AddForce(directionCameraSpace * forceMag * Time.deltaTime, ForceMode.Acceleration);
                draggingBoat.transform.LookAt(draggingBoat.velocity);
            }

            // Line drawing
            lineRenderer.enabled = true;
            float colStrength = Mathf.Clamp(mousePos.magnitude * 20, 0f, 1f);
            Color lineColor = new Color(1f, 1f - colStrength, 1f - colStrength);
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.SetPositions(new Vector3[] {
                draggingBoat.transform.position,
                draggingBoat.transform.position + directionCameraSpace * dragLength
            });
        } else {
            lineRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
