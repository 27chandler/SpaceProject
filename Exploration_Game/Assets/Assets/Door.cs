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

    private TileBase current_tile;
    private TileBase open_door;
    private TileBase closed_door;
    private Tilemap target_tilemap;
    private Grid tile_grid;

    public Door(Vector3Int i_tile_pos,TileBase i_tiletype,Tilemap i_tilemap) : base(i_tile_pos)
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();

        target_tilemap = i_tilemap;
        //open_door = tm.standard_door.inactive_tile;
        closed_door = i_tiletype;

        if (is_open)
        {
            //current_tile = open_door;
            target_tilemap.SetColor(position, new Color(1.0f, 1.0f, 1.0f, 0.3f));
        }
        else
        {
            //current_tile = closed_door;
            target_tilemap.SetColor(position, Color.white);
        }

        target_tilemap.SetTile(position, current_tile);

        tile_grid = target_tilemap.GetComponentInParent<Grid>();
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

        if (target_tilemap.GetTile(position) != current_tile)
        {
            if (is_open)
            {
                //current_tile = open_door;
                target_tilemap.SetColor(position, new Color(1.0f, 1.0f, 1.0f, 0.3f));
            }
            else
            {
                target_tilemap.SetColor(position, Color.white);
                current_tile = closed_door;
            }

            target_tilemap.SetTile(position, current_tile);
        }

        if (is_open)
        {

            if (Vector3.Distance(player_transform.position, tile_grid.CellToWorld(position)) > activation_distance)
            {
                Activate(activation_cost);
            }
        }
        else
        {
            if (Vector3.Distance(player_transform.position, tile_grid.CellToWorld(position)) <= activation_distance)
            {
                Activate(activation_cost);
            }
        }

        base.Tile_Update();
    }

    protected override void Activation_Behaviour()
    {
        is_open = !is_open;

        target_tilemap.SetTileFlags(position, TileFlags.None);

        if (is_open)
        {
            //current_tile = open_door;
            target_tilemap.SetColor(position, new Color(0.0f, 0.0f, 1.0f, 0.3f));
            target_tilemap.SetColliderType(position, Tile.ColliderType.None);
        }
        else
        {
            target_tilemap.SetColliderType(position, Tile.ColliderType.Grid);
            target_tilemap.SetColor(position, Color.white);
            current_tile = closed_door;
        }

        target_tilemap.SetTile(position, current_tile);

        Debug.Log("Door is now: " + is_open);
        base.Activation_Behaviour();
    }
}
