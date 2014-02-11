using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Parser;

namespace jargtests {
    /// <summary>
    ///     Это класс теста для UniversalParserTest, в котором должны
    ///     находиться все модульные тесты UniversalParserTest
    /// </summary>
    [TestClass]
    public class UniversalParserTest {
        /// <summary>
        ///     Получает или устанавливает контекст теста, в котором предоставляются
        ///     сведения о текущем тестовом запуске и обеспечивается его функциональность.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Дополнительные атрибуты теста

        #endregion

        /// <summary>
        ///     Тест для Parser
        /// </summary>
        public KeyValuePair<string, object> ParserTestHelper<T>(string s, string filePos, Type basetype) {
            List<KeyValuePair<string, object>> actual = UniversalParser.Parser<T>(s, filePos, basetype);
            return actual[0];
        }

        [TestMethod]
        public void ParserTest() {
            KeyValuePair<string, object> a = ParserTestHelper<BlockData>("~Block,1\nName=Imya", @"C:\filepos\",
                                                                         typeof (Block));
            Assert.AreEqual("Imya", ((BlockData) a.Value).Name);
        }

        [TestMethod]
        public void ParserTest2() {
            KeyValuePair<string, object> a = ParserTestHelper<BlockData>("~Block,1\nololo=Imya", @"C:\filepos\",
                                                                         typeof (Block));
            Assert.AreEqual(null, ((BlockData) a.Value).Name);
        }


        [TestMethod]
        public void ParserTestNoPrototypeField() {
            KeyValuePair<string, object> a = ParserTestHelper<FloorData>("~Floor,1", @"C:\filepos\", typeof (Floor));
            Assert.AreEqual("1", a.Key);
        }

        [TestMethod]
        public void ParserTestArray() {
            KeyValuePair<string, object> a = ParserTestHelper<BlockData>("~Block,1\nAlterMtex={1,2,3}", @"C:\filepos\",
                                                                         typeof (Block));
            CollectionAssert.AreEqual(new[] {"1", "2", "3"}, ((BlockData) a.Value).AlterMtex);
        }

        /// <summary>
        ///     Тест для NoIdParser
        /// </summary>
        public object NoIdParserTestHelper<T>(string s, string filePos, Type basetype) {
            List<object> actual = UniversalParser.NoIdParser<T>(s, filePos, basetype);
            return actual[0];
        }

        [TestMethod]
        public void NoIdParserTest() {
            object a = NoIdParserTestHelper<BlockData>("~Block\nName=Imya", @"C:\filepos\", typeof (Block));
            Assert.AreEqual("Imya", ((BlockData) a).Name);
        }
    }
}