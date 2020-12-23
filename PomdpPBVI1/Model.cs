using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpPBVI1
{
    public class Model
    {

        String[] S;
        String[] A;
        String[] O;
        double[] bInit;

        Dictionary<String, double[][]> Ta = new Dictionary<String, double[][]>(); // [S.Length][S.Length];
        Dictionary<String, double[][]> Pa = new Dictionary<String, double[][]>(); // [S.Length][O.Length];
        Dictionary<String, double[]> R = new Dictionary<String, double[]>(); // [S.Length];
        Dictionary<String, double[][]> C = new Dictionary<String, double[][]>(); // [S.Length][S.Length];

        String lastErrStatus = "";

        public String getLastErrStatus()
        {
            return lastErrStatus;
        }

        public bool check()
        {

            bool ret = true;
            String errs = "";

            lastErrStatus = "";
            try
            {
                if (A.Length != Ta.Keys.Count)
                    errs = errs + "A != Ta\n";

                if (A.Length != Pa.Keys.Count)
                    errs = errs + "A != Pa\n";

                if (A.Length != R.Keys.Count)
                    errs = errs + "A != R\n";

                for (int i = 0; i < Ta.Keys.Count; i++)
                {
                    String key = Ta.Keys.ElementAt(i);

                    double[][] value = Ta[key];

                    if (value.Length != S.Length)
                        errs = errs + "Ta = " + key + "[" + value.Length + "]\n";

                    for (int si = 0; si < S.Length; si++)
                    {
                        if (value[si].Length != S.Length)
                        {
                            errs = errs + "Ta = " + key + "[" + value.Length + "]["
                                    + value[si].Length + "]\n";
                        }
                    }

                }

                for (int i = 0; i < Pa.Keys.Count; i++)
                {
                    String key = Pa.Keys.ElementAt(i);

                    double[][] value = Pa[key];

                    if (value.Length != S.Length)
                        errs = errs + "Pa = " + key + "[" + value.Length + "]\n";

                    for (int si = 0; si < S.Length; si++)
                    {
                        if (value[si].Length != O.Length)
                        {
                            errs = errs + "Pa = " + key + "[" + value.Length + "]["
                                    + value[si].Length + "]\n";
                        }
                    }

                }

                for (int i = 0; i < R.Keys.Count; i++)
                {
                    String key = R.Keys.ElementAt(i);

                    double[] value = R[key];

                    if (value.Length != S.Length)
                        errs = errs + "R = " + key + "[" + value.Length + "]\n";

                    // for(int si = 0; si < S.Length; si++){
                    // System.out.println(key + " - " + S[si] + " - " + value[si]);
                    // }

                }
            }
            catch (Exception ex)
            {
                errs = "Wrong File";
            }

            if (errs.Trim().Length > 0)
            {
                lastErrStatus = errs;
                ret = false;
            }
            return ret;
        }

        public double[] getBInit()
        {
            return bInit;
        }

        public void setBInit(double[] init)
        {
            bInit = init;
        }

        public String[] getS()
        {
            return S;
        }

        public void setS(String[] s)
        {
            S = s;
        }

        public String[] getA()
        {
            return A;
        }

        public void setA(String[] a)
        {
            A = a;
        }

        public String[] getO()
        {
            return O;
        }

        public void setO(String[] o)
        {
            O = o;
        }

        public int indexS(String s)
        {
            int ret = -1;
            for (int i = 0; i < S.Length; i++)
            {
                if (S[i].Equals(s))
                {
                    ret = i;
                    break;
                }
            }

            return ret;
        }

        public int indexA(String a)
        {
            int ret = -1;
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i].Equals(a))
                {
                    ret = i;
                    break;
                }
            }

            return ret;
        }

        public int indexO(String o)
        {
            int ret = -1;
            for (int i = 0; i < O.Length; i++)
            {
                if (O[i].Equals(o))
                {
                    ret = i;
                    break;
                }
            }

            return ret;
        }

        public void resetS()
        {
            S = null;
        }

        public void resetA()
        {
            A = null;
        }

        public void resetO()
        {
            O = null;
        }

        public void resetTa()
        {
            Ta.Clear();
        }

        public void resetPa()
        {
            Pa.Clear();
        }

        public void resetR()
        {
            R.Clear();
        }

        public void resetC()
        {
            C.Clear();
        }

        public void setTa(String a, double[][] t)
        {
            Ta.Add(a, t);
        }

        public void setPa(String a, double[][] p)
        {
            Pa.Add(a, p);
        }

        public void setR(String a, double[] r)
        {
            R.Add(a, r);
        }

        public void setR(String a, int sli, double value)
        {
            double[] r = R[a];
            r[sli] = value;
            R.Add(a, r);
        }

        public void setC(String a, double[][] c)
        {
            C.Add(a, c);
        }

        public double[][] getTa(String a)
        {
            return Ta[a];
        }

        public double[][] getPa(String a)
        {
            return Pa[a];
        }

        public double[] getR(String a)
        {
            return R[a];
        }

        public double[][] getC(String a)
        {
            return C[a];
        }

        public void Clear()
        {

            resetS();
            resetA();
            resetO();

            resetTa();
            resetPa();
            resetR();
            resetC();

        }
    }

}
