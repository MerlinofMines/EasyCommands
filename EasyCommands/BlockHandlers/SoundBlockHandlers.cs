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
                defaultStringProperty = StringPropertyType.SOUND;
                defaultBooleanProperty = BooleanPropertyType.POWER;
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.VOLUME);
                numericPropertySetters.Add(NumericPropertyType.VOLUME, new SimpleNumericPropertySetter<IMySoundBlock>((b) => b.Volume, (b, v) => b.Volume = v,1));
                numericPropertySetters.Add(NumericPropertyType.RANGE, new SimpleNumericPropertySetter<IMySoundBlock>((b) => b.Range, (b, v) => b.Range = v,50));
                numericPropertySetters.Add(NumericPropertyType.HEIGHT, new SimpleNumericPropertySetter<IMySoundBlock>((b) => b.LoopPeriod, (b, v) => b.LoopPeriod = v, 60));
                numericPropertyGetters.Add(NumericPropertyType.VOLUME, (b) => b.Volume);
                numericPropertyGetters.Add(NumericPropertyType.RANGE, (b) => b.Range);
                numericPropertyGetters.Add(NumericPropertyType.HEIGHT, (b) => b.LoopPeriod);
                stringPropertyGetters.Add(StringPropertyType.SOUND, (b) => b.SelectedSound);
                stringPropertySetters.Add(StringPropertyType.SOUND, (b, v) => b.SelectedSound = v);
                booleanPropertySetters[BooleanPropertyType.POWER] = (b, v) => { if (v) b.Play(); else b.Stop(); };
            }
        }
    }
}
