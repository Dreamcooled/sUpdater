using SUpdater.Utils;

namespace SUpdater.Model
{
    public enum ValueType
    {
        Boolean,

        [ResourceImage("Resources/Icons/typeInteger.png")]
        Integer,
        [ResourceImage("Resources/Icons/typeFloat.png")]
        Double,
        [ResourceImage("Resources/Icons/typeDate.png")]
        Date,
        [ResourceImage("Resources/Icons/typeString.png")]
        String,
        [ResourceImage("Resources/Icons/typeLongString.png")]
        LongString,
        [ResourceImage("Resources/Icons/typeLink.png")]
        Link,
        [ResourceImage("Resources/Icons/typeImage.png")]
        Image,
        [ResourceImage("Resources/Icons/typeRating.png")]
        Rating
    }
}   