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
                AddNumericHandler(Property.STRENGTH, b => b.GravityAcceleration, (b,v) => b.GravityAcceleration = v, 0.25f);
                AddNumericHandler(Property.RANGE, b => b.Radius, (b, v) => b.Radius = v, 25);
            }
        }

        public class GravityGeneratorBlockHandler : FunctionalBlockHandler<IMyGravityGenerator> {
            public GravityGeneratorBlockHandler() {
                AddNumericHandler(Property.STRENGTH, b => b.GravityAcceleration, (b, v) => b.GravityAcceleration = v, 0.25f);
                AddDirectionHandlers(Property.RANGE, Direction.NONE,
                    DirectionalHandler(new SimplePropertyHandler<IMyGravityGenerator>(
                        (b, p) => ResolvePrimitive(new Vector3D(b.FieldSize)),
                        (b, p, v) => {
                            if (v.returnType == Return.VECTOR) b.FieldSize = CastVector(v);
                            else {
                                var size = CastNumber(v);
                                b.FieldSize = new Vector3(size, size, size);
                            }
                        }, ResolvePrimitive(25)), Direction.NONE),
                    DirectionalHandler(NumericHandler(b => b.FieldSize.Y, (b, v) => b.FieldSize = new Vector3(b.FieldSize.X, v, b.FieldSize.Z)), Direction.UP, Direction.DOWN),
                    DirectionalHandler(NumericHandler(b => b.FieldSize.X, (b, v) => b.FieldSize = new Vector3(v, b.FieldSize.Y, b.FieldSize.Z)), Direction.LEFT, Direction.RIGHT),
                    DirectionalHandler(NumericHandler(b => b.FieldSize.Z, (b, v) => b.FieldSize = new Vector3(b.FieldSize.X, b.FieldSize.Y, v)), Direction.FORWARD, Direction.BACKWARD)
                    );

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.STRENGTH;
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.RANGE;
            }
        }
    }
}