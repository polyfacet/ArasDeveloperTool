using System;
using Hille.Aras.DevTool.Interfaces.Aras;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestArasDevTool.ArasDevToolInterfaceTests {
    [TestClass]
    public class ConnectionTests {
        [TestMethod]
        public void GetDatabaseList() {
            string[] dbList = ArasConnection.GetDBList(TestData.URL);
            Assert.IsNotNull(dbList);
            if (dbList.Length != 2) {
                Assert.Inconclusive("Expecting two databases");
            }
            Console.WriteLine("DB count: " + dbList.Length);
            foreach (string db in dbList) {
                Console.WriteLine(db);
            }
        }
    }
}
