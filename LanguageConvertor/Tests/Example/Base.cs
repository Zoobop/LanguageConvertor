using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Output;

public class Base
{
    protected virtual void Func1(int obj) { }
    protected virtual void Func2(string obj) { }
}

public interface IInterface
{
    public void Func3(object obj);
}
