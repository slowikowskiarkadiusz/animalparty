using System;
using UnityEditor;
using UnityEngine;

public enum FieldEventType
{
    CoinGivingEvent,
    VendorFieldEvent,
}

[Serializable]
public class FieldEventDataSet
{
    public FieldEventType type;
    public CoinGivingEventDataSet CoinGivingEventDataSet;
    public VendorFieldEventDataSet VendorFieldEventDataSet;
}

[Serializable]
public class CoinGivingEventDataSet
{
    public GameObject CoinPrefab;
    public Transform CoinGiverActor;
}

[Serializable]
public class VendorFieldEventDataSet
{
    public Transform ShopActor;
}

[CustomPropertyDrawer(typeof(FieldEventDataSet))]
public class FieldEventDataSet_PropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position.height = EditorGUIUtility.singleLineHeight;

        var type = property.FindPropertyRelative("type");
        EditorGUI.PropertyField(position, type);
        var typeValue = (FieldEventType)type.enumValueIndex;

        position.y += position.height + 2;

        EditorGUI.PropertyField(position, property.FindPropertyRelative($"{typeValue}DataSet"), true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight + 2;
        var type = property.FindPropertyRelative("type");
        var typeValue = (FieldEventType)type.enumValueIndex;

        return height + EditorGUI.GetPropertyHeight(property.FindPropertyRelative($"{typeValue}DataSet"), true);
    }
}