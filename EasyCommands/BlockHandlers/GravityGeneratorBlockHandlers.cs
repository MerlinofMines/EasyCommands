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
        public class SphericalGravityGeneratorBlockHandler : FunctionalBlockHandler<IMyGravityGeneratorSphere> {
            public SphericalGravityGeneratorBlockHandler() {
                var rangeHandler = NumericHandler(b => b.Radius, (b, v) => b.Radius = v, 25);
                AddPropertyHandler(Property.RANGE, rangeHandler);
                AddPropertyHandler(Property.LEVEL, rangeHandler);
                AddNumericHandler(Property.STRENGTH, b => b.GravityAcceleration, (b,v) => b.GravityAcceleration = v, 0.25f);

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.STRENGTH;
            }
        }

        public class GravityGeneratorBlockHandler : FunctionalBlockHandler<IMyGravityGenerator> {
            public GravityGeneratorBlockHandler() {
                AddNumericHandler(Property.STRENGTH, b => b.GravityAcceleration, (b, v) => b.GravityAcceleration = v, 0.25f);
                var rangeHandler = DirectionalTypedHandler(Direction.NONE,
                    TypeHandler(ReturnTypedHandler(Return.VECTOR,
                        TypeHandler(VectorHandler(b => b.FieldSize, (b, v) => b.FieldSize = v), Return.VECTOR),
                        TypeHandler(NumericHandler(b => b.FieldSize.Length(), (b, v) => b.FieldSize = new Vector3(v, v, v), 25), Return.NUMERIC)), Direction.NONE),
                    TypeHandler(NumericHandler(b => b.FieldSize.Y, (b, v) => b.FieldSize = new Vector3(b.FieldSize.X, v, b.FieldSize.Z)), Direction.UP, Direction.DOWN),
                    TypeHandler(NumericHandler(b => b.FieldSize.X, (b, v) => b.FieldSize = new Vector3(v, b.FieldSize.Y, b.FieldSize.Z)), Direction.LEFT, Direction.RIGHT),
                    TypeHandler(NumericHandler(b => b.FieldSize.Z, (b, v) => b.FieldSize = new Vector3(b.FieldSize.X, b.FieldSize.Y, v)), Direction.FORWARD, Direction.BACKWARD)
                    );
                AddPropertyHandler(Property.RANGE, rangeHandler);
                AddPropertyHandler(Property.LEVEL, rangeHandler);

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.STRENGTH;
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.RANGE;
            }
        }
    }
}