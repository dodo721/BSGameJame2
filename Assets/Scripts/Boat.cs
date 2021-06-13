using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Boat : MonoBehaviour
{

    public bool attachAtStart;
    public Boat attachAtStartTo;
    public List<Boat> inTow = new List<Boat>();
    private bool isPlayer;

    public bool draggable;
    public struct Attachment {
        public Joint joint;
        public LineRenderer lineRenderer;
        public Boat connectedTo;
        public Attachment (Joint joint, LineRenderer lineRenderer, Boat connectedTo) {
            this.joint = joint;
            this.lineRenderer = lineRenderer;
            this.connectedTo = connectedTo;
        }
    }
    public List<Attachment> attachments = new List<Attachment>();
    public GameObject lineRendererPrefab;

    private Rigidbody rb;
    [Min(2)]
    public int ropeDensity;
    public float a;
    public float b;
    public float c;

    private Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        isPlayer = CompareTag("Player");
        if (!isPlayer) {
            rb = GetComponent<Rigidbody>();
            if (attachAtStart) {
                Connect(attachAtStartTo.GetComponent<Rigidbody>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayer) {
            foreach(Attachment attachment in attachments) {
                if (attachment.connectedTo != null && attachment.joint != null) {
                    Vector3 point1 = transform.position;
                    Vector3 point2 = attachment.connectedTo.transform.position;
                    Vector3[] points = new Vector3[ropeDensity];
                    for (int i = 0; i < ropeDensity; i++) {
                        float fac = (float)i / (float)ropeDensity;
                        float x = point1.x + ((point2.x - point1.x) * fac);
                        float z = point1.z + ((point2.z - point1.z) * fac);
                        float y = point1.y + ((a * (fac * fac)) + (b * fac) + c);
                        points[i] = new Vector3(x, y, z);
                    }
                    attachment.lineRenderer.enabled = true;
                    attachment.lineRenderer.positionCount = ropeDensity;
                    attachment.lineRenderer.SetPositions(points);
                } else {
                    attachment.lineRenderer.enabled = false;
                }
            }
        }
    }

    public void Connect (Rigidbody to) {
        if (isPlayer) throw new Exception("Player cannot connect to others!");
        Boat boat = to.GetComponent<Boat>();
        if (boat == null) throw new Exception("Must connect to boat!");
        if (boat == this || IsConnectedToBoat(boat) || boat.IsConnectedToBoat(this)) return;
        if (attachments.Count + inTow.Count >= 3 ||
            boat.attachments.Count + boat.inTow.Count >= 3) return;
        SpringJoint joint = gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = to;
        joint.spring = 100;
        joint.enableCollision = true;
        joint.autoConfigureConnectedAnchor = false;
        joint.damper = 100;
        LineRenderer lineRenderer = Instantiate(lineRendererPrefab, transform.position, Quaternion.identity, transform).GetComponent<LineRenderer>();
        Attachment attachment = new Attachment(joint, lineRenderer, boat);
        attachments.Add(attachment);
        boat.inTow.Add(this);
        Vector3 diff = boat.transform.position - transform.position;
        joint.anchor = diff * 2;
        joint.connectedAnchor = Vector3.zero;
    }

    public bool IsConnectedToBoat (Boat other) {
        foreach(Attachment attachment in attachments) {
            if (attachment.connectedTo == other) return true;
        }
        return false;
    }

    public bool IsConnectedToPlayer () {
        if (isPlayer) return true;
        List<Boat> done = new List<Boat>();
        return ConnectedToPlayerSearch(this, done);
    }

    private bool ConnectedToPlayerSearch (Boat current, List<Boat> done) {
        if (current.isPlayer) return true;
        if (done.Contains(current)) return false;
        done.Add(current);
        foreach(Attachment attachment in current.attachments) {
            if (ConnectedToPlayerSearch(attachment.connectedTo, done)) return true;
        }
        return false;
    }

    void FixedUpdate () {
        
    }

    public void HoverHighlight () {
        outline.OutlineColor = Color.yellow;
        outline.enabled = true;
    }

    public void ClickHighlight () {
        outline.OutlineColor = Color.red;
        outline.enabled = true;
    }

    public void DisableHighlight () {
        outline.enabled = false;
    }
}
