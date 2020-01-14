using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Door : Energy_Receptor
{
    private Tile_Manager tm;
    private Transform player_transform;
    private float activation_distance = 1.5f;

    private bool is_open = false;

    [SerializeField]
    private TileBase open_door;
    private TileBase closed_door;
    private Tilemap target_tilemap;

    public Door(Vector3Int i_tile_pos,Tilemap i_tilemap) : base(i_tile_pos)
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();

        target_tilemap = i_tilemap;
        open_door = target_tilemap.GetTile(i_tile_pos);
        closed_door = tm.standard_door.activate_tile;

        if (is_open)
        {
            target_tilemap.SetTile(position, open_door);
        }
        else
        {
            target_tilemap.SetTile(position, closed_door);
        }
    }

    private void Get_Player()
    {
        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void Tile_Update()
    {
        if (player_transform == null)
        {
            Get_Player();
        }

        if (is_open)
        {
            if (Vector3.Distance(player_transform.position,position) > activation_distance)
            {
                Activate(activation_cost);
            }
        }
        else
        {
            if (Vector3.Distance(player_transform.position, position) <= activation_distance)
            {
                Activate(activation_cost);
            }
        }

        base.Tile_Update();
    }

    protected override void Activation_Behaviour()
    {
        is_open = !is_open;

        if (is_open)
        {
            target_tilemap.SetTile(position,open_door);
        }
        else
        {
            target_tilemap.SetTile(position, closed_door);
        }

        Debug.Log("Door is now: " + is_open);
        base.Activation_Behaviour();
    }
}
