using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests
{
    [TestClass]
    public abstract class ForceLocale {
        [TestInitialize]
        public void SetLocale ()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }
    }
}
