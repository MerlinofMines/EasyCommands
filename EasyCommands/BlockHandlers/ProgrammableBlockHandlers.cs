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
        public class ProgramBlockHandler : BlockHandler<IMyProgrammableBlock>
        {
            protected override List<BooleanPropertyGetter<IMyProgrammableBlock>> GetBooleanPropertyGetters()
            {
                return new List<BooleanPropertyGetter<IMyProgrammableBlock>>()
                {
                    new OnOffPropertyGetter<IMyProgrammableBlock>(),
                    new ProgramRunningGetter()
                };
            }

            protected override List<StringPropertySetter<IMyProgrammableBlock>> GetStringPropertySetters()
            {
                return new List<StringPropertySetter<IMyProgrammableBlock>>()
                {
                    new ProgramRunSetter(),
                };
            }
        }

        public class ProgramRunningGetter : BooleanPropertyGetter<IMyProgrammableBlock>
        {
            public ProgramRunningGetter() : base(BooleanPropertyType.RUNNING) { }

            public override bool GetPropertyValue(IMyProgrammableBlock block)
            {
                return !block.DetailedInfo.EndsWith("Execution Complete\n");
            }
        }

        public class ProgramRunSetter : StringPropertySetter<IMyProgrammableBlock>
        {
            public ProgramRunSetter() : base(StringPropertyType.RUN) {}

            public override void SetPropertyValue(IMyProgrammableBlock block, string value)
            {
                block.TryRun(value);
            }
        }
    }
}
