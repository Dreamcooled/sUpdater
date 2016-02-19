namespace SUpdater.Model
{
    public enum ValueUpdateStrategy
    {
        Never, //The value is never autoupdated
        OnValueCreate, // The value is automatically refetched when the value is created (e.g. when loaded from db). Handling code: value constructor
        OnValueFetch, //The value will be fetched when the get accessor of the  "data" member of the value class is called. Handling Code: Value.String.getter
        Periodically //Handling Code: ?
    }


}