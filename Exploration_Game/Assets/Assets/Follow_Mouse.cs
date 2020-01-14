using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Mouse : MonoBehaviour
{
    [SerializeField] Camera main_cam;
    [SerializeField] Transform player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public Vector3 Get_World_Position()
    {
        return main_cam.ScreenToWorldPoint(transform.position);
    }
}
