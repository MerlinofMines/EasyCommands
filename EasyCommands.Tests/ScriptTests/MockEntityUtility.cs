using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage;
using VRageMath;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace EasyCommands.Tests.ScriptTests {
    class MockEntityUtility {

        public static Mock<ITerminalAction> MockCalledAction<T>(Mock<T> mockBlock, String actionName) where T : class, IMyTerminalBlock {
            Mock<ITerminalAction> terminalAction = new Mock<ITerminalAction>();

            mockBlock.Setup(b => b.GetActionWithName(actionName)).Returns(terminalAction.Object);

            return terminalAction;
        }

        public static MyDetectedEntityInfo MockDetectedEntity(Vector3D position) {
            return MockDetectedEntity(position, Vector3D.Zero);
        }

        public static MyDetectedEntityInfo MockDetectedEntity(Vector3D position, Vector3D velocity) {
            return new MyDetectedEntityInfo(123, "name", MyDetectedEntityType.LargeGrid, position,
                new MatrixD(), velocity, VRage.Game.MyRelationsBetweenPlayerAndBlock.Neutral, new BoundingBoxD(), 123);
        }

        public static MyDetectedEntityInfo MockNoDetectedEntity() {
            return new MyDetectedEntityInfo();
        }

        public static void MockTextSurfaces<T>(Mock<T> surfaceProvider, params Mock<IMyTextSurface>[] mockSurfaces) where T : class, IMyTextSurfaceProvider {
            surfaceProvider.Setup(x => x.SurfaceCount).Returns(mockSurfaces.Length);
            for(int i = 0; i < mockSurfaces.Length; i++) {
                surfaceProvider.Setup(x => x.GetSurface(i)).Returns(mockSurfaces[i].Object);
            }
        }

        public static void MockInventories<T>(Mock<T> inventoryProvider, params Mock<IMyInventory>[] mockInventories) where T : class, IMyTerminalBlock {
            inventoryProvider.Setup(x => x.InventoryCount).Returns(mockInventories.Length);
            for (int i = 0; i < mockInventories.Length; i++) {
                mockInventories[i].Setup(owner => owner.Owner).Returns(inventoryProvider.Object);
                inventoryProvider.Setup(x => x.GetInventory(i)).Returns(mockInventories[i].Object);
            }
        }

        public static void MockInventoryItems(Mock<IMyInventory> inventory, params MyInventoryItem[] items) {
            inventory.Setup(i => i.CurrentMass).Returns(items.Select(item => item.Amount).Aggregate((sum, val) => sum + val));
            inventory.Setup(i => i.GetItems(It.IsAny<List<MyInventoryItem>>(), It.IsAny<Func<MyInventoryItem, bool>>()))
                .Callback(MockItems(items.ToList()));
        }

        public static Action<List<MyInventoryItem>, Func<MyInventoryItem, bool>> MockItems(List<MyInventoryItem> items) {
            return (i, filter) => {
                Assert.IsTrue(items.TrueForAll(item => filter.Invoke(item)));
                i.AddRange(items);
            };
        }

        public static MyInventoryItem MockOre(String subType, float amount = 1) {
            return new MyInventoryItem(MyItemType.MakeOre(subType), 0, (MyFixedPoint)amount);
        }

        public static MyInventoryItem MockIngot(String subType, float amount = 1) {
            return new MyInventoryItem(MyItemType.MakeIngot(subType), 0, (MyFixedPoint)amount);
        }

        public static MyInventoryItem MockComponent(String subType, float amount = 1) {
            return new MyInventoryItem(MyItemType.MakeComponent(subType), 0, (MyFixedPoint)amount);
        }

        public static MyInventoryItem MockAmmo(String subType, float amount = 1) {
            return new MyInventoryItem(MyItemType.MakeAmmo(subType), 0, (MyFixedPoint)amount);
        }

        public static MyInventoryItem MockTool(String subType, float amount = 1) {
            return new MyInventoryItem(MyItemType.MakeTool(subType), 0, (MyFixedPoint)amount);
        }

        public static MyInventoryItem MockConsumable(String subType, float amount = 1) {
            return MockItem(typeof(MyObjectBuilder_ConsumableItem), subType, amount);
        }

        public static MyInventoryItem MockPhysicalObject(String subType, float amount = 1) {
            return MockItem(typeof(MyObjectBuilder_PhysicalObject), subType, amount);
        }

        public static MyInventoryItem MockItem(Type itemType, String subType, float amount = 1) {
            var type = new MyItemType(new MyObjectBuilderType(itemType), MyStringHash.GetOrCompute(subType));
            return new MyInventoryItem(type, 0, (MyFixedPoint)amount);
        }
    }
}
