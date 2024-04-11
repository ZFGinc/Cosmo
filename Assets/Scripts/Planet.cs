using System;
using UnityEngine;

public class Planet: MonoBehaviour
{
    public float Gravity { get; private set; }

    private void Awake()
    {
        SetGravity();
    }

    private void SetGravity()
    {
        Gravity = UnityEngine.Random.Range(-30f, -10f);
        Physics.gravity = new Vector3(0, Gravity, 0);
    }
}