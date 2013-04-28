using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rglikeworknamelib.Dungeon;

namespace jargtests
{
    [TestClass]
    public class GWLTests
    {
        [TestMethod]
        public void IsNightAt0()
        {
            DateTime td = new DateTime(1, 10, 1, 0, 0, 0);
            Assert.IsTrue(GlobalWorldLogic.IsNight(td));
        }

        [TestMethod]
        public void IsDayAt12()
        {
            DateTime td = new DateTime(1, 10, 1, 12, 0, 0);
            Assert.IsTrue(GlobalWorldLogic.IsDay(td));
        }

        [TestMethod]
        public void IsWinter2()
        {
            DateTime td = new DateTime(1, 2, 1);
            Assert.IsTrue(GlobalWorldLogic.IsWinter(td));
        }

        [TestMethod]
        public void IsWinter11()
        {
            DateTime td = new DateTime(1, 11, 1);
            Assert.IsTrue(GlobalWorldLogic.IsWinter(td));
        }

        [TestMethod]
        public void IsNotWinter7()
        {
            DateTime td = new DateTime(1, 7, 1);
            Assert.IsFalse(GlobalWorldLogic.IsWinter(td));
        }
    }
}
