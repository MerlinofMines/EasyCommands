using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Globalization;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommands.Tests.ScriptTests
{
    [TestClass]
    public class SorterBlockTests
    {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void SorterCanDrain()
        {
            String script = @"
:main
drain the ""sorters""
";
            using (ScriptTest test = new ScriptTest(script))
            {
                var mockSorter = new Mock<IMyConveyorSorter>();
                test.MockBlocksInGroup("sorters", mockSorter);

                test.RunOnce();

                mockSorter.VerifySet(s => s.DrainAll = true);
            }
        }

        [TestMethod]
        public void SorterIsDraining()
        {
            String script = @"
:main
if ""sorters"" are draining
    Print ""Call a plumber, dude""
";
            using (ScriptTest test = new ScriptTest(script))
            {
                var mockSorter = new Mock<IMyConveyorSorter>();
                mockSorter.Setup(s => s.DrainAll).Returns(true);
                test.MockBlocksInGroup("sorters", mockSorter);

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Call a plumber, dude"));
                mockSorter.Verify(s => s.DrainAll);
            }
        }
    }
}
