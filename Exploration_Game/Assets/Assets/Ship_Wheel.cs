using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship_Wheel : Energy_Receptor
{
    private Tile_Manager tm;

    public Ship_Wheel(Vector3Int i_tile_pos) : base(i_tile_pos)
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
        activation_cost = 5;
    }

    protected override void Activation_Behaviour()
    {

        base.Activation_Behaviour();
    }
}
