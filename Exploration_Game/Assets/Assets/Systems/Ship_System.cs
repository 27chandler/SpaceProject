﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ship_System : Tile_System
{
    [SerializeField] private Transform player_transform;
    [SerializeField] private Movement player_movement;

    [SerializeField] private Rigidbody2D ship_rb;
    private float ship_default_angular_drag;
    [SerializeField] private Movement movement;

    private Tile_Manager tm;

    private Vector3Int wheel_pos;

    [SerializeField] private List<Tilemap> tilemaps = new List<Tilemap>();
    [Space]
    [SerializeField] private Grid ship_grid;
    private Movement ship_movement;
    [SerializeField] private List<Tilemap> ship_tilemaps = new List<Tilemap>();

    private bool is_converting_to_ship, is_snap_in_progress = false;
    private bool is_ship = false;

    private Dictionary<Vector3Int, bool> ship_tile_positions = new Dictionary<Vector3Int, bool>();

    private void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
        ship_movement = ship_grid.GetComponent<Movement>();

        tm.Init_Ship_Systems();

        Find_Wheel();
    }

    // Update is called once per frame
    void Update()
    {
        if (!is_ship)
        {
            ship_rb.velocity = new Vector2(0.0f, 0.0f);
            ship_rb.position = new Vector2(0.0f, 0.0f);
            ship_rb.angularVelocity = 0.0f;
            ship_rb.rotation = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Find_Wheel();
            if (Vector2.Distance((ship_grid.CellToWorld(wheel_pos)), player_transform.position) < 1.0f)
            {
                is_converting_to_ship = !is_converting_to_ship;

                if ((is_converting_to_ship) && (!is_ship))
                {
                    tm.Init_Ship_Systems();

                    Find_Wheel();

                    Convert_To_Ship(wheel_pos);
                    Copy_Over(tilemaps, ship_tilemaps, 0, false);
                    Copy_Touching_Layers(tilemaps, ship_tilemaps, 0, false);

                    movement.Set_Parent(ship_rb);
                    ship_movement.enabled = true;

                    is_converting_to_ship = false;
                    is_ship = true;
                }
                else if (is_ship)
                {
                    Debug.Log(ship_rb.angularVelocity);
                    if (Mathf.Abs(ship_rb.angularVelocity) > 10.0f)
                    {
                        Debug.Log("Ship rotating too fast to dock");
                    }
                    else if (Mathf.Abs(ship_rb.velocity.magnitude) > 5.0f)
                    {
                        Debug.Log("Ship moving too fast to dock");
                    }
                    else
                    {
                        Rotation_Snap();
                        is_snap_in_progress = true;
                        ship_default_angular_drag = ship_rb.angularDrag;
                    }
                }
            }
        }

        if (is_snap_in_progress)
        {
            if (Rotation_Snap())
            {
                ship_rb.angularDrag = ship_default_angular_drag;
                is_snap_in_progress = false;

                Convert_To_World(wheel_pos);
                Copy_Over(ship_tilemaps, tilemaps, 0, true);
                Copy_Touching_Layers(ship_tilemaps, tilemaps, 0, true);

                movement.Set_Parent(null);
                ship_movement.enabled = false;

                ship_rb.velocity = new Vector2(0.0f, 0.0f);
                ship_rb.position = new Vector2(0.0f, 0.0f);
                ship_rb.angularVelocity = 0.0f;
                ship_rb.rotation = 0.0f;

                is_converting_to_ship = false;
                is_ship = false;
            }
        }
    }

    private bool Rotation_Snap()
    {
        float angle = ship_rb.rotation;
        if (Mathf.Abs(angle) % 90.0f == 0.0f)
        {
            return true;
        }

        float rotation_direction = 1.0f;

        if (angle < 0.0f)
        {
            rotation_direction = -1.0f;
        }

        ship_rb.AddTorque(-ship_rb.angularVelocity * 400.0f);
        if (Mathf.Abs((Mathf.Abs(angle) % 90.0f)) < 5.0f)
        {

            if (Mathf.Abs((Mathf.Abs(angle) % 90.0f)) < 0.1f)
            {
                ship_rb.rotation = ship_rb.rotation - (angle % 90.0f);
                return true;
            }
        }
        else
        {
            //ship_rb.angularDrag = ship_default_angular_drag;
        }
        if (Mathf.Abs(angle) % 90.0f < 45.0f)
        {
            ship_rb.angularVelocity -= ((Mathf.Abs(angle) % 90.0f) * rotation_direction) / 70.0f;
            // Rotate clockwise
        }
        else
        {
            ship_rb.angularVelocity += ((Mathf.Abs(angle) % 90.0f) * rotation_direction) / 70.0f;
            // Rotate anti-clockwise
        }

        return false;
    }

    private void Find_Wheel()
    {
        Tilemap wheel_tilemap = tm.Grab_Layer(system_tiles["Wheel"][0]);

        foreach (var pos in wheel_tilemap.cellBounds.allPositionsWithin)
        {
            if (wheel_tilemap.GetTile(pos) == system_tiles["Wheel"][0])
            {
                wheel_pos = pos;
            }
        }
    }

    private void Convert_To_Ship(Vector3Int i_start_pos)
    {
        ship_tile_positions.Clear();

        ship_tile_positions.Add(i_start_pos, true);

        bool is_conversion_finished = false;
        int tilemap_index = 0;
        while (!is_conversion_finished)
        {
            List<Vector3Int> edge_tiles = new List<Vector3Int>();

            foreach (var pos in ship_tile_positions)
            {

                Vector3Int check_pos = pos.Key + new Vector3Int(1, 0, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }

                check_pos = pos.Key + new Vector3Int(-1, 0, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }

                check_pos = pos.Key + new Vector3Int(0, 1, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }

                check_pos = pos.Key + new Vector3Int(0, -1, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }
            }

            if (edge_tiles.Count == 0)
            {
                is_conversion_finished = true;
            }

            foreach (var tile_pos in edge_tiles)
            {
                if (!ship_tile_positions.ContainsKey(tile_pos))
                {
                    ship_tile_positions.Add(tile_pos, true);
                }
            }
            edge_tiles.Clear();

        }

        Debug.Log("Conversion finished with " + ship_tile_positions.Count + " tiles found");
    }

    private void Convert_To_World(Vector3Int i_start_pos)
    {
        ship_tile_positions.Clear();

        ship_tile_positions.Add(i_start_pos, true);

        bool is_conversion_finished = false;
        int tilemap_index = 0;
        while (!is_conversion_finished)
        {
            List<Vector3Int> edge_tiles = new List<Vector3Int>();

            foreach (var pos in ship_tile_positions)
            {

                Vector3Int check_pos = pos.Key + new Vector3Int(1, 0, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (ship_tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }

                check_pos = pos.Key + new Vector3Int(-1, 0, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (ship_tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }

                check_pos = pos.Key + new Vector3Int(0, 1, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (ship_tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }

                check_pos = pos.Key + new Vector3Int(0, -1, 0);
                if ((!ship_tile_positions.ContainsKey(check_pos)) && (ship_tilemaps[tilemap_index].GetTile(check_pos) != null))
                {
                    edge_tiles.Add(check_pos);
                }
            }

            if (edge_tiles.Count == 0)
            {
                is_conversion_finished = true;
            }

            foreach (var tile_pos in edge_tiles)
            {
                if (!ship_tile_positions.ContainsKey(tile_pos))
                {
                    ship_tile_positions.Add(tile_pos, true);
                }
            }
            edge_tiles.Clear();

        }

        Debug.Log("Conversion finished with " + ship_tile_positions.Count + " tiles found");
    }

    private void Copy_Over(List<Tilemap> i_from, List<Tilemap> i_to, int i_index, bool i_do_convert_to_world)
    {
        foreach (var tile in ship_tile_positions)
        {
            Vector3Int pos = tile.Key;

            if (i_do_convert_to_world)
            {
                Vector3 float_pos = ship_grid.CellToWorld(pos);
                pos.x = Mathf.RoundToInt(float_pos.x);
                pos.y = Mathf.RoundToInt(float_pos.y);

                tm.Add_Tile(pos, i_from[i_index].GetTile(tile.Key));
                tm.Ship_Remove_Tile(tile.Key, i_from[i_index].GetTile(tile.Key));

            }
            else
            {
                tm.Ship_Add_Tile(pos, i_from[i_index].GetTile(tile.Key));
                tm.Remove_Tile(tile.Key, i_from[i_index].GetTile(tile.Key));
            }
        }
    }

    private void Copy_Touching_Layers(List<Tilemap> i_from, List<Tilemap> i_to, int i_excluded_layer, bool i_do_convert_to_world)
    {
        foreach (var tile in ship_tile_positions)
        {
            int index = 0;
            foreach (var tilemap in tilemaps)
            {
                if (index != i_excluded_layer)
                {
                    if (i_from[index].GetTile(tile.Key) != null)
                    {
                        Vector3Int pos = tile.Key;

                        if (i_do_convert_to_world)
                        {
                            Vector3 float_pos = ship_grid.CellToWorld(pos);
                            pos.x = Mathf.RoundToInt(float_pos.x);
                            pos.y = Mathf.RoundToInt(float_pos.y);

                            tm.Add_Tile(pos, i_from[index].GetTile(tile.Key));
                            tm.Ship_Remove_Tile(tile.Key, i_from[index].GetTile(tile.Key));
                        }
                        else
                        {
                            tm.Ship_Add_Tile(pos, i_from[index].GetTile(tile.Key));
                            tm.Remove_Tile(tile.Key, i_from[index].GetTile(tile.Key));

                        }

                        //i_to[index].SetTile(pos, i_from[index].GetTile(tile.Key));
                        //tm.Grab_Systems(i_from[index].GetTile(tile.Key)).Remove_Tile(tile.Key, i_from[index].GetTile(tile.Key));
                        //i_from[index].SetTile(tile.Key, null);
                    }
                }

                index++;
            }
        }
    }
}
