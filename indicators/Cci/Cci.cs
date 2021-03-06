﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // COMMODITY CHANNEL INDEX
        public static IEnumerable<CciResult> GetCci(IEnumerable<Quote> history, int lookbackPeriod = 20)
        {

            // clean quotes
            List<Quote> historyList = history.Sort();

            // validate parameters
            ValidateCci(history, lookbackPeriod);

            // initialize
            List<CciResult> results = new List<CciResult>();


            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];
                int index = i + 1;

                CciResult result = new CciResult
                {
                    Date = h.Date,
                    Tp = (h.High + h.Low + h.Close) / 3
                };
                results.Add(result);

                if (index >= lookbackPeriod)
                {
                    // average TP over lookback
                    decimal avgTp = 0;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        CciResult d = results[p];
                        avgTp += (decimal)d.Tp;
                    }
                    avgTp /= lookbackPeriod;

                    // average Deviation over lookback
                    decimal avgDv = 0;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        CciResult d = results[p];
                        avgDv += Math.Abs(avgTp - (decimal)d.Tp);
                    }
                    avgDv /= lookbackPeriod;

                    result.Cci = (avgDv == 0) ? null
                        : (result.Tp - avgTp) / ((decimal)0.015 * avgDv);
                }
            }

            return results;
        }


        private static void ValidateCci(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Commodity Channel Index.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Commodity Channel Index.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
