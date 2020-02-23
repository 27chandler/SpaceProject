using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ship_Wheel : Energy_Receptor
{
    private Tile_Manager tm;
    private List<Transform> player_transforms = new List<Transform>();
    private Ship_System ship_sys;
    private Grid ship_grid;

    public Ship_Wheel(Vector3Int i_tile_pos,Tilemap i_tilemap) : base(i_tile_pos)
    {
        tm = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile_Manager>();
        activation_cost = 5;

        ship_sys = (Ship_System)(tm.Get_Systems(1));
        ship_grid = i_tilemap.GetComponentInParent<Grid>();

        if (ship_sys == null)
        {
            Debug.LogError("System not found in SHIP_WHEEL");
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var obj in players)
        {
            player_transforms.Add(obj.transform);
        }

        // Listeners
        Input_Manager.static_input["Use Wheel"].press_actions += Use_Wheel;
        Input_Manager.static_input["Toggle Control"].press_actions += Toggle_Manual_Control;
        //
    }

    private void Use_Wheel()
    {
        foreach (var player in player_transforms)
        {
            if (Vector2.Distance(ship_grid.GetCellCenterWorld(position), player.position) < 1.0f)
            {
                Activate(activation_cost);
            }
        }
    }

    private void Toggle_Manual_Control()
    {
        foreach (var player in player_transforms)
        {
            if (Vector2.Distance(ship_grid.GetCellCenterWorld(position), player.position) < 1.0f)
            {
                ship_sys.Toggle_Player_Movement();
            }
        }
    }

    public override void Destroy()
    {
        Input_Manager.static_input["Use Wheel"].press_actions -= Use_Wheel;
        Input_Manager.static_input["Toggle Control"].press_actions -= Toggle_Manual_Control;
    }

    public override void Tile_Update()
    {
        base.Tile_Update();
    }

    protected override void Activation_Behaviour()
    {

        ship_sys.is_ship_control_activated = !ship_sys.is_ship_control_activated;
        base.Activation_Behaviour();
    }
}
