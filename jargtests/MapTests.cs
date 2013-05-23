using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Creatures;
using Microsoft.Xna.Framework;

namespace jargtests
{
    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void IsInMapBounds0_0()
        {
            GameLevel gl = new GameLevel(10, 10);

            Assert.IsTrue(gl.IsInMapBounds(0, 0));
        }

        [TestMethod]
        public void IsInMapBoundsrx_ry()
        {
            GameLevel gl = new GameLevel(15, 15);

            Assert.IsTrue(gl.IsInMapBounds(15 - 1, 15 - 1));
        }

        [TestMethod]
        public void IsNotInMapBounds1_1()
        {
            GameLevel gl = new GameLevel(10, 10);

            Assert.IsFalse(gl.IsInMapBounds(-1, -1));
        }

        [TestMethod]
        public void IsInMapBoundsrx_ry2()
        {
            GameLevel gl = new GameLevel(15, 15);

            Assert.IsFalse(gl.IsInMapBounds(15, 15));
        }

        [TestMethod]
        public void IsMelee()
        {
            GameLevel glg = new GameLevel(10, 10);
            Player pl = new Player();
            pl.SetPositionInBlocks(2, 2);

            Assert.IsTrue(glg.IsCreatureMeele(1, 2, pl));
            Assert.IsTrue(glg.IsCreatureMeele(2, 1, pl));
            Assert.IsTrue(glg.IsCreatureMeele(3, 2, pl));
            Assert.IsTrue(glg.IsCreatureMeele(2, 3, pl));

            Assert.IsTrue(glg.IsCreatureMeele(1, 1, pl));
            Assert.IsTrue(glg.IsCreatureMeele(3, 1, pl));
            Assert.IsTrue(glg.IsCreatureMeele(3, 1, pl));
            Assert.IsTrue(glg.IsCreatureMeele(3, 3, pl));
        }

        [TestMethod]
        public void IsNotMelee()
        {
            GameLevel glg = new GameLevel(10, 10);
            Player pl = new Player();
            pl.SetPositionInBlocks(2, 2);

            Assert.IsFalse(glg.IsCreatureMeele(1, 4, pl));
        }

        [TestMethod]
        public void InMapBounds()
        {
            GameLevel gl = new GameLevel(10, 10);
            Assert.IsTrue(gl.IsInMapBounds(new Vector2(1,1)));
        }

        [TestMethod]
        public void NotInMapBounds()
        {
            GameLevel gl = new GameLevel(10, 10);
            Assert.IsFalse(gl.IsInMapBounds(new Vector2(10, 10)));
        }
    }
}
