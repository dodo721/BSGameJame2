using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosOnlyParent : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float zoom = 1;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) transform.position = target.position + (offset * zoom);
    }
}
