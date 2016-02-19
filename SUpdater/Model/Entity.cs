using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using SUpdater.Provider;

namespace SUpdater.Model
{
    public class Entity
    {

        public Entity(String name, EntityType type)
        {
           //used when creating new entities
            _isNew = true;
            Name = name;
            Type = type;
            Values = new ReadOnlyDictionary<String, Value>(_values);
            Entities = new ReadOnlyObservableCollection<Entity>(_entities);
        }

        public Entity()
        {

            //used when importing from db
            _isNew = false;
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
        private readonly Mutex _addValueMutex = new Mutex();

        internal Value AddGetValue(ValueDefinition def, out bool created)
        {
            _addValueMutex.WaitOne();
            Value o;
            if (Values.TryGetValue(def.Name, out o)) //value already existed
            {
                created = false;
                _addValueMutex.ReleaseMutex();
                return o;
            }

            //value did not exist
            var val = new Value(def, this);
            _values.Add(def.Name,val);
            created = true;
            _addValueMutex.ReleaseMutex();
            return val;
        }


        private readonly ObservableCollection<Entity> _entities = new ObservableCollection<Entity>(); 
        public ReadOnlyObservableCollection<Entity> Entities  { get; internal set; }
        private readonly Mutex _addEntityMutex = new Mutex();

        internal Entity AddGetEntity(String name, EntityType type, out bool created)
        {
            _addEntityMutex.WaitOne();
            Entity e = Entities.FirstOrDefault(et => et.Name == name);
            if (e!=null) //entity already existed
            {
                created = false;
                _addEntityMutex.ReleaseMutex();
                return e;
            }

            //entity did not exist
            var ent = new Entity(name,type);
            ent.Parent = this;
            _entities.Add(ent);
            created = true;
            _addEntityMutex.ReleaseMutex();
            return ent;
        }


    }
}