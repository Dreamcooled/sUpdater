using System;
using System.Data;
using SUpdater.Utils;

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

        private String _stringData;
        private bool _requested;
        private bool _loaded;

        public String StringData
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
                return _stringData;
            }
            internal set
            {
                _stringData = value;
                Loaded = true;
                OnPropertyChanged();
                if (Definition.Type == ValueType.Image)
                {
                    OnPropertyChanged(nameof(Value.ImageData));
                } else if (Definition.Type == ValueType.Integer)
                {
                    OnPropertyChanged(nameof(Value.IntData));
                }
            }
        }

        private CachedImage _imageData;
        public CachedImage ImageData
        {
            get
            {
                if (Definition.Type != ValueType.Image)
                {
                    throw new InvalidOperationException();
                }
                if (_imageData != null)
                {
                    return _imageData;
                }
                string d = StringData;
                if (string.IsNullOrEmpty(d))
                {
                    return null;
                }
                _imageData = new CachedImage(d);
                return _imageData;
            }
        }

        private int? _intData;

        public int IntData
        {
            get
            {
                if (Definition.Type != ValueType.Integer)
                {
                    throw new InvalidOperationException();
                }
                if (_intData.HasValue)
                {
                    return _intData.Value;
                }
                if (_stringData == null) return 0;
                int parsedInt;
                if (int.TryParse(_stringData, out parsedInt))
                {
                    _intData = parsedInt;
                    return _intData.Value;
                }
                throw new InvalidOperationException();
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
            StringData = data;
            return true;
        }
    }
}