namespace SUpdater.Model
{
    public enum ValueFetchStrategy
    {
        Never, //The value will never be fetched
        OnEntityCreate, //The value will be fetched when the owning entity is created. Handling Code: Entity.Init()
        OnValueCreate, //The value will be fetched when the value is created (e.g. by indexer of Entity). Handling Code: Entity.indexer
        OnValueFetch //The value will be fetched when the get accessor of the  "data" member of the value class is called. Handling Code: Value.Data.getter
    }
}