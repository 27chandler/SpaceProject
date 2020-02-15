using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Zoom_Control : MonoBehaviour
{
    private Camera cam;
    private float cam_default_zoom;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam_default_zoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set_Zoom(float i_zoom)
    {
        cam.orthographicSize = i_zoom;
    }

    public void Reset_Zoom()
    {
        cam.orthographicSize = cam_default_zoom;
    }
}
