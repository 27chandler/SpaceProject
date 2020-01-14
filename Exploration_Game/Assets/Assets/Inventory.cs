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

    [SerializeField] private Tile_Manager.Tile_Info place_obj;

    [SerializeField] private Text inventory_debug_show;

    private int tile_count = 0;

    // Start is called before the first frame update
    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        inventory_debug_show.text = "Wires: " + tile_count;

        if ((Input.GetMouseButton(0)) && (tile_count > 0))
        {
            Vector3Int rounded_cursor_pos = new Vector3Int();
            rounded_cursor_pos.x = Mathf.FloorToInt(cursor_obj.Get_World_Position().x);
            rounded_cursor_pos.y = Mathf.FloorToInt(cursor_obj.Get_World_Position().y);
            rounded_cursor_pos.z = 0;

            if (place_obj.tilemap.GetTile(rounded_cursor_pos) == null)
            {
                tm.Add_Tile(rounded_cursor_pos, place_obj.tile);
                tile_count--;
                //tile_sys.Add_Tile(rounded_cursor_pos, place_obj.tile);
            }
        }

        if (Input.GetMouseButton(1))
        {
            Vector3Int rounded_cursor_pos = new Vector3Int();
            rounded_cursor_pos.x = Mathf.FloorToInt(cursor_obj.Get_World_Position().x);
            rounded_cursor_pos.y = Mathf.FloorToInt(cursor_obj.Get_World_Position().y);
            rounded_cursor_pos.z = 0;

            if (place_obj.tilemap.GetTile(rounded_cursor_pos) == place_obj.tile)
            {
                tm.Remove_Tile(rounded_cursor_pos, place_obj.tile);
                tile_count++;
                //tile_sys.Add_Tile(rounded_cursor_pos, place_obj.tile);
            }
        }
    }
}
