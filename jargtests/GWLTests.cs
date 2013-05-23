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

        [TestMethod]
        public void IsMinus12At1Night()
        {
            DateTime dt = new DateTime(1, 1, 1, 0, 0, 0);
            Assert.AreEqual(-12, GlobalWorldLogic.GetNormalTemp(dt));
        }

        [TestMethod]
        public void IsMinus4_2At2Day()
        {
            DateTime dt = new DateTime(1, 2, 1, 12, 0, 0);
            Assert.AreEqual(-4.2f, GlobalWorldLogic.GetNormalTemp(dt));
        }
    }
}
