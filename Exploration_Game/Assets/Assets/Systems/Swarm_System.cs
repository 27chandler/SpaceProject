using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Swarm_System : Tile_System
{
    private Tile_Manager tm;
    private Dictionary<Vector3Int, Spawner_Info> swarm_dictionary = new Dictionary<Vector3Int, Spawner_Info>();
    private const float SPREAD_DELAY = 100.0f;

    public class Spawner_Info
    {
        public float spread_percentage;
        public float spread_speed;
    }

    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
    }

    protected override void System_Update()
    {
        Dictionary<Vector3Int, float> swarm_buffer = new Dictionary<Vector3Int, float>();
        foreach (var spawner in swarm_dictionary)
        {
            swarm_buffer.Add(spawner.Key, spawner.Value.spread_percentage + (Time.deltaTime * spawner.Value.spread_speed));
        }

        foreach (var buffer in swarm_buffer)
        {
            if (buffer.Value >= SPREAD_DELAY)
            {
                swarm_dictionary[buffer.Key].spread_percentage = 0.0f;
                Convert_Neighbour(buffer.Key);
            }
            else
            {
                swarm_dictionary[buffer.Key].spread_percentage = buffer.Value;
            }
        }
    }

    public override void Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
        if (tm.Check_Layer_Name(i_tile,"Spawner"))
        {
            if (!swarm_dictionary.ContainsKey(i_pos))
            {
                Spawner_Info temp_spawner_info = new Spawner_Info();
                temp_spawner_info.spread_percentage = 0.0f;
                temp_spawner_info.spread_speed = Random.Range(1.0f, 3.0f);
                swarm_dictionary.Add(i_pos, temp_spawner_info);
            }
        }
    }

    public override void Remove_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        if (swarm_dictionary.ContainsKey(i_pos))
        {
            swarm_dictionary.Remove(i_pos);
        }
    }


    private void Convert_Neighbour(Vector3Int i_pos)
    {
        bool has_spread = false;

        TileBase spawner_tile = system_tiles["Spawner"][0];
        Tilemap spawner_tilemap = tm.Grab_Layer(spawner_tile);

        Vector3Int up_neighbour = i_pos + Vector3Int.up;
        if ((spawner_tilemap.GetTile(up_neighbour) != null) && (Count_Swarm_Neighbours(up_neighbour) < 2))
        {
            if (!swarm_dictionary.ContainsKey(up_neighbour))
            {
                tm.Add_Tile(up_neighbour, spawner_tile);
                has_spread = true;
            }
        }

        Vector3Int down_neighbour = i_pos + Vector3Int.down;
        if (((spawner_tilemap.GetTile(down_neighbour) != null) && (!has_spread)) && (Count_Swarm_Neighbours(down_neighbour) < 2))
        {
            if (!swarm_dictionary.ContainsKey(down_neighbour))
            {
                tm.Add_Tile(down_neighbour, spawner_tile);
                has_spread = true;
            }
        }

        Vector3Int left_neighbour = i_pos + Vector3Int.left;
        if (((spawner_tilemap.GetTile(left_neighbour) != null) && (!has_spread)) && (Count_Swarm_Neighbours(left_neighbour) < 2))
        {
            if (!swarm_dictionary.ContainsKey(left_neighbour))
            {
                tm.Add_Tile(left_neighbour, spawner_tile);
                has_spread = true;
            }
        }

        Vector3Int right_neighbour = i_pos + Vector3Int.right;
        if (((spawner_tilemap.GetTile(right_neighbour) != null) && (!has_spread)) && (Count_Swarm_Neighbours(right_neighbour) < 2))
        {
            if (!swarm_dictionary.ContainsKey(right_neighbour))
            {
                tm.Add_Tile(right_neighbour, spawner_tile);
                has_spread = true;
            }
        }
    }

    private int Count_Swarm_Neighbours(Vector3Int i_pos)
    {
        int swarm_count = 0;

        Vector3Int up_neighbour = i_pos + Vector3Int.up;
        if (swarm_dictionary.ContainsKey(up_neighbour))
        {
            swarm_count++;
        }

        Vector3Int down_neighbour = i_pos + Vector3Int.down;
        if (swarm_dictionary.ContainsKey(down_neighbour))
        {
            swarm_count++;
        }

        Vector3Int left_neighbour = i_pos + Vector3Int.left;
        if (swarm_dictionary.ContainsKey(left_neighbour))
        {
            swarm_count++;
        }

        Vector3Int right_neighbour = i_pos + Vector3Int.right;
        if (swarm_dictionary.ContainsKey(right_neighbour))
        {
            swarm_count++;
        }

        return swarm_count;
    }
}
