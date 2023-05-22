using Pool;
using UnityEngine;

namespace UI.Dialog
{
    public class DialogManager
    {
        private readonly RectTransform _parentForDialogs;
        private readonly RectTransform _parentForLocationUI;
        private readonly DialogPool _pool = new DialogPool();

        public DialogManager(RectTransform parentForDialogs, RectTransform parentForLocationUI)
        {
            _parentForDialogs = parentForDialogs;
            _parentForLocationUI = parentForLocationUI;
        }


        /// <summary>
        /// Возвращает новый UI от камеры
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetNewLocationDialog<T>() where T: DialogBase
        {
            return _pool.GetOrCreateDialog<T>(_parentForLocationUI);
        }
        
        /// <summary>
        /// Возвращает полностью новый диалог, даже если такой же есть уже открытый
        /// </summary>
        public T GetNewDialog<T>() where T : DialogBase
        {
            return _pool.GetOrCreateDialog<T>(_parentForDialogs);
        }
        
        /// <summary>
        /// Может вернуть уже открытый дилог
        /// </summary>
        public T GetDialog<T>() where T : DialogBase
        {
            var usedDialog = _pool.GetDialogFromUsed<T>();
            return usedDialog == null ? GetNewDialog<T>() : usedDialog;
        }
    }
}