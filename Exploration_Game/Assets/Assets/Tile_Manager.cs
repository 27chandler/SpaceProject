using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class Tile_Manager : MonoBehaviour
{
    [Serializable]
    public struct Tile_Info
    {
        public TileBase tile;
        public Tilemap tilemap;
        public List<System_Layer> system_data;
    }

    [Serializable]
    public struct System_Layer
    {
        public string layer_name;
        public Tile_System system;
    }

    [Flags]
    public enum DIRECTION { NULL = 0, UP = 1, RIGHT = 2, DOWN = 4, LEFT = 8, OVER = 16 };

    public struct Internal_Tile_Properties
    {
        public TileBase base_tile;
        public TileBase alternate_tile;

        public int energy_level;
        public DIRECTION transfer_direction;
        public DIRECTION energy_origin;

        public bool has_changed_this_step;
    }

    [SerializeField] private List<Tile_Info> tile_layer_data = new List<Tile_Info>();
    [SerializeField] private List<Tile_System> tile_systems = new List<Tile_System>();
    [Space]
    [SerializeField] private List<Tile_Info> ship_tile_layer_data = new List<Tile_Info>();
    [SerializeField] private List<Tile_System> ship_tile_systems = new List<Tile_System>();

    [Serializable]
    public struct Tile_States
    {
        public TileBase activate_tile;
        public TileBase inactive_tile;
    }

    [SerializeField] public Tile_States standard_door;

    void OnEnable()
    {
        Add_Tile_Types_To_Systems();
        Add_World_Tiles_To_Systems();
    }

    public void Init_Ship_Systems()
    {
        foreach (var sys in ship_tile_systems)
        {
            sys.enabled = true;
        }

        Add_Ship_Tile_Types_To_Systems();
        Add_Ship_World_Tiles_To_Systems();
    }

    private void Add_Tile_Types_To_Systems()
    {
        foreach (var tile in tile_layer_data)
        {
            foreach (var sys in tile.system_data)
            {
                sys.system.Add_Tile_To_System(sys.layer_name, tile.tile);
            }
        }
    }

    private void Add_World_Tiles_To_Systems()
    {
        foreach (var tile in tile_layer_data)
        {
            //foreach (var sys in tile.system_data)
            //{
                foreach (var tile_pos in tile.tilemap.cellBounds.allPositionsWithin)
                {
                    if (tile.tilemap.GetTile(tile_pos) == tile.tile)
                    {
                        Add_Tile(tile_pos, tile.tile);
                    }
                }
            //}
        }
    }

    private void Add_Ship_Tile_Types_To_Systems()
    {
        foreach (var tile in ship_tile_layer_data)
        {
            foreach (var sys in tile.system_data)
            {
                sys.system.Add_Tile_To_System(sys.layer_name, tile.tile);
            }
        }
    }

    private void Add_Ship_World_Tiles_To_Systems()
    {
        foreach (var tile in ship_tile_layer_data)
        {
            //foreach (var sys in tile.system_data)
            //{
                foreach (var tile_pos in tile.tilemap.cellBounds.allPositionsWithin)
                {
                    if (tile.tilemap.GetTile(tile_pos) == tile.tile)
                    {
                        Ship_Add_Tile(tile_pos, tile.tile);
                    }
                }
            //}
        }
    }


    public Tilemap Grab_Layer(TileBase i_tile)
    {
        foreach (var info in tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                return info.tilemap;
            }
        }

        return null;
    }

    public bool Check_Layer_Name(TileBase i_tile,string i_layername)
    {
        foreach (var info in tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                if (!(info.system_data.FindIndex(search_string => search_string.layer_name == i_layername) < 0))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Tile_System Grab_Systems(TileBase i_tile)
    {
        foreach (var info in tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                return info.system_data[0].system;
            }
        }

        return null;
    }

    public Tilemap Grab_Ship_Layer(TileBase i_tile)
    {
        foreach (var info in ship_tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                return info.tilemap;
            }
        }

        return null;
    }

    public Tile_System Grab_Ship_Systems(TileBase i_tile)
    {
        foreach (var info in ship_tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                return info.system_data[0].system;
            }
        }

        return null;
    }

    public void Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        Tilemap target_tilemap = new Tilemap();

        foreach (var info in tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                target_tilemap = info.tilemap;

                foreach (var sys in info.system_data)
                {
                    sys.system.Add_Tile(i_pos, i_tile);
                }
            }
        }

        if (target_tilemap != null)
        {
            target_tilemap.SetTile(i_pos, i_tile);
        }
        else
        {
            Debug.LogWarning("Tilemap not found");
        }

    }

    public void Remove_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        Tilemap target_tilemap = new Tilemap();

        foreach (var info in tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                target_tilemap = info.tilemap;

                foreach (var sys in info.system_data)
                {
                    sys.system.Remove_Tile(i_pos,i_tile);
                }
            }
        }

        if (target_tilemap != null)
        {
            target_tilemap.SetTile(i_pos, null);
        }
        else
        {
            Debug.LogWarning("Tilemap not found");
        }
    }

    public void Ship_Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        Tilemap target_tilemap = new Tilemap();

        foreach (var info in ship_tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                target_tilemap = info.tilemap;

                foreach (var sys in info.system_data)
                {
                    sys.system.Add_Tile(i_pos, i_tile);
                }
            }
        }

        if (target_tilemap != null)
        {
            target_tilemap.SetTile(i_pos, i_tile);
        }
        else
        {
            Debug.LogWarning("Tilemap not found");
        }

    }

    public void Ship_Remove_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        Tilemap target_tilemap = new Tilemap();

        foreach (var info in ship_tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                target_tilemap = info.tilemap;

                foreach (var sys in info.system_data)
                {
                    sys.system.Remove_Tile(i_pos, i_tile);
                }
            }
        }

        if (target_tilemap != null)
        {
            target_tilemap.SetTile(i_pos, null);
        }
        else
        {
            Debug.LogWarning("Tilemap not found");
        }
    }


}
