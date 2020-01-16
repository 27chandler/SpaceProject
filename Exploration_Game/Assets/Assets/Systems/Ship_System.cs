using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ship_System : Tile_System
{
    [SerializeField] private Rigidbody2D ship_rb;
    [SerializeField] private Movement movement;

    private Tile_Manager tm;


    [SerializeField] private List<Tilemap> tilemaps = new List<Tilemap>();
    [Space]
    [SerializeField] private Grid ship_grid;
    [SerializeField] private List<Tilemap> ship_tilemaps = new List<Tilemap>();

    private bool is_converting_to_ship = false;
    private bool is_ship = false;

    private Dictionary<Vector3Int,bool> ship_tile_positions = new Dictionary<Vector3Int, bool>();

    private void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            is_converting_to_ship = !is_converting_to_ship;

            if ((is_converting_to_ship) && (!is_ship))
            {
                Convert_To_Ship(new Vector3Int(-5, 23, 0));
                Copy_Over(tilemaps,ship_tilemaps,0, false);
                Copy_Touching_Layers(tilemaps, ship_tilemaps, 0,false);

                movement.Set_Parent(ship_rb);

                tm.Init_Ship_Systems();

                is_converting_to_ship = false;
                is_ship = true;
            }
            else if (is_ship)
            {
                Convert_To_World(new Vector3Int(-5, 23, 0));
                Copy_Over(ship_tilemaps, tilemaps, 0, true);
                Copy_Touching_Layers(ship_tilemaps, tilemaps, 0, true);

                movement.Set_Parent(null);

                ship_rb.velocity = new Vector2(0.0f,0.0f);
                ship_rb.position = new Vector2(0.0f, 0.0f);
                ship_rb.angularVelocity = 0.0f;
                ship_rb.rotation = 0.0f;

                is_converting_to_ship = false;
                is_ship = false;
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

    private void Copy_Over(List<Tilemap> i_from, List<Tilemap> i_to, int i_index,bool i_do_convert_to_world)
    {
        foreach (var tile in ship_tile_positions)
        {
            Vector3Int pos = tile.Key;

            if (i_do_convert_to_world)
            {
                Vector3 float_pos = ship_grid.CellToWorld(pos);
                pos.x = Mathf.RoundToInt(float_pos.x);
                pos.y = Mathf.RoundToInt(float_pos.y);
            }

            i_to[i_index].SetTile(pos, i_from[i_index].GetTile(tile.Key));
            i_from[i_index].SetTile(tile.Key, null);
        }
    }

    private void Copy_Touching_Layers(List<Tilemap> i_from, List<Tilemap> i_to,int i_excluded_layer, bool i_do_convert_to_world)
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
                        }

                        i_to[index].SetTile(pos, i_from[index].GetTile(tile.Key));
                        i_from[index].SetTile(tile.Key, null);
                    }
                }

                index++;
            }
        }
    }
}
