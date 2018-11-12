using Microsoft.SqlServer.Server;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;

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

                BitArray bitArray = sqlBinary.IsNull ?
                    new BitArray(0) : new BitArray(sqlBinary.Value);

                if (bitArray.Length <= index - 1)
                {
                    bool[] boolArray =
                        new bool[bitArray.Length + (index - bitArray.Length)];
                    bitArray.CopyTo(boolArray, 0);
                    bitArray = new BitArray(boolArray);
                }

                bitArray[index - 1] = flag;

                if (bitArray.Length > index - 1 && !flag)
                {
                    int indexOf = -1;
                    for (int i = bitArray.Length - 1; i >= 0; i--)
                        if (bitArray[i])
                        {
                            indexOf = i + 1;
                            break;
                        }
                    if (indexOf > -1)
                        bitArray = new BitArray(bitArray.Cast<bool>()
                        .Take(indexOf).ToArray());
                    else
                        bitArray = new BitArray(0);
                }

                byte[] byteArray = 
                    new byte[(bitArray.Length - 1) / 8 + 1];
                bitArray.CopyTo(byteArray, 0);
                return new SqlBinary(byteArray);
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
                if (sqlBinary.IsNull) return false;
                BitArray bitArray = new BitArray(sqlBinary.Value);
                if (bitArray.Length < index) return false;
                return bitArray.Length > 0 ? bitArray[index - 1] : false;
            }
        }
    }
}
