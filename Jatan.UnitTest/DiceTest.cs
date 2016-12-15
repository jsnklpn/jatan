using System;
using System.Collections.Generic;
using Jatan.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jatan.UnitTest
{
    [TestClass]
    public class DiceTest
    {
        [TestMethod]
        public void TestRoll()
        {
            var dice = new Dice();

            int low = int.MaxValue;
            int high = int.MinValue;
            for (int i = 0; i < 10000; i++)
            {
                int roll = dice.Roll();
                if (roll < low)
                    low = roll;
                if (roll > high)
                    high = roll;
            }

            Assert.AreEqual(2, low, "The lowest roll should be a 2.");
            Assert.AreEqual(12, high, "The highest roll should be a 12.");
        }

        [TestMethod]
        public void TestLog()
        {
            var dice = new Dice();
            int rollCount = 100;
            var log = new int[rollCount];

            for (int i = 0; i < rollCount; i++)
            {
                int roll = dice.Roll();
                log[i] = roll;
            }

            Assert.AreEqual(rollCount, dice.RollLog.Count, "The log is not the correct size.");
            for (int i = 0; i < rollCount; i++)
            {
                Assert.AreEqual(log[i], dice.RollLog[i], "The values in the log should be the same.");
            }
        }
    }
}
