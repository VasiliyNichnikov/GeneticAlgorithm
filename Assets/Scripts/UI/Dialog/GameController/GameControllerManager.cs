using System;
using UnityEngine;

namespace UI.Dialog.GameController
{
    public class GameControllerManager : IDisposable
    {
        private readonly GameControllerDialog _controllerDialog;

        public GameControllerManager()
        {
            Main.Instance.OnUpdateGame += CustomUpdate;
            
            _controllerDialog = Main.Instance.DialogManager.GetDialog<GameControllerDialog>();
            _controllerDialog.gameObject.SetActive(false);
        }
        
        private void CustomUpdate()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                _controllerDialog.gameObject.SetActive(!_controllerDialog.gameObject.activeSelf);
            }
        }

        public void Dispose()
        {
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }
    }
}