using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

public interface IComponent
{
    public string Name { get; set; }

    public bool IsScope();
    public void AddComponent(in IComponent component) { }
}
