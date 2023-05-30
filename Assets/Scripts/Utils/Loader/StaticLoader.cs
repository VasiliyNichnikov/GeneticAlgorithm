using System;
using UI.Dialog;
using Newtonsoft.Json;
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

        public static T LoadJson<T>(string pathFile)
        {
            var textAsset = (TextAsset)Resources.Load(pathFile, typeof(TextAsset));
            if (textAsset == null)
            {
                return default(T);
            }
            
            var data = JsonConvert.DeserializeObject<T>(textAsset.text);
            return (T)Convert.ChangeType(data, typeof(T));
        }
    }
}