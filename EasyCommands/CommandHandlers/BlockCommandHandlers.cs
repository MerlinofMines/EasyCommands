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
        public class BooleanBlockPropertyCommandHandler : TwoParameterEntityCommandHandler<BooleanPropertyCommandParameter, BooleanCommandParameter>
        {
            private BlockHandler blockHandler;

            public BooleanBlockPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetBooleanPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class StringBlockPropertyCommandHandler : TwoParameterEntityCommandHandler<StringPropertyCommandParameter, StringCommandParameter>
        {
            private BlockHandler blockHandler;

            public StringBlockPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetStringPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class SetNumericPropertyCommandHandler : TwoParameterEntityCommandHandler<NumericPropertyCommandParameter, NumericCommandParameter>
        {
            private BlockHandler blockHandler;

            public SetNumericPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class SetNumericDirectionPropertyCommandHandler : ThreeParameterEntityCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter>
        {
            private BlockHandler blockHandler;

            public SetNumericDirectionPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue(), GetParameter3().GetValue()));
                return true;
            }
        }

        public class IncrementNumericPropertyCommandHandler : ThreeParameterEntityCommandHandler<NumericPropertyCommandParameter, NumericCommandParameter, RelativeCommandParameter>
        {
            private BlockHandler blockHandler;

            public IncrementNumericPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.IncrementNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class IncrementNumericDirectionPropertyCommandHandler : FourParameterEntityCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter, RelativeCommandParameter>
        {
            private BlockHandler blockHandler;

            public IncrementNumericDirectionPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.IncrementNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue(), GetParameter3().GetValue()));
                return true;
            }
        }

        public class MoveNumericDirectionPropertyCommandHandler : TwoParameterEntityCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter>
        {
            private BlockHandler blockHandler;

            public MoveNumericDirectionPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.MoveNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        //TODO: Remove duplication using a command parameter pre-processor
        public class ReverseBlockCommandHandler : OneParameterEntityCommandHandler<ReverseCommandParameter>
        {
            private BlockHandler blockHandler;

            public ReverseBlockCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.ReverseNumericPropertyValue(entity, blockHandler.GetDefaultNumericProperty(blockHandler.GetDefaultDirection())));
                return true;
            }
        }

        public class ReverseBlockPropertyCommandHandler : TwoParameterEntityCommandHandler<ReverseCommandParameter, NumericPropertyCommandParameter>
        {
            private BlockHandler blockHandler;

            public ReverseBlockPropertyCommandHandler(IEntityProvider entityProvider, BlockHandler blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.ReverseNumericPropertyValue(entity, GetParameter2().GetValue()));
                return true;
            }
        }
    }
}
