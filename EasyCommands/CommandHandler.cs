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
        public interface IEntityProvider<T> where T : class, IMyTerminalBlock
        {
            List<T> GetEntities(MyGridProgram program);
        }

        public class SelectorEntityProvider<T> : IEntityProvider<T> where T : class, IMyTerminalBlock
        {
            protected SelectorCommandParameter selector;

            public SelectorEntityProvider(SelectorCommandParameter selector)
            {
                this.selector = selector;
            }

            public List<T> GetEntities(MyGridProgram program)
            {
                List<T> entities = new List<T>();
                program.GridTerminalSystem.GetBlocksOfType<T>(entities);

                return entities.FindAll(entity => (entity is IMyTerminalBlock)
                    && ((IMyTerminalBlock)entity).CustomName.ToLower() == selector.getSelector());
            }
        }

        public class SelectorGroupEntityProvider<T> : SelectorEntityProvider<T> where T : class, IMyTerminalBlock
        {
            public SelectorGroupEntityProvider(SelectorCommandParameter selector) : base(selector)
            {
            }

            new public List<T> GetEntities(MyGridProgram program)
            {
                IMyBlockGroup group = program.GridTerminalSystem.GetBlockGroupWithName(selector.getSelector());

                if (group == null)
                {
                    throw new Exception("Unable to find requested block group: " + selector.getSelector());
                }

                List<T> entities = new List<T>();
                group.GetBlocksOfType<T>(entities);
                return entities;
            }
        }

        public abstract class CommandHandler
        {
            public abstract bool CanHandle(List<CommandParameter> commandParameters);
            public abstract bool Handle(MyGridProgram program);

            public bool Supports<T>(List<CommandParameter> commandParameters, out List<CommandParameter> others, out T t) where T : class, CommandParameter
            {
                others = new List<CommandParameter>(commandParameters);
                t = null;

                int index = others.FindIndex(parameter => parameter is T);

                if (index < 0) return false;
                t = (T) others[index];
                others.RemoveAt(index);

                return true;
            }
        }

        public abstract class OneParameterCommandHandler<T> : CommandHandler where T : class, CommandParameter
        {
            T parameter = null;

            public override bool CanHandle(List<CommandParameter> commandParameters)
            {
                if (commandParameters.Count != 1) return false;
                List<CommandParameter> others = new List<CommandParameter>();
                bool supports = Supports<T>(commandParameters, out others, out parameter);

                return true;
            }

            protected T GetParameter()
            {
                return parameter;
            }
        }

        public abstract class TwoParameterCommandHandler<T,U> : CommandHandler
            where T : class, CommandParameter 
            where U : class, CommandParameter
        {
            T parameter1 = null;
            U parameter2 = null;

            public override bool CanHandle(List<CommandParameter> commandParameters)
            {
                if (commandParameters.Count != 2) return false;

                List<CommandParameter> others = new List<CommandParameter>();

                bool supportsT = Supports<T>(commandParameters, out others, out parameter1);
                if (!supportsT) return false;

                bool supportsU = Supports<U>(others, out others, out parameter2);
                return supportsU;
            }

            protected T getParameter1()
            {
                return parameter1;
            }

            protected U getParameter2()
            {
                return parameter2;
            }
        }

        public abstract class OneParameterEntityCommandHandler<T, U> : OneParameterCommandHandler<T> 
            where T : class, CommandParameter
            where U : class, IMyTerminalBlock
        {
            protected IEntityProvider<U> entityProvider;

            public OneParameterEntityCommandHandler(IEntityProvider<U> entityProvider)
            {
                this.entityProvider = entityProvider;
            }
        }

        public class ActivationHandler<U> : OneParameterEntityCommandHandler<ActivationCommandParameter, U> where U : class, IMyTerminalBlock
        {
            public ActivationHandler(IEntityProvider<U> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                List<U> entities = entityProvider.GetEntities(program);

                if (GetParameter().isActivate())
                {
                    entities.ForEach(block => block.ApplyAction("OnOff_On"));
                }
                else
                {
                    entities.ForEach(block => block.ApplyAction("OnOff_Off"));
                }
                return true;
            }
        }
    }
}
