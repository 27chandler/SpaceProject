using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_Text : MonoBehaviour
{
    private Text debug_text;
    [SerializeField] private Camera cam;
    [SerializeField] private Energy_System system;
    // Start is called before the first frame update
    void Start()
    {
        debug_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse_pos = Input.mousePosition;

        mouse_pos = cam.ScreenToWorldPoint(mouse_pos);
        Vector3Int rounded_mouse_pos = new Vector3Int();
        rounded_mouse_pos.x = Mathf.FloorToInt(mouse_pos.x);
        rounded_mouse_pos.y = Mathf.FloorToInt(mouse_pos.y);
        rounded_mouse_pos.z = 0;

        Energy_System.Energy_Tile temp_tile = system.Grab_Data(rounded_mouse_pos);

        if (temp_tile != null)
        {
            debug_text.text = "";
            debug_text.text += "Position: " + rounded_mouse_pos + "\n";
            debug_text.text += "Energy Level: " + temp_tile.energy_level + "\n";
            debug_text.text += "Energy Direction: " + temp_tile.transfer_direction + "\n";
            debug_text.text += "Energy Origin: " + temp_tile.energy_origin + "\n";
        }
        else
        {
            debug_text.text = "ERROR";
        }
    }
}
