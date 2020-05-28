using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class Tile_Manager : MonoBehaviour
{
    // Enum for types of systems present in the game
    private enum SYSTEM_ID { ENERGY = 0, SHIP = 1, SWARM = 2, LIQUID = 3 };

    // Stores properties for a tiletype
    [Serializable]
    public struct PropertyData
    {
        public float conductivity; // Conducitivity measures if a tile can carry energy to other conductive tiles
        public float hardness;
        public float openility;
        public float power_generation;
        public float thrust;
        public float ship_blocker;
        public float spread_value;
    };


    private Dictionary<Vector3Int, PropertyData> all_tiles = new Dictionary<Vector3Int, PropertyData>();

    [SerializeField] private Tilemap test_tilemap;

    public static bool is_inited = false;

    [Serializable]
    public struct Tile_Info
    {
        public TileBase tile;
        public Tilemap tilemap;
        public PropertyData property_data;
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

    void Init()
    {
        Populate_World_Dictionary();

        Add_Tile_Types_To_Systems();
        Add_World_Tiles_To_Systems();

        foreach (var sys in tile_systems)
        {
            sys.enabled = true;
        }
    }

    public List<TileBase> Grab_All_Tiletypes()
    {
        List<TileBase> return_tiles = new List<TileBase>();

        foreach (var tile_data in tile_layer_data)
        {
            return_tiles.Add(tile_data.tile);
        }
        return return_tiles;
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

    private void Populate_World_Dictionary()
    {
        foreach (var tile_pos in test_tilemap.cellBounds.allPositionsWithin)
        {
            all_tiles.Add(tile_pos, Grab_Property_Data(test_tilemap.GetTile(tile_pos)));
        }
    }

    // Retrieves the property data for a specific tile type
    private PropertyData Grab_Property_Data(TileBase i_tile)
    {
        PropertyData return_data = new PropertyData();

        if (i_tile == null)
        {
            return return_data;
        }

        foreach (var tile in tile_layer_data)
        {
            if (tile.tile == i_tile)
            {
                return tile.property_data;
            }
        }

        Debug.LogError("Invalid tile in property data");
        return return_data;
    }

    private void Add_Tile_Types_To_Systems()
    {
        foreach (var tile in tile_layer_data)
        {
            if (tile.property_data.conductivity >= 1.0f) // Adds tiles that can conduct energy to the energy system
            {
                tile_systems[(int)(SYSTEM_ID.ENERGY)].Add_Tile_To_System("Transmitters", tile.tile);
            }
            if (tile.property_data.power_generation >= 1.0f) // Adds tiles that generate power to the energy system
            {
                tile_systems[(int)(SYSTEM_ID.ENERGY)].Add_Tile_To_System("Generators", tile.tile);
            }
            if (tile.property_data.openility >= 1.0f) // Adds tiles that can be opened as doors to the energy system
            {
                tile_systems[(int)(SYSTEM_ID.ENERGY)].Add_Tile_To_System("Receptors", tile.tile);
            }
            if (tile.property_data.spread_value > 0.0f) // Adds tiles that can be opened as doors to the energy system
            {
                tile_systems[(int)(SYSTEM_ID.SWARM)].Add_Tile_To_System("Spawner", tile.tile);
            }

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



                    //
                    //if (all_tiles[tile_pos].conductivity >= 1.0f)
                    //{
                    //    Debug.Log("Added tile to energy");
                    //    tile_systems[0].Add_Tile(tile_pos, tile.tilemap.GetTile(tile_pos));
                    //}
                    //
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

    public Tile_System Get_Systems(int i_index)
    {
        if (i_index < tile_systems.Count)
        {
            return tile_systems[i_index];
        }
        else
        {
            return null;
        }
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

    private List<Tile_System> Grab_Relevant_Systems(TileBase i_tile)
    {
        List<Tile_System> return_systems = new List<Tile_System>();

        foreach (var sys in tile_systems)
        {
            if (sys.Check_System_Tiles(i_tile))
            {
                return_systems.Add(sys);
            }
        }

        return return_systems;
    }

    public void Add_Tile(Vector3Int i_pos, TileBase i_tile,Quaternion i_rotation)
    {
        Add_Tile(i_pos, i_tile);

        Tilemap target_tilemap = new Tilemap();

        foreach (var info in tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                target_tilemap = info.tilemap;
            }
        }

        if (target_tilemap != null)
        {
            TileBase tile = target_tilemap.GetTile(i_pos);
            target_tilemap.SetTransformMatrix(i_pos, Matrix4x4.TRS(Vector3.zero, i_rotation, Vector3.one));
            Debug.Log("Rotated");
        }
    }

    public void Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        Tilemap target_tilemap = new Tilemap();

        foreach (var info in tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                target_tilemap = info.tilemap;
                Remove_Tile(i_pos, target_tilemap.GetTile(i_pos));

                List<Tile_System> systems = Grab_Relevant_Systems(i_tile);

                foreach (var sys in systems)
                {
                    sys.Add_Tile(i_pos, i_tile);
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

    public void Ship_Add_Tile(Vector3Int i_pos, TileBase i_tile, Quaternion i_rotation)
    {
        Ship_Add_Tile(i_pos, i_tile);

        Tilemap target_tilemap = new Tilemap();

        foreach (var info in ship_tile_layer_data)
        {
            if (info.tile == i_tile)
            {
                target_tilemap = info.tilemap;
            }
        }

        if (target_tilemap != null)
        {
            TileBase tile = target_tilemap.GetTile(i_pos);
            target_tilemap.SetTransformMatrix(i_pos, Matrix4x4.TRS(Vector3.zero, i_rotation, Vector3.one));
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
                Ship_Remove_Tile(i_pos, target_tilemap.GetTile(i_pos));

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

    private void Update()
    {
        if (!is_inited && Input_Manager.is_inited)
        {
            is_inited = true;
            Init();
        }
    }


}
