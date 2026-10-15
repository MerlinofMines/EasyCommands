using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class RegistrationRegressionTests {
        [TestMethod]
        public void GroupedDefaultsPreserveInheritedAndDirectionalProperties() {
            var battery = new BatteryBlockHandler();
            Assert.AreEqual(Property.NAME, battery.defaultPropertiesByPrimitive[Return.STRING]);
            Assert.AreEqual(Property.ENABLE, battery.defaultPropertiesByPrimitive[Return.BOOLEAN]);
            Assert.AreEqual(Property.RATIO, battery.defaultPropertiesByPrimitive[Return.NUMERIC]);
            Assert.AreEqual(Property.RATIO, battery.defaultPropertiesByDirection[Direction.UP]);

            var piston = new PistonBlockHandler();
            Assert.AreEqual(Property.LEVEL, piston.defaultPropertiesByDirection[Direction.UP]);
            Assert.AreEqual(Property.LEVEL, piston.defaultPropertiesByDirection[Direction.DOWN]);

            var rotor = new RotorBlockHandler(b => true);
            Assert.AreEqual(Property.ANGLE, rotor.defaultPropertiesByPrimitive[Return.NUMERIC]);
            Assert.AreEqual(Property.LEVEL, rotor.defaultPropertiesByDirection[Direction.UP]);
            Assert.AreEqual(Property.LEVEL, rotor.defaultPropertiesByDirection[Direction.DOWN]);
            Assert.AreEqual(Property.ANGLE, rotor.defaultPropertiesByDirection[Direction.CLOCKWISE]);
            Assert.AreEqual(Property.ANGLE, rotor.defaultPropertiesByDirection[Direction.COUNTERCLOCKWISE]);
        }

        [TestMethod]
        public void IndependentAliasesDoNotRegisterACompoundProperty() {
            var antenna = new AntennaBlockHandler();
            Assert.AreSame(antenna.propertyHandlers[Property.RANGE + ""], antenna.propertyHandlers[Property.RADIUS + ""]);
            var compound = new PropertySupplier(NewList(new PropertyValue(Property.RANGE + ""), new PropertyValue(Property.RADIUS + "")));
            Assert.IsFalse(antenna.propertyHandlers.ContainsKey(antenna.GetPropertyHash(compound)));
        }

        [TestMethod]
        public void BuildAliasesRetainTheirExplicitCompoundProperty() {
            var handler = new TerminalBlockHandler<Sandbox.ModAPI.Ingame.IMyTerminalBlock>();
            var build = handler.propertyHandlers[Property.BUILD + ""];
            Assert.AreSame(build, handler.propertyHandlers[Property.COMPLETE + ""]);
            var compound = new PropertySupplier(NewList(new PropertyValue(Property.BUILD + ""), new PropertyValue(Property.COMPLETE + "")));
            Assert.AreSame(build, handler.GetPropertyHandler(compound));
        }
    }
}
