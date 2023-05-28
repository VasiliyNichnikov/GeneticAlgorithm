using System;
using System.Linq;
using Players;
using UnityEngine;

namespace Storages
{
    public class ColorStorage : MonoBehaviour
    {
        [Serializable]
        private struct ColorData
        {
            public PlayerType Player => _player;
            public Color Color => _color;
            
            [SerializeField] private PlayerType _player;
            [SerializeField] private Color _color;
        }

        [SerializeField] private ColorData[] _colors;

        public Color GetColorForPlayer(PlayerType player)
        {
            var selectedMaterial = _colors.FirstOrDefault(data => data.Player == player);
            return selectedMaterial.Color;
        }
    }
}