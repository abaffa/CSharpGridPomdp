using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.baffa.db
{
    public class Cotacao
    {
        public int IDARQUIVO { get; set; }

        public int IDCOTACAO { get; set; }

        public DateTime DTPREG { get; set; }

        public String CODBDI { get; set; }

        public String CODNEG { get; set; }

        public int TPMERC { get; set; }

        public String NOMRES { get; set; }

        public String ESPECI { get; set; }

        public String PRAZOT { get; set; }

        public String MODREF { get; set; }

        public double PREABE { get; set; }

        public double PREMAX { get; set; }

        public double PREMIN { get; set; }

        public double PREMED { get; set; }

        public double PREULT { get; set; }

        public double PREOFC { get; set; }

        public double PREOFV { get; set; }

        public int TOTNEG { get; set; }

        public long QUATOT { get; set; }

        public double VOLTOT { get; set; }

        public double PREEXE { get; set; }

        public int INDOPC { get; set; }

        public DateTime DATVEN { get; set; }

        public long FATCOT { get; set; }

        public double PTOEXE { get; set; }

        public String CODISI { get; set; }

        public int DISMES { get; set; }

    }

}
