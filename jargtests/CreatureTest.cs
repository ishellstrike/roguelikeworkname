using rglikeworknamelib.Creatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;

namespace jargtests
{
    
    
    /// <summary>
    ///Это класс теста для CreatureTest, в котором должны
    ///находиться все модульные тесты CreatureTest
    ///</summary>
    [TestClass()]
    public class CreatureTest
    {
        /// <summary>
        ///Тест для LastPos
        ///</summary>
        [TestMethod()]
        public void LastPosTest()
        {
            Creature target = new Creature();
            Vector3 expected = new Vector3(3,3,0);
            Vector3 newpos = new Vector3(5,5,0);
            target.Position = expected;
            target.Position = newpos;
            Vector3 actual;
            actual = target.LastPos;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для Position
        ///</summary>
        [TestMethod()]
        public void PositionTest()
        {
            Creature target = new Creature(); 
            Vector3 expected = new Vector3(3,3,0); 
            Vector3 actual;
            target.Position = expected;
            actual = target.Position;
            Assert.AreEqual(expected, actual);
        }
    }
}
