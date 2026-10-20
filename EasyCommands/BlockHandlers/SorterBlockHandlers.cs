using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;
    partial class Program
    {
        public class SorterBlockerHandler : FunctionalBlockHandler<IMyConveyorSorter>
        {
            public SorterBlockerHandler()
            {
                AddBooleanHandler(Property.AUTO, (b) => b.DrainAll, (b,v) => b.DrainAll = v);
            }
        }
    }

}
