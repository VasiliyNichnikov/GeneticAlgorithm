using UnityEditor;
using UnityEngine;

namespace ShipLogic.Editor
{
    [CustomEditor(typeof(ShipCharacteristics))]
    public class ShipCharacteristicsEditor : UnityEditor.Editor
    {
        private SerializedProperty _minWeight;
        private SerializedProperty _maxWeight;

        private SerializedProperty _speedMovement;
        private SerializedProperty _rateOfFire;
        private SerializedProperty _visibilityRadius;
        private SerializedProperty _gunPower;
        private SerializedProperty _armor;

        private delegate (float minValue, float maxValue) GetRangeData(SerializedProperty property);

        public void OnEnable()
        {
            _minWeight = serializedObject.FindProperty("_minWeight");
            _maxWeight = serializedObject.FindProperty("_maxWeight");

            _speedMovement = serializedObject.FindProperty("_speedMovement");
            _visibilityRadius = serializedObject.FindProperty("_visibilityRadius");
            _rateOfFire = serializedObject.FindProperty("_rateOfFire");
            _gunPower = serializedObject.FindProperty("_gunPower");
            _armor = serializedObject.FindProperty("_armor");
        }

        public override void OnInspectorGUI()
        {
            _minWeight.floatValue = 0.0f;
            _maxWeight.floatValue = 1.0f;
            
            
            GUILayout.Label($"Минимальный вес корабля: {_minWeight.floatValue}.\nМаксимальный вес корабля: {_maxWeight.floatValue}.\nСвободное пространство: {1 - GetOccupiedSpace()}");
            DrawRangeData(_speedMovement, GetRangeForSelectedPropertyFloat, "Скорость движения корабля:");
            DrawRangeData(_rateOfFire, GetRangeForSelectedPropertyFloat, "Время перезарядки:");
            DrawRangeData(_visibilityRadius, GetRangeForSelectedPropertyFloat, "Видимость радиуса:");
            DrawRangeData(_gunPower, GetRangeForSelectedPropertyFloat, "Мощность оружия:");
            DrawRangeData(_armor, GetRangeForSelectedPropertyFloat, "Броня:");

            if (GUILayout.Button("Сохранить данные"))
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void DrawRangeData(SerializedProperty propertyFloat, GetRangeData rangeData, string label)
        {
            var minValue = rangeData.Invoke(propertyFloat).minValue;
            var maxValue = rangeData.Invoke(propertyFloat).maxValue;

            EditorGUILayout.Slider(propertyFloat, minValue, maxValue, label);
        }
        
        private (float minValue, float maxValue) GetRangeForSelectedPropertyFloat(SerializedProperty property)
        {
            var occupiedSpace = GetOccupiedSpace();
            return (_minWeight.floatValue, occupiedSpace + property.floatValue);
        }

        private float GetOccupiedSpace()
        {
            return _maxWeight.floatValue - 
                   _minWeight.floatValue - 
                   _speedMovement.floatValue -
                   _visibilityRadius.floatValue - 
                   _gunPower.floatValue - 
                   _armor.floatValue - 
                   _rateOfFire.floatValue;
        }
    }
}