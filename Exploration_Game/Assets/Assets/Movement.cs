using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private Tilemap ship_floor_tilemap;

    private Rigidbody2D rb;

    private Rigidbody2D parent_rigidbody;

    private Vector2 last_frame_anchor;
    private Vector2 ship_anchor;
    private bool is_anchor_point_set = false;
    private bool on_ship_floor = false;

    private float current_rotation = 0.0f;

    [SerializeField] private float movement_speed, rotation_speed, drag_force, relative_constant;
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
            rb.AddRelativeForce(Vector3.up * movement_speed);
        }

        if (Input.GetKey(down_key))
        {
            rb.AddRelativeForce((-Vector3.up) * movement_speed);
        }

        if (Input.GetKey(left_key))
        {
            rb.AddRelativeForce((-Vector3.right) * movement_speed);
        }

        if (Input.GetKey(right_key))
        {
            rb.AddRelativeForce(Vector3.right * movement_speed);
        }

        //

        if (Input.GetKey(clockwise_key))
        {
            rb.AddTorque(-rotation_speed);
            current_rotation -= rotation_speed;
        }

        if (Input.GetKey(anticlockwise_key))
        {
            rb.AddTorque(rotation_speed);
            current_rotation += rotation_speed;
        }
    }

    private void FixedUpdate()
    {
        if ((parent_rigidbody == null) || (!on_ship_floor))
        {
            rb.AddForce(-(rb.velocity * drag_force));
        }

        if (parent_rigidbody != null)
        {
            Vector3Int ship_floor_pos = new Vector3Int();
            Vector2 conversion_pos = parent_rigidbody.GetPoint(new Vector2(transform.position.x, transform.position.y));
            ship_floor_pos.x = Mathf.FloorToInt(conversion_pos.x);
            ship_floor_pos.y = Mathf.FloorToInt(conversion_pos.y);

            if (ship_floor_tilemap.GetTile(ship_floor_pos) != null)
            {
                on_ship_floor = true;
            }
            else
            {
                on_ship_floor = false;
            }

            if (on_ship_floor)
            {
                if (last_frame_anchor != parent_rigidbody.GetRelativePoint(ship_anchor))
                {
                    if (is_anchor_point_set)
                    {
                        rb.velocity = ((parent_rigidbody.GetRelativePoint(ship_anchor) - last_frame_anchor) * 50.0f);
                        rb.angularVelocity = parent_rigidbody.angularVelocity;
                    }
                    else
                    {
                        is_anchor_point_set = true;
                    }

                    last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
                }

                if (Vector2.Distance(parent_rigidbody.GetRelativePoint(ship_anchor), transform.position) > 0.1f)
                {
                    Set_Anchor_Point();
                    is_anchor_point_set = false;
                    Debug.Log("Re-Anchored");
                }
            }
        }
    }

    public void Set_Parent(Rigidbody2D i_rb)
    {
        parent_rigidbody = i_rb;
        last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
        Set_Anchor_Point();
    }

    private void Set_Anchor_Point()
    {
        ship_anchor = parent_rigidbody.GetPoint(rb.position);
    }
}
