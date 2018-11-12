using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Data.SqlTypes;

namespace BinaryFlag.Functions
{
    public class BinaryFlag
    {
        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static SqlBinary SetBinaryFlag(int index,bool flag, SqlBinary sqlBinary)
        {
#if !DEBUG
            using (SqlConnection conn
                = new SqlConnection("context connection=true"))
#endif
            {
#if !DEBUG
                conn.Open();
#endif

                byte[] bytes = new byte[0];

                int byteIndex = (int)Math.Ceiling(index / 8f) - 1;
                byteIndex = byteIndex < 0 ? 0 : byteIndex;

                if (sqlBinary.IsNull || byteIndex >= sqlBinary.Length)
                {
                    bytes = new byte[byteIndex + 1];
                    if (!sqlBinary.IsNull)
                        for (int i = 0; i < sqlBinary.Length; i++)
                            bytes[i] = sqlBinary[i];
                }
                else if (!sqlBinary.IsNull)
                    bytes = sqlBinary.Value;

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

                return new SqlBinary(bytes);
            }
        }

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static bool HasBinaryFlag(int index, SqlBinary sqlBinary)
        {
#if !DEBUG
            using (SqlConnection conn
                = new SqlConnection("context connection=true"))
#endif
            {
#if !DEBUG
                conn.Open();
#endif

                int byteIndex = (int)Math.Ceiling(index / 8f) - 1;
                if (sqlBinary.IsNull || sqlBinary.Length <= byteIndex)
                    return false;

                return (sqlBinary.Value[byteIndex] &
                    (byte)Math.Pow(2, (index - 1) - (byteIndex * 8))) != 0;
            }
        }
    }
}
