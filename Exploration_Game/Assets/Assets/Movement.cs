using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float movement_speed, rotation_speed;
    [Space]
    [SerializeField] private KeyCode up_key;
    [SerializeField] private KeyCode down_key;
    [SerializeField] private KeyCode left_key;
    [SerializeField] private KeyCode right_key;
    [Space]
    [SerializeField] private KeyCode clockwise_key;
    [SerializeField] private KeyCode anticlockwise_key;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(up_key))
        {
            rb.AddForce(transform.up * movement_speed);
        }

        if (Input.GetKey(down_key))
        {
            rb.AddForce((-transform.up) * movement_speed);
        }

        if (Input.GetKey(left_key))
        {
            rb.AddForce((-transform.right) * movement_speed);
        }

        if (Input.GetKey(right_key))
        {
            rb.AddForce(transform.right * movement_speed);
        }

        //

        if (Input.GetKey(clockwise_key))
        {
            rb.AddTorque(-rotation_speed);
        }

        if (Input.GetKey(anticlockwise_key))
        {
            rb.AddTorque(rotation_speed);
        }
    }
}
