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
        public delegate bool CanHandle(List<CommandParameter> parameters);
        public delegate void Handle(object e);
        //public delegate void OneParameterBlockDelegate<T>(BlockHandler b, Object e, ref T t);
        public delegate void TwoParameterBlockDelegate<T, U>(BlockHandler b, Object e, T t, U u);
        public delegate void ThreeParameterBlockDelegate<T, U, V>(BlockHandler b, Object e, T t, U u, V v);
        public delegate void FourParameterBlockDelegate<T, U, V, W>(BlockHandler b, Object e, T t, U u, V v, W w);

        public class SelectorEntityProvider {
            protected SelectorCommandParameter selector;

            public SelectorEntityProvider(SelectorCommandParameter selector) {
                this.selector = selector;
            }

            public List<Object> GetEntities() {
                List<Object> entities = selector.isGroup ? BlockHandlerRegistry.GetBlocksInGroup(selector.blockType, selector.selector) : BlockHandlerRegistry.GetBlocks(selector.blockType, selector.selector);
                return selector.selectorIndex.HasValue ? new List<Object>() { entities[selector.selectorIndex.Value] } : entities;
            }
            public override String ToString() {
                return selector.blockType + (selector.isGroup ? " in group named " : " named " + selector.selector);
            }
        }

        public class BlockCommandHandler {
            public SelectorEntityProvider e;
            public BlockHandler b;
            public bool Supports = true;
            public CanHandle canHandle;
            public Handle handle;
            public List<CommandParameter> Bind<X>(List<CommandParameter> parameters, ref X t) {
                List<CommandParameter> ps = new List<CommandParameter>(parameters);
                int i = ps.FindIndex(p => p is X);
                if (i >= 0) { t = (X)ps[i]; ps.RemoveAt(i); } else Supports = false;
                return ps;
            }
            public void Execute() {
                e.GetEntities().ForEach(entity => handle(entity));
            }
        }
//        Uncomment when needed
//        public class TwoParamBlockHandler<T, U, V> : BlockCommandHandler {
//            T p1; U p2; V p3;
//            public ThreeParamBlockHandler(ThreeParameterBlockDelegate<T, U, V> action) {
//                canHandle = (p) => Bind<V>(Bind<U>(Bind<T>(p, ref p1), ref p2), ref p3).Count == 0 && Supports;
//                handle = (e) => action(b, e, ref p1, ref p2, ref p3);
//            }
//        }
        public class BlockCommandHandler2<T, U> : BlockCommandHandler {
            T p1; U p2;
            public BlockCommandHandler2(TwoParameterBlockDelegate<T, U> action) {
                canHandle = (p) => Bind<U>(Bind<T>(p, ref p1), ref p2).Count == 0 && Supports;
                handle = (e) => action(b, e, p1, p2);
            }
        }
        public class BlockCommandHandler3<T, U, V> : BlockCommandHandler {
            T p1; U p2; V p3;
            public BlockCommandHandler3(ThreeParameterBlockDelegate<T, U, V> action) {
                canHandle = (p) => Bind<V>(Bind<U>(Bind<T>(p, ref p1), ref p2), ref p3).Count == 0 && Supports;
                handle = (e) => action(b, e, p1, p2, p3);
            }
        }
        public class BlockCommandHandler4<T, U, V, W> : BlockCommandHandler {
            T p1; U p2; V p3; W p4;
            public BlockCommandHandler4(FourParameterBlockDelegate<T, U, V, W> action) {
                canHandle = (p) => Bind<W>(Bind<V>(Bind<U>(Bind<T>(p, ref p1), ref p2), ref p3), ref p4).Count == 0 && Supports;
                handle = (e) => action(b, e, p1, p2, p3, p4);
            }
        }
    }
}
