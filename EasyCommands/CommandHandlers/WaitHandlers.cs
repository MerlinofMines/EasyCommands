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
        public class WaitForDurationUnitHandler : TwoParameterCommandHandler<NumericCommandParameter, UnitCommandParameter>
        {
            int ticks = 0;

            public override bool Handle()
            {
                Print("Waited for " + ticks + " ticks");
                ticks++;
                return ticks > getTicks(parameter1.Value, parameter2.GetUnit());
            }

            public override void Reset()
            {
                ticks = 0;
            }
        }

        static int getTicks(float numeric, UnitType unitType)
        {
            switch(unitType)
            {
                case UnitType.SECONDS:
                    return (int)(numeric * 60);//Assume 60 ticks / second
                case UnitType.TICKS:
                    return (int)numeric;
                default:
                    throw new Exception("Unsupported Unit Type: " + unitType);
            }
        }
    }
}
