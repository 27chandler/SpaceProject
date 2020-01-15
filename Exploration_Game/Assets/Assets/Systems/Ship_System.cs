using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ship_System : Tile_System
{
    [SerializeField] private Rigidbody2D ship_rb;
    [SerializeField] private Movement movement;

    [SerializeField] private List<Tilemap> tilemaps = new List<Tilemap>();
    [SerializeField] private List<Tilemap> ship_tilemaps = new List<Tilemap>();
    [SerializeField] private Transform ship_grid;

    private bool is_converting_to_ship = false;

    private Dictionary<Vector3Int,bool> ship_tile_positions = new Dictionary<Vector3Int, bool>();

    private void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            is_converting_to_ship = !is_converting_to_ship;
        }

        if (is_converting_to_ship)
        {
            Convert_To_Ship(new Vector3Int(-5,23,0));
            Copy_Over(0);
            Copy_Touching_Layers(0);
            
            movement.Set_Parent(ship_rb);

            is_converting_to_ship = false;
        }
    }

    private void Convert_To_Ship(Vector3Int i_start_pos)
    {
        ship_tile_positions.Add(i_start_pos,true);

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

    private void Copy_Over(int i_index)
    {
        foreach (var tile in ship_tile_positions)
        {
            ship_tilemaps[i_index].SetTile(tile.Key, tilemaps[i_index].GetTile(tile.Key));
            tilemaps[i_index].SetTile(tile.Key, null);
        }
    }

    private void Copy_Touching_Layers(int i_excluded_layer)
    {
        foreach (var tile in ship_tile_positions)
        {
            int index = 0;
            foreach (var tilemap in tilemaps)
            {
                if (index != i_excluded_layer)
                {
                    if (tilemaps[index].GetTile(tile.Key) != null)
                    {
                        ship_tilemaps[index].SetTile(tile.Key, tilemaps[index].GetTile(tile.Key));
                        tilemaps[index].SetTile(tile.Key, null);
                    }
                }

                index++;
            }
        }
    }
}
