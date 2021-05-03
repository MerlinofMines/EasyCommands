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
                propertyHandlers.Add(Property.RANGE, new GravityFieldHandler());
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.STRENGTH;
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.RANGE;
            }
        }

        class GravityFieldHandler : SimplePropertyHandler<IMyGravityGenerator> {
            public GravityFieldHandler() : base(
                b => new VectorPrimitive(new Vector3D(b.FieldSize)),
                (b, v) => {
                    switch (v.GetPrimitiveType()) {
                        case Return.NUMERIC:
                            float value = CastNumber(v).GetNumericValue();
                            b.FieldSize = new Vector3(value, value, value);
                            break;
                        case Return.VECTOR:
                            b.FieldSize = CastVector(v).GetVectorValue();
                            break;
                        default:
                            throw new Exception("Cannot set gravity field to type: " + v.GetPrimitiveType());
                    }
                },
                new NumberPrimitive(25)) {

                GetDirection = (b,d) => {
                    switch(d) {
                        case Direction.UP:
                        case Direction.DOWN:
                            return new NumberPrimitive(b.FieldSize.Y);
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            return new NumberPrimitive(b.FieldSize.X);
                        case Direction.FORWARD:
                        case Direction.BACKWARD:
                            return new NumberPrimitive(b.FieldSize.Z);
                        default:
                            throw new Exception("Bad");
                    }
                };

                SetDirection = (b, d, v) => {
                    float value = CastNumber(v).GetNumericValue();
                    float x = b.FieldSize.X;
                    float y = b.FieldSize.Y;
                    float z = b.FieldSize.Z;
                    switch (d) {
                        case Direction.UP:
                        case Direction.DOWN:
                            y = value;
                            break;
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            x = value;
                            break;
                        case Direction.FORWARD:
                        case Direction.BACKWARD:
                            z = value;
                            break;
                        default:
                            throw new Exception("Bad");
                    }
                    b.FieldSize = new Vector3(x, y, z);
                };
            }
        }
    }
}