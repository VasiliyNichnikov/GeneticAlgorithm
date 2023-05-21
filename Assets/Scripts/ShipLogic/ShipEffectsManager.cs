using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace ShipLogic
{
    public class ShipEffectsManager : MonoBehaviour
    {
        private enum TypeEffect
        {
            Engine,
            Destroy
        }
        
        [Serializable]
        private struct Effect
        {
            public TypeEffect Type => _type;
            public IReadOnlyCollection<ParticleSystem> Parts => _parts;
            
            [SerializeField]
            private ParticleSystem[] _parts;
            [SerializeField]
            private TypeEffect _type;
        }

        [SerializeField] private Effect[] _effects;

        private void Start()
        {
            foreach (var effectType in Enum.GetValues(typeof(TypeEffect)))
            {
                ChangeActiveEffect((TypeEffect)effectType, false);
            }
        }

        public void TurnOnEngine()
        {
            ChangeActiveEffect(TypeEffect.Engine, true);
        }

        public void TurnOffEngine()
        {
            ChangeActiveEffect(TypeEffect.Engine, false);
        }

        public void DestroyShip()
        {
            ChangeActiveEffect(TypeEffect.Destroy, true);
        }

        private void ChangeActiveEffect(TypeEffect effect, bool state)
        {
            var parts = GetPartsByTypeEffect(effect);
            foreach (var part in parts)
            {
                part.gameObject.SetActive(state);
                if (state)
                {
                    part.Play();
                }
                else
                {
                    part.Stop();
                }
            }
        }
        
        private IReadOnlyCollection<ParticleSystem> GetPartsByTypeEffect(TypeEffect effect)
        {
            var selectedBlockEffects = _effects.FirstOrDefault(e => e.Type == effect);
            if (selectedBlockEffects.Parts == null)
            {
                return ArraySegment<ParticleSystem>.Empty;
            }
            
            return selectedBlockEffects.Parts;
        }
    }
}