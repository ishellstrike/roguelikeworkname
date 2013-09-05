using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace jargtests
{
    
    
    /// <summary>
    ///Это класс теста для UniversalParserTest, в котором должны
    ///находиться все модульные тесты UniversalParserTest
    ///</summary>
    [TestClass()]
    public class UniversalParserTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Дополнительные атрибуты теста
        // 
        //При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        //ClassInitialize используется для выполнения кода до запуска первого теста в классе
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //TestInitialize используется для выполнения кода перед запуском каждого теста
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //TestCleanup используется для выполнения кода после завершения каждого теста
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Тест для Parser
        ///</summary>
        public KeyValuePair<string, object> ParserTestHelper<T>(string s, string filePos, Type basetype)
        {
            List<KeyValuePair<string, object>> actual = UniversalParser.Parser<T>(s, filePos, basetype);
            return actual[0];
        }

        [TestMethod()]
        public void ParserTest()
        {
            var a = ParserTestHelper<BlockData>("~Block,1\nName=Imya", @"C:\filepos\", typeof(Block));
            Assert.AreEqual("Imya", ((BlockData)a.Value).Name);
        }

        [TestMethod()]
        public void ParserTest2()
        {
            var a = ParserTestHelper<BlockData>("~Block,1\nololo=Imya", @"C:\filepos\", typeof(Block));
            Assert.AreEqual(null, ((BlockData)a.Value).Name);
        }

        [TestMethod()]
        public void ParserTest3()
        {
            var a = ParserTestHelper<BlockData>("~StorageBlock,1", @"C:\filepos\", typeof(Block));
            Assert.AreEqual(typeof(StorageBlock), ((BlockData)a.Value).Prototype);
        }

        [TestMethod()]
        public void ParserTest4()
        {
            var a = ParserTestHelper<BlockData>("~NoSuchBlock,1", @"C:\filepos\", typeof(Block));
            Assert.AreEqual(typeof(Block), ((BlockData)a.Value).Prototype);
        }

        [TestMethod()]
        public void ParserTest5()
        {
            var a = ParserTestHelper<BlockData>("~NoSuchBlock,1", @"C:\filepos\", typeof(Block));
            Assert.AreEqual(typeof(Block), ((BlockData)a.Value).Prototype);
        }

        [TestMethod()]
        public void ParserTestNoPrototypeField()
        {
            var a = ParserTestHelper<FloorData>("~Floor,1", @"C:\filepos\", typeof(Floor));
            Assert.AreEqual("1", a.Key);
        }

        [TestMethod()]
        public void ParserTestArray()
        {
            var a = ParserTestHelper<BlockData>("~Block,1\nAlterMtex={1,2,3}", @"C:\filepos\", typeof(Block));
            CollectionAssert.AreEqual(new[] {"1","2","3"}, ((BlockData)a.Value).AlterMtex);
        }

        /// <summary>
        ///Тест для NoIdParser
        ///</summary>
        public object NoIdParserTestHelper<T>(string s, string filePos, Type basetype)
        {
            List<object> actual = UniversalParser.NoIdParser<T>(s, filePos, basetype);
            return actual[0];
        }

        [TestMethod()]
        public void NoIdParserTest()
        {
            var a = NoIdParserTestHelper<BlockData>("~Block\nName=Imya", @"C:\filepos\", typeof(Block));
            Assert.AreEqual("Imya", ((BlockData)a).Name);
        }
    }
}
