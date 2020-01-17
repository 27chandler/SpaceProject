using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Energy_System : Tile_System
{
    private Tile_Manager tm;

    [Flags]
    public enum DIRECTION { NULL = 0,UP = 1,RIGHT = 2,DOWN = 4,LEFT = 8, OVER = 16};

    [SerializeField] private List<Color> intensity_colors = new List<Color>();

    public class Energy_Tile
    {
        public int energy_level;
        public DIRECTION transfer_direction;
        public DIRECTION energy_origin;

        public bool has_changed_this_step;
    }

    [Space]
    [SerializeField] private float energy_burst_interval;
    [SerializeField] private float energy_step_interval;
    [SerializeField] private float receptor_step_interval;

    [SerializeField] private TileBase closed_door_tile;

    private float burst_timer, step_timer, receptor_timer;

    private Dictionary<Vector3Int, Energy_Tile> energy_dictionary = new Dictionary<Vector3Int, Energy_Tile>();
    private Dictionary<Vector3Int, Energy_Tile> energy_buffer = new Dictionary<Vector3Int, Energy_Tile>();
    private Dictionary<Vector3Int, Energy_Tile> energy_subtraction_buffer = new Dictionary<Vector3Int, Energy_Tile>();
    private Dictionary<Vector3Int, int> generator_dictionary = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, Energy_Receptor> receptor_dictionary = new Dictionary<Vector3Int, Energy_Receptor>();

    // Start is called before the first frame update
    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
    }

    public Energy_Tile Grab_Data(Vector3Int i_pos)
    {
        if (energy_dictionary.ContainsKey(i_pos))
        {
            return energy_dictionary[i_pos];
        }
        return null;
    }

    public override void Add_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        bool is_valid = false;

        if (tm == null)
        {
            tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
        }

        if (Check_Tiletype(system_tiles["Transmitters"], i_tile))
        {
            energy_dictionary.Add(i_pos, Create_Energy_Tile());
            is_valid = true;
        }
        else if (Check_Tiletype(system_tiles["Generators"], i_tile))
        {
            generator_dictionary.Add(i_pos, 1);
            is_valid = true;
        }
        else if (Check_Tiletype(system_tiles["Receptors"], i_tile))
        {
            if (is_ship_mode)
            {
                receptor_dictionary.Add(i_pos, new Door(i_pos, tm.Grab_Ship_Layer(i_tile)));
            }
            else
            {
                receptor_dictionary.Add(i_pos, new Door(i_pos, tm.Grab_Layer(i_tile)));
            }
            is_valid = true;
        }

        
        if (is_valid)
        {
            base.Add_Tile(i_pos, i_tile);
        }
        else
        {
            Debug.LogWarning("Invalid tile addition at:" + i_pos + " with " + i_tile);
        }
    }

    public override void Remove_Tile(Vector3Int i_pos, TileBase i_tile)
    {
        if (tm == null)
        {
            tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
        }

        if (Check_Tiletype(system_tiles["Transmitters"], i_tile))
        {
            if (energy_dictionary.ContainsKey(i_pos))
            {
                energy_dictionary.Remove(i_pos);
            }
            else
            {
                Debug.LogWarning("Invalid tile removal at:" + i_pos + " with " + i_tile);
            }
        }
        else if (Check_Tiletype(system_tiles["Generators"], i_tile))
        {
            if (generator_dictionary.ContainsKey(i_pos))
            {
                generator_dictionary.Remove(i_pos);
            }
            else
            {
                Debug.LogWarning("Invalid tile removal at:" + i_pos + " with " + i_tile);
            }
        }
        else if (Check_Tiletype(system_tiles["Receptors"], i_tile))
        {
            if (receptor_dictionary.ContainsKey(i_pos))
            {
                receptor_dictionary.Remove(i_pos);
            }
            else
            {
                Debug.LogWarning("Invalid tile removal at:" + i_pos + " with " + i_tile);
            }
        }
    }

    private Energy_Tile Create_Energy_Tile()
    {
        Energy_Tile temp_energy = new Energy_Tile();
        temp_energy.energy_level = 0;
        temp_energy.energy_origin = DIRECTION.NULL;
        temp_energy.transfer_direction = DIRECTION.NULL;
        temp_energy.has_changed_this_step = false;

        return temp_energy;
    }

    // Update is called once per frame
    void Update()
    {
        burst_timer += Time.deltaTime;
        step_timer += Time.deltaTime;
        receptor_timer += Time.deltaTime;

        int total_energy_count = 0;
        Tilemap colour_set_tilemap;
        if (is_ship_mode)
        {
            colour_set_tilemap = tm.Grab_Ship_Layer(system_tiles["Transmitters"][0]);
        }
        else
        {
            colour_set_tilemap = tm.Grab_Layer(system_tiles["Transmitters"][0]);
        }

        foreach (var wire in energy_dictionary)
        {
            colour_set_tilemap.SetColor(wire.Key, intensity_colors[wire.Value.energy_level]);
            total_energy_count += wire.Value.energy_level;
        }

        //Debug.Log(total_energy_count);

        if (burst_timer >= energy_burst_interval)
        {
            Generator_Simulate();
            burst_timer = 0.0f;
        }

        if (step_timer >= energy_step_interval)
        {
            Energy_Simulate();
            Merge_Buffers();
            step_timer = 0.0f;
        }

        if (receptor_timer >= receptor_step_interval)
        {
            Receptor_Update();
            receptor_timer = 0.0f;
        }
    }

    private void Receptor_Update()
    {
        foreach (var receptor in receptor_dictionary)
        {
            receptor.Value.Tile_Update();
        }
    }

    private void Energy_Simulate()
    {
        foreach (var wire in energy_dictionary)
        {
            if (!wire.Value.has_changed_this_step)
            {
                if (wire.Value.energy_level > 0)
                {
                    Check_Valid_Directions(wire.Key, wire.Value);
                }
            }
            else
            {
                wire.Value.has_changed_this_step = false;
            }
        }
    }

    private DIRECTION Check_Valid_Directions(Vector3Int i_pos, Energy_Tile i_tile)
    {
        DIRECTION return_dir = new DIRECTION();

        if (energy_dictionary.ContainsKey(i_pos))
        {
            if (energy_dictionary.ContainsKey(i_pos + Vector3Int.up))
            {
                return_dir |= DIRECTION.UP;
            }
            if (energy_dictionary.ContainsKey(i_pos + Vector3Int.right))
            {
                return_dir |= DIRECTION.RIGHT;
            }
            if (energy_dictionary.ContainsKey(i_pos - Vector3Int.up))
            {
                return_dir |= DIRECTION.DOWN;
            }
            if (energy_dictionary.ContainsKey(i_pos - Vector3Int.right))
            {
                return_dir |= DIRECTION.LEFT;
            }

            DIRECTION change_dir = i_tile.transfer_direction;

            if (change_dir == DIRECTION.NULL)
            {
                change_dir = DIRECTION.UP;
            }

            int counter = 0;
            bool has_completed = false;

            while (counter < 4)
            {
                change_dir = (DIRECTION)((int)(change_dir) << 1);

                if (change_dir == DIRECTION.OVER)
                {
                    change_dir = DIRECTION.UP;
                }

                if (((change_dir & return_dir) == change_dir) && (((change_dir & i_tile.energy_origin) != i_tile.energy_origin) || (i_tile.energy_origin == DIRECTION.NULL)))
                {
                    //Debug.Log(change_dir);
                    i_tile.transfer_direction = change_dir;
                    has_completed = true;
                    counter = 4;
                }
                counter++;
            }

            if (has_completed)
            {
                // do transfer
                Transfer_Energy(i_pos, i_tile.energy_level, change_dir);
            }
            else
            {
                Transfer_Energy(i_pos, i_tile.energy_level, DIRECTION.OVER);

                // Send energy to the receptor or dissipate it
                
                if (!Send_To_Receptor(i_pos, i_tile.energy_level))
                {
                    Debug.Log("Energy Dissipated: " + i_tile.energy_level);
                }
            }

            //if (change_dir )
        }

        //Debug.Log(energy_dictionary[new Vector3Int(3, 0, 0)].transfer_direction);

        return return_dir;
    }

    private bool Send_To_Receptor(Vector3Int i_pos, int i_energy)
    {
        if (receptor_dictionary.ContainsKey(i_pos))
        {
            receptor_dictionary[i_pos].Input_Energy(i_energy);
            return true;
        }
        return false;
    }

    private void Transfer_Energy(Vector3Int i_pos,int i_amount,DIRECTION i_dir)
    {
        //Debug.Log(i_dir);
        Vector3Int transfer_position = new Vector3Int();
        if (i_dir == DIRECTION.UP)
        {
            transfer_position = i_pos + Vector3Int.up;
            Initiate_Transfer(energy_buffer,transfer_position, DIRECTION.DOWN, i_dir, i_amount, true);
        }
        if (i_dir == DIRECTION.RIGHT)
        {
            transfer_position = i_pos + Vector3Int.right;
            Initiate_Transfer(energy_buffer,transfer_position, DIRECTION.LEFT, i_dir, i_amount, true);
        }
        if (i_dir == DIRECTION.DOWN)
        {
            transfer_position = i_pos - Vector3Int.up;
            Initiate_Transfer(energy_buffer,transfer_position, DIRECTION.UP, i_dir, i_amount, true);
        }
        if (i_dir == DIRECTION.LEFT)
        {
            transfer_position = i_pos - Vector3Int.right;
            Initiate_Transfer(energy_buffer,transfer_position, DIRECTION.RIGHT, i_dir, i_amount,true);
        }

        //Debug.Log(i_dir);

        if (i_dir != DIRECTION.NULL) 
        {
            Initiate_Transfer(energy_subtraction_buffer,i_pos, energy_dictionary[i_pos].energy_origin, energy_dictionary[i_pos].transfer_direction,i_amount,false);
        }
    }

    private void Initiate_Transfer(Dictionary<Vector3Int, Energy_Tile> i_buffer, Vector3Int i_transfer_pos, DIRECTION i_origin_dir, DIRECTION i_transfer_direction, int i_delta,bool is_cumulative)
    {
        //Debug.Log("TRANSFER");
        Energy_Tile new_tile = new Energy_Tile();
        if (is_cumulative)
        {
            new_tile.energy_level = energy_dictionary[i_transfer_pos].energy_level + i_delta;
        }
        else
        {
            new_tile.energy_level = i_delta;
        }

        new_tile.energy_origin = i_origin_dir;
        new_tile.has_changed_this_step = false;
        new_tile.transfer_direction = energy_dictionary[i_transfer_pos].transfer_direction;

        if (i_buffer.ContainsKey(i_transfer_pos))
        {
            i_buffer[i_transfer_pos].energy_level += i_delta;
            i_buffer[i_transfer_pos].transfer_direction = energy_dictionary[i_transfer_pos].transfer_direction;
            i_buffer[i_transfer_pos].energy_origin = i_origin_dir;
        }
        else
        {
            i_buffer.Add(i_transfer_pos, new_tile);
        }
    }

    private void Merge_Buffers()
    {
        foreach (var energy_tile in energy_buffer)
        {
            if (energy_dictionary.ContainsKey(energy_tile.Key))
            {
                energy_dictionary.Remove(energy_tile.Key);
            }

            energy_dictionary.Add(energy_tile.Key, energy_tile.Value);
        }

        energy_buffer.Clear();


        foreach (var energy_tile in energy_subtraction_buffer)
        {
            if (energy_dictionary.ContainsKey(energy_tile.Key))
            {
                energy_dictionary[energy_tile.Key].energy_level -= energy_tile.Value.energy_level;
            }
        }

        energy_subtraction_buffer.Clear();
    }

    private void Generator_Simulate()
    {
        Vector3 offset = (0.5f * Vector3.right) + (0.5f * Vector3.up);
        foreach (var generator in generator_dictionary)
        {
            // If the generator has a wire under it
            if (energy_dictionary.ContainsKey(generator.Key))
            {
                Energy_Tile new_tile = new Energy_Tile();
                new_tile.energy_level = energy_dictionary[generator.Key].energy_level + 1;
                new_tile.energy_origin = DIRECTION.NULL;
                new_tile.has_changed_this_step = false;

                if (energy_buffer.ContainsKey(generator.Key))
                {
                    energy_buffer[generator.Key] = new_tile;
                }
                else
                {
                    energy_buffer.Add(generator.Key, new_tile);
                }
            }
        }
    }
}
