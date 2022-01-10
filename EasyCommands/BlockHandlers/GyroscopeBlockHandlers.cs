using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class GyroscopeBlockHandler<T> : FunctionalBlockHandler<T> where T : class, IMyGyro {
            public GyroscopeBlockHandler() {
                var powerHandler = NumericHandler(b => b.GyroPower, (b, v) => b.GyroPower = v, 0.1f);
                AddPropertyHandler(Property.RANGE, powerHandler);
                AddPropertyHandler(Property.POWER, powerHandler);

                AddBooleanHandler(Property.AUTO, b => !b.GyroOverride, (b, v) => b.GyroOverride = !v);

                var overrideHandler = DirectionalTypedHandler(Direction.NONE,
                        TypeHandler(ReturnTypedHandler(Return.VECTOR,
                            TypeHandler(VectorHandler(b => new Vector3D(GetPitch(b), GetYaw(b), GetRoll(b)), (b, v) => {
                                SetPitch(b, (float)v.X);
                                SetYaw(b, (float)v.Y);
                                SetRoll(b, (float)v.Z);
                            }), Return.VECTOR),
                            TypeHandler(BooleanHandler(b => b.GyroOverride, (b, v) => b.GyroOverride = v), Return.BOOLEAN))
                        , Direction.NONE),
                        TypeHandler(NumericHandler(GetPitch, SetPitch, 5), Direction.UP),
                        TypeHandler(NumericHandler(b => -GetPitch(b), (b, v) => SetPitch(b, -v), 5), Direction.DOWN),
                        TypeHandler(NumericHandler(b => -GetYaw(b), (b, v) => SetYaw(b, -v), 5), Direction.LEFT),
                        TypeHandler(NumericHandler(GetYaw, SetYaw, 5), Direction.RIGHT),
                        TypeHandler(NumericHandler(GetRoll, SetRoll, 5), Direction.CLOCKWISE),
                        TypeHandler(NumericHandler(b => -GetRoll(b), (b, v) => SetRoll(b, -v), 5), Direction.COUNTERCLOCKWISE));

                AddPropertyHandler(Property.OVERRIDE, overrideHandler);
                AddPropertyHandler(Property.ROLL_INPUT, overrideHandler);
                AddPropertyHandler(Property.INPUT, overrideHandler);

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.POWER;
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.OVERRIDE;
            }

            float GetPitch(IMyGyro block) => block.GetValueFloat("Pitch");
            float GetYaw(T block) => block.GetValueFloat("Yaw");
            float GetRoll(T block) => block.GetValueFloat("Roll");

            void SetPitch(T block, float value) => block.SetValueFloat("Pitch", value);
            void SetYaw(T block, float value) => block.SetValueFloat("Yaw", value);
            void SetRoll(T block, float value) => block.SetValueFloat("Roll", value);
        }
    }
}
