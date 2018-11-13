using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;

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

                return new SqlBinary(SetBinaryFlag(index, flag, sqlBinary.Value));
            }
        }

        public static byte[] SetBinaryFlag(int index, bool flag, byte[] sqlBytes = null)
        {
            if (index < 1)
                throw new IndexOutOfRangeException("Index cannot be less than zero or negative.");

            if (sqlBytes == null)
                sqlBytes = new byte[0];

            byte[] bytes = new byte[0];

            int byteIndex = (int)Math.Ceiling(index / 8f) - 1;
            byteIndex = byteIndex < 0 ? 0 : byteIndex;

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

            if (!flag && bytes.Length > 0 && bytes[bytes.Length - 1] == 0)
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

                return HasBinaryFlag(index, sqlBinary.Value);
            }
        }

        public static bool HasBinaryFlag(int index, byte[] sqlBytes = null)
        {
            if (index < 1)
                throw new IndexOutOfRangeException("Index cannot be less than zero or negative.");

            if (sqlBytes == null)
                sqlBytes = new byte[0];

            int byteIndex = (int)Math.Ceiling(index / 8f) - 1;
            if (sqlBytes.Length <= byteIndex)
                return false;

            return (sqlBytes[byteIndex] &
                (byte)Math.Pow(2, (index - 1) - (byteIndex * 8))) != 0;
        }
    }
}
