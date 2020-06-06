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
        public delegate void OneParameterBlockDelegate<T>(BlockHandler b, Object e, T t);
        public delegate void TwoParameterBlockDelegate<T, U>(BlockHandler b, Object e, T t, U u) where T : class, CommandParameter where U : class, CommandParameter;
        public delegate void ThreeParameterBlockDelegate<T, U, V>(BlockHandler b, Object e, T t, U u, V v) where T : class, CommandParameter where U : class, CommandParameter where V : class, CommandParameter;
        public delegate void FourParameterBlockDelegate<T, U, V, W>(BlockHandler b, Object e, T t, U u, V v, W w) where T : class, CommandParameter where U : class, CommandParameter where V : class, CommandParameter where W : class, CommandParameter;

        public class OneParamBlockCommandHandler<T> : OneParameterCommandHandler<T> where T : class, CommandParameter {
            private OneParameterBlockDelegate<T> action;
            protected SelectorEntityProvider entityProvider;
            protected BlockHandler blockHandler;

            public OneParamBlockCommandHandler(SelectorEntityProvider entityProvider, BlockHandler blockHandler, OneParameterBlockDelegate<T> action) {
                this.entityProvider = entityProvider;
                this.blockHandler = blockHandler;
                this.action = action;
            }

            public override bool Handle() {
                entityProvider.GetEntities().ForEach(entity => action(blockHandler, entity, parameter));
                return true;
            }
        }

        public class TwoParamBlockCommandHandler<T, U> : TwoParameterCommandHandler<T, U> where T : class, CommandParameter where U : class, CommandParameter {
            private TwoParameterBlockDelegate<T, U> action;
            protected SelectorEntityProvider entityProvider;
            protected BlockHandler blockHandler;

            public TwoParamBlockCommandHandler(SelectorEntityProvider entityProvider, BlockHandler blockHandler, TwoParameterBlockDelegate<T, U> action) {
                this.entityProvider = entityProvider;
                this.blockHandler = blockHandler;
                this.action = action;
            }

            public override bool CanHandle(List<CommandParameter> commandParameters) {
                List<CommandParameter> others;
                return Supports<T, U>(commandParameters, out others, out parameter1, out parameter2);
            }

            public override bool Handle() {
                entityProvider.GetEntities().ForEach(entity => action(blockHandler, entity, parameter1, parameter2));
                return true;
            }
        }

        public class ThreeParamBlockCommandHandler<T, U, V> : ThreeParameterCommandHandler<T, U, V> where T : class, CommandParameter where U : class, CommandParameter where V : class, CommandParameter {
            private ThreeParameterBlockDelegate<T, U, V> action;
            protected SelectorEntityProvider entityProvider;
            protected BlockHandler blockHandler;

            public ThreeParamBlockCommandHandler(SelectorEntityProvider entityProvider, BlockHandler blockHandler, ThreeParameterBlockDelegate<T, U, V> action) {
                this.entityProvider = entityProvider;
                this.blockHandler = blockHandler;
                this.action = action;
            }

            public override bool Handle() {
                entityProvider.GetEntities().ForEach(entity => action(blockHandler, entity, parameter1, parameter2, parameter3));
                return true;
            }
        }

        public class FourParamBlockCommandHandler<T, U, V, W> : FourParameterCommandHandler<T, U, V, W> where T : class, CommandParameter where U : class, CommandParameter where V : class, CommandParameter where W : class, CommandParameter {
            private FourParameterBlockDelegate<T, U, V, W> action;
            protected SelectorEntityProvider entityProvider;
            protected BlockHandler blockHandler;

            public FourParamBlockCommandHandler(SelectorEntityProvider entityProvider, BlockHandler blockHandler, FourParameterBlockDelegate<T, U, V, W> action) {
                this.entityProvider = entityProvider;
                this.blockHandler = blockHandler;
                this.action = action;
            }

            public override bool Handle() {
                entityProvider.GetEntities().ForEach(entity => action(blockHandler, entity, parameter1, parameter2, parameter3, parameter4));
                return true;
            }
        }
    }
}
