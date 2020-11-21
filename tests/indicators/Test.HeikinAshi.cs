﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class HeikinAshiTests : TestBase
    {

        [TestMethod()]
        public void GetHeikinAshi()
        {

            List<HeikinAshiResult> results = Indicator.GetHeikinAshi(history).ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);

            // sample value
            HeikinAshiResult r = results[501];
            Assert.AreEqual(241.3018m, Math.Round(r.Open, 4));
            Assert.AreEqual(245.54m, Math.Round(r.High, 4));
            Assert.AreEqual(241.3018m, Math.Round(r.Low, 4));
            Assert.AreEqual(244.6525m, Math.Round(r.Close, 4));
        }

        [TestMethod()]
        public void GetHeikinAshiBadData()
        {
            IEnumerable<HeikinAshiResult> r = Indicator.GetHeikinAshi(historyBad);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<IQuote> h = History.GetHistory(1);
            Indicator.GetHeikinAshi(h);
        }

    }
}