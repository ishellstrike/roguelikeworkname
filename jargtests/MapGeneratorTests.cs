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
            Vector2[] a = {new Vector2(1, 1), new Vector2(1, 2),new Vector2(2, 2), new Vector2(2, 1)};
            var real = MapGenerators.GetSquaresFromNodes(a.ToList());
            CollectionAssert.AreEqual(real, complete);
        }

         [TestMethod]
        public void SingleSquareFrom6Nodes()
        {
            List<MinMax> complete = new List<MinMax>() { new MinMax(1, 1, 2, 2), new MinMax(1, 2, 2, 3) };
            Vector2[] a = { new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2), new Vector2(2, 1), new Vector2(1, 3)};
            var real = MapGenerators.GetSquaresFromNodes(a.ToList());
            CollectionAssert.AreEqual(real, complete);
        }
    }
}
