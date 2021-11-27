using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommands.Tests.ParameterParsingTests {
    class ParsingTestUtility {
        public static T GetDelegateProperty<T>(String propertyId, Delegate target) {
            var fields = target.Target.GetType().GetFields().Select(t => t.Name).ToList();
            return (T)target.Target.GetType().GetFields().Where(t => t.Name == propertyId).Select(t => t.GetValue(target.Target)).First();
        }
    }
}
