using System;
using UnityEngine;

namespace UI.Dialog
{
    public abstract class DialogBase : MonoBehaviour
    {
        private Action _onHideAction;
        
        public void InitHideAction(Action onHideAction)
        {
            _onHideAction = onHideAction;
        }
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            _onHideAction?.Invoke();
            gameObject.SetActive(false);
        }
    }
}