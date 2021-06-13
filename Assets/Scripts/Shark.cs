using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{

    public Animator animator;
    public Boat target;
    public float speed;
    public float turnSpeed;
    public float maxRange;
    private Vector3 originalPos;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && Vector3.Distance(transform.position, originalPos) < maxRange) {
            rb.MovePosition(Vector3.Lerp(transform.position, target.transform.position, speed * Time.deltaTime));
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), turnSpeed * Time.deltaTime));
            if (Vector3.Distance(transform.position, originalPos) >= maxRange) target = null;
        } else {
            rb.MovePosition(Vector3.Lerp(transform.position, originalPos, speed * Time.deltaTime));
            Quaternion face = Vector3.Distance(transform.position, originalPos) < 4 ? Quaternion.identity : Quaternion.LookRotation(originalPos - transform.position);
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, face, turnSpeed * Time.deltaTime));
        }
        animator.SetBool("Chasing", target != null && Vector3.Distance(transform.position, originalPos) > 2);
    }

    void OnTriggerEnter (Collider other) {
        if (target == null && (other.CompareTag("Boat") || other.CompareTag("Player")) && other.GetComponent<Boat>().IsConnectedToPlayer()) {
            target = other.GetComponent<Boat>();
        }
    }
}
