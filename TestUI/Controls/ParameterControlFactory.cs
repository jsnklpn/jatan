using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestUI.Controls
{
    public static class ParameterControlFactory
    {
        public static Control GetControl(ParameterInfo parameter)
        {
            var type = parameter.ParameterType;
            if (type == typeof(string))
            {
                return new StringControl();
            }
            if (type == typeof(int))
            {
                return new IntegerControl();
            }
            if (type == typeof(bool))
            {
                return new BooleanControl();
            }
            if (type.IsEnum)
            {
                return new EnumControl(type);
            }
            if (type == typeof(Jatan.Core.Hexagon))
            {
                return new HexagonControl();
            }
            if (type == typeof(Jatan.Core.HexEdge))
            {
                return new HexEdgeControl();
            }
            if (type == typeof(Jatan.Core.HexPoint))
            {
                return new HexPointControl();
            }
            if (type == typeof(Jatan.Models.ResourceStack))
            {
                return new ResourceStackControl();
            }
            if (type == typeof(Jatan.Models.TradeOffer))
            {
                return new TradeOfferControl();
            }

            return new Label() { Text = "Unable to set parameter." };
        }
    }
}
