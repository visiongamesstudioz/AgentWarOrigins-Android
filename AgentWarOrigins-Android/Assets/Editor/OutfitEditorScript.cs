using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CustomEditor(typeof(Outfit))]
public class OutfitEditorScript : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var outfit = target as Outfit;

        if (outfit)
        {
           // OutfitAbility outfitAbility = outfit.OutfitAbility;
            //if (outfitAbility == Outfit.OutfitAbility.LowDamage)
            //{
            //    outfit.DamageMultiplier = EditorGUILayout.FloatField("Damage Multiplier",outfit.DamageMultiplier);

            //}

            //else if (outfitAbility == Outfit.OutfitAbility.Revive)
            //{
            //    outfit.NoofReviveTimes = EditorGUILayout.FloatField("No of Revives", outfit.NoofReviveTimes);
            //    outfit.ReviveEnergyAmountFactor = EditorGUILayout.FloatField("Revive Energy Amount", outfit.ReviveEnergyAmountFactor);
            //}
            //EditorGUI.BeginChangeCheck();

            //if (outfit.NoofUpdrages > 0)
            //{
            //    SerializedProperty upgradeLockTypes = serializedObject.FindProperty("UpgradeLockTypes");
            //    for (int i = 0; i < upgradeLockTypes.arraySize; i++)
            //    {
            //        Debug.Log("no of enums" + upgradeLockTypes.arraySize );
            //        SerializedProperty upgradecost = upgradeLockTypes.GetArrayElementAtIndex(i);
            //        GUIContent upgradecostLabel = new GUIContent(string.Concat("UpgradeLockTypes", i + 1));
            //        EditorGUILayout.PropertyField(upgradecost, upgradecostLabel);
            //    }
            //}

            //EditorGUI.EndChangeCheck();
        }
        EditorUtility.SetDirty(target);



    }
}
