using Microsoft.VisualStudio.TestTools.UnitTesting;
using rglikeworknamelib.Generation;

namespace jargtests {
    [TestClass]
    public class MapGeneratorTests {
        [TestMethod]
        public void MinMaxLength() {
            var mm = new MinMax(1, 1, 10, 1);
            Assert.AreEqual(9, mm.Length);
        }
    }
}