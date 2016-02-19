using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUpdater.Provider
{
    public static class ProviderManager
    {
        private static List<IProvider> _providers = new List<IProvider>();

        static  ProviderManager()
        {
            _providers.Add(new TmdbProvider());
        }
             

        static public IEnumerable<IProvider> GetProviders()
        {
            return _providers;
        } 
    }
}
