using System;
using System.Collections.Generic;

namespace BinaryFlag.Standard.Functions
{
    public static class BinaryFunctions
    {
        private const int MaxBytes = 268435456;
        private const byte LastByte = 64;

        public static byte[] SetBinaryFlag(int index, bool flag, byte[] sqlBytes = null, bool cleanTail = true)
        {
            if (index < 1)
                throw new IndexOutOfRangeException("Index cannot be less than zero or negative.");
            if (sqlBytes != null &&
                (sqlBytes.Length > MaxBytes ||
                    (sqlBytes.Length == MaxBytes && sqlBytes[MaxBytes - 1] > LastByte)))
                throw new ArgumentException($"Maximum[{MaxBytes - 1}] = (byte){LastByte} bytes number exceeded. Int.MaxValue");

            if (sqlBytes == null)
                sqlBytes = new byte[1];

            byte[] bytes;

            int byteIndex = (int)Math.Ceiling(index / 8f) - 1;

            if (byteIndex >= sqlBytes.Length)
            {
                bytes = new byte[byteIndex + 1];
                for (int i = 0; i < sqlBytes.Length; ++i)
                    bytes[i] = sqlBytes[i];
            }
            else
                bytes = sqlBytes;

            if (flag)
                bytes[byteIndex] = (byte)(bytes[byteIndex] | (byte)Math.Pow(2, (index - 1) - (byteIndex * 8)));
            else
                bytes[byteIndex] = (byte)(bytes[byteIndex] & ~(byte)Math.Pow(2, (index - 1) - (byteIndex * 8)));

            if (cleanTail && !flag && bytes.Length > 0 && bytes[bytes.Length - 1] == 0)
                for (int i = bytes.Length - 1; i > -1; i--)
                    if (bytes[i] > 0)
                    {
                        byte[] cleanBytes = new byte[i + 1];
                        for (int x = 0; x < i + 1; ++x)
                            cleanBytes[x] = bytes[x];

                        bytes = cleanBytes;
                        break;
                    }

            return bytes;
        }

        public static bool HasBinaryFlag(int index, byte[] sqlBytes)
        {
            if (index < 1)
                throw new IndexOutOfRangeException("Index cannot be less than zero or negative.");
            if (sqlBytes != null &&
                (sqlBytes.Length > MaxBytes ||
                    (sqlBytes.Length == MaxBytes && sqlBytes[MaxBytes - 1] > LastByte)))
                throw new ArgumentException($"Maximum[{MaxBytes - 1}] = (byte){LastByte} bytes number exceeded. Int.MaxValue");

            if (sqlBytes == null)
                sqlBytes = new byte[1];

            int byteIndex = (int)Math.Ceiling(index / 8f) - 1;
            if (sqlBytes.Length <= byteIndex)
                return false;

            return (sqlBytes[byteIndex] &
                (byte)Math.Pow(2, (index - 1) - (byteIndex * 8))) != 0;
        }

        public static IEnumerable<int> FindBinaryIndexes(byte[] sqlBytes)
        {
            if (sqlBytes == null)
                sqlBytes = new byte[0];
            if (sqlBytes != null &&
                (sqlBytes.Length > MaxBytes ||
                    (sqlBytes.Length == MaxBytes && sqlBytes[MaxBytes - 1] > LastByte)))
                throw new ArgumentException($"Maximum[{MaxBytes - 1}] = (byte){LastByte} bytes number exceeded. Int.MaxValue");

            List<int> result = new List<int>();
            for (int i = 0; i < sqlBytes.Length; ++i)
            {
                if (sqlBytes[i] > 0)
                {
                    for (int b = 1; b < 9; ++b)
                    {
                        int index = (i * 8) + b;
                        if (index < 0) break;
                        if (HasBinaryFlag(index, sqlBytes))
                            result.Add(index);
                    }
                }
            }

            return result;
        }

        public static byte[] CreateBinaryIndexes(IEnumerable<int> indexes)
        {
            if (indexes == null)
                indexes = new List<int>(0);

            int count = 0;
            int biggerIndex = 0;
            foreach (int i in indexes)
            {
                if (biggerIndex < i)
                    biggerIndex = i;
                ++count;
            }
            int bytesLength = (int)Math.Ceiling(biggerIndex / 8f);
            byte[] bytes = new byte[bytesLength];

            foreach (int index in indexes)
                bytes = SetBinaryFlag(index, true, bytes, false);

            return bytes;
        }
    }
}
