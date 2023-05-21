using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Dialog;
using UnityEngine;

namespace Pool
{
    public class DialogPool
    {
        private readonly List<DialogBase> _unusedDialogs = new List<DialogBase>();
        private readonly List<DialogBase> _usedDialogs = new List<DialogBase>();

        [CanBeNull]
        public T GetDialogFromUsed<T>() where T : DialogBase
        {
            var usedDialog = _usedDialogs.FirstOrDefault(d => d.GetType() == typeof(T));
            return (T)usedDialog;
        }
        
        public T GetOrCreateDialog<T>(Transform parent) where T : DialogBase
        {
            var unusedDialog = _unusedDialogs.FirstOrDefault(d => d.GetType() == typeof(T));
            if (unusedDialog != null)
            {
                _unusedDialogs.Remove(unusedDialog);
                _usedDialogs.Add(unusedDialog);
                unusedDialog.Show();
                return (T)unusedDialog;
            }

            var newDialog = DialogCreator.CreateDialog<T>(parent);
            if (newDialog == null)
            {
                throw new Exception($"Not created new dialog: {typeof(T)}");
            }

            newDialog.InitHideAction(() => HideDialog(newDialog));
            _usedDialogs.Add(newDialog);
            newDialog.Show();
            return newDialog;
        }

        private void HideDialog(DialogBase dialog)
        {
            var foundDialog = _usedDialogs.FirstOrDefault(d => d == dialog);
            if (foundDialog == null)
            {
                Debug.LogWarning($"Not found dialog in used list");
                return;
            }

            _usedDialogs.Remove(foundDialog);
            _unusedDialogs.Add(foundDialog);
        }
    }
}