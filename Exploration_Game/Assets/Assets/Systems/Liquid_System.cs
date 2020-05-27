using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Liquid_System : Tile_System
{
    private Tile_Manager tm;

    [SerializeField] private Tilemap blocking_tilemap;

    [SerializeField][Range(0.0f,1.0f)] private float min_water,max_water,min_differential; 

    private Dictionary<Vector3Int, Liquid_Info> liquid_dictionary = new Dictionary<Vector3Int, Liquid_Info>();
    [SerializeField] private float SPREAD_DELAY = 2.0f;

    private Dictionary<Vector3Int, float> spread_value_dictionary = new Dictionary<Vector3Int, float>(); // Dictionary for storing the future liquid amount on the tiles for when the tile is added to the system

    public class Liquid_Info
    {
        public float spread_percentage;
        public float spread_speed;
        public float liquid_amount;
        public TileBase tile;
    }

    // Start is called before the first frame update
    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<Vector3Int, float> liquid_buffer = new Dictionary<Vector3Int, float>();
        foreach (var liquid_tile in liquid_dictionary)
        {
            liquid_buffer.Add(liquid_tile.Key, liquid_tile.Value.spread_percentage + (Time.deltaTime * liquid_tile.Value.spread_speed));
        }

        foreach (var buffer in liquid_buffer)
        {
            if (buffer.Value >= SPREAD_DELAY)
            {
                liquid_dictionary[buffer.Key].spread_percentage = 0.0f;
                Spread_Neighbour(buffer.Key);
            }
            else
            {
                liquid_dictionary[buffer.Key].spread_percentage = buffer.Value;
            }
        }

        Set_Tile_Colours();
    }

    private void Set_Tile_Colours()
    {
        Tilemap liquid_tilemap;
        foreach (var liquid_tile in liquid_dictionary)
        {
            liquid_tilemap = tm.Grab_Layer(liquid_tile.Value.tile);
            liquid_tilemap.SetTileFlags(liquid_tile.Key, TileFlags.None);
            liquid_tilemap.SetColor(liquid_tile.Key, new Color(1.0f, 1.0f, 1.0f, Mathf.Clamp(liquid_tile.Value.liquid_amount,min_water, max_water)));
        }
    }

    private void Spread_Neighbour(Vector3Int i_pos)
    {
        TileBase liquid_tile;
        Tilemap liquid_tilemap;

        //spawner_tile = system_tiles["Spawner"][0];
        liquid_tile = liquid_dictionary[i_pos].tile;

        if (is_ship_mode)
        {
            //spawner_tile = system_tiles["Spawner"][0];
            liquid_tilemap = tm.Grab_Ship_Layer(liquid_tile);
        }
        else
        {
            liquid_tilemap = tm.Grab_Layer(liquid_tile);
        }



        Vector3Int up_neighbour = i_pos + Vector3Int.up;
        if (Capacity_Compare(i_pos,up_neighbour))
        {
            if (!liquid_dictionary.ContainsKey(up_neighbour))
            {
                Drain(i_pos, up_neighbour);
                if (is_ship_mode)
                {
                    tm.Ship_Add_Tile(up_neighbour, liquid_tile);
                }
                else
                {
                    tm.Add_Tile(up_neighbour, liquid_tile);
                }
            }
            else
            {
                Equalize(i_pos, up_neighbour);
            }
        }

        Vector3Int down_neighbour = i_pos + Vector3Int.down;
        if (Capacity_Compare(i_pos, down_neighbour))
        {
            if (!liquid_dictionary.ContainsKey(down_neighbour))
            {
                Drain(i_pos, down_neighbour);
                if (is_ship_mode)
                {
                    tm.Ship_Add_Tile(down_neighbour, liquid_tile);
                }
                else
                {
                    tm.Add_Tile(down_neighbour, liquid_tile);
                }
            }
            else
            {
                Equalize(i_pos, down_neighbour);
            }
        }

        Vector3Int left_neighbour = i_pos + Vector3Int.left;
        if (Capacity_Compare(i_pos, left_neighbour))
        {
            if (!liquid_dictionary.ContainsKey(left_neighbour))
            {
                Drain(i_pos, left_neighbour);
                if (is_ship_mode)
                {
                    tm.Ship_Add_Tile(left_neighbour, liquid_tile);
                }
                else
                {
                    tm.Add_Tile(left_neighbour, liquid_tile);
                }
            }
            else
            {
                Equalize(i_pos, left_neighbour);
            }
        }

        Vector3Int right_neighbour = i_pos + Vector3Int.right;
        if (Capacity_Compare(i_pos, right_neighbour))
        {
            if (!liquid_dictionary.ContainsKey(right_neighbour))
            {
                Drain(i_pos, right_neighbour);
                if (is_ship_mode)
                {
                    tm.Ship_Add_Tile(right_neighbour, liquid_tile);
                }
                else
                {
                    tm.Add_Tile(right_neighbour, liquid_tile);
                }
            }
            else
            {
                Equalize(i_pos, right_neighbour);
            }
        }
    }

    public override void Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
        if (!liquid_dictionary.ContainsKey(i_pos))
        {
            Liquid_Info temp_spawner_info = new Liquid_Info();
            temp_spawner_info.spread_percentage = 0.0f;
            temp_spawner_info.spread_speed = Random.Range(1.0f, 3.0f);
            temp_spawner_info.tile = i_tile;

            if (spread_value_dictionary.ContainsKey(i_pos))
            {
                temp_spawner_info.liquid_amount = spread_value_dictionary[i_pos];
                spread_value_dictionary.Remove(i_pos);
            }
            else
            {
                temp_spawner_info.liquid_amount = 10.0f;
            }

            liquid_dictionary.Add(i_pos, temp_spawner_info);
        }
    }

    public override void Remove_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        if (liquid_dictionary.ContainsKey(i_pos))
        {
            liquid_dictionary.Remove(i_pos);
        }
    }

    // Totals the liquid in both tiles, then distributes it equally
    private void Equalize(Vector3Int i_pos_1, Vector3Int i_pos_2)
    {
        Debug.Log("Equalized");
        float total_liquid = liquid_dictionary[i_pos_1].liquid_amount + liquid_dictionary[i_pos_2].liquid_amount;

        liquid_dictionary[i_pos_1].liquid_amount = total_liquid / 2.0f;
        liquid_dictionary[i_pos_2].liquid_amount = total_liquid / 2.0f;
    }

    private void Drain(Vector3Int i_pos,Vector3Int i_dest)
    {
        liquid_dictionary[i_pos].liquid_amount /= 2.0f;

        if (spread_value_dictionary.ContainsKey(i_dest))
        {
            spread_value_dictionary[i_dest] += liquid_dictionary[i_pos].liquid_amount;
        }
        else
        {
            spread_value_dictionary.Add(i_dest, liquid_dictionary[i_pos].liquid_amount);
        }
    }

    private bool Capacity_Compare(Vector3Int i_origin_pos, Vector3Int i_compare_pos)
    {
        if (blocking_tilemap.GetTile(i_compare_pos) != null) // Checks if the comparison tile is a wall
        {
            return false;
        }

        if (!liquid_dictionary.ContainsKey(i_compare_pos)) // Checks if the comparison tile has liquid
        {
            return true;
        }

        if (Mathf.Abs(liquid_dictionary[i_origin_pos].liquid_amount - liquid_dictionary[i_compare_pos].liquid_amount) < min_differential) // Checks if the two tiles difference in liquid capacity is lower than the required threshold to begin a swap
        {
            return false;
        }

        if (liquid_dictionary[i_origin_pos].liquid_amount > liquid_dictionary[i_compare_pos].liquid_amount)
        {
            return true;
        }
        return false;
    }
}
