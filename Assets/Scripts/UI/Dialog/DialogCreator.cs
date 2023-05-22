using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Dialog.InfoAboutMiningPlanet;
using UI.Dialog.InfoAboutPlayerPlanet;
using UI.Dialog.InfoAboutShip;
using UnityEngine;
using Utils.Loader;
using Object = UnityEngine.Object;

namespace UI.Dialog
{
    public static class DialogCreator
    {
        private static readonly Dictionary<Type, string> _dialogPaths = new Dictionary<Type, string>
        {
            { typeof(InfoAboutShipDialog), "Prefabs/UI/InfoAboutShip/InfoAboutShipDialog" },
            {typeof(InfoAboutMiningPlanetDialog), "Prefabs/UI/InfoAboutMiningPlanet/InfoAboutMiningPlanetDialog"},
            {typeof(InfoAboutPlayerPlanetDialog), "Prefabs/UI/InfoAboutPlayerPlanet/InfoAboutPlayerPlanetDialog"}
        };

        [CanBeNull]
        public static T CreateDialog<T>(Transform parent = null) where T: DialogBase
        {
            if (!_dialogPaths.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Not found dialog: {typeof(T)}");
                return null;
            }
            var dialog = _dialogPaths.FirstOrDefault(kvp => kvp.Key == typeof(T));
            var prefab = StaticLoader.LoadDialogFromResource<T>(dialog.Value);
            return Object.Instantiate(prefab, parent, false);
        }
    }
}