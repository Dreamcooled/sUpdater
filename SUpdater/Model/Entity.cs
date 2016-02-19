using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using SUpdater.Provider;

namespace SUpdater.Model
{
    public class Entity
    {

        private Dispatcher _dispatcher;
        public Entity(String name, EntityType type)
        {
           //used when creating new entities
            _isNew = true;
            _dispatcher = Dispatcher.CurrentDispatcher;
            Name = name;
            Type = type;
            Values = new ReadOnlyDictionary<String, Value>(_values);
            Entities = new ReadOnlyObservableCollection<Entity>(_entities);
        }

        public Entity()
        {

            //used when importing from db
            _isNew = false;
            _dispatcher = Dispatcher.CurrentDispatcher;
            Values =  new ReadOnlyDictionary<String, Value>(_values);
            Entities = new ReadOnlyObservableCollection<Entity>(_entities);
        }

        private readonly bool _isNew;

        public bool Init()
        {
            //create values where FetchStrategy == ValueFetchStrategy.OnEntityCreate
   
            foreach (IProvider prov in ProviderManager.GetProviders())
            {
                if (_isNew)
                {
                    var defs = prov.Values.Where(vd => vd.EntityType == Type && vd.FetchStrategy == ValueFetchStrategy.OnEntityCreate);
                    foreach (var def in defs)
                    {

                        bool created;
                        var val = AddGetValue( def, out created);
                        if(created) { 
                            Console.WriteLine("Reason:  FetchStrategy == ValueFetchStrategy.OnEntityCreate");
                            prov.RequestValue(val);
                        }
                    }
                }
                prov.RequestEntities(this);
            }


            return true;
        }

        public Entity Parent { get; internal set; }

        public String Name
        {
            get;
            internal set;
        }
        public EntityType Type
        {
            get;
            internal set;
        }

        public Value this[String name]
        {
            get
            {
                Value o;
                if (Values.TryGetValue(name, out o))
                {
                    return o;
                }
                foreach (IProvider prov in ProviderManager.GetProviders())
                {
                   var def = prov.Values.FirstOrDefault(vd => vd.EntityType == Type && vd.Name == name);
                    if (def != null)
                    {
                        bool created;
                        var val = AddGetValue(def, out created);
                        if (created && def.FetchStrategy == ValueFetchStrategy.OnValueCreate)
                        {
                            Console.WriteLine("Reason:  FetchStrategy == ValueFetchStrategy.OnValueCreate");
                            def.Provider.RequestValue(val);
                        }
                        return val;
                    }
                }
                throw new ArgumentOutOfRangeException(nameof(name),@"No such cached value found and no provider has a matching value definition");
            }
        }


        private readonly Dictionary<String, Value> _values = new Dictionary<string, Value>();
        public ReadOnlyDictionary<String, Value> Values { get; }

        internal Value AddGetValue(ValueDefinition def, out bool created)
        {
            bool c=false;

            var val = _dispatcher.Invoke(delegate
            {
                Value o;
                if (Values.TryGetValue(def.Name, out o)) //value already existed
                {
                    return o;
                }

                 //value did not exist
                c = true;
                var v = new Value(def, this);
                _values.Add(def.Name, v);
                return v;
            });

            created = c;
            return val;
        }


        private readonly ObservableCollection<Entity> _entities = new ObservableCollection<Entity>(); 
        public ReadOnlyObservableCollection<Entity> Entities  { get; internal set; }

        internal Entity AddGetEntity(String name, EntityType type, out bool created)
        {
            bool c = false;

            var ent = _dispatcher.Invoke(delegate
            {
                Entity e = Entities.FirstOrDefault(et => et.Name == name);
                if (e != null) //entity already existed
                {
                    return e;
                }

                //entity did not exist
                c = true;
                var en = new Entity(name, type);
                en.Parent = this;
                _entities.Add(en);
                return en;
            });

            created = c;
            return ent;
        }


    }
}