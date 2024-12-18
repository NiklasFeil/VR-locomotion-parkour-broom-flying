using System;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.UI;
using UnityEngine;

public class FlyInCircle : MonoBehaviour
{
    [SerializeField] private Vector3 centerPosition;
    private double x = 0;
    private double y = 0;
    public float flightSpeed;
    public float circleRadius;
    private Vector3 rotationAxis;
    private float rotationAxisOffsetMultiplier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        centerPosition = transform.position;
        rotationAxis = new Vector3(15, 30, 45);
    }

    // Update is called once per frame
    void Update()
    {
        // Update position
        x += flightSpeed;
        y += flightSpeed;
        float current_offset_x = (float) Math.Cos(x);
        float current_offset_z = (float) Math.Sin(y);
        Vector3 current_offset = new Vector3(current_offset_x, 0, current_offset_z);
        transform.position = centerPosition + current_offset * circleRadius;

        // Update orientation
        Vector3 newOffset = rotationAxisOffsetMultiplier * UnityEngine.Random.onUnitSphere;
        rotationAxis = (rotationAxis + newOffset).normalized;
        transform.Rotate(rotationAxis, Space.Self);
    }
}
