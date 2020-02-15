using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_Manager : MonoBehaviour
{
    public static Input_Manager current;

    public static bool is_inited = false;

    //public event System.Action On_Forward_Pressed;
    //public event System.Action On_Backward_Pressed;
    //public event System.Action On_Left_Pressed;
    //public event System.Action On_Right_Pressed;

    //[SerializeField] private KeyCode forward_key;
    //[SerializeField] private KeyCode backward_key;
    //[SerializeField] private KeyCode left_key;
    //[SerializeField] private KeyCode right_key;

    [System.Serializable]
    public class Keys
    {
        public string name;
        public KeyCode key;
        public bool is_pressed;

        public event System.Action actions;
        public event System.Action press_actions;
        public event System.Action unpress_actions;

        public void Trigger()
        {
            if (actions != null)
            {
                actions();
            }
        }

        public void Press_Trigger()
        {
            if (press_actions != null)
            {
                press_actions();
            }
        }

        public void Unpress_Trigger()
        {
            if (unpress_actions != null)
            {
                unpress_actions();
            }
        }
    }

    [SerializeField] private List<Keys> input_list = new List<Keys>();

    [SerializeField] public static Dictionary<string,Keys> static_input = new Dictionary<string, Keys>();


    // Start is called before the first frame update
    void Awake()
    {
        current = this;

        foreach (var input in input_list)
        {
            static_input.Add(input.name, input);
        }
        is_inited = true;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var input in static_input)
        {
            if (Input.GetKey(input.Value.key))
            {
                input.Value.Trigger();
            }
            if (Input.GetKeyDown(input.Value.key))
            {
                input.Value.Press_Trigger();
            }
            if (Input.GetKeyUp(input.Value.key))
            {
                input.Value.Unpress_Trigger();
            }
        }
    }
}
