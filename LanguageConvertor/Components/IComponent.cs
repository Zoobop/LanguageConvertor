using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageConvertor.Components;

internal interface IComponent<T> where T : struct
{
    public static T Parse(string fieldLine) => default;
}
