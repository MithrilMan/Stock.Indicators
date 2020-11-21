﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HULL MOVING AVERAGE
        public static IEnumerable<HmaResult> GetHma(IEnumerable<IQuote> history, int lookbackPeriod)
        {

            // clean quotes
            List<IQuote> historyList = history.Sort();

            // check parameters
            ValidateHma(history, lookbackPeriod);

            // initialize
            List<IQuote> synthHistory = new List<IQuote>();

            List<WmaResult> wmaN1 = GetWma(history, lookbackPeriod).ToList();
            List<WmaResult> wmaN2 = GetWma(history, lookbackPeriod / 2).ToList();

            // create interim synthetic history
            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                IQuote h = historyList[i];

                IQuote sh = new Quote
                {
                    Date = h.Date
                };

                WmaResult w1 = wmaN1[i];
                WmaResult w2 = wmaN2[i];

                if (w1.Wma != null && w2.Wma != null)
                {
                    sh.Close = (decimal)(w2.Wma * 2m - w1.Wma);
                    synthHistory.Add(sh);
                }
            }

            // initialize results, add back truncated null results
            int sqN = (int)Math.Sqrt(lookbackPeriod);
            int shiftQty = lookbackPeriod - 1;

            List<HmaResult> results = historyList
                .Take(shiftQty)
                .Select(x => new HmaResult
                {
                    Date = x.Date
                })
                .ToList();

            // calculate final HMA = WMA with period SQRT(n)
            List<HmaResult> hmaResults = GetWma(synthHistory, sqN)
                .Select(x => new HmaResult
                {
                    Date = x.Date,
                    Hma = x.Wma
                })
                .ToList();

            // add WMA to results
            results.AddRange(hmaResults);
            results = results.OrderBy(x => x.Date).ToList();

            return results;
        }


        private static void ValidateHma(IEnumerable<IQuote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for HMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for HMA.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
