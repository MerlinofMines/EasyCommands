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

namespace IngameScript {
    partial class Program {
        public class SelectorEntityProvider {
            protected SelectorCommandParameter selector;

            public SelectorEntityProvider(SelectorCommandParameter selector) {
                this.selector = selector;
            }

            public List<Object> GetEntities() {
                if (selector.isGroup) {
                    List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                    PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);

                    IMyBlockGroup group = blockGroups.Find(g => g.Name.ToLower() == selector.selector);

                    if (group == null) {
                        throw new Exception("Unable to find requested block group: " + selector.selector);
                    }

                    return BlockHandlerRegistry.GetBlocks(group, selector.blockType);

                } else {
                    return BlockHandlerRegistry.GetBlocks(selector.blockType, selector.selector);
                }
            }
            public override String ToString() {
                return selector.blockType + (selector.isGroup ? " in group named " : " named " + selector.selector);
            }
        }

        public abstract class CommandHandler {
            public abstract bool CanHandle(List<CommandParameter> commandParameters);
            public abstract bool Handle();
            public virtual void Reset() { }
            public virtual CommandHandler Clone() { return this; }
            public bool Supports<T>(List<CommandParameter> commandParameters, out List<CommandParameter> others, out T t) where T : class, CommandParameter {
                t = null;
                others = new List<CommandParameter>(commandParameters);
                int index = others.FindIndex(parameter => parameter is T);

                if (index < 0) return false;
                t = (T)others[index];
                others.RemoveAt(index);

                return true;
            }

            public bool Supports<T, U>(List<CommandParameter> commandParameters, out List<CommandParameter> others, out T t, out U u)
                where T : class, CommandParameter
                where U : class, CommandParameter {
                others = new List<CommandParameter>();
                bool supportsT = Supports<T>(commandParameters, out others, out t);
                bool supportsU = Supports<U>(others, out others, out u);
                return supportsT && supportsU;
            }

            public bool Supports<T, U, V>(List<CommandParameter> commandParameters, out List<CommandParameter> others, out T t, out U u, out V v)
                where T : class, CommandParameter
                where U : class, CommandParameter
                where V : class, CommandParameter {
                others = new List<CommandParameter>();
                bool supportsTU = Supports<T, U>(commandParameters, out others, out t, out u);
                bool supportsV = Supports<V>(others, out others, out v);
                return supportsTU && supportsV;
            }

            public bool Supports<T, U, V, W>(List<CommandParameter> commandParameters, out List<CommandParameter> others, out T t, out U u, out V v, out W w)
                where T : class, CommandParameter
                where U : class, CommandParameter
                where V : class, CommandParameter
                where W : class, CommandParameter {
                others = new List<CommandParameter>();
                bool supportsTUV = Supports<T, U, V>(commandParameters, out others, out t, out u, out v);
                bool supportsW = Supports<W>(others, out others, out w);
                return supportsTUV && supportsW;
            }
        }

        public abstract class OneParameterCommandHandler<T> : CommandHandler where T : class, CommandParameter {
            protected T parameter = null;

            public override bool CanHandle(List<CommandParameter> commandParameters) {
                if (commandParameters.Count != 1) return false;
                List<CommandParameter> others = new List<CommandParameter>();
                return Supports(commandParameters, out others, out parameter);
            }
        }

        public abstract class TwoParameterCommandHandler<T, U> : CommandHandler
            where T : class, CommandParameter
            where U : class, CommandParameter {
            protected T parameter1 = null;
            protected U parameter2 = null;

            public override bool CanHandle(List<CommandParameter> commandParameters) {
                if (commandParameters.Count != 2) return false;
                List<CommandParameter> others = new List<CommandParameter>();
                return Supports(commandParameters, out others, out parameter1, out parameter2);
            }
        }

        public abstract class ThreeParameterCommandHandler<T, U, V> : CommandHandler
            where T : class, CommandParameter
            where U : class, CommandParameter
            where V : class, CommandParameter {
            protected T parameter1 = null;
            protected U parameter2 = null;
            protected V parameter3 = null;

            public override bool CanHandle(List<CommandParameter> commandParameters) {
                if (commandParameters.Count != 3) return false;
                List<CommandParameter> others = new List<CommandParameter>();
                return Supports(commandParameters, out others, out parameter1, out parameter2, out parameter3);
            }
        }

        public abstract class FourParameterCommandHandler<T, U, V, W> : CommandHandler
            where T : class, CommandParameter
            where U : class, CommandParameter
            where V : class, CommandParameter
            where W : class, CommandParameter {
            protected T parameter1 = null;
            protected U parameter2 = null;
            protected V parameter3 = null;
            protected W parameter4 = null;

            public override bool CanHandle(List<CommandParameter> commandParameters) {
                if (commandParameters.Count != 4) return false;
                List<CommandParameter> others = new List<CommandParameter>();
                return Supports(commandParameters, out others, out parameter1, out parameter2, out parameter3, out parameter4);
            }
        }
    }
}
