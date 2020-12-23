using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.Context
{
    public class StateDefinitions
    {

        Dictionary<String, String> priceDef = new Dictionary<String, String>();

        String _default = "";

        // ArrayList arrayList = new ArrayList();

        // price
        // pv
        // pm

        // volume
        // vv
        // vm

        // quantity
        // qv
        // qm

        public StateDefinitions()
        {

            priceDef.Clear();
            _default = "";

            _default = "N";
            priceDef.Add("VAL >= 0.02", "A");
            priceDef.Add("-0.02 < VAL < 0.02", _default);
            priceDef.Add("VAL <= -0.02", "B");

        }

        public Dictionary<String, String> getPriceDefHash()
        {
            return priceDef;
        }

        public List<String> getPriceDefs()
        {
            return new List<String>(priceDef.Values);
        }

        public List<String> getStateDefs()
        {
            List<String> ret = new List<String>();
            ret.Add("comprado");
            ret.Add("vendido");
            ret.Add("nada");
            return ret;
        }

        public String getDefault()
        {
            return _default;
        }
    }

}
