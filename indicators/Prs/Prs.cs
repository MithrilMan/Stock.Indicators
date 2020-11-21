using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PRICE RELATIVE STRENGTH
        public static IEnumerable<PrsResult> GetPrs(
            IEnumerable<IQuote> historyBase, IEnumerable<IQuote> historyEval, int? lookbackPeriod = null, int? smaPeriod = null)
        {
            // clean quotes
            List<IQuote> historyBaseList = historyBase.Sort();
            List<IQuote> historyEvalList = historyEval.Sort();

            // validate parameters
            ValidatePriceRelative(historyBase, historyEval, lookbackPeriod, smaPeriod);

            // initialize
            List<PrsResult> results = new List<PrsResult>();


            // roll through history for interim data
            for (int i = 0; i < historyEvalList.Count; i++)
            {
                IQuote bi = historyBaseList[i];
                IQuote ei = historyEvalList[i];
                int index = i + 1;

                if (ei.Date != bi.Date)
                {
                    throw new BadHistoryException(nameof(historyEval), ei.Date,
                        "Date sequence does not match.  Price Relative requires matching dates in provided histories.");
                }

                PrsResult r = new PrsResult
                {
                    Date = ei.Date,
                    Prs = (bi.Close == 0) ? null : ei.Close / bi.Close  // relative strength ratio
                };
                results.Add(r);

                if (lookbackPeriod != null && index > lookbackPeriod)
                {
                    IQuote bo = historyBaseList[i - (int)lookbackPeriod];
                    IQuote eo = historyEvalList[i - (int)lookbackPeriod];

                    if (bo.Close != 0 && eo.Close != 0)
                    {
                        decimal pctB = (bi.Close - bo.Close) / bo.Close;
                        decimal pctE = (ei.Close - eo.Close) / eo.Close;

                        r.PrsPercent = pctE - pctB;
                    }
                }

                // optional moving average of PRS
                if (smaPeriod != null && index >= smaPeriod)
                {
                    decimal? sumRs = 0m;
                    for (int p = index - (int)smaPeriod; p < index; p++)
                    {
                        PrsResult d = results[p];
                        sumRs += d.Prs;
                    }
                    r.Sma = sumRs / smaPeriod;
                }

            }

            return results;
        }


        private static void ValidatePriceRelative(
            IEnumerable<IQuote> historyBase, IEnumerable<IQuote> historyEval, int? lookbackPeriod, int? smaPeriod)
        {

            // check parameters
            if (lookbackPeriod != null && lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Price Relative Strength.");
            }

            if (smaPeriod != null && smaPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriod), smaPeriod,
                    "SMA period must be greater than 0 for Price Relative Strength.");
            }

            // check history

            int qtyHistoryEval = historyEval.Count();
            int qtyHistoryBase = historyBase.Count();

            int? minHistory = lookbackPeriod;
            if (minHistory != null && qtyHistoryEval < minHistory)
            {
                string message = "Insufficient history provided for Price Relative Strength.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistoryEval, minHistory);

                throw new BadHistoryException(nameof(historyEval), message);
            }

            if (qtyHistoryBase != qtyHistoryEval)
            {
                throw new BadHistoryException(nameof(historyBase),
                    "Base history should have at least as many records as Eval history for Price Relative.");
            }

            // NOTE: history of less than 1 is caught in the Cleaner
        }
    }

}
