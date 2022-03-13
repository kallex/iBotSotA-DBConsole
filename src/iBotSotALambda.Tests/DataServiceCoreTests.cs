using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DataServiceCore;
using Services;
using Xunit;

namespace iBotSotALambda.Tests
{
    public class DataServiceCoreTests
    {
        [Fact]
        public void FloatCompression_VectorToleranceTest()
        {
            const double Tolerance = 0.01;
            Vec3[] vectors = getCompressionTestVectors(1000);

            var originalFloats = vectors.Select(item => item.X)
                .Concat(vectors.Select(item => item.Y))
                .Concat(vectors.Select(item => item.Z)).ToArray();

            var originalFloatSpan = new Span<float>(originalFloats);
            var originalFloatBytes = MemoryMarshal.AsBytes(originalFloatSpan).ToArray();

            var packedFloats = FloatCompression.CompressVectors(vectors, Tolerance);
            var packedFloatBytes = packedFloats.packedX.Concat(packedFloats.packedY).Concat(packedFloats.packedZ)
                .ToArray();

            var packedRatio = packedFloatBytes.Length * 100.0 / originalFloatBytes.Length;
            Assert.True(packedRatio < 50, "packedRatio < 50");
        }

        private Vec3[] getCompressionTestVectors(int count)
        {
            // Time based random is good, tolerance tests shouldn't expect the fixed set
            var rand = new Random();

            float xBase = (float)rand.NextDouble() * 100;
            float yBase = (float)rand.NextDouble() * 100;
            float zBase = (float)rand.NextDouble() * 100;

            var result = new List<Vec3>();

            float xCurr = xBase;
            float yCurr = yBase;
            float zCurr = zBase;

            const double DeltaMax = 0.2;

            while (count-- > 0)
            {
                xCurr += getDelta(DeltaMax);
                yCurr += getDelta(DeltaMax);
                zCurr += getDelta(DeltaMax);

                var vec = new Vec3()
                {
                    X = xCurr,
                    Y = yCurr,
                    Z = zCurr,
                };
                result.Add(vec);
            }

            return result.ToArray();

            float getDelta(double deltaMax)
            {
                var normalizedDelta = rand.NextDouble() - 0.5;
                var delta = normalizedDelta * deltaMax;
                return (float) delta;
            }
        }
    }
}