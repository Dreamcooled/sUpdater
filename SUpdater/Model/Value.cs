using System;

namespace SUpdater.Model
{
    public class Value : PropertyChangedImpl
    {

        public Value(ValueDefinition def, Entity entity)
        {
            //used when creating new values
            Definition = def;
            Entity = entity;
        }

        public Value()
        {
            //used when importing from db
            //TODO: handle ValueUpdateStrategy.OnValueCreate
        }

        public ValueDefinition Definition { get; internal set; }

        private String _data;
        private bool _requested;
        private bool _loaded;

        public String Data
        {
            get
            {
                if (!Loaded && !Requested)
                {
                    if (Definition.FetchStrategy == ValueFetchStrategy.OnValueFetch)
                    {
                        Console.WriteLine("Reason:  FetchStrategy == ValueFetchStrategy.OnValueFetch");
                        Definition.Provider.RequestValue(this);
                        Requested = true;
                    }
                 
                } else if (Loaded)
                {
                    if (Definition.UpdateStrategy == ValueUpdateStrategy.OnValueFetch)
                    {
                        Console.WriteLine("Reason:  UpdateStrategy == ValueFetchStrategy.OnValueFetch");
                        Definition.Provider.RequestValue(this);
                    }
                }
                return _data;
            }
            internal set
            {
                _data = value;
                Loaded = true;
                OnPropertyChanged();
            }
        }

        public bool Loaded
        {
            get { return _loaded; }
            internal set
            {
                _loaded = value; 
                OnPropertyChanged();
            }
        }

        public Entity Entity { get; internal set; }

        public bool Requested
        {
            get { return _requested; }
            internal set
            {
                _requested = value; 
                OnPropertyChanged();
            }
        }

        public bool SetValue(String data)
        {
            //TODO: check thread
            Data = data;
            return true;
        }
    }
}