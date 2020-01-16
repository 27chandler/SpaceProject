using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private Tilemap ship_floor_tilemap;
    [SerializeField] private Tilemap floor_tilemap;

    [SerializeField] private float anchor_constant;

    private Rigidbody2D rb;

    private Rigidbody2D parent_rigidbody;
    private float parent_speed = 0.0f;

    private Vector2 last_frame_anchor;
    private Vector2 ship_anchor;
    private bool is_anchor_point_set = false;
    private bool on_ship_floor,on_floor = false;

    private float current_rotation = 0.0f;

    [SerializeField] private bool do_relative_movement;

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

        //if (has_moved_this_frame && parent_rigidbody != null)
        //{
        //    Set_Anchor_Point();
        //}
    }

    private void FixedUpdate()
    {
        Vector3Int floor_pos = new Vector3Int();
        floor_pos.x = Mathf.FloorToInt(transform.position.x);
        floor_pos.y = Mathf.FloorToInt(transform.position.y);

        if (floor_tilemap.GetTile(floor_pos) != null)
        {
            on_floor = true;
        }
        else
        {
            on_floor = false;
        }


        if (do_relative_movement)
        {
            if (parent_rigidbody != null)
            {
                parent_speed = parent_rigidbody.velocity.magnitude;

                Vector2 conversion_pos = parent_rigidbody.GetPoint(new Vector2(transform.position.x, transform.position.y));
                floor_pos.x = Mathf.FloorToInt(conversion_pos.x);
                floor_pos.y = Mathf.FloorToInt(conversion_pos.y);

                if (ship_floor_tilemap.GetTile(floor_pos) != null)
                {
                    if (!on_ship_floor)
                    {
                        last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
                        Set_Anchor_Point();
                    }
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
                            rb.velocity = ((parent_rigidbody.GetRelativePoint(ship_anchor) - last_frame_anchor) * anchor_constant);
                            rb.angularVelocity = parent_rigidbody.angularVelocity;
                        }
                        else
                        {
                            is_anchor_point_set = true;
                        }

                        last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
                    }

                    if (Vector2.Distance(parent_rigidbody.GetRelativePoint(ship_anchor), transform.position) > 0.5f)
                    {
                        Set_Anchor_Point();
                        is_anchor_point_set = false;
                    
                    }
                }
            }

            //if ((parent_rigidbody == null) || (!on_ship_floor))
            //{
            //    rb.AddForce(-(rb.velocity * drag_force));
            //}

            if (on_floor)
            {
                rb.AddForce(-(rb.velocity * drag_force));
            }
        }
    }

    public void Set_Parent(Rigidbody2D i_rb)
    {
        parent_rigidbody = i_rb;

        if (i_rb != null)
        {
            last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
            Set_Anchor_Point();
        }
    }

    private void Set_Anchor_Point()
    {
        Debug.Log("Re-Anchored");
        ship_anchor = parent_rigidbody.GetPoint(rb.position);
    }
}
