using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkSpeed : MonoBehaviour
{
    public Animator Anm;
    public float min = 0.5f;
    public float max = 1.5f;

    void Awake()
    {
        Anm.speed = Random.Range(min, max);
    }
}
