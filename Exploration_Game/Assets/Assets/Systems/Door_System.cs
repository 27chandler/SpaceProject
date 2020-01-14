using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Door_System : Tile_System
{
    private Tile_Manager tm;

    [SerializeField] private Transform player_transform;
    [SerializeField] private List<TileBase> open_doors = new List<TileBase>();
    [SerializeField] private List<TileBase> closed_doors = new List<TileBase>();
    [SerializeField] private Tilemap wall_tilemap;

    [Serializable]
    public struct Door_Tile
    {
        public float activation_distance;
        public TileBase open_tile;
        public TileBase closed_tile;
        public bool is_open;
        public Energy_Receptor receptor;
    }

    private Dictionary<Vector3Int, Door_Tile> door_dictionary = new Dictionary<Vector3Int, Door_Tile>();


    // Start is called before the first frame update
    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();

        foreach (var tile_pos in wall_tilemap.cellBounds.allPositionsWithin)
        {
            if (Check_Tiletype(open_doors, wall_tilemap.GetTile(tile_pos)))
            {
                door_dictionary.Add(tile_pos, Create_Door_Tile(0,tile_pos));
                tm.Add_Tile(tile_pos, wall_tilemap.GetTile(tile_pos));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Door_Tile Create_Door_Tile(int i_type_index,Vector3Int i_pos)
    {
        Door_Tile temp_door = new Door_Tile();
        temp_door.activation_distance = 1.5f;
        temp_door.closed_tile = closed_doors[i_type_index];
        temp_door.open_tile = open_doors[i_type_index];
        temp_door.receptor = new Energy_Receptor(i_pos);
        temp_door.is_open = false;

        return temp_door;
    }

    public override void Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        base.Add_Tile(i_pos, i_tile);
    }

    public override void Remove_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        base.Remove_Tile(i_pos, i_tile);
    }
}
