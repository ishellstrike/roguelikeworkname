using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Generation;
using Microsoft.Xna.Framework;

namespace jargtests
{
    [TestClass]
    public class MapGeneratorTests
    {
         [TestMethod]
         public void MinMaxLength()
         {
             MinMax mm = new MinMax(1, 1, 10, 1);
             Assert.AreEqual(9, mm.Length);
         }

    }
}
