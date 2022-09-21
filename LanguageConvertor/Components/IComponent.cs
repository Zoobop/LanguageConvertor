using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

internal enum ComponentType
{
    Container,
    Class,
    Method,
    Property,
    Field
}

internal interface IComponent
{
    public string Name { get; set; }

    public void AddComponent(in IComponent component);
}
