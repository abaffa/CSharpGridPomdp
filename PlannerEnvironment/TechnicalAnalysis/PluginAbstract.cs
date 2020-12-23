using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace PlannerEnvironment.TechnicalAnalysis
{
    public abstract class PluginAbstract
    {



        protected bool isSimilarToSeries = false;
        protected bool isCanceled = false;
        protected bool isConfigurable = false;

        protected String pluginName = "";
        protected String pluginDescription = "";

        protected Series series;

        public enum chartTypes
        {
            CONTINUOUS, DOTTY
        };

        protected chartTypes chartType = chartTypes.CONTINUOUS;

        public String GetName()
        {
            return pluginName;
        }

        public String GetDescription()
        {
            return pluginDescription;
        }

        public bool IsConfigurable()
        {
            return isConfigurable;
        }

        public bool IsCanceled()
        {
            return isCanceled;
        }

        public bool IsSimilarToSeries()
        {
            return isSimilarToSeries;
        }

        public chartTypes GetChartType()
        {
            return chartType;
        }

        public void SetSeries(Series series)
        {
            this.series = series;
        }

        protected void Cancel()
        {
            isCanceled = true;
        }


        public abstract List<String> GetResultDescription();
        public abstract List<double[]> GetResultValues();

        public abstract List<String> GetBuySellDescription();
        public abstract List<String[]> GetBuySellValues();

    }

}
