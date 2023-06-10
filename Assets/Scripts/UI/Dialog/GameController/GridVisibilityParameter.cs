using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialog.GameController
{
    public class GridVisibilityParameter : MonoBehaviour
    {
        public bool IsSelected;

        [SerializeField] private Text _title;
        [SerializeField] private Toggle _toggle;

        private Action<bool> _onChangeToggle;
        
        public void Init(string title, Action<bool> onChangeToggle)
        {
            _title.text = title;
            _onChangeToggle = onChangeToggle;
            _toggle.isOn = false;
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }
        
        public void OffToggle()
        {
            _toggle.isOn = false;
            IsSelected = false;
        }
        
        private void OnValueChanged(bool value)
        {
            _onChangeToggle?.Invoke(value);
        }
    }
}