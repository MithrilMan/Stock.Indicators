﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CORRELATION COEFFICIENT
        public static IEnumerable<CorrResult> GetCorrelation(
            IEnumerable<Quote> historyA, IEnumerable<Quote> historyB, int lookbackPeriod)
        {
            // clean quotes
            List<Quote> historyListA = historyA.Sort();
            List<Quote> historyListB = historyB.Sort();

            // validate parameters
            ValidateCorrelation(historyA, historyB, lookbackPeriod);

            // initialize
            List<CorrResult> results = new List<CorrResult>();


            // roll through history for interim data
            for (int i = 0; i < historyListA.Count; i++)
            {
                Quote a = historyListA[i];
                Quote b = historyListB[i];
                int index = i + 1;

                if (a.Date != b.Date)
                {
                    throw new BadHistoryException(nameof(historyA), a.Date,
                        "Date sequence does not match.  Correlation requires matching dates in provided histories.");
                }

                CorrResult r = new CorrResult
                {
                    Date = a.Date
                };

                // compute correlation
                if (index >= lookbackPeriod)
                {
                    decimal sumPriceA = 0m;
                    decimal sumPriceB = 0m;
                    decimal sumPriceA2 = 0m;
                    decimal sumPriceB2 = 0m;
                    decimal sumPriceAB = 0m;

                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        Quote qa = historyListA[p];
                        Quote qb = historyListB[p];

                        sumPriceA += qa.Close;
                        sumPriceB += qb.Close;
                        sumPriceA2 += qa.Close * qa.Close;
                        sumPriceB2 += qb.Close * qb.Close;
                        sumPriceAB += qa.Close * qb.Close;
                    }

                    decimal avgA = sumPriceA / lookbackPeriod;
                    decimal avgB = sumPriceB / lookbackPeriod;
                    decimal avgA2 = sumPriceA2 / lookbackPeriod;
                    decimal avgB2 = sumPriceB2 / lookbackPeriod;
                    decimal avgAB = sumPriceAB / lookbackPeriod;

                    r.VarianceA = avgA2 - avgA * avgA;
                    r.VarianceB = avgB2 - avgB * avgB;
                    r.Covariance = avgAB - avgA * avgB;

                    r.Correlation = (r.VarianceA == 0 || r.VarianceB == 0) ? null
                       : r.Covariance / (decimal)Math.Sqrt((double)(r.VarianceA * r.VarianceB));

                    r.RSquared = r.Correlation * r.Correlation;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateCorrelation(IEnumerable<Quote> historyA, IEnumerable<Quote> historyB, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Correlation.");
            }

            // check history
            int qtyHistoryA = historyA.Count();
            int minHistoryA = lookbackPeriod;
            if (qtyHistoryA < minHistoryA)
            {
                string message = "Insufficient history provided for Correlation.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistoryA, minHistoryA);

                throw new BadHistoryException(nameof(historyA), message);
            }

            int qtyHistoryB = historyB.Count();
            if (qtyHistoryB != qtyHistoryA)
            {
                throw new BadHistoryException(nameof(historyB),
                    "B history should have at least as many records as A history for Correlation.");
            }
        }
    }

}
