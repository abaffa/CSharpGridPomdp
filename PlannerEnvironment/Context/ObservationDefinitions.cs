using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.Context
{
    public class ObservationDefinitions
    {

        Dictionary<String, String> observationDef = new Dictionary<String, String>();

        String _default = "";

        public ObservationDefinitions(String type)
        {
            // Add elements to ArrayList
            // arrayList.add("A");
            // arrayList.add("B");
            // arrayList.add("D");
            // arrayList.add("E");
            // arrayList.add("F");
            observationDef.Clear();
            _default = "";

            if (type.Equals("RSI1"))
            {
                _default = "pnada";

                observationDef.Add("SHORT > LONG!SHORT < LONG", "pcompra");
                observationDef.Add("SHORT < LONG!SHORT > LONG", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("RSI2"))
            {
                _default = "pnada";

                observationDef.Add("VAL >= 0.2!OLD <= 0.2", "pcompra");
                observationDef.Add("OLD >= 0.8!VAL <= 0.8", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("RSI3"))
            {
                _default = "pnada";

                observationDef.Add("VAL >= 0.3!OLD <= 0.3", "pcompra");
                observationDef.Add("OLD >= 0.7!VAL <= 0.7", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("MACD1"))
            {
                _default = "pnada";

                observationDef.Add("MACD > SIGNAL!MACD < SIGNAL", "pcompra");
                observationDef.Add("MACD < SIGNAL!MACD > SIGNAL", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("MACD2"))
            {
                _default = "pnada";

                observationDef.Add("VAL > 0!OLD < 0", "pcompra");
                observationDef.Add("OLD > 0!VAL < 0", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("MACD3"))
            {
                _default = "pnada";

                observationDef.Add("VAL > 0!OLD < 0", "pcompra");
                observationDef.Add("OLD > 0!VAL < 0", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("OST1"))
            {
                _default = "pnada";

                observationDef.Add("VAL > 0!OLD < 0", "pcompra");
                observationDef.Add("OLD > 0!VAL < 0", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("OST2"))
            {
                _default = "pnada";

                observationDef.Add("VAL >= 0.2!OLD <= 0.2", "pcompra");
                observationDef.Add("OLD >= 0.8!VAL <= 0.8", "pvenda");
                observationDef.Add("else", _default);

            }
            else if (type.Equals("OST3"))
            {
                _default = "pnada";

                observationDef.Add("VAL >= 0.2!OLD <= 0.2", "pcompra");
                observationDef.Add("OLD >= 0.8!VAL <= 0.8", "pvenda");
                observationDef.Add("else", _default);
            }

        }

        public Dictionary<String, String> getobservationDefHash()
        {
            return observationDef;
        }

        public List<String> getObservationDefs()
        {
            return new List<String>(observationDef.Values);
        }

        public String getDefault()
        {
            return _default;
        }

    }

}
