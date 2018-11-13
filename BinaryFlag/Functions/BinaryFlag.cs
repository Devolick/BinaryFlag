using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace BinaryFlag.Functions
{
    public class BinaryFlag
    {
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
                if(!sqlBinary.IsNull)
                    return new SqlBinary(SetBinaryFlag(index, flag, sqlBinary.Value));

                return new SqlBinary(SetBinaryFlag(index, flag));
            }
        }

        public static byte[] SetBinaryFlag(int index, bool flag, byte[] sqlBytes = null, bool cleanTail = true)
        {
            if (index < 1)
                throw new IndexOutOfRangeException("Index cannot be less than zero or negative.");

            if (sqlBytes == null)
                sqlBytes = new byte[1];

            byte[] bytes;

            int byteIndex = (int)Math.Ceiling(index / 8f) - 1;

            if (byteIndex >= sqlBytes.Length)
            {
                bytes = new byte[byteIndex + 1];
                for (int i = 0; i < sqlBytes.Length; i++)
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
                        for (int x = 0; x < i + 1; x++)
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

                if(!sqlBinary.IsNull)
                    return HasBinaryFlag(index, sqlBinary.Value);

                return false;
            }
        }

        public static bool HasBinaryFlag(int index, byte[] sqlBytes = null)
        {
            if (index < 1)
                throw new IndexOutOfRangeException("Index cannot be less than zero or negative.");

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
        public static string SQLViewBinaryIndexes(SqlBinary sqlBinary, string separator = ",")
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
                    return string.Join(
                        separator, 
                        FindBinaryIndexes(sqlBinary.Value).Select(s=>s.ToString())
                            .ToArray());

                return string.Empty;
            }
        }

        public static IEnumerable<int> FindBinaryIndexes(byte[] sqlBytes)
        {
            if (sqlBytes == null)
                sqlBytes = new byte[0];

            for (int i = 0; i < sqlBytes.Length; i++)
                for (int b = 1; b < 9; b++)
                {
                    int index = (i * 8) + b;
                    if (HasBinaryFlag(index, sqlBytes))
                        yield return index;
                }
        }

        public static byte[] MarkBinaryIndexes(IEnumerable<int> indexes)
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
