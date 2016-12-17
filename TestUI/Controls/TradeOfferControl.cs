using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jatan.Models;

namespace TestUI.Controls
{
    public partial class TradeOfferControl : UserControl, IParameterControl
    {
        public TradeOfferControl()
        {
            InitializeComponent();
        }

        public Type GetParameterType()
        {
            return typeof(TradeOffer);
        }

        public object GetValue()
        {
            return new TradeOffer((int)playerId.GetValue(), (ResourceStack)toGive.GetValue(), (ResourceStack)toReceive.GetValue());
        }
    }
}
