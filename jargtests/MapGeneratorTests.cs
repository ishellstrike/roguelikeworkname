using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rglikeworknamelib.Generation;
using Microsoft.Xna.Framework;

namespace jargtests
{
    [TestClass]
    public class MapGeneratorTests
    {
        [TestMethod]
        public void SingleSquareFrom4Nodes()
        {
            List<MinMax> complete = new List<MinMax>(){new MinMax(1, 1, 2, 2)};
            Point[] a = { new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(2, 1) };
            var real = MapGenerators.GetSquaresFromNodes(a.ToList());
            CollectionAssert.AreEqual(complete, real);
        }

         [TestMethod]
        public void SingleSquareFrom6Nodes()
        {
            List<MinMax> complete = new List<MinMax>() { new MinMax(1, 1, 2, 2), new MinMax(1, 2, 2, 3) };
            Point[] a = { new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(2, 1), new Point(1, 3) };
            var real = MapGenerators.GetSquaresFromNodes(a.ToList());
            CollectionAssert.AreEqual(complete, real);
        }

         [TestMethod]
         public void MinMaxLength()
         {
             MinMax mm = new MinMax(1, 1, 10, 1);
             Assert.AreEqual(9, mm.Length);
         }
    }
}
