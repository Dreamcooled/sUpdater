using System;
using System.Collections.Generic;
using SUpdater.Model;

namespace SUpdater.Provider
{
    public interface IProvider
    {
        String Name { get; }
        List<ValueDefinition> Values { get;  }
        bool RequestValue(Value value);
        bool RequestEntities(Entity parent);
    }
}