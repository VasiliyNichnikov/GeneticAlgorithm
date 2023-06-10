using Loaders.Data;
using Utils.Loader;

namespace Loaders
{
    public class SignalsLoader : ILoader
    {
        private const string PathFile = "Json/SignalsData";

        private HelpSignal _helpSignal;
        
        public HelpSignal GetHelpSignal()
        {
            return _helpSignal;
        }
        
        public void Load()
        {
            var signalsData = StaticLoader.LoadJson<SignalsData>(PathFile);
            _helpSignal = signalsData.HelpSignal;
        }
    }
}