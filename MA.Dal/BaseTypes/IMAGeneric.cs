using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dal.BaseTypes
{
    public interface IMAGeneric<T>
    {
        T DataObject { get; }
    }
}
