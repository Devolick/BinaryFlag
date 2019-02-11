using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace BinaryFlag.NET.Functions
{
    public static class BinaryFunctions
    {
        private const int MaxBytes = 268435456;
        private const byte LastByte = 64;

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SqlBinary SQLSetBinaryFlag(int index, bool flag, SqlBinary sqlBinary)
        {
#if !DEBUG
            using (SqlConnection conn
                = new SqlConnection("context connection=true"))
#endif
            {
#if !DEBUG
                conn.Open();
#endif
                if (!sqlBinary.IsNull)
                    return new SqlBinary(SetBinaryFlag(index, flag, sqlBinary.Value));

                return new SqlBinary(SetBinaryFlag(index, flag));
            }
        }

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

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SQLHasBinaryFlag(int index, SqlBinary sqlBinary)
        {
#if !DEBUG
            using (SqlConnection conn
                = new SqlConnection("context connection=true"))
#endif
            {
#if !DEBUG
                conn.Open();
#endif

                if (!sqlBinary.IsNull)
                    return HasBinaryFlag(index, sqlBinary.Value);

                return false;
            }
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

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static string SQLFindBinaryIndexes(SqlBinary sqlBinary, string separator = ",")
        {
#if !DEBUG
            using (SqlConnection conn
                = new SqlConnection("context connection=true"))
#endif
            {
#if !DEBUG
                conn.Open();
#endif
                if (string.IsNullOrEmpty(separator))
                    throw new ArgumentNullException(nameof(separator));

                if (!sqlBinary.IsNull)
                    return string.Join(
                        separator,
                        FindBinaryIndexes(sqlBinary.Value)
                        .Select(s => s.ToString()).ToArray());

                return string.Empty;
            }
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

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SqlBinary SQLCreateBinaryIndexes(string separatedIndexes, string separator = ",")
        {
#if !DEBUG
            using (SqlConnection conn
                = new SqlConnection("context connection=true"))
#endif
            {
#if !DEBUG
                conn.Open();
#endif

                if (string.IsNullOrEmpty(separator))
                    throw new ArgumentNullException(nameof(separator));

                if (string.IsNullOrEmpty(separatedIndexes))
                    throw new ArgumentNullException(nameof(separatedIndexes));

                IEnumerable<int> splitIndexes = separatedIndexes
                    .Split(new string[1] { separator },
                            StringSplitOptions.None)
                    .Select(s => int.Parse(s));
                byte[] bytes = CreateBinaryIndexes(splitIndexes);

                return new SqlBinary(bytes);
            }
        }

        public static byte[] CreateBinaryIndexes(IEnumerable<int> indexes)
        {
            if (indexes == null)
                indexes = new List<int>(0);
            
            int biggerIndex = 0;
            foreach (int i in indexes)
            {
                if (biggerIndex < i)
                    biggerIndex = i;
            }
            int bytesLength = (int)Math.Ceiling(biggerIndex / 8f);
            byte[] bytes = new byte[bytesLength];

            foreach (int index in indexes)
                bytes = SetBinaryFlag(index, true, bytes, false);

            return bytes;
        }


        public static byte[] CreateBinaryIndexes(IEnumerable<KeyValuePair<int, bool[]>> indexes, byte size)
        {
            if (indexes == null)
                indexes = new Dictionary<int, bool[]>();

            if (!indexes.Any(a => a.Value.Length == size))
                throw new IndexOutOfRangeException("All indexes must have same length size.");

            int biggerIndex = 0;
            foreach (KeyValuePair<int, bool[]> i in indexes)
            {
                if (biggerIndex < i.Key)
                    biggerIndex = i.Key;
            }

            int bytesLength = (int)Math.Ceiling((biggerIndex * size) / 8f);
            byte[] bytes = new byte[bytesLength];

            

            return bytes;
        }
    }
}
