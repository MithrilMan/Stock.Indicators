﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class SmaTests : TestBase
    {

        [TestMethod()]
        public void GetSma()
        {
            int lookbackPeriod = 20;
            List<SmaResult> results = Indicator.GetSma(history, lookbackPeriod, true).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Sma != null).Count());

            // sample value
            SmaResult r = results[501];
            Assert.AreEqual(251.86m, r.Sma);
            Assert.AreEqual(9.45m, r.Mad);
            Assert.AreEqual(119.2510m, Math.Round((decimal)r.Mse, 4));
            Assert.AreEqual(0.037637m, Math.Round((decimal)r.Mape, 6));
        }

        [TestMethod()]
        public void GetSmaBadData()
        {
            IEnumerable<SmaResult> r = Indicator.GetSma(historyBad, 15, true);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetSma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(9);
            Indicator.GetSma(h, 10);
        }
    }
}