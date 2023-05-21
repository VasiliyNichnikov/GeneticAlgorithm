using UI.Dialog;
using UnityEngine;

namespace Utils.Loader
{
    public static class StaticLoader
    {
        public static T LoadDialogFromResource<T>(string pathDialog) where T : DialogBase
        {
            var data = (T)Resources.Load(pathDialog, typeof(T));
            return data == null ? default(T) : data;
        }
    }
}