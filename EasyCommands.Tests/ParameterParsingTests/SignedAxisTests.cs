using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class SignedAxisTests : ForceLocale {
        class Axis { public float value; }
        class Handler : BlockHandler<Axis> {
            public Handler(bool readOnly = false) {
                SetTypedProperty<Axis, float> set = readOnly ? null : (SetTypedProperty<Axis, float>)((b, v) => b.value = v);
                AddDirectionHandlers(Property.INPUT, Direction.UP,
                    SignedAxis(b => b.value, set, Direction.UP, Direction.DOWN, 3));
            }
            public override string Name(Axis block) => "axis";
        }

        [DataTestMethod]
        [DataRow(Direction.UP, 1f)]
        [DataRow(Direction.DOWN, -1f)]
        public void OppositeDirectionsKeepSignedReadsWritesAndIncrements(Direction direction, float sign) {
            CreateProgram();
            var handler = new Handler();
            var block = new Axis { value = 4 };
            var property = new PropertySupplier(NewList(new PropertyValue(Property.INPUT + ""))).WithDirection(direction);
            Assert.AreEqual(4 * sign, CastNumber(handler.GetPropertyValue(block, property)));
            handler.UpdatePropertyValue(block, property.WithPropertyValue(GetStaticVariable(7)));
            Assert.AreEqual(7 * sign, block.value);
            handler.IncrementPropertyValue(block, property.WithPropertyValue(GetStaticVariable(2)).WithIncrement(true));
            Assert.AreEqual(9 * sign, block.value);
            handler.IncrementPropertyValue(block, property.WithPropertyValue(GetStaticVariable(2)).WithIncrement(false));
            Assert.AreEqual(7 * sign, block.value);
            handler.UpdatePropertyValue(block, property);
            Assert.AreEqual(10 * sign, block.value);
            handler.IncrementPropertyValue(block, property.WithIncrement(true));
            Assert.AreEqual(13 * sign, block.value);
        }

        [TestMethod]
        public void ReadOnlyAxesStillValidateNumericInput() {
            CreateProgram();
            var handler = new Handler(true);
            var block = new Axis { value = 4 };
            var property = new PropertySupplier(NewList(new PropertyValue(Property.INPUT + ""))).WithDirection(Direction.DOWN);
            handler.UpdatePropertyValue(block, property.WithPropertyValue(GetStaticVariable(7)));
            Assert.AreEqual(4f, block.value);
            Assert.ThrowsException<RuntimeException>(() => handler.UpdatePropertyValue(block,
                property.WithPropertyValue(GetStaticVariable(new KeyedList { keyedValues = NewList<KeyedVariable>() }))));
            Assert.AreEqual(4f, block.value);
        }
    }
}
