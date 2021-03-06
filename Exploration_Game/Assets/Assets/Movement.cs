﻿using System.Collections;
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
    private Grid parent_grid;

    private Vector2 last_frame_anchor;
    private Vector2 ship_anchor;
    private bool is_anchor_point_set = false;
    private bool on_ship_floor,on_floor,has_entered_ship,is_in_space = false;

    private float current_rotation = 0.0f;

    [SerializeField] private bool has_independant_movement, do_relative_movement;

    [SerializeField] private float movement_speed, rotation_speed, drag_force,space_speed_multiplier;
    private float default_speed;
    [Space]
    [SerializeField] private string up_event;
    [SerializeField] private string down_event;
    [SerializeField] private string left_event;
    [SerializeField] private string right_event;
    [Space]
    [SerializeField] private string clockwise_event;
    [SerializeField] private string anticlockwise_event;

    private Quaternion rotation;

    public void Set_Independant_Movement(bool i_flag)
    {
        has_independant_movement = i_flag;
    }

    public void Set_Movement_Speed(float i_speed)
    {
        movement_speed = i_speed;
    }

    public void Set_Rotation(Quaternion i_rotation)
    {
        rotation = i_rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        default_speed = movement_speed;

        Input_Manager.static_input[up_event].actions += Move_Forward;
        Input_Manager.static_input[down_event].actions += Move_Backward;
        Input_Manager.static_input[left_event].actions += Move_Left;
        Input_Manager.static_input[right_event].actions += Move_Right;

        Input_Manager.static_input[clockwise_event].actions += Rotate_Clockwise;
        Input_Manager.static_input[anticlockwise_event].actions += Rotate_AntiClockwise;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_in_space)
        {
            movement_speed = default_speed * space_speed_multiplier;
        }
        else
        {
            movement_speed = default_speed;
        }
    }

    void Move_Forward()
    {
        Vector3 forward = rotation * Vector3.up;

        if (has_independant_movement)
        {
            rb.AddRelativeForce(forward * movement_speed);
        }
    }

    void Move_Backward()
    {
        Vector3 forward = rotation * Vector3.up;

        if (has_independant_movement)
        {
            rb.AddRelativeForce((-forward) * movement_speed);
        }
    }

    void Move_Left()
    {
        Vector3 right = rotation * Vector3.right;

        if (has_independant_movement)
        {
            rb.AddRelativeForce((-right) * movement_speed);
        }
    }

    void Move_Right()
    {
        Vector3 right = rotation * Vector3.right;

        if (has_independant_movement)
        {
            rb.AddRelativeForce(right * movement_speed);
        }
    }

    void Rotate_Clockwise()
    {
        if (has_independant_movement)
        {
            rb.AddTorque(-rotation_speed);
            current_rotation -= rotation_speed;
        }
    }

    void Rotate_AntiClockwise()
    {
        if (has_independant_movement)
        {
            rb.AddTorque(rotation_speed);
            current_rotation += rotation_speed;
        }
    }

    private void FixedUpdate()
    {
        if (do_relative_movement)
        {
            Vector3Int floor_pos = new Vector3Int();
            floor_pos.x = Mathf.RoundToInt(transform.position.x);
            floor_pos.y = Mathf.RoundToInt(transform.position.y);

            Vector3Int floor_pos_floor = new Vector3Int();
            floor_pos_floor.x = Mathf.FloorToInt(transform.position.x);
            floor_pos_floor.y = Mathf.FloorToInt(transform.position.y);

            if (parent_grid != null)
            {
                if (ship_floor_tilemap.GetTile(parent_grid.WorldToCell(transform.position)) != null)
                {
                    on_ship_floor = true;
                }
                else
                {
                    on_ship_floor = false;
                }
            }
            else
            {
                on_ship_floor = false;
            }

            if (floor_tilemap.GetTile(floor_pos_floor) != null)
            {
                on_floor = true;
            }
            else
            {
                on_floor = false;
            }

            if ((parent_rigidbody != null) && (on_ship_floor))
            {
                is_in_space = false;
                if (!has_entered_ship)
                {
                    last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
                    Set_Anchor_Point(true);

                    rb.velocity = parent_rigidbody.velocity;
                    rb.angularVelocity = parent_rigidbody.angularVelocity;

                    is_anchor_point_set = false;
                }

                has_entered_ship = true;
                parent_speed = parent_rigidbody.velocity.magnitude;

                Vector2 conversion_pos = parent_rigidbody.GetPoint(new Vector2(transform.position.x, transform.position.y));

                if (!on_ship_floor)
                {
                    last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
                    Set_Anchor_Point(true);
                }

                if (on_ship_floor)
                {
                    //if (last_frame_anchor != parent_rigidbody.GetRelativePoint(ship_anchor))
                    //{
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
                    //}

                    if (Vector2.Distance(parent_rigidbody.GetRelativePoint(ship_anchor), transform.position) > 0.5f)
                    {
                        Set_Anchor_Point(true);
                        is_anchor_point_set = false;

                    }
                }
            }
            else if (!on_floor && !on_ship_floor)
            {
                if (!is_in_space && !has_entered_ship)
                {
                    rb.angularVelocity = rb.angularVelocity / drag_force;
                }

                rb.angularVelocity = 0.0f;
                has_entered_ship = false;
                is_in_space = true;
            }
            else
            {
                rb.velocity = new Vector2(0.0f,0.0f);
                rb.angularVelocity = 0.0f;
                has_entered_ship = false;
                is_in_space = false;
            }
        }
    }

    public void Set_Parent(Rigidbody2D i_rb)
    {
        parent_rigidbody = i_rb;

        if (i_rb != null)
        {
            parent_grid = parent_rigidbody.GetComponent<Grid>();
            last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
            Set_Anchor_Point(true);
        }
        else
        {
            parent_grid = null;
        }
    }

    private void Set_Anchor_Point(bool is_local)
    {
        Debug.Log("Re-Anchored");
        if (is_local)
        {
            ship_anchor = parent_rigidbody.GetPoint(rb.position);
            last_frame_anchor = parent_rigidbody.GetRelativePoint(ship_anchor);
        }
        else
        {
            ship_anchor = new Vector2();
            ship_anchor.x = transform.position.x;
            ship_anchor.y = transform.position.y;
        }
        
    }
}
