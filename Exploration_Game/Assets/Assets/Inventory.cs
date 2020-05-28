using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private Tile_Manager tm;

    [Serializable]
    public class Inventory_Slot
    {
        public TileBase tile;
        public int amount;
    }
    private List<Inventory_Slot> tile_inventory = new List<Inventory_Slot>();
    private int slot_selection = 0;

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

        Init_Inventory_Slots();

    }

    private void Init_Inventory_Slots()
    {
        List<TileBase> tile_list = tm.Grab_All_Tiletypes();

        foreach (var tiles in tile_list)
        {
            tile_inventory.Add(new Inventory_Slot { tile = tiles, amount = 100 });
        }
    }

    private void Selection_Update()
    {
        // Controls for moving between inventory selections
        if (Input.mouseScrollDelta.y < 0.0f)
        {
            slot_selection -= 1;
        }
        if (Input.mouseScrollDelta.y > 0.0f)
        {
            slot_selection += 1;
        }
        //

        // Forces the slot selection to roll over back to the other side of the selection
        if (slot_selection < 0)
        {
            slot_selection = tile_inventory.Count - 1;
        }

        if (slot_selection >= tile_inventory.Count)
        {
            slot_selection = 0;
        }
        //
    }

    // Update is called once per frame
    void Update()
    {
        Selection_Update();

        inventory_debug_show.text = "Slot: " + slot_selection + ": " + tile_inventory[slot_selection].amount;

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

        float distance = Vector2.Distance(hit_position, cursor_obj.Get_World_Position());

        if (distance < 1.0f)
        {
            if ((Input.GetMouseButton(0)) && (tile_inventory[slot_selection].amount > 0))
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
                    if (tm.Grab_Layer(tile_inventory[slot_selection].tile).GetTile(default_grid.WorldToCell(cursor_obj.Get_World_Position())) == null)
                    {
                        tm.Add_Tile(default_grid.WorldToCell(cursor_obj.Get_World_Position()), tile_inventory[slot_selection].tile);
                        tile_inventory[slot_selection].amount --;
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
                    TileBase temp = tm.Grab_Layer(tile_inventory[slot_selection].tile).GetTile(default_grid.WorldToCell(cursor_obj.Get_World_Position()));
                    if (tm.Grab_Layer(tile_inventory[slot_selection].tile).GetTile(default_grid.WorldToCell(cursor_obj.Get_World_Position())) == tile_inventory[slot_selection].tile)
                    {
                        tm.Remove_Tile(default_grid.WorldToCell(cursor_obj.Get_World_Position()), tile_inventory[slot_selection].tile);
                        tile_inventory[slot_selection].amount++;
                    }
                }
            }
        }
    }
}
