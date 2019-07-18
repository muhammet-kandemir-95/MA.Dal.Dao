using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dal
{
    public static class MADataExtensions
    {
        #region Methods

        #region Command

        #region Convert Result
        #region DotNET Types
        public static MAData.DataReader.ValueAndIsNull GetFirstRowColumn(this MAData.Command command)
        {
            using (MAData.DataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    return new MAData.DataReader.ValueAndIsNull(reader[0], reader.IsDBNull(0));
                }
            }
            return new MAData.DataReader.ValueAndIsNull(null, true);
        }
        public static TResult GetFirstRowColumn_Class<TResult>(this MAData.Command command, Func<object, TResult> getFunc) where TResult : class
        {
            MAData.DataReader.ValueAndIsNull value = GetFirstRowColumn(command);
            if (value.IsNull)
                return null;
            return getFunc(value.Value);
        }
        public static Nullable<TResult> GetFirstRowColumn_Struct<TResult>(this MAData.Command command, Func<object, Nullable<TResult>> getFunc) where TResult : struct
        {
            MAData.DataReader.ValueAndIsNull value = GetFirstRowColumn(command);
            if (value.IsNull)
                return null;
            return getFunc(value.Value);
        }

        public static string GetFirstRowColumnString(this MAData.Command command)
        {
            return GetFirstRowColumn_Class<string>(command, o => o.ToString());
        }

        public static Nullable<bool> GetFirstRowColumnBool(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<bool>(command, o => Convert.ToBoolean(o));
        }

        public static Nullable<char> GetFirstRowColumnChar(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<char>(command, o => Convert.ToChar(o));
        }

        public static Nullable<byte> GetFirstRowColumnByte(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<byte>(command, o => Convert.ToByte(o));
        }
        public static Nullable<sbyte> GetFirstRowColumnSByte(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<sbyte>(command, o => Convert.ToSByte(o));
        }

        public static Nullable<short> GetFirstRowColumnShort(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<short>(command, o => Convert.ToInt16(o));
        }
        public static Nullable<ushort> GetFirstRowColumnUShort(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<ushort>(command, o => Convert.ToUInt16(o));
        }

        public static Nullable<int> GetFirstRowColumnInt(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<int>(command, o => Convert.ToInt32(o));
        }
        public static Nullable<uint> GetFirstRowColumnUInt(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<uint>(command, o => Convert.ToUInt32(o));
        }

        public static Nullable<long> GetFirstRowColumnLong(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<long>(command, o => Convert.ToInt64(o));
        }
        public static Nullable<ulong> GetFirstRowColumnULong(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<ulong>(command, o => Convert.ToUInt64(o));
        }

        public static Nullable<float> GetFirstRowColumnFloat(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<float>(command, o => Convert.ToSingle(o));
        }

        public static Nullable<double> GetFirstRowColumnDouble(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<double>(command, o => Convert.ToDouble(o));
        }

        public static Nullable<decimal> GetFirstRowColumnDecimal(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<decimal>(command, o => Convert.ToDecimal(o));
        }

        public static Nullable<DateTime> GetFirstRowColumnDateTime(this MAData.Command command)
        {
            return GetFirstRowColumn_Struct<DateTime>(command, o => Convert.ToDateTime(o));
        }

        #endregion

        public static TResult GetFirstRowColumnEnum<TResult>(this MAData.Command command, object[] enums)
        {
            MAData.DataReader.ValueAndIsNull value = GetFirstRowColumn(command);
            if (value.IsNull)
                return (TResult)enums[0];
            else if (value.Value is string)
            {
                string valueString = (string)value.Value;
                foreach (var item in enums)
                {
                    string itemString = item.ToString();
                    if (itemString == valueString)
                        return (TResult)item;
                }
            }
            else
            {
                int valueInt = (int)value.Value;
                foreach (var item in enums)
                {
                    var itemInt = (int)item;
                    if (itemInt == valueInt)
                        return (TResult)item;
                }
            }
            return (TResult)enums[0];
        }
        public static TResult GetFirstRowColumnEnum<TResult>(this MAData.Command command)
        {
            TResult[] enums = (TResult[])Enum.GetValues(typeof(TResult));
            return GetFirstRowColumnEnum<TResult>(command, enums.Select(e => (object)e).ToArray());
        }
        #endregion
        #endregion

        #endregion
    }
}
