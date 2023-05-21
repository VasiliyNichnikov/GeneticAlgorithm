using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using ShipLogic;
using UnityEngine;

namespace UI.Dialog.InfoAboutShip
{
    public class InfoAboutShipDialog : DialogBase
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private ParameterItem _parameterPrefab;

        private readonly Queue<ParameterItem> _cacheParameters = new Queue<ParameterItem>();
        
        public void Init(ReadOnlyCollection<ParameterShipData> parameters)
        {
            RefreshParameters(parameters);
        }

        private void RefreshParameters(ReadOnlyCollection<ParameterShipData> parameters)
        {
            var activeParameters = new List<ParameterItem>();
            foreach (var parameter in parameters)
            {
                var name = parameter.Name;
                var value = parameter.Value;

                var parameterItem = GetOrCreate();
                parameterItem.Init(name, value);
                activeParameters.Add(parameterItem);
            }

            AddRangeToCache(activeParameters);
        }

        private ParameterItem GetOrCreate()
        {
            if (_cacheParameters.Count != 0)
            {
                return _cacheParameters.Dequeue();
            }
            var newParameter = Instantiate(_parameterPrefab, _content, false);
            return newParameter;

        }

        private void AddRangeToCache(List<ParameterItem> parameterItems)
        {
            foreach (var item in parameterItems)
            {
                _cacheParameters.Enqueue(item);
            }
        }
    }
}