using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy_Receptor
{
    public Energy_System sys;

    protected int activation_cost = 2;

    protected int energy_level = 0;
    private int max_energy_level = 20;
    protected Vector3Int position;

    public Energy_Receptor(Vector3Int i_pos)
    {
        position = i_pos;
    }

    public void Input_Energy(int i_value)
    {
        energy_level += i_value;

        if (energy_level > max_energy_level)
        {
            energy_level = max_energy_level;
            Debug.Log("Energy input of " + i_value + " exceeded maxmimum");
        }
        else
        {
            Debug.Log("Energy input of " + i_value + " inputted");
            Debug.Log("Current energy level at " + energy_level);
        }
    }

    public void Activate(int i_cost)
    {
        if (i_cost <= energy_level)
        {
            energy_level -= i_cost;
            Activation_Behaviour();
        }
    }

    public virtual void Destroy()
    {
        
    }

    public virtual void Tile_Update()
    {

    }

    protected virtual void Activation_Behaviour()
    {
        
    }
}
