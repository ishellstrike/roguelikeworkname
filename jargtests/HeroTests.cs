using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using rglikeworknamelib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Creatures;
using Microsoft.Xna.Framework;

namespace jargtests
{
    [TestClass]
    public class HeroTests
    {
        [TestMethod]
        public void CreatureHeroPositionChange()
        {
            Player pl = new Player();
            pl.Position = new Vector2(2, 2);
            Assert.AreEqual(pl.Position, new Vector2(2, 2));
        }

        //[TestMethod]
        //public void 
    }
}
