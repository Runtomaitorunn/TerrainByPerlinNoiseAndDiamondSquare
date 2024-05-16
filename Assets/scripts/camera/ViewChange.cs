using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ViewChange : MonoBehaviour
{
    public Vector3 target;      
    public float rotationSpeed = 20.0f;  
    public float radius = 120.0f;
    private float angle = 0.0f;
   
    public Slider FOVSlider; 
    private float lastFOVValue;
    public Slider ZoomSlider;
    public float zoomSpeed = 100f;
    private float lastZoomValue;

    private void Start()
    {

        lastFOVValue = FOVSlider.value;
        lastZoomValue = ZoomSlider.value;
    }

    private void Update()
    {
        // Ensure that a target is assigned
        if (target == null)
        {
            Debug.LogError("Target not assigned for camera rotation.");
            return;
        }

        // Calculate the position on the circle based on time and rotation speed
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Set the camera position relative to the target
        transform.position = new Vector3(x, transform.position.y, z);

        // Rotate the camera around the target position
        transform.RotateAround(target, Vector3.up, rotationSpeed * Time.deltaTime);

        // Ensure that the camera is always looking at the center of the target
        transform.LookAt(target);

        // Update the angle for the next frame
        angle += rotationSpeed * Time.deltaTime;
        if (FOVSlider.value != lastFOVValue)
        {
            ChangeFOV(FOVSlider.value);
            lastFOVValue = FOVSlider.value;
        }
        if (ZoomSlider.value != lastZoomValue)
        {
            Changezoom(ZoomSlider.value);
            lastZoomValue = ZoomSlider.value;
        }
    }

    void ChangeFOV(float fovValue)
    {
        radius = fovValue;
        
    }

    void Changezoom(float fovValue)
    {
        
        Camera.main.fieldOfView = fovValue;
    }
}
