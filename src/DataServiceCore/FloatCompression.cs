using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DataServiceCore;
using ZfpDotnet;

namespace Services
{
    public class FloatCompression
    {
        public static (byte[] packedX, byte[] packedY, byte[] packedZ) CompressVectors(Vec3[] vectors, double tolerance = 0.0)
        {
            var packedX = PackFloatArray(vectors.Select(item => item.X), tolerance);
            var packedY = PackFloatArray(vectors.Select(item => item.Y), tolerance);
            var packedZ = PackFloatArray(vectors.Select(item => item.Z), tolerance);

            return (packedX, packedY, packedZ);
        }

        public static (byte[] packedX, byte[] packedY, byte[] packedZ, byte[] packedW) CompressQuartenions(Quart4[] quartenions, double tolerance = 0.0)
        {
            var packedX = PackFloatArray(quartenions.Select(item => item.X), tolerance);
            var packedY = PackFloatArray(quartenions.Select(item => item.Y), tolerance);
            var packedZ = PackFloatArray(quartenions.Select(item => item.Z), tolerance);
            var packedW = PackFloatArray(quartenions.Select(item => item.W), tolerance);

            return (packedX, packedY, packedZ, packedW);
        }

        public static Vec3[] DecompressVectors((byte[] packedX, byte[] packedY, byte[] packedZ) packedData, int originalVectorCount)
        {
            var x = UnpackFloatArray(packedData.packedX, originalVectorCount);
            var y = UnpackFloatArray(packedData.packedY, originalVectorCount);
            var z = UnpackFloatArray(packedData.packedZ, originalVectorCount);

            var vectors = new Vec3[originalVectorCount];
            for (int i = 0; i < vectors.Length; i++)
            {
                var vec = new Vec3()
                {
                    X = x[i],
                    Y = y[i],
                    Z = z[i]
                };
                vectors[i] = vec;
            }

            return vectors;
        }

        public static Quart4[] DecompressQuartenions((byte[] packedX, byte[] packedY, byte[] packedZ, byte[] packedW) packedData, int originalQuartenionCount)
        {
            var x = UnpackFloatArray(packedData.packedX, originalQuartenionCount);
            var y = UnpackFloatArray(packedData.packedY, originalQuartenionCount);
            var z = UnpackFloatArray(packedData.packedZ, originalQuartenionCount);
            var w = UnpackFloatArray(packedData.packedW, originalQuartenionCount);

            var quartenions = new Quart4[originalQuartenionCount];
            for (int i = 0; i < quartenions.Length; i++)
            {
                var quart = new Quart4()
                {
                    X = x[i],
                    Y = y[i],
                    Z = z[i],
                    W = w[i]
                };
                quartenions[i] = quart;
            }
            return quartenions;
        }


        public static float[] UnpackFloatArray(IEnumerable<byte> byteEnumerable, int originalFloatCount)
        {
            var sourceBytes = byteEnumerable.ToArray();
            var originalData = new byte[originalFloatCount * sizeof(float)];
            ZfpNative.Decompress(sourceBytes, originalData, out var fieldType, out var unitCount);
            //originalData = originalData.Take((int) unitCount).ToArray();
            var originalFloats = MemoryMarshal.Cast<byte, float>(originalData).ToArray();
            return originalFloats;
        }

        public static byte[] PackFloatArray(IEnumerable<float> floatEnumerable, double tolerance)
        {
            var floats = floatEnumerable.ToArray();
            var packedData = new byte[floats.Length * sizeof(float)];
            var packedSize = ZfpNative.Compress(floats, packedData, tolerance);
            packedData = packedData.Take((int)packedSize).ToArray();
            return packedData;
        }
    }
}