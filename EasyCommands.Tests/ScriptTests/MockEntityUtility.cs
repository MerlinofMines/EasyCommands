using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
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

        public static void MockTextSurfaces<T>(Mock<T> surfaceProvider, params Mock<IMyTextSurface>[] mockSurfaces) where T : class, IMyTextSurfaceProvider {
            surfaceProvider.Setup(x => x.SurfaceCount).Returns(mockSurfaces.Length);
            for(int i = 0; i < mockSurfaces.Length; i++) {
                surfaceProvider.Setup(x => x.GetSurface(i)).Returns(mockSurfaces[i].Object);
            }
        }
    }
}
