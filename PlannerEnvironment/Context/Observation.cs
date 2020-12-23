using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.Context
{
    public class Observation
    {

        LogControl log;

        State states;
        String observationDefinition = "";

        public Observation(String obsDef, State states, LogControl log)
        {
            this.observationDefinition = obsDef;
            this.states = states;

            this.log = log;
        }

        public List<String> observationList()
        {
            List<List<String>> defs = new List<List<String>>();

            List<String> ret = new List<String>();

            String[] aDef = observationDefinition.Split('#');

            for (int i = 0; i < aDef.Length; i++)
            {
                String type = aDef[i].Substring(0, 1);

                if (type.Equals("p"))
                    defs.Add(states.getStateDefinitions().getPriceDefs());
                else if (type.Equals("v"))
                    defs.Add(states.getStateDefinitions().getPriceDefs());
                else if (type.Equals("q"))
                    defs.Add(states.getStateDefinitions().getPriceDefs());
                else
                    defs.Add((new ObservationDefinitions(aDef[i]))
                            .getObservationDefs());

            }

            ret = test("", defs, 0);

            return ret;
        }

        public List<String> test(String a, List<List<String>> b,
                int level)
        {

            List<String> ret = new List<String>();

            List<String> actual = b[level];

            for (int i = 0; i < actual.Count; i++)
            {

                if (level >= b.Count - 1)
                {

                    String temp = "";

                    if (a.Trim().Equals(""))
                        temp = actual[i];
                    else
                        temp = a + "-" + actual[i];

                    ret.Add(temp);

                }
                else
                {
                    int newLevel = level + 1;
                    List<String> _in = test(actual[i], b, newLevel);

                    for (int j = 0; j < _in.Count; j++)
                    {

                        String temp = "";

                        if (a.Trim().Equals(""))
                            temp = _in[j];
                        else
                            temp = a + "-" + _in[j];

                        ret.Add(temp);
                    }
                }

            }

            return ret;
        }

        public String getSettings()
        {
            return observationDefinition;
        }

        public String getObservationDefinition()
        {
            return observationDefinition;
        }

        public State getStates()
        {
            return states;
        }
    }

}
