#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ShipLogic.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ShipBase))]
    public class ShipBaseEditor : UnityEditor.Editor
    {
        private ShipBase _ship;

        private void OnEnable()
        {
            _ship = (ShipBase)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                return;
            }

            EditorGUILayout.LabelField($"Скорость корабля: {_ship.CalculatedShipData.SpeedMovement}");
            EditorGUILayout.LabelField($"Зона видимости корабля: {_ship.CalculatedShipData.VisibilityRadius}");
            EditorGUILayout.LabelField($"Скорострельность корабля: {_ship.CalculatedShipData.RateOfFire}");
            EditorGUILayout.LabelField($"Мощность оружия: {_ship.CalculatedShipData.GunPower}");
            EditorGUILayout.LabelField($"Броня корабля: {_ship.CalculatedShipData.Armor}");
            EditorGUILayout.LabelField($"Состояние: {_ship.NameCurrentState}");
        }
    }
}
#endif