﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class MfiTests : TestBase
    {

        [TestMethod()]
        public void GetMfi()
        {
            int lookbackPeriod = 14;
            List<MfiResult> results = Indicator.GetMfi(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod, results.Where(x => x.Mfi != null).Count());

            // sample values
            MfiResult r1 = results[501];
            Assert.AreEqual(39.9494m, Math.Round((decimal)r1.Mfi, 4));

            MfiResult r2 = results[439];
            Assert.AreEqual(69.0622m, Math.Round((decimal)r2.Mfi, 4));
        }

        [TestMethod()]
        public void GetMfiBadData()
        {
            IEnumerable<MfiResult> r = Indicator.GetMfi(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void GetMfiSmall()
        {
            int lookbackPeriod = 4;
            List<MfiResult> results = Indicator.GetMfi(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod, results.Where(x => x.Mfi != null).Count());

            // sample values
            MfiResult r1 = results[31];
            Assert.AreEqual(100m, Math.Round((decimal)r1.Mfi, 4));

            MfiResult r2 = results[43];
            Assert.AreEqual(0m, Math.Round((decimal)r2.Mfi, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetMfi(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for N+1.")]
        public void InsufficientHistoryA()
        {
            IEnumerable<Quote> h = History.GetHistory(14);
            Indicator.GetMfi(h, 14);
        }

    }
}