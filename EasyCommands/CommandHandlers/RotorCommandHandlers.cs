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
        public class RotorRelativeVelocityHandler : FourParameterEntityCommandHandler<NumericCommandParameter, RelativeCommandParameter, VelocityCommandParameter, IncrementCommandParameter, IMyMotorStator>
        {
            public RotorRelativeVelocityHandler(IEntityProvider<IMyMotorStator> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = Math.Abs(GetParameter1().getValue());
                bool incremental = GetParameter4().isIncrement();
                float deltaValue = numeric * (incremental ? 1 : -1);

                entityProvider.GetEntities(program).ForEach(rotor =>
                {
                    float newSpeed = Math.Abs(rotor.TargetVelocityRPM) + deltaValue;
                    float negativeMultiplier = (rotor.TargetVelocityRPM > 0) ? 1 : -1;
                    rotor.TargetVelocityRPM = newSpeed * negativeMultiplier;
                });
                return true;
            }
        }

        public class RotorIncrementalAbsoluteVelocityHandler : ThreeParameterEntityCommandHandler<NumericCommandParameter, VelocityCommandParameter, IncrementCommandParameter, IMyMotorStator>
        {
            public RotorIncrementalAbsoluteVelocityHandler(IEntityProvider<IMyMotorStator> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = Math.Abs(GetParameter1().getValue());
                entityProvider.GetEntities(program).ForEach(rotor =>
                {
                    float negativeMultiplier = (rotor.TargetVelocityRPM > 0) ? 1 : -1;
                    rotor.TargetVelocityRPM = numeric * negativeMultiplier;
                });

                return true;
            }
        }

        public class RotorAbsoluteVelocityHandler : TwoParameterEntityCommandHandler<NumericCommandParameter, VelocityCommandParameter, IMyMotorStator>
        {
            public RotorAbsoluteVelocityHandler(IEntityProvider<IMyMotorStator> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = Math.Abs(GetParameter1().getValue());
                entityProvider.GetEntities(program).ForEach(Rotor =>
                {
                    float negativeMultiplier = (Rotor.TargetVelocityRPM > 0) ? 1 : -1;
                    Rotor.TargetVelocityRPM = numeric * negativeMultiplier;
                });

                return true;
            }
        }

        public class RotorRelativePositionHandler : ThreeParameterEntityCommandHandler<NumericCommandParameter, IncrementCommandParameter, RelativeCommandParameter, IMyMotorStator>
        {
            public RotorRelativePositionHandler(IEntityProvider<IMyMotorStator> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = GetParameter1().getValue();
                bool incremental = GetParameter2().isIncrement();
                float deltaValue = numeric * (incremental ? 1 : -1);

                //TODO: Need Direction Handled (Using IncrementCommandParameter))
                entityProvider.GetEntities(program).ForEach(rotor => rotateToValue(rotor, rotor.Angle + deltaValue));

                return true;
            }
        }

        public class RotorIncrementalAbsolutePositionHandler : TwoParameterEntityCommandHandler<NumericCommandParameter, IncrementCommandParameter, IMyMotorStator>
        {
            public RotorIncrementalAbsolutePositionHandler(IEntityProvider<IMyMotorStator> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = GetParameter1().getValue();
                //TODO: Need Direction Handled (Using IncrementCommandParameter))
                entityProvider.GetEntities(program).ForEach(rotor => rotateToValue(rotor, numeric));
                return true;
            }
        }

        public class RotorAbsolutePositionHandler : OneParameterEntityCommandHandler<NumericCommandParameter, IMyMotorStator>
        {
            public RotorAbsolutePositionHandler(IEntityProvider<IMyMotorStator> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                float numeric = GetParameter().getValue();
                program.Echo("Value: " + numeric);
                entityProvider.GetEntities(program).ForEach(rotor => rotateToValue(rotor, numeric));
                return true;
            }
        }

        public class RotorIncrementalPositionHandler : OneParameterEntityCommandHandler<IncrementCommandParameter, IMyMotorStator>
        {
            public RotorIncrementalPositionHandler(IEntityProvider<IMyMotorStator> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                program.Echo("Here: " + GetType());
                if (GetParameter().isIncrement())
                {
                    entityProvider.GetEntities(program).ForEach(piston => piston.TargetVelocityRPM = Math.Abs(piston.TargetVelocityRPM));
                }
                else
                {
                    entityProvider.GetEntities(program).ForEach(piston => piston.TargetVelocityRPM = -Math.Abs(piston.TargetVelocityRPM));


                }
                return true;
            }
        }

        //TODO: Directions may become important.  Below needs a lot of work
        static void rotateToValue(IMyMotorStator rotor, float value)
        {
            float newValue = value;

            if (newValue > 360) newValue %= 360;

            if (newValue < -360)
            {
                newValue = -((-newValue) % 360);
            }

            //TODO: We might find that in some cases, it's faster to go the other way.

            if (rotor.Angle < value)
            {
                rotor.UpperLimitDeg = newValue;
                rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
            }
            else
            {
                rotor.LowerLimitDeg = newValue;
                rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
            }
        }
    }
}
