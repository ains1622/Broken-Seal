using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    WeaponData weaponData;
    string[] weaponSubTypes;
    int selectedWeaponSubtype;

    void OnEnable()
    {
        // Cache the weapon data value.
        weaponData = (WeaponData)target;

        // Retrieve all the weapon subtypes and cache it.
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        // Add a None option in front.
        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubTypes = subTypesString.ToArray();

        // Ensure that we are using the correct weapon subtype.
        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubTypes, weaponData.behaviour));
    }

    public override void OnInspectorGUI()
    {
        // Draw a dropdown in the Inspector.
        selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubTypes);

        if (selectedWeaponSubtype > 0)
        {
            // Updates the behaviour field.
            weaponData.behaviour = weaponSubTypes[selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(weaponData); // Marks the object to save
            DrawDefaultInspector(); // Draw the default inspector elements
        }
    }
}
