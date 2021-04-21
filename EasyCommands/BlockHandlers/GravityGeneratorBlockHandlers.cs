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
                AddNumericHandler(PropertyType.STRENGTH, b => b.GravityAcceleration, (b,v) => b.GravityAcceleration = v, 0.25f);
                AddNumericHandler(PropertyType.RANGE, b => b.Radius, (b, v) => b.Radius = v, 25);
            }
        }

        public class GravityGeneratorBlockHandler : FunctionalBlockHandler<IMyGravityGenerator> {
            public GravityGeneratorBlockHandler() {
                AddNumericHandler(PropertyType.STRENGTH, b => b.GravityAcceleration, (b, v) => b.GravityAcceleration = v, 0.25f);
                propertyHandlers.Add(PropertyType.RANGE, new GravityFieldHandler());
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.STRENGTH;
                defaultPropertiesByPrimitive[PrimitiveType.VECTOR] = PropertyType.RANGE;
            }
        }

        class GravityFieldHandler : SimplePropertyHandler<IMyGravityGenerator> {
            public GravityFieldHandler() : base(
                b => new VectorPrimitive(new Vector3D(b.FieldSize)),
                (b, v) => {
                    switch (v.GetPrimitiveType()) {
                        case PrimitiveType.NUMERIC:
                            float value = CastNumber(v).GetNumericValue();
                            b.FieldSize = new Vector3(value, value, value);
                            break;
                        case PrimitiveType.VECTOR:
                            b.FieldSize = CastVector(v).GetVectorValue();
                            break;
                        default:
                            throw new Exception("Cannot set gravity field to type: " + v.GetPrimitiveType());
                    }
                },
                new NumberPrimitive(25)) {

                GetDirection = (b,d) => {
                    switch(d) {
                        case DirectionType.UP:
                        case DirectionType.DOWN:
                            return new NumberPrimitive(b.FieldSize.Y);
                        case DirectionType.LEFT:
                        case DirectionType.RIGHT:
                            return new NumberPrimitive(b.FieldSize.X);
                        case DirectionType.FORWARD:
                        case DirectionType.BACKWARD:
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
                        case DirectionType.UP:
                        case DirectionType.DOWN:
                            y = value;
                            break;
                        case DirectionType.LEFT:
                        case DirectionType.RIGHT:
                            x = value;
                            break;
                        case DirectionType.FORWARD:
                        case DirectionType.BACKWARD:
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