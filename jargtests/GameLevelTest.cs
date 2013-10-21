using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace jargtests
{
    
    
    /// <summary>
    ///Это класс теста для GameLevelTest, в котором должны
    ///находиться все модульные тесты GameLevelTest
    ///</summary>
    [TestClass()]
    public class GameLevelTest
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

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext) {
        }

        /// <summary>
        ///Тест для ExploreAllMap
        ///</summary>
        //[TestMethod()]
        //public void ExploreAllMapTest()
        //{
        //    var target = new GameLevel_Accessor(2, 2);
        //    target.ExploreAllMap();

        //    Assert.IsTrue(target.blocks_[0].explored);
        //    Assert.IsTrue(target.blocks_[1].explored);
        //    Assert.IsTrue(target.blocks_[2].explored);
        //    Assert.IsTrue(target.blocks_[3].explored);
        //}

        ///// <summary>
        /////Тест для GetBlock
        /////</summary>
        //[TestMethod()]
        //public void GetBlockTest()
        //{
        //    var target = new GameLevel_Accessor(2, 2); 

        //    Block expected = target.blocks_[0];
        //    Block actual = target.GetBlock(0, 0);
        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        /////Тест для GetId
        /////</summary>
        //[TestMethod()]
        //public void GetIdTest()
        //{
        //    var target = new GameLevel_Accessor(3, 3);
        //    target.blocks_[4].id = 1;
        //    int expected = 1;
        //    int actual = target.GetId(1, 1);
        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        /////Тест для GetId
        /////</summary>
        //[TestMethod()]
        //public void GetIdTest1()
        //{
        //    var target = new GameLevel_Accessor(3, 3);
        //    target.blocks_[4].id = 1;
        //    int expected = 1;
        //    int actual = target.GetId(4);
        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        /////Тест для CreateAllMapFromArray
        /////</summary>
        //[TestMethod()]
        //public void CreateAllMapFromArrayTest()
        //{
        //    var target = new GameLevel_Accessor(3, 3);
        //    target.blockDataBase = blockDataBase;
            
        //    int[] arr = {0,0,0,0,1,0,0,0,0}; 
        //    target.CreateAllMapFromArray(arr);
        //    int expected = 1;
        //    int actual = target.blocks_[4].id;
        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        /////Тест для IsExplored
        /////</summary>
        //[TestMethod()]
        //public void IsExploredTest()
        //{
        //    var target = new GameLevel_Accessor(1, 1);
        //    bool actual = target.IsExplored(0);
        //    Assert.IsFalse(actual);
        //}

        ///// <summary>
        /////Тест для IsExplored
        /////</summary>
        //[TestMethod()]
        //public void IsExploredTest1()
        //{
        //    var target = new GameLevel_Accessor(1, 1);
        //    bool actual = target.IsExplored(0,0);
        //    Assert.IsFalse(actual);
        //}

        /// <summary>
        ///Тест для IsCreatureMeele
        ///</summary>
        [TestMethod()]
        public void IsCreatureMeeleTest()
        {
           
        }

        /// <summary>
        ///Тест для GetStorageBlocks
        ///</summary>
        [TestMethod()]
        public void GetStorageBlocksTest()
        {
           
        }

        /// <summary>
        ///Тест для SetBlock
        ///</summary>
        //[TestMethod()]
        //public void SetBlockTest()
        //{
        //    var target = new GameLevel_Accessor(1,1);
        //    target.blockDataBase = blockDataBase;

        //    Vector2 where = new Vector2(0,0);
        //    int id = 1; 
        //    target.SetBlock(where, id);
        //    Assert.AreEqual(target.blocks_[0].id, 1);
        //}

        ///// <summary>
        /////Тест для SetBlock
        /////</summary>
        //[TestMethod()]
        //public void SetBlockTest1()
        //{
        //    var target = new GameLevel_Accessor(1, 1);
        //    target.blockDataBase = blockDataBase;

        //    int id = 1;
        //    target.SetBlock(0, 0, id);
        //    Assert.AreEqual(target.blocks_[0].id, 1);
        //}

        ///// <summary>
        /////Тест для SetBlock
        /////</summary>
        //[TestMethod()]
        //public void SetBlockTest2()
        //{
        //    var target = new GameLevel_Accessor(1, 1);
        //    target.blockDataBase = blockDataBase;

        //    int id = 1;
        //    target.SetBlock(0, id);
        //    Assert.AreEqual(target.blocks_[0].id, 1);
        //}

        ///// <summary>
        /////Тест для SetFloor
        /////</summary>
        //[TestMethod()]
        //public void SetFloorTest()
        //{
        //    var target = new GameLevel_Accessor(1, 1);
        //    target.floorDataBase = floorDataBase;

        //    int id = 1;
        //    target.SetFloor(0, 0, id);
        //    Assert.AreEqual(target.floors_[0].ID, 1);
        //}
    }
}
