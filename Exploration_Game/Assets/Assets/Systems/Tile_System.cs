using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile_System : MonoBehaviour
{
    private Tilemap target_tilemap;
    [SerializeField] public bool is_ship_mode;

    protected Dictionary<string,List<TileBase>> system_tiles = new Dictionary<string, List<TileBase>>();

    public void Add_Tile_To_System(string i_layer, TileBase i_tile)
    {
        if (!system_tiles.ContainsKey(i_layer))
        {
            system_tiles.Add(i_layer, new List<TileBase>());
        }
        system_tiles[i_layer].Add(i_tile);
    }

    public virtual void Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
    }

    protected void Set_Target_Tilemap(Tilemap i_tilemap)
    {
        target_tilemap = i_tilemap;
    }

    public virtual void Remove_Tile(Vector3Int i_pos, TileBase i_tile)
    {
    }

    protected bool Check_Tiletype(List<TileBase> i_list, TileBase i_tile)
    {
        foreach (var tile in i_list)
        {
            if (tile == i_tile)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (Tile_Manager.is_inited && Input_Manager.is_inited)
        {
            System_Update();
        }
    }

    protected virtual void System_Update()
    {

    }
}
