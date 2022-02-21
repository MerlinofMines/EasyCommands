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
        public class SoundBlockHandler : FunctionalBlockHandler<IMySoundBlock> {
            public SoundBlockHandler() {
                AddNumericHandler(Property.VOLUME, b => b.Volume, (b, v) => b.Volume = v, 0.1f);
                AddNumericHandler(Property.RANGE, b => b.Range, (b, v) => b.Range = v, 50);
                AddNumericHandler(Property.LEVEL, b => b.LoopPeriod, (b, v) => b.LoopPeriod = v, 10);
                AddPropertyHandler(Property.MEDIA, ReturnTypedHandler(Return.STRING,
                    TypeHandler(BooleanHandler((b) => b.DetailedInfo.Contains("Loop timer"), (b, v) => { if (v) b.Play(); else b.Stop(); }), Return.BOOLEAN),
                    TypeHandler(StringHandler(b => b.SelectedSound, (b, v) => b.SelectedSound = v), Return.STRING)));

                AddListHandler(Property.MEDIA_LIST, b => {
                    var availableSounds = NewList<string>();
                    b.GetSounds(availableSounds);
                    return NewKeyedList(availableSounds.Select(GetStaticVariable));
                });
                defaultPropertiesByPrimitive[Return.STRING] = Property.MEDIA;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.VOLUME;
            }
        }
    }
}
