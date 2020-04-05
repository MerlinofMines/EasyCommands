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
        public class BooleanBlockPropertyCommandHandler<T> : TwoParameterEntityCommandHandler<BooleanPropertyCommandParameter, BooleanCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public BooleanBlockPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetBooleanPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class StringBlockPropertyCommandHandler<T> : TwoParameterEntityCommandHandler<StringPropertyCommandParameter, StringCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public StringBlockPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetStringPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class SetNumericPropertyCommandHandler<T> : TwoParameterEntityCommandHandler<NumericPropertyCommandParameter, NumericCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public SetNumericPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class SetNumericDirectionPropertyCommandHandler<T> : ThreeParameterEntityCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public SetNumericDirectionPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.SetNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue(), GetParameter3().GetValue()));
                return true;
            }
        }

        public class IncrementNumericPropertyCommandHandler<T> : ThreeParameterEntityCommandHandler<NumericPropertyCommandParameter, NumericCommandParameter, RelativeCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public IncrementNumericPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.IncrementNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue()));
                return true;
            }
        }

        public class IncrementNumericDirectionPropertyCommandHandler<T> : FourParameterEntityCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter, RelativeCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public IncrementNumericDirectionPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.IncrementNumericPropertyValue(entity, GetParameter1().GetValue(), GetParameter2().GetValue(), GetParameter3().GetValue()));
                return true;
            }
        }

        public class MoveNumericDirectionPropertyCommandHandler<T> : TwoParameterEntityCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public MoveNumericDirectionPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
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
        public class ReverseBlockCommandHandler<T> : OneParameterEntityCommandHandler<ReverseCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public ReverseBlockCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
            {
                this.blockHandler = blockHandler;
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => blockHandler.ReverseNumericPropertyValue(entity, blockHandler.GetDefaultNumericProperty(blockHandler.GetDefaultDirection())));
                return true;
            }
        }

        public class ReverseBlockPropertyCommandHandler<T> : TwoParameterEntityCommandHandler<ReverseCommandParameter, NumericPropertyCommandParameter, T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public ReverseBlockPropertyCommandHandler(IEntityProvider<T> entityProvider, BlockHandler<T> blockHandler) : base(entityProvider)
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
