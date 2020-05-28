using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Crafting_System : MonoBehaviour
{
    [Serializable]
    public struct Material
    {
        public string name;
        public float conductivity;
        public float hardness;
    }

    [SerializeField] private List<Material> material_types = new List<Material>();

    [SerializeField] private List<string> crafting_slot = new List<string>();
    [SerializeField] private bool do_craft = false;

    [Space]

    [SerializeField] private Material output_material;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (do_craft)
        {
            do_craft = false;
            Craft_Item();
        }
    }

    private void Craft_Item()
    {
        output_material = new Material();

        float conductivity_average = 0.0f;

        foreach (var mat in crafting_slot)
        {
            Material next_mat = Grab_Material(mat);
            Calculate_Hardness(next_mat.hardness);

            conductivity_average += next_mat.conductivity;
        }
        conductivity_average /= crafting_slot.Count;
        output_material.conductivity = conductivity_average;
    }

    private void Calculate_Hardness(float i_hardness)
    {
        // Addition of total hardness values
        output_material.hardness += i_hardness;
    }

    private Material Grab_Material(string i_name)
    {
        foreach (var mat_type in material_types)
        {
            if (mat_type.name == i_name)
            {
                return mat_type;
            }
        }

        Debug.LogError("Invalid Material name in crafting");
        return new Material();
    }
}
