using Assets.src.ThreadingExample;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.ThreadingExample {
    [TestClass()]
    public class ThreadExampleTests {
        [TestMethod()]
        public void MainTest() {
            ThreadExample.Main();
        }
    }
}