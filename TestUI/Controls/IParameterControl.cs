using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUI.Controls
{
    interface IParameterControl
    {
        Type GetParameterType();
        object GetValue();
    }
}
