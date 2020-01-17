using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private Tile_Manager tm;

    [SerializeField] private Follow_Mouse cursor_obj;
    [SerializeField] private Tile_System tile_sys;

    [SerializeField] private TileBase place_tile;

    [SerializeField] private Tilemap place_tilemap;
    [SerializeField] private Tilemap alternate_place_tilemap;
    private Grid default_grid;
    private Grid ship_grid;

    [SerializeField] private Tilemap floor_tilemap;
    [SerializeField] private Tilemap alternate_floor_tilemap;


    [SerializeField] private Text inventory_debug_show;

    private int tile_count = 0;

    // Start is called before the first frame update
    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();

        default_grid = place_tilemap.GetComponentInParent<Grid>();
        ship_grid = alternate_place_tilemap.GetComponentInParent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        inventory_debug_show.text = "Wires: " + tile_count;

        Vector3Int rounded_cursor_pos = new Vector3Int();
        rounded_cursor_pos.x = Mathf.FloorToInt(cursor_obj.Get_World_Position().x);
        rounded_cursor_pos.y = Mathf.FloorToInt(cursor_obj.Get_World_Position().y);
        rounded_cursor_pos.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rounded_cursor_pos - transform.position, Vector3.Distance(transform.position, rounded_cursor_pos));

        Vector3 hit_position;

        if (hit.collider != null)
        {
            hit_position = hit.point;
        }
        else
        {
            hit_position = cursor_obj.Get_World_Position();
        }

        Debug.DrawLine(transform.position, hit_position,Color.red);
        float distance = Vector2.Distance(hit_position, cursor_obj.Get_World_Position());
        Debug.Log(distance);

        if (distance < 1.0f)
        {
            if ((Input.GetMouseButton(0)) && (tile_count > 0))
            {
                if (alternate_floor_tilemap.GetTile(ship_grid.WorldToCell(cursor_obj.Get_World_Position())) != null)
                {
                    if (alternate_place_tilemap.GetTile(ship_grid.WorldToCell(cursor_obj.Get_World_Position())) == null)
                    {
                        tm.Ship_Add_Tile(ship_grid.WorldToCell(cursor_obj.Get_World_Position()), place_tile);
                        tile_count--;
                    }
                }
                else if (floor_tilemap.GetTile(default_grid.WorldToCell(cursor_obj.Get_World_Position())) != null)
                {
                    if (place_tilemap.GetTile(default_grid.WorldToCell(cursor_obj.Get_World_Position())) == null)
                    {
                        tm.Add_Tile(default_grid.WorldToCell(cursor_obj.Get_World_Position()), place_tile);
                        tile_count--;
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                if (alternate_floor_tilemap.GetTile(ship_grid.WorldToCell(cursor_obj.Get_World_Position())) != null)
                {
                    if (alternate_place_tilemap.GetTile(ship_grid.WorldToCell(cursor_obj.Get_World_Position())) == place_tile)
                    {
                        tm.Ship_Remove_Tile(ship_grid.WorldToCell(cursor_obj.Get_World_Position()), place_tile);
                        tile_count++;
                    }
                }
                else if (floor_tilemap.GetTile(default_grid.WorldToCell(cursor_obj.Get_World_Position())) != null)
                {
                    if (place_tilemap.GetTile(default_grid.WorldToCell(cursor_obj.Get_World_Position())) == place_tile)
                    {
                        tm.Remove_Tile(default_grid.WorldToCell(cursor_obj.Get_World_Position()), place_tile);
                        tile_count++;
                    }
                }
            }
        }
    }
}
