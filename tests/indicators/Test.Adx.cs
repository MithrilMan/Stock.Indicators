﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class AvgDirectionalTests : TestBase
    {

        [TestMethod()]
        public void GetAdx()
        {
            int lookbackPeriod = 14;
            List<AdxResult> results = Indicator.GetAdx(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(475, results.Where(x => x.Adx != null).Count());

            // sample values
            AdxResult r1 = results[501];
            Assert.AreEqual(17.7565m, Math.Round((decimal)r1.Pdi, 4));
            Assert.AreEqual(31.1510m, Math.Round((decimal)r1.Mdi, 4));
            Assert.AreEqual(34.2987m, Math.Round((decimal)r1.Adx, 4));

            AdxResult r2 = results[248];
            Assert.AreEqual(32.3167m, Math.Round((decimal)r2.Pdi, 4));
            Assert.AreEqual(18.2471m, Math.Round((decimal)r2.Mdi, 4));
            Assert.AreEqual(30.5903m, Math.Round((decimal)r2.Adx, 4));

            AdxResult r3 = results[19];
            Assert.AreEqual(21.0361m, Math.Round((decimal)r3.Pdi, 4));
            Assert.AreEqual(25.0124m, Math.Round((decimal)r3.Mdi, 4));
            Assert.AreEqual(null, r3.Adx);

            AdxResult r4 = results[29];
            Assert.AreEqual(37.9719m, Math.Round((decimal)r4.Pdi, 4));
            Assert.AreEqual(14.1658m, Math.Round((decimal)r4.Mdi, 4));
            Assert.AreEqual(19.7949m, Math.Round((decimal)r4.Adx, 4));
        }

        [TestMethod()]
        public void GetAdxBadData()
        {
            IEnumerable<AdxResult> r = Indicator.GetAdx(historyBad, 20);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback period.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetAdx(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(159);
            Indicator.GetAdx(h, 30);
        }

    }
}