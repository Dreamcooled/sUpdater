using System.Windows.Media;
using SUpdater.Provider;
using SUpdater.Utils;

namespace SUpdater.Model
{
    public class ValueDefinition
    {
        public ValueDefinition(IProvider provider, string name, ValueType type, EntityType entityType, 
                                ValueFetchStrategy fetchStrategy=ValueFetchStrategy.OnValueCreate, ValueUpdateStrategy updateStrategy=ValueUpdateStrategy.Periodically,
                                bool stored = true, bool editable = false)
        {
            Name = name;
            Type = type;
            Stored = stored;
            Editable = editable;
            UpdateStrategy = updateStrategy;
            FetchStrategy = fetchStrategy;
            Provider = provider;
            EntityType = entityType;
        }

        public string Name{get;}
        public ValueType Type{get;}

        public ImageSource TypeImage => ResourceImageAttribute.GetImageSource(Type);

        public bool Stored{get;}
        public bool Editable{get;}
        public ValueUpdateStrategy UpdateStrategy{get;}
        public ValueFetchStrategy FetchStrategy{get;}
        public IProvider Provider{get;}
        public EntityType EntityType{get;}
    }
}