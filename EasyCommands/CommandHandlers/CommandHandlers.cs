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
            protected StringCommandParameter selector;

            public SelectorEntityProvider(StringCommandParameter selector)
            {
                this.selector = selector;
            }

            public virtual List<T> GetEntities(MyGridProgram program)
            {
                List<T> entities = new List<T>();
                program.GridTerminalSystem.GetBlocksOfType<T>(entities);

                program.Echo("I'm Here");

                return entities.FindAll(entity => (entity is IMyTerminalBlock)
                    && ((IMyTerminalBlock)entity).CustomName.ToLower() == selector.GetValue());
            }
        }

        public class SelectorGroupEntityProvider<T> : SelectorEntityProvider<T> where T : class, IMyTerminalBlock
        {
            public SelectorGroupEntityProvider(StringCommandParameter selector) : base(selector)
            {
            }

            public override List<T> GetEntities(MyGridProgram program)
            {
                List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                program.GridTerminalSystem.GetBlockGroups(blockGroups);

                IMyBlockGroup group = blockGroups.Find(g => g.Name.ToLower() == selector.GetValue());

                if (group == null)
                {
                    throw new Exception("Unable to find requested block group: " + selector.GetValue());
                }

                List<T> entities = new List<T>();
                group.GetBlocksOfType<T>(entities);

                program.Echo("Found " + entities.Count + " blocks in Group Selector");

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

            protected T GetParameter() => parameter;

            public override bool CanHandle(List<CommandParameter> commandParameters)
            {
                if (commandParameters.Count != 1) return false;
                List<CommandParameter> others = new List<CommandParameter>();
                bool supports = Supports<T>(commandParameters, out others, out parameter);

                return supports;
            }
        }

        public abstract class TwoParameterCommandHandler<T,U> : CommandHandler
            where T : class, CommandParameter
            where U : class, CommandParameter
        {
            T parameter1 = null;
            U parameter2 = null;

            protected T GetParameter1() => parameter1;
            protected U GetParameter2() => parameter2;

            public override bool CanHandle(List<CommandParameter> commandParameters)
            {
                if (commandParameters.Count != 2) return false;

                List<CommandParameter> others = new List<CommandParameter>();

                bool supportsT = Supports<T>(commandParameters, out others, out parameter1);
                if (!supportsT) return false;

                bool supportsU = Supports<U>(others, out others, out parameter2);
                return supportsU;
            }
        }

        public abstract class ThreeParameterCommandHandler<T, U, V> : CommandHandler
            where T : class, CommandParameter
            where U : class, CommandParameter
            where V : class, CommandParameter
        {
            T parameter1 = null;
            U parameter2 = null;
            V parameter3 = null;

            protected T GetParameter1() => parameter1;
            protected U GetParameter2() => parameter2;
            protected V GetParameter3() => parameter3;

            public override bool CanHandle(List<CommandParameter> commandParameters)
            {
                if (commandParameters.Count != 3) return false;

                List<CommandParameter> others = new List<CommandParameter>();

                bool supportsT = Supports<T>(commandParameters, out others, out parameter1);
                if (!supportsT) return false;

                bool supportsU = Supports<U>(others, out others, out parameter2);
                if (!supportsU) return false;

                bool supportsV = Supports<V>(others, out others, out parameter3);
                return supportsV;
            }
        }

        public abstract class FourParameterCommandHandler<T, U, V, W> : CommandHandler
            where T : class, CommandParameter
            where U : class, CommandParameter
            where V : class, CommandParameter
            where W : class, CommandParameter
        {
            T parameter1 = null;
            U parameter2 = null;
            V parameter3 = null;
            W parameter4 = null;

            protected T GetParameter1() => parameter1;
            protected U GetParameter2() => parameter2;
            protected V GetParameter3() => parameter3;
            protected W GetParameter4() => parameter4;

            public override bool CanHandle(List<CommandParameter> commandParameters)
            {
                if (commandParameters.Count != 4) return false;

                List<CommandParameter> others = new List<CommandParameter>();

                bool supportsT = Supports<T>(commandParameters, out others, out parameter1);
                if (!supportsT) return false;

                bool supportsU = Supports<U>(others, out others, out parameter2);
                if (!supportsU) return false;

                bool supportsV = Supports<V>(others, out others, out parameter3);
                if (!supportsV) return false;

                bool supportsW = Supports<W>(others, out others, out parameter4);
                return supportsW;
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

        public abstract class TwoParameterEntityCommandHandler<T, U, V> : TwoParameterCommandHandler<T, U>
            where T : class, CommandParameter
            where U : class, CommandParameter
            where V : class, IMyTerminalBlock
        {
            protected IEntityProvider<V> entityProvider;

            public TwoParameterEntityCommandHandler(IEntityProvider<V> entityProvider)
            {
                this.entityProvider = entityProvider;
            }
        }

        public abstract class ThreeParameterEntityCommandHandler<T, U, V, W> : ThreeParameterCommandHandler<T, U, V>
            where T : class, CommandParameter
            where U : class, CommandParameter
            where V : class, CommandParameter
            where W : class, IMyTerminalBlock
        {
            protected IEntityProvider<W> entityProvider;

            public ThreeParameterEntityCommandHandler(IEntityProvider<W> entityProvider)
            {
                this.entityProvider = entityProvider;
            }
        }

        public abstract class FourParameterEntityCommandHandler<T, U, V, W, X> : FourParameterCommandHandler<T, U, V, W>
            where T : class, CommandParameter
            where U : class, CommandParameter
            where V : class, CommandParameter
            where W : class, CommandParameter
            where X : class, IMyTerminalBlock
        {
            protected IEntityProvider<X> entityProvider;

            public FourParameterEntityCommandHandler(IEntityProvider<X> entityProvider)
            {
                this.entityProvider = entityProvider;
            }
        }
    }
}
