#if UNITY_EDITOR
using Planets.PlayerPlanet;
using Players;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace ShipLogic.Editor
{
    [CustomEditor(typeof(PlayerPlanet))]
    public class PlayerShipControllerEditor : UnityEditor.Editor
    {
        private PlayerPlanet _controller;

        private void OnEnable()
        {
            _controller = (PlayerPlanet)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                return;
            }

            if (GUILayout.Button("Create ship"))
            {
                _controller.CreateRandomShipDebug();
            }
        }
    }
}
#endif