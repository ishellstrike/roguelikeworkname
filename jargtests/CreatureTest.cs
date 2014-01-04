using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace jargtests {
    /// <summary>
    ///     Это класс теста для CreatureTest, в котором должны
    ///     находиться все модульные тесты CreatureTest
    /// </summary>
    [TestClass]
    public class CreatureTest {
        /// <summary>
        ///     Тест для LastPos
        /// </summary>
        [TestMethod]
        public void LastPosTest() {
            var target = new Creature();
            var expected = new Vector2(3, 3);
            var newpos = new Vector2(5, 5);
            target.Position = expected;
            target.Position = newpos;
            Vector2 actual = target.LastPos;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     Тест для Position
        /// </summary>
        [TestMethod]
        public void PositionTest() {
            var target = new Creature();
            var expected = new Vector2(3, 3);
            Vector2 actual;
            target.Position = expected;
            actual = target.Position;
            Assert.AreEqual(expected, actual);
        }
    }
}