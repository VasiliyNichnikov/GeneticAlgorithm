#if UNITY_EDITOR
using ShipLogic.Individual;
using UnityEditor;
using UnityEngine;

namespace ShipLogic.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ShipIndividual))]
    public class ShipIndividualEditor : UnityEditor.Editor
    {
        private ShipIndividual _individual;

        private void OnEnable()
        {
            _individual = (ShipIndividual)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                return;
            }

            EditorGUILayout.LabelField($"Скорость корабля: {_individual.CalculatedShipData.SpeedMovement}");
            EditorGUILayout.LabelField($"Зона видимости корабля: {_individual.CalculatedShipData.VisibilityRadius}");
            EditorGUILayout.LabelField($"Скорострельность корабля: {_individual.CalculatedShipData.RateOfFire}");
            EditorGUILayout.LabelField($"Мощность оружия: {_individual.CalculatedShipData.GunPower}");
            EditorGUILayout.LabelField($"Броня корабля: {_individual.CalculatedShipData.Armor}");
            EditorGUILayout.LabelField($"Состояние: {_individual.NameCurrentState}");
        }
    }
}
#endif