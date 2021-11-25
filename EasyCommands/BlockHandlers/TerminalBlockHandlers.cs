using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public delegate object GetBlockPropertyValue(IMyTerminalBlock block, String propertyId);
        public delegate void SetBlockPropertyValue(IMyTerminalBlock block, String propertyId, Primitive value);

        public class TerminalPropertyConverter {
            public GetBlockPropertyValue GetValue;
            public SetBlockPropertyValue SetValue;
        }

        public static TerminalPropertyConverter PropertyConverter(GetBlockPropertyValue GetValue, SetBlockPropertyValue SetValue) => new TerminalPropertyConverter {
            GetValue = GetValue,
            SetValue = SetValue
        };

        public class TerminalBlockPropertyHandler<T> : SimplePropertyHandler<T> where T : class, IMyTerminalBlock {
            public TerminalBlockPropertyHandler(String propertyId, Primitive delta) : base(
                (b, p) => ResolvePrimitive(GetPropertyConverter(b, propertyId).GetValue(b, propertyId)),
                (b, p, v) => GetPropertyConverter(b, propertyId).SetValue(b, propertyId, v), delta) { }

            static Dictionary<String, TerminalPropertyConverter> TerminalPropertyValueConversion = new Dictionary<String, TerminalPropertyConverter> {
                { "StringBuilder", PropertyConverter((b,p) => b.GetValue<StringBuilder>(p).ToString(), (b,p,v) => b.SetValue(p, new StringBuilder(CastString(v))))},
                { "Boolean", PropertyConverter((b,p) => b.GetValueBool(p), (b,p,v) => b.SetValueBool(p, CastBoolean(v)))},
                { "Single", PropertyConverter((b,p) => b.GetValueFloat(p), (b,p,v) => b.SetValueFloat(p, CastNumber(v)))},
                { "Int64", PropertyConverter((b,p) => (float)b.GetValue<long>(p), (b,p,v) => b.SetValue(p, (long)CastNumber(v)))},
                { "Color", PropertyConverter((b,p) => b.GetValueColor(p), (b,p,v) => b.SetValueColor(p, CastColor(v)))}
            };

            static TerminalPropertyConverter GetPropertyConverter(T block, String propertyId) {
                var property = block.GetProperty(propertyId);
                if (property == null) throw new Exception(block.BlockDefinition.SubtypeName + " does not have property: " + propertyId);
                return TerminalPropertyValueConversion[property.TypeName];
            }
        }

        public class FunctionalBlockHandler<T> : TerminalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public FunctionalBlockHandler() : base() {
                AddBooleanHandler(Property.ENABLE, b => b.Enabled, (b, v) => b.Enabled = v);
                AddBooleanHandler(Property.POWER, b => b.Enabled, (b, v) => b.Enabled = v);
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.POWER;
            }
        }

        public class TerminalBlockHandler<T> : BlockHandler<T> where T : class, IMyTerminalBlock {
            public TerminalBlockHandler() {
                AddVectorHandler(Property.POSITION, block => block.GetPosition());
                AddStringHandler(Property.NAME, b => b.CustomName, (b, v) => b.CustomName = v);
                AddBooleanHandler(Property.SHOW, b => b.ShowInTerminal, (b, v) => b.ShowInTerminal = v);
                AddListHandler(Property.PROPERTIES, b => {
                    var properties = NewList<ITerminalProperty>();
                    b.GetProperties(properties);
                    return new KeyedList(properties.Select(p => GetStaticVariable(p.Id)).ToArray());
                });
                AddListHandler(Property.ACTIONS, b => {
                    var actions = NewList<ITerminalAction>();
                    b.GetActions(actions);
                    return new KeyedList(actions.Select(p => GetStaticVariable(p.Id)).ToArray());
                });

                AddPropertyHandler(ValueProperty.ACTION, new SimplePropertyHandler<T>((b, p) => p.attributeValue.GetValue(), (b, p, v) => b.ApplyAction(CastString(p.attributeValue.GetValue())), ResolvePrimitive(0)));

                AddDirectionHandlers(Property.DIRECTION, Direction.FORWARD,
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Forward)), Direction.FORWARD),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Backward)), Direction.BACKWARD),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Up)), Direction.UP),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Down)), Direction.DOWN),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Left)), Direction.LEFT),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Right)), Direction.RIGHT));

                defaultPropertiesByPrimitive[Return.VECTOR] = Property.POSITION;
            }

            public override List<T> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector) {
                var blocks = NewList<T>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType<T>(blocks, selector);
                return blocks;
            }

            public override List<T> GetBlocksOfTypeInGroup(String groupName) {
                var blockGroups = NewList<IMyBlockGroup>();
                PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);
                IMyBlockGroup group = blockGroups.Find(g => g.Name.Equals(groupName));
                var blocks = NewList<T>();
                if (group == null) return blocks;
                group.GetBlocksOfType<T>(blocks);
                return blocks;
            }

            public override PropertyHandler<T> GetPropertyHandler(PropertySupplier property) {
                try {
                    return base.GetPropertyHandler(property);
                } catch (Exception) {
                    return TerminalBlockPropertyHandler(property.propertyType, 1);
                }
            }

            public override string Name(T block) { return block.CustomName; }

            public String GetCustomProperty(T block, String key) { return GetCustomData(block).GetValueOrDefault(key, null); }

            public void SetCustomProperty(T block, String key, String value) {
                var d = GetCustomData(block);
                d[key] = value; SaveCustomData(block, d);
            }

            public void DeleteCustomProperty(T block, String key) {
                var d = GetCustomData(block);
                if (d.ContainsKey(key)) d.Remove(key);
                SaveCustomData(block, d);
            }

            public void SaveCustomData(T block, Dictionary<String, String> data) {
                block.CustomData = String.Join("\n", data.Keys.Select(k => k + "=" + data[k]).ToList());
            }

            public Dictionary<String, String> GetCustomData(T block) {
                List<String> keys = block.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return keys.ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);
            }

            public PropertyHandler<T> TerminalBlockPropertyHandler(String propertyId, object delta) => new TerminalBlockPropertyHandler<T>(propertyId, ResolvePrimitive(delta));

            //Taken from https://forum.keenswh.com/threads/how-do-i-get-the-world-position-and-rotation-of-a-ship.7363867/
            MatrixD GetBlock2WorldTransform(IMyCubeBlock blk) {
                Matrix blk2grid;
                blk.Orientation.GetMatrix(out blk2grid);
                return blk2grid *
                       MatrixD.CreateTranslation(new Vector3D(blk.Min + blk.Max) / 2.0) *
                       GetGrid2WorldTransform(blk.CubeGrid);
            }

            MatrixD GetGrid2WorldTransform(IMyCubeGrid grid) {
                Vector3D origin = grid.GridIntegerToWorld(new Vector3I(0, 0, 0));
                Vector3D plusY = grid.GridIntegerToWorld(new Vector3I(0, 1, 0)) - origin;
                Vector3D plusZ = grid.GridIntegerToWorld(new Vector3I(0, 0, 1)) - origin;
                return MatrixD.CreateScale(grid.GridSize) * MatrixD.CreateWorld(origin, -plusZ, plusY);
            }

            Vector3D Normalize(Vector3D vector) => vector / vector.Length();
        }

        /// <summary>
        ///Provides a consistent way to get the most accurate position for a detected entity.
        ///Not all detected entities will have a HitPosition.  Attempt to use HitPosition first.  If not present, use Position instead.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static Vector3D GetPosition(MyDetectedEntityInfo entity) {
            return entity.HitPosition.GetValueOrDefault(entity.Position);
        }
    }
}
