using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraderWhatever.Business
{
    public class TimeSeries : List<TimeSeriesDTO>
    {
        public String name = "";
        public String Name { get { return name; } }
        public TimeSeries()
        {

        }

        public TimeSeries(String name)
        {
            this.name = name;
        }

        public void Add(DateTime date, Double value)
        {
            base.Add(new TimeSeriesDTO { Date = date, Value = value });
        }
    }
}
