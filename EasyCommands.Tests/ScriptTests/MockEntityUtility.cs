using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    class MockEntityUtility {

        public static MyDetectedEntityInfo MockDetectedEntity(Vector3D position) {
            return new MyDetectedEntityInfo(123, "name", MyDetectedEntityType.LargeGrid, position,
                new MatrixD(), Vector3D.Zero, VRage.Game.MyRelationsBetweenPlayerAndBlock.Neutral, new BoundingBoxD(), 123);
        }

        public static MyDetectedEntityInfo MockNoDetectedEntity() {
            return new MyDetectedEntityInfo();
        }
    }
}
