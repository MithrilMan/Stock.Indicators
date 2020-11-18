﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class SlopeTests : TestBase
    {

        [TestMethod()]
        public void GetSlope()
        {
            int lookbackPeriod = 20;
            List<SlopeResult> results = Indicator.GetSlope(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Slope != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.StdDev != null).Count());
            Assert.AreEqual(lookbackPeriod, results.Where(x => x.Line != null).Count());

            // sample values
            SlopeResult r1 = results[501];
            Assert.AreEqual(-1.689143m, Math.Round((decimal)r1.Slope, 6));
            Assert.AreEqual(1083.7629m, Math.Round((decimal)r1.Intercept, 4));
            Assert.AreEqual(0.7955m, Math.Round((decimal)r1.RSquared, 4));
            Assert.AreEqual(10.9202m, Math.Round((decimal)r1.StdDev, 4));
            Assert.AreEqual(235.8131m, Math.Round((decimal)r1.Line, 4));

            SlopeResult r2 = results[482];
            Assert.AreEqual(-0.337015m, Math.Round((decimal)r2.Slope, 6));
            Assert.AreEqual(425.1111m, Math.Round((decimal)r2.Intercept, 4));
            Assert.AreEqual(0.1730m, Math.Round((decimal)r2.RSquared, 4));
            Assert.AreEqual(4.6719m, Math.Round((decimal)r2.StdDev, 4));
            Assert.AreEqual(267.9069m, Math.Round((decimal)r2.Line, 4));

            SlopeResult r3 = results[249];
            Assert.AreEqual(0.312406m, Math.Round((decimal)r3.Slope, 6));
            Assert.AreEqual(180.4164m, Math.Round((decimal)r3.Intercept, 4));
            Assert.AreEqual(0.8056m, Math.Round((decimal)r3.RSquared, 4));
            Assert.AreEqual(2.0071m, Math.Round((decimal)r3.StdDev, 4));
            Assert.AreEqual(null, r3.Line);
        }

        [TestMethod()]
        public void GetSlopeBadData()
        {
            IEnumerable<SlopeResult> r = Indicator.GetSlope(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetSlope(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(29);
            Indicator.GetSlope(h, 30);
        }
    }
}