using PlannerInterfaces.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpPBVI1
{
    public class Planner : PlannerInterfaces.POMDP.Planner
    {

        PlannerModel plannerModel = new PlannerModel();

        Dictionary<double[], int> polB = new Dictionary<double[], int>();
        Dictionary<double[], int> polAlpha = new Dictionary<double[], int>();

        int initBIndex = -1;
        int numIterations = 20;
        int numBackups = 1;

        int timeLimit = 0;
        bool cancelled = false;


        public override String getName()
        {
            return "pomdp-PBVI";
        }


        public override String getVersion()
        {
            return "v. 1.0";
        }


        public override void setEpoch(int maxEpoch)
        {
            this.numIterations = maxEpoch;
        }


        public override void setTimeLimit(int timeLimit)
        {
            this.timeLimit = timeLimit;
        }

        public Planner()
        {

        }

        public Planner(LogControl log)
        {
            this.log = log;
        }


        public override int getBInitIndex()
        {
            return initBIndex;
        }


        public override void cancel()
        {
            cancelled = true;
        }


        public override void pomdp(String pomdpFilename)
        {
            cancelled = false;
            long startTime = Utils.UnixTimeStamp();

            Settings.setProjectName(pomdpFilename);

            writeln(" //**************\\\\");
            writeln("||   " + getName() + "   ||");
            writeln("||     " + getVersion() + "     ||");
            writeln(" \\\\**************//");
            writeln("- - - - - - - - - - - - - - - - - - - -");
            writeln("pomdp     = " + Settings.pomdpFileName);
            writeln("discount  = " + Utils.Round(Settings.gamma, 5));
            writeln("epochs    = " + numIterations);
            writeln("backups   = " + numBackups);
            writeln("timeLimit = " + timeLimit);
            writeln("- - - - - - - - - - - - - - - - - - - -");
            write("[Initializing POMDP ... ");

            Clear();

            plannerModel.loadModel();
            // plannerModel.loadModelTiger();

            if (plannerModel.checkModel())
            {
                writeln("done.]", false);

                List<double[]> initB = plannerModel.initBList();

                writeln("[Initial policy has 1 vector.]");
                writeln("++++++++++++++++++++++++++++++++++++++++");



                List<double[]> tau = pbvi(initB, createTau0(initB),
                        numIterations, numBackups);

                writeln("++++++++++++++++++++++++++++++++++++++++");
                if (cancelled)
                {
                    writeln("Execution cancelled.");
                }
                else
                {
                    writeln("Solution found.  See file:");

                    write("	Writing policy file...");
                    PlannerUtils.writePolFile(plannerModel.getModel(), polB);
                    writeln(Settings.polFileName, false);

                    write("	Writing alpha file...");
                    PlannerUtils.writeAlphaFile(polAlpha);
                    writeln(Settings.alphaFileName, false);
                }
                long totalTime = (Utils.UnixTimeStamp() - startTime);

                writeln("++++++++++++++++++++++++++++++++++++++++");
                writeln("Total execution time = "
                        + Utils.formatIntoHHMMSS(totalTime) + ". (= " + totalTime
                        + " secs)");

                SearchInitialNode();
            }
            else
            {
                writeln("err.]", false);
                writeln(plannerModel.getLastErrStatus());
            }

        }

        public void Clear()
        {
            initBIndex = -1;
            polB.Clear();
            polAlpha.Clear();
        }

        public void rewriteFiles(String pomdpFilename)
        {
            cancelled = false;
            long startTime = Utils.UnixTimeStamp();

            Settings.setProjectName(pomdpFilename);

            writeln(" //**************\\\\");
            writeln("||   " + getName() + "   ||");
            writeln("||     " + getVersion() + "     ||");
            writeln(" \\\\**************//");
            writeln("- - - - - - - - - - - - - - - - - - - -");
            writeln("pomdp = " + Settings.pomdpFileName);
            writeln("discount = " + Utils.Round(Settings.gamma, 5));
            writeln("epochs    = " + numIterations);
            writeln("backups   = " + numBackups);
            writeln("timeLimit = " + timeLimit);
            writeln("- - - - - - - - - - - - - - - - - - - -");
            write("[Initializing POMDP ... ");

            Clear();

            plannerModel.loadModel();

            if (plannerModel.checkModel())
            {
                writeln("done.]", false);

                PlannerUtils.reloadAlpha(polAlpha);
                PlannerUtils.reloadPol(polB);

                if ((Utils.IsLinux && Settings.projectName.LastIndexOf("/") > -1) ||
     Settings.projectName.LastIndexOf("\\") > -1)
                {
                    int lastIndex = Utils.IsLinux ? Settings.projectName.LastIndexOf("/") : Settings.projectName.LastIndexOf("\\");

                    String filename = Settings.projectName.Substring(lastIndex + 1);
                    String path = Settings.projectName.Substring(0, lastIndex + 1);
                    Settings.setProjectName(path + "new_" + filename);
                }
                else
                    Settings.setProjectName("new_" + Settings.projectName);

                writeln("++++++++++++++++++++++++++++++++++++++++");
                if (cancelled)
                {
                    writeln("Execution cancelled.");
                }
                else
                {
                    writeln("Rewriting files:");

                    write("	Writing policy file...");
                    PlannerUtils.writePolFile(plannerModel.getModel(), polB);
                    writeln(Settings.polFileName, false);
                    //
                    // write("	Writing alpha file...");
                    // PlannerUtils.writeAlphaFile(polAlpha);
                    // writeln(Settings.alphaFileName, false);
                    //
                }
                long totalTime = (Utils.UnixTimeStamp() - startTime);

                writeln("++++++++++++++++++++++++++++++++++++++++");
                writeln("Total execution time = "
                        + Utils.formatIntoHHMMSS(totalTime) + ". (= " + totalTime
                        + " secs)");

                SearchInitialNode();
            }
            else
            {
                writeln("err.]", false);
                writeln(plannerModel.getLastErrStatus());
            }
        }


        public override bool SearchInitialNode()
        {

            bool ret = false;

            writeln("++++++++++++++++++++++++++++++++++++++++");
            write("Calculating initial node...");
            initBIndex = PlannerUtils.findBIndex(polB, plannerModel.initB());

            if (initBIndex > -1)
            {
                ret = true;
                writeln(initBIndex + " done!", false);
            }
            else
                writeln(" not found!", false);

            writeln("++++++++++++++++++++++++++++++++++++++++");

            return ret;
        }

        /**
         * Expand
         */

        public List<double[]> expand(List<double[]> B,
                List<double[]> Tau)
        {
            // return expandRA(B, Tau);
            // return expandSSRA(B, Tau);
            return expandSSGA_modified(B, Tau);
        }




        public List<double[]> expandRA(List<double[]> B,
                List<double[]> Tau)
        {
            List<double[]> Bnew = B.Select(p => (double[])p.Clone()).ToList();

            int S = plannerModel.getModel().getS().Length;

            for (int b = 0; b < B.Count; b++)
            {

                double[] btmp = new double[S];

                for (int i = 0; i < S; i++)
                {

                    btmp[i] = Randomize.Double(); // double 0-1
                }
                Array.Sort(btmp);

                double[] bnew = (double[])btmp.Clone();
                double sum = 0;
                double newb = 0;
                for (int i = 0; i < S - 1; i++)
                {

                    newb = btmp[i + 1] - btmp[i];

                    if (sum >= 1)
                        bnew[i] = 0;
                    else if (sum + newb >= 1)
                        bnew[i] = 1 - newb;
                    else
                        bnew[i] = newb;

                    sum = sum + bnew[i];
                }

                if (sum >= 1)
                    bnew[S - 1] = 0;
                else if (sum + bnew[S - 1] >= 1)
                    bnew[S - 1] = 1 - sum;
                else
                    bnew[S - 1] = 1 - sum;

                // // /---
                // for (int i = 0; i < bnew.Length; i++)
                // bnew[i] = Round(bnew[i], dec);
                // // ---

                addNewB(Bnew, bnew); // Bnew = Bnew U bnew
            }

            return Bnew;
        }




        public List<double[]> expandSSRA(List<double[]> B,
                List<double[]> Tau)
        {
            List<double[]> Bnew = B.Select(p => (double[])p.Clone()).ToList();

            for (int bi = 0; bi < B.Count; bi++)
            {

                if (cancelled)
                    break;

                int si = 0;
                int ai = 0;
                int sli = 0;
                int oi = 0;


                si = Randomize.Integer(plannerModel.getModel().getS().Length);
                ai = Randomize.Integer(plannerModel.getModel().getA().Length);
                sli = PlannerUtils.retValidRandomSLI(plannerModel.getModel(), si,
                        ai);
                oi = PlannerUtils
                        .retValidRandomOI(plannerModel.getModel(), sli, ai);

                double[] bnew = PlannerUtils.newBelief(plannerModel.getModel(), B
                        [bi], ai, oi);

                // // /---
                // for (int i = 0; i < bnew.Length; i++)
                // bnew[i] = Round(bnew[i], dec);
                // // ---

                addNewB(Bnew, bnew);
            }

            return Bnew;
        }




        public List<double[]> expandSSGA(List<double[]> B,
                List<double[]> Tau)
        {
            List<double[]> Bnew = B.Select(p => (double[])p.Clone()).ToList();

            for (int bi = 0; bi < B.Count; bi++)
            {

                if (cancelled)
                    break;

                int si = 0;
                int ai = 0;
                int sli = 0;
                int oi = 0;


                si = Randomize.Integer(plannerModel.getModel().getS().Length);
                ai = Randomize.Integer(plannerModel.getModel().getA().Length);
                sli = PlannerUtils.retValidRandomSLI(plannerModel.getModel(), si,
                        ai);
                oi = PlannerUtils
                        .retValidRandomOI(plannerModel.getModel(), sli, ai);

                double[] bnew = PlannerUtils.newBelief(plannerModel.getModel(), B
                        [bi], ai, oi);

                // // /---
                // for (int i = 0; i < bnew.Length; i++)
                // bnew[i] = Round(bnew[i], dec);
                // // ---

                addNewB(Bnew, bnew);
            }

            return Bnew;
        }




        public List<double[]> expandSSGA_modified(List<double[]> B,
                List<double[]> Tau)
        {
            List<double[]> Bnew = B.Select(p => (double[])p.Clone()).ToList();

            for (int bi = 0; bi < B.Count; bi++)
            {

                if (cancelled)
                    break;

                for (int ai = 0; ai < plannerModel.getModel().getA().Length; ai++)
                {
                    for (int oi = 0; oi < plannerModel.getModel().getO().Length; oi++)
                    {

                        // int ai = (int) polB.get(B[bi]);
                        double[] bnew = PlannerUtils.newBelief(plannerModel
                                .getModel(), B[bi], ai, oi);

                        // // /---
                        // for (int i = 0; i < bnew.Length; i++)
                        // bnew[i] = Round(bnew[i], dec);
                        // // ---

                        addNewB(Bnew, bnew);
                    }
                }
            }
            return Bnew;
        }

        /**
         * PBVI
         */



        public List<double[]> pbvi(List<double[]> Binit,
                List<double[]> Tau0, int N, int T)
        {

            List<double[]> B_Actual = Binit.Select(p => (double[])p.Clone()).ToList();
            List<double[]> Tau = Tau0.Select(p => (double[])p.Clone()).ToList();

            long startTime = Utils.UnixTimeStamp();
            long RoundStartTime = startTime;
            for (int n = 0; n < N; n++)
            {

                if (cancelled)
                    break;

                double errActual = 0;
                long backupStartTime = Utils.UnixTimeStamp();
                for (int t = 0; t < T; t++)
                {

                    long totalSecs2 = (Utils.UnixTimeStamp() - startTime);
                    long backupRoundSecs = (Utils.UnixTimeStamp() - backupStartTime);
                    if (t > 0)
                    {
                        long backupExpectedTotalSecs = (backupRoundSecs * T) / (t);
                        long backupExpectedRemainingSecs = backupExpectedTotalSecs
                                - backupRoundSecs;

                        writeln("+ Backup: " + (t + 1) + "..." + Tau.Count
                                + " vectors - " + B_Actual.Count + " beliefs in "
                                + backupRoundSecs + " secs. Expected "
                                + backupExpectedRemainingSecs + " secs. ("
                                + totalSecs2 + " total)");
                    }
                    else
                        writeln("+ Backup: " + (t + 1) + "..." + Tau.Count
                                + " vectors - " + B_Actual.Count + " beliefs in "
                                + backupRoundSecs + " secs. (" + totalSecs2
                                + " total)");

                    List<double[]> TauMinusOne = Tau.Select(p => (double[])p.Clone()).ToList();
                    Tau = backup(B_Actual, Tau);
                    errActual = calcRMSE(TauMinusOne, Tau);

                }

                long totalSecs = (Utils.UnixTimeStamp() - startTime);
                long RoundSecs = (Utils.UnixTimeStamp() - RoundStartTime);
                if (n > 0)
                {
                    long expectedTotalSecs = (totalSecs * N) / (n);
                    long expectedRemainingSecs = expectedTotalSecs - totalSecs;

                    writeln("Epoch: " + (n + 1) + "..." + Tau.Count
                            + " vectors - " + B_Actual.Count + " beliefs in "
                            + RoundSecs + " secs. Expected "
                            + expectedRemainingSecs + " secs. (" + totalSecs
                            + " total) (err=" + errActual + ")");
                }
                else
                    writeln("Epoch: " + (n + 1) + "..." + Tau.Count
                            + " vectors - " + B_Actual.Count + " beliefs in "
                            + RoundSecs + " secs. (" + totalSecs + " total)");

                if (n < N - 1)
                {
                    List<double[]> bnew = expand(B_Actual, Tau);
                    B_Actual = bnew.Select(p => (double[])p.Clone()).ToList(); // B = B U Bnew
                }
                RoundStartTime = Utils.UnixTimeStamp();
            }

            long totalTime = (Utils.UnixTimeStamp() - startTime);

            writeln("++++++++++++++++++++++++++++++++++++++++");
            writeln("Total planning time = "
                    + Utils.formatIntoHHMMSS(totalTime) + ". (= "
                    + totalTime + " secs)");

            return Tau;
        }

        public List<double[]> createTau0(List<double[]> Binit)
        {
            List<double[]> Tau0 = new List<double[]>();
            double[] B_Actual = Binit[0];
            double[] Alpha = new double[plannerModel.getModel().getS().Length];
            for (int sa = 0; sa < Alpha.Length; sa++)
                Alpha[sa] = Double.MinValue;

            // //

            for (int ai = 0; ai < plannerModel.getModel().getA().Length; ai++)
            {
                double rTest = 0;
                double rAlpha = 0;
                String a = plannerModel.getModel().getA()[ai];
                for (int si = 0; si < plannerModel.getModel().getS().Length; si++)
                {
                    rTest = rTest
                            + (plannerModel.getModel().getR(a)[si] * B_Actual[si]);

                    if (Alpha[si] == Double.MinValue)
                        rAlpha = rAlpha
                                + (plannerModel.getModel().getR(a)[si] * B_Actual[si]);
                    else
                        rAlpha = rAlpha + (Alpha[si] * B_Actual[si]);

                }

                if (rTest >= rAlpha)
                    Alpha = plannerModel.getModel().getR(a);
            }
            // --------------

            Tau0.Add(Alpha);

            return Tau0;
        }

        // -- Backup
        // -----------------------------------------------------------------


        public List<double[]> backup(List<double[]> B_Actual,
                List<double[]> Tau_1)
        {

            List<ArrayList> Tau_az = new List<ArrayList>();

            for (int ai = 0; ai < plannerModel.getModel().getA().Length; ai++)
            {

                if (cancelled)
                    break;

                String a = plannerModel.getModel().getA()[ai];
                // ----------------
                for (int oi = 0; oi < plannerModel.getModel().getO().Length; oi++)
                {

                    for (int Alphai = 0; Alphai < Tau_1.Count; Alphai++)
                    {
                        double[] Alpha_i = (double[])Tau_1[Alphai];

                        Double[] Alphai_az = new Double[plannerModel.getModel()
                                .getS().Length];

                        for (int si = 0; si < plannerModel.getModel().getS().Length; si++)
                        {

                            double sum_S = 0;
                            for (int sli = 0; sli < plannerModel.getModel().getS().Length; sli++)
                                sum_S = sum_S
                                        + (plannerModel.getModel().getTa(a)[si][sli]
                                                * plannerModel.getModel().getPa(a)[sli][oi] * Alpha_i[sli]);

                            Alphai_az[si] = Settings.gamma * sum_S;
                        }
                        ArrayList item = new ArrayList();
                        item.Add(ai); // 0
                        item.Add(oi); // 1
                        item.Add(Alphai_az); // 2

                        Tau_az.Add(item);

                        // writeln("#a" + ai + "/" + model.getA().Length
                        // + " o" + oi + "/" + model.getO().Length
                        // + " Alpha" + Alphai + "/" + Tau_1.Count);
                    }
                }
            }

            List<double[]> Tau = new List<double[]>();

            for (int bi = 0; bi < B_Actual.Count; bi++)
            {

                if (cancelled)
                    break;

                double[] b_actual = B_Actual[bi];

                ArrayList bestAlpha = getBestAlpha(b_actual, Tau_az);
                // 0 bestAlphaAValue
                // 1 bestAlphaABValue
                // 2 bestA
                // 3 bestAIndex
                // 4 bestAlphaA
                // 5 bestAlphaAB

                int bestAIndex = (int)bestAlpha[3];
                double[] Alpha = (double[])bestAlpha[5]; // double[] AlphaB =
                                                         // getAlphaB(bestAIndex,
                                                         // b_actual,
                                                         // Tau_az);

                polB.Add(b_actual, bestAIndex);
                polAlpha.Add(Alpha, bestAIndex);
                addAlphaBTau(Tau, Alpha);

                // if (bi == 0)
                // writeln("#Alpha " + AlphaAB[0]);

                // writeln("#bi" + bi + "/" + B_Actual.Count);
            }

            return Tau;
        }

        public void addNewB(List<double[]> B, double[] newB)
        {

            bool found = false;
            for (int i = 0; i < B.Count; i++)
            {

                bool eq = true;
                for (int si = 0; si < plannerModel.getModel().getS().Length; si++)
                {
                    if (Math.Abs(Utils.Round(B[i][si], Settings.dec)
                            - Utils.Round(newB[si], Settings.dec)) > Settings.diffErr)
                    {
                        eq = false;
                        break;
                    }
                }

                if (eq)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                B.Add(newB);
        }

        public void addAlphaBTau(List<double[]> Tau, double[] AlphaB)
        {

            bool found = false;
            for (int i = 0; i < Tau.Count; i++)
            {

                bool eq = true;
                for (int si = 0; si < plannerModel.getModel().getS().Length; si++)
                {
                    if (Math.Abs(Utils.Round(Tau[i][si], Settings.dec)
                            - Utils.Round(AlphaB[si], Settings.dec)) > Settings.diffErr)
                    {
                        eq = false;
                        break;
                    }
                }

                if (eq)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                Tau.Add(AlphaB);
        }




        public ArrayList getBestAlpha(double[] b_actual, List<ArrayList> Tau_az)
        {

            String bestA = "";
            int bestAIndex = -1;

            double bestAlphaAValue = Double.MinValue;
            double bestAlphaABValue = Double.MinValue;
            double[] bestAlphaA = null;
            double[] bestAlphaAB = null;

            for (int ai = 0; ai < plannerModel.getModel().getA().Length; ai++)
            {
                String a = plannerModel.getModel().getA()[ai];

                double parcialAlphaA = 0;
                double parcialAlphaAB = 0;

                double[] AlphaA = new double[plannerModel.getModel().getS().Length];
                double[] AlphaAB = new double[plannerModel.getModel().getS().Length];

                for (int si = 0; si < plannerModel.getModel().getS().Length; si++)
                {
                    double r_actual = (plannerModel.getModel().getR(a)[si] * b_actual[si]);

                    double o_actualA = 0;
                    double o_actualAB = 0;
                    for (int oi = 0; oi < plannerModel.getModel().getO().Length; oi++)
                    {
                        o_actualA = o_actualA
                                + ((Double[])maxTauAz(ai, oi, Tau_az, b_actual)[1])[si];

                        o_actualAB = o_actualAB
                                + (((Double[])maxTauAz(ai, oi, Tau_az, b_actual)[1])[si] * b_actual[si]);
                    }

                    AlphaA[si] = r_actual + o_actualA;
                    AlphaAB[si] = r_actual + o_actualAB;

                    parcialAlphaA = parcialAlphaA + AlphaA[si];
                    parcialAlphaAB = parcialAlphaAB + AlphaAB[si];
                }

                if (bestAlphaABValue == Double.MinValue
                        || parcialAlphaAB > bestAlphaABValue)
                {
                    bestAlphaAValue = parcialAlphaA;
                    bestAlphaABValue = parcialAlphaAB;
                    bestA = a;
                    bestAIndex = ai;

                    bestAlphaA = AlphaA;
                    bestAlphaAB = AlphaAB;
                }
            }
            ArrayList ret = new ArrayList();
            ret.Add(bestAlphaAValue);
            ret.Add(bestAlphaABValue);
            ret.Add(bestA);
            ret.Add(bestAIndex);
            ret.Add(bestAlphaA);
            ret.Add(bestAlphaAB);

            return ret;
        }




        public ArrayList maxTauAz(int ai, int oi, List<ArrayList> Tau_az, double[] b)
        {

            ArrayList ret = new ArrayList();
            Double result = Double.MinValue;
            Double[] bestAlpha = null;

            for (int i = 0; i < Tau_az.Count; i++)
            {

                ArrayList item = (ArrayList)Tau_az[i];
                if ((int)item[0] == ai && (int)item[1] == oi)
                {
                    double local = 0;
                    Double[] Alpha = (Double[])item[2];
                    for (int si = 0; si < plannerModel.getModel().getS().Length; si++)
                    {
                        local = local + (b[si] * Alpha[si]);
                    }

                    if (result == Double.MinValue || local > result)
                    {
                        result = local;
                        bestAlpha = Alpha; // best alfa deveria ser alfa * b, como o
                                           // local
                    }
                }

            }

            if (bestAlpha == null)
                writeln("null");

            ret.Add(result);
            ret.Add(bestAlpha);
            return ret;
        }

        public double calcRMSE(List<double[]> TauMinusOne,
                List<double[]> Tau)
        {
            double ret = -1.0;

            if (Tau.Count > 0)
            {
                ret = 0.0;
                double[] errSum = new Double[Tau[0].Length];
                Array.Copy(Tau[0], errSum, Tau[0].Length);

                for (int i = 0; i < Tau.Count; i++)
                {

                    double[] parcialErr = new Double[Tau[i].Length];
                    Array.Copy(Tau[i], parcialErr, Tau[i].Length);

                    double[] alfai = new Double[Tau[i].Length];
                    Array.Copy(Tau[i], alfai, Tau[i].Length);

                    double[] alfaiMinusOne = new double[alfai.Length];
                    if (i < TauMinusOne.Count)
                        alfaiMinusOne = (double[])TauMinusOne[i].Clone();

                    for (int si = 0; si < alfai.Length; si++)
                    {
                        if (i < TauMinusOne.Count)
                            parcialErr[si] = Math.Pow(
                                    (alfai[si] - alfaiMinusOne[si]), 2);

                        if (parcialErr[si] == Double.PositiveInfinity
                                || Double.IsNaN(parcialErr[si]))
                            parcialErr[si] = 0.0;
                        errSum[si] = errSum[si] + parcialErr[si];
                    }

                }

                for (int si = 0; si < errSum.Length; si++)
                {
                    errSum[si] = Math.Sqrt(errSum[si] / Tau.Count);

                    if (errSum[si] == Double.PositiveInfinity
                            || Double.IsNaN(errSum[si]))
                        errSum[si] = 0.0;

                    ret = ret + errSum[si];
                }
                ret = ret / errSum.Length;

            }

            return ret;
        }


    }

}