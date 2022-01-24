﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public static void MockGetProperty<T, U>(Mock<T> mockBlock, String propertyId, U value) where T : class, IMyTerminalBlock {
            var property = MockProperty<T,U>(mockBlock, propertyId);
            property.Setup(b => b.GetValue(It.IsAny<IMyCubeBlock>())).Returns(value);
        }

        public static Mock<ITerminalProperty<U>> MockProperty<T,U>(Mock<T> mockBlock, String propertyId) where T : class, IMyTerminalBlock {
            var mockProperty = new Mock<ITerminalProperty<U>>();
            mockProperty.Setup(p => p.Id).Returns(propertyId);
            MockPropertyType(mockProperty);
            mockBlock.Setup(b => b.GetProperty(propertyId)).Returns(mockProperty.Object);
            return mockProperty;
        }

        public static void MockPropertyType<U>(Mock<ITerminalProperty<U>> property) {
            if (typeof(U) == typeof(bool)) {
                property.Setup(b => b.TypeName).Returns("Boolean");
            } else if (typeof(U) == typeof(StringBuilder)) {
                property.Setup(b => b.TypeName).Returns("StringBuilder");
            } else if (typeof(U) == typeof(Color)) {
                property.Setup(b => b.TypeName).Returns("Color");
            } else if (typeof(U) == typeof(long)) {
                property.Setup(b => b.TypeName).Returns("Int64");
            } else if (typeof(U) == typeof(float)) {
                property.Setup(b => b.TypeName).Returns("Single");
            } else throw new Exception("Unsupported Property Value Type: " + typeof(U));
        }

        public static Mock<ITerminalAction> MockAction<T>(Mock<T> mockBlock, String actionName) where T : class, IMyTerminalBlock {
            Mock<ITerminalAction> terminalAction = new Mock<ITerminalAction>();
            terminalAction.Setup(action => action.Id).Returns(actionName);
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

        public static void MockShipVelocities<T>(Mock<T> mockShip, Vector3D linearVelocity, Vector3D angularVelocity) where T : class, IMyShipController {
            mockShip.Setup(b => b.GetShipVelocities()).Returns(new MyShipVelocities(linearVelocity, angularVelocity));
            mockShip.Setup(b => b.WorldMatrix).Returns(MatrixD.Identity);
        }

        public static void MockWorldMatrix<T>(Mock<T> mockBlock, Vector3D position, Vector3D forward, Vector3D up) where T : class, IMyTerminalBlock {
            mockBlock.Setup(b => b.WorldMatrix).Returns(MatrixD.CreateWorld(position, forward, up));
        }

        public static void MockOrientation<T>(Mock<T> mockBlock, Vector3 forward, Vector3 up) where T : class, IMyTerminalBlock {
            Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
            mockGrid.Setup(g => g.GridIntegerToWorld(new Vector3I(0, 0, 0))).Returns(new Vector3D(0, 0, 0));
            mockGrid.Setup(g => g.GridIntegerToWorld(new Vector3I(0, 1, 0))).Returns(new Vector3D(0, 1, 0));
            mockGrid.Setup(g => g.GridIntegerToWorld(new Vector3I(0, 0, 1))).Returns(new Vector3D(0, 0, 1));
            mockGrid.Setup(g => g.GridSize).Returns(1f);
            mockBlock.Setup(g => g.CubeGrid).Returns(mockGrid.Object);
            mockBlock.Setup(g => g.Min).Returns(Vector3I.Zero);
            mockBlock.Setup(g => g.Max).Returns(Vector3I.Zero);
            var matrix = Matrix.CreateFromDir(forward, up);
            mockBlock.Setup(g => g.Orientation).Returns(new MyBlockOrientation(ref matrix));
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

        public static void MockProductionQueue<T>(Mock<T> productionBlock, params MyProductionItem[] queue) where T : class, IMyProductionBlock {
            productionBlock.Setup(p => p.GetQueue(It.IsAny<List<MyProductionItem>>())).Callback<List<MyProductionItem>>(
                list => list.AddRange(queue.ToList()));
        }

        public static MyProductionItem MockProductionItem(String itemId, int amount = 1) {
            var item = new MyProductionItem(0, MockBlueprint(itemId), amount);
            return item;
        }

        public static Action<List<MyInventoryItem>, Func<MyInventoryItem, bool>> MockItems(List<MyInventoryItem> items) {
            return (i, filter) => {
                Assert.IsTrue(items.TrueForAll(item => filter.Invoke(item)));
                i.AddRange(items);
            };
        }

        public static void MockBlockDefinition<T>(Mock<T> mockBlock, String subtypeId) where T : class, IMyCubeBlock {
            mockBlock.Setup(b => b.BlockDefinition).Returns(new SerializableDefinitionId(new MyObjectBuilderType(typeof(T)), subtypeId));
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

        public static MyDefinitionId MockBlueprint(String itemId) {
            MyDefinitionId definition;
            MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition", itemId, out definition);
            return definition;
        }
    }
}
