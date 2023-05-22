using System;
using UnityEngine;

namespace UI.Dialog.InfoAboutPlayerPlanet
{
    public class CreatorShip : MonoBehaviour
    {
        private Action _onClickHandler;

        public void Init(Action onClickHandler)
        {
            _onClickHandler = onClickHandler;
        }

        // Called from unity
        public void OnClickHandler()
        {
            _onClickHandler?.Invoke();
        }
    }
}