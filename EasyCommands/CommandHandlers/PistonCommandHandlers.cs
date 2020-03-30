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

namespace IngameScript
{
    partial class Program
    {
        public class PistonReverseHandler : OneParameterEntityCommandHandler<ReverseCommandParameter, IMyPistonBase>
        {
            public PistonReverseHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                entityProvider.GetEntities(program).ForEach(piston => piston.Reverse());
                return true;
            }
        }

        public class PistonRelativeVelocityHandler : FourParameterEntityCommandHandler<NumericCommandParameter, RelativeCommandParameter, VelocityCommandParameter, IncrementCommandParameter, IMyPistonBase>
        {
            public PistonRelativeVelocityHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = Math.Abs(GetParameter1().getValue());
                bool incremental = GetParameter4().isIncrement();
                float deltaValue = numeric * (incremental ? 1 : -1);

                entityProvider.GetEntities(program).ForEach(piston => {
                    float newSpeed = Math.Abs(piston.Velocity) + deltaValue;
                    float negativeMultiplier = (piston.Velocity > 0) ? 1 : -1;
                    piston.Velocity = newSpeed * negativeMultiplier;});
            
                return true;
            }
        }

        public class PistonIncrementalAbsoluteVelocityHandler : ThreeParameterEntityCommandHandler<NumericCommandParameter, VelocityCommandParameter, IncrementCommandParameter, IMyPistonBase>
        {
            public PistonIncrementalAbsoluteVelocityHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = Math.Abs(GetParameter1().getValue());
                entityProvider.GetEntities(program).ForEach(piston => {
                    float negativeMultiplier = (piston.Velocity > 0) ? 1 : -1;
                    piston.Velocity = numeric * negativeMultiplier;
                });

                return true;
            }
        }

        public class PistonAbsoluteVelocityHandler : TwoParameterEntityCommandHandler<NumericCommandParameter, VelocityCommandParameter, IMyPistonBase>
        {
            public PistonAbsoluteVelocityHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = Math.Abs(GetParameter1().getValue());
                entityProvider.GetEntities(program).ForEach(piston => {
                    float negativeMultiplier = (piston.Velocity > 0) ? 1 : -1;
                    piston.Velocity = numeric * negativeMultiplier;
                });

                return true;
            }
        }

        public class PistonRelativePositionHandler : ThreeParameterEntityCommandHandler<NumericCommandParameter, IncrementCommandParameter, RelativeCommandParameter, IMyPistonBase>
        {
            public PistonRelativePositionHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = GetParameter1().getValue();
                bool incremental = GetParameter2().isIncrement();
                float deltaValue = numeric * (incremental ? 1 : -1);

                entityProvider.GetEntities(program).ForEach(piston => extendPistonToValue(piston, piston.CurrentPosition + deltaValue));

                return true;
            }
        }

        public class PistonIncrementalAbsolutePositionHandler : TwoParameterEntityCommandHandler<NumericCommandParameter, IncrementCommandParameter, IMyPistonBase>
        {
            public PistonIncrementalAbsolutePositionHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = GetParameter1().getValue();
                entityProvider.GetEntities(program).ForEach(piston => extendPistonToValue(piston, numeric));
                return true;
            }
        }

        public class PistonAbsolutePositionHandler : OneParameterEntityCommandHandler<NumericCommandParameter, IMyPistonBase>
        {
            public PistonAbsolutePositionHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = GetParameter().getValue();
                program.Echo("Value: " + numeric);
                entityProvider.GetEntities(program).ForEach(piston => extendPistonToValue(piston, numeric));
                return true;
            }
        }

        public class PistonIncrementalPositionHandler : OneParameterEntityCommandHandler<IncrementCommandParameter, IMyPistonBase>
        {
            public PistonIncrementalPositionHandler(IEntityProvider<IMyPistonBase> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                if (GetParameter().isIncrement())
                {
                    entityProvider.GetEntities(program).ForEach(piston => piston.Extend());
                }
                else
                {
                    entityProvider.GetEntities(program).ForEach(piston => piston.Retract());

                }
                return true;
            }
        }
        static void extendPistonToValue(IMyPistonBase piston, float value)
        {
            if (piston.CurrentPosition < value)
            {
                piston.SetValue("UpperLimit", value);
                piston.Extend();
            }
            else
            {
                piston.SetValue("LowerLimit", value);
                piston.Retract();
            }
        }
    }
}
