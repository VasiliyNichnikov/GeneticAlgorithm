using System;
using System.Linq;
using Players;
using UnityEngine;

namespace Storages
{
    public class MaterialStorage : MonoBehaviour
    {
        private enum MaterialType
        {
            Planet,
            Ship
        }
        
        [Serializable]
        private struct MaterialData
        {
            public PlayerType Player => _player;
            public MaterialType Type => _type;
            public Material Material => _material;

            [SerializeField] private PlayerType _player;
            [SerializeField] private Material _material;
            [SerializeField] private MaterialType _type;
        }

        [SerializeField]
        private MaterialData[] _materials;

        public Material GetMaterialForShip(PlayerType player)
        {
            var selectedMaterialData =
                _materials.FirstOrDefault(data => data.Player == player && data.Type == MaterialType.Ship);
            if (selectedMaterialData.Material == null)
            {
                Debug.LogError("Material is null");
                return null;
            }
            return selectedMaterialData.Material;
        }
        
        public Material GetMaterialForPlanet(PlayerType player)
        {
            var selectedMaterialData =
                _materials.FirstOrDefault(data => data.Player == player && data.Type == MaterialType.Planet);
            if (selectedMaterialData.Material == null)
            {
                Debug.LogError("Material is null");
                return null;
            }
            return selectedMaterialData.Material;
        }
    }
}