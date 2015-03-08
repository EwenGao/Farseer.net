using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using FS.Mapping.Table;

namespace FS.Extend
{
    public static class IDataReaderExtend
    {
        public static List<T> ToList<T>(this IDataReader reader) where T : class, new()
        {
            return EntityConverter<T>.ToList(reader);
        }
        public static T ToInfo<T>(this IDataReader reader) where T : class, new()
        {
            return EntityConverter<T>.ToList(reader).FirstOrDefault();
        }

        #region Static Readonly Fields
        private static readonly MethodInfo DataRecordItemGetterInt = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo DataRecordGetOrdinal = typeof(IDataRecord).GetMethod("GetOrdinal");
        private static readonly MethodInfo DataReaderRead = typeof(IDataReader).GetMethod("Read");
        private static readonly MethodInfo ConvertIsDbNull = typeof(Convert).GetMethod("IsDBNull");
        private static readonly MethodInfo DataRecordGetDateTime = typeof(IDataRecord).GetMethod("GetDateTime");
        private static readonly MethodInfo DataRecordGetDecimal = typeof(IDataRecord).GetMethod("GetDecimal");
        private static readonly MethodInfo DataRecordGetDouble = typeof(IDataRecord).GetMethod("GetDouble");
        private static readonly MethodInfo DataRecordGetInt32 = typeof(IDataRecord).GetMethod("GetInt32");
        private static readonly MethodInfo DataRecordGetInt64 = typeof(IDataRecord).GetMethod("GetInt64");
        private static readonly MethodInfo DataRecordGetString = typeof(IDataRecord).GetMethod("GetString");
        private static readonly MethodInfo DataRecordIsDbNull = typeof(IDataRecord).GetMethod("IsDBNull");
        #endregion

        private class EntityConverter<T> where T : class, new()
        {
            private struct DbColumnInfo
            {
                private readonly string _propertyName;
                public readonly string ColumnName;
                public readonly Type Type;
                public readonly MethodInfo SetMethod;

                public DbColumnInfo(PropertyInfo prop, ColumnAttribute attr)
                {
                    _propertyName = prop.Name;
                    ColumnName = attr.Name ?? prop.Name;
                    Type = prop.PropertyType;
                    SetMethod = prop.GetSetMethod(false);
                }
            }

            private static Converter<IDataReader, List<T>> _batchDataLoader;

            private static Converter<IDataReader, List<T>> BatchDataLoader
            {
                get
                {
                    return _batchDataLoader ?? (_batchDataLoader = CreateBatchDataLoader(new List<DbColumnInfo>(GetProperties())));
                }
            }

            private static IEnumerable<DbColumnInfo> GetProperties()
            {
                var dbResult = Attribute.GetCustomAttribute(typeof(T), typeof(DBAttribute), true) as DBAttribute;
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (prop.GetIndexParameters().Length > 0) { continue; }
                    var setMethod = prop.GetSetMethod(false);
                    if (setMethod == null) { continue; }
                    var attr = Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute), true) as ColumnAttribute;
                    if (dbResult != null)
                    {
                        if (attr == null) { attr = new ColumnAttribute(); }
                    }
                    else if (attr == null) { continue; }
                    yield return new DbColumnInfo(prop, attr);
                }
            }

            internal static List<T> ToList(IDataReader reader)
            {
                return BatchDataLoader(reader);
            }
            #region Init Methods

            private static Converter<IDataReader, List<T>> CreateBatchDataLoader(List<DbColumnInfo> columnInfoes)
            {
                var dm = new DynamicMethod(String.Empty, typeof(List<T>), new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
                var il = dm.GetILGenerator();
                var list = il.DeclareLocal(typeof(List<T>));
                var item = il.DeclareLocal(typeof(T));
                var exit = il.DefineLabel();
                var loop = il.DefineLabel();
                // List<T> list = new List<T>();
                il.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, list);
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                LocalBuilder[] colIndices = GetColumnIndices(il, columnInfoes);
                // while (arg.Read()) {
                il.MarkLabel(loop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, DataReaderRead);
                il.Emit(OpCodes.Brfalse, exit);
                //      T item = new T { %Property% =  };
                BuildItem(il, columnInfoes, item, colIndices);
                //      list.Add(item);
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Callvirt, typeof(List<T>).GetMethod("Add"));
                // }
                il.Emit(OpCodes.Br, loop);
                il.MarkLabel(exit);
                // return list;
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ret);
                return (Converter<IDataReader, List<T>>)dm.CreateDelegate(typeof(Converter<IDataReader, List<T>>));
            }

            private static LocalBuilder[] GetColumnIndices(ILGenerator il, IList<DbColumnInfo> columnInfoes)
            {
                var colIndices = new LocalBuilder[columnInfoes.Count];
                for (var i = 0; i < colIndices.Length; i++)
                {
                    // int %index% = arg.GetOrdinal(%ColumnName%);
                    colIndices[i] = il.DeclareLocal(typeof(int));
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, columnInfoes[i].ColumnName);
                    il.Emit(OpCodes.Callvirt, DataRecordGetOrdinal);
                    il.Emit(OpCodes.Stloc_S, colIndices[i]);
                }
                return colIndices;
            }

            private static void BuildItem(ILGenerator il, List<DbColumnInfo> columnInfoes, LocalBuilder item, LocalBuilder[] colIndices)
            {
                // T item = new T();
                il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, item);
                for (var i = 0; i < colIndices.Length; i++)
                {
                    if (IsCompatibleType(columnInfoes[i].Type, typeof(int)))
                    {
                        // item.%Property% = arg.GetInt32(%index%);
                        ReadInt32(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(int?)))
                    {
                        // item.%Property% = arg.IsDBNull ? default(int?) : (int?)arg.GetInt32(%index%);
                        ReadNullableInt32(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(long)))
                    {
                        // item.%Property% = arg.GetInt64(%index%);
                        ReadInt64(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(long?)))
                    {
                        // item.%Property% = arg.IsDBNull ? default(long?) : (long?)arg.GetInt64(%index%);
                        ReadNullableInt64(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(decimal)))
                    {
                        // item.%Property% = arg.GetDecimal(%index%);
                        ReadDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(decimal?))
                    {
                        // item.%Property% = arg.IsDBNull ? default(decimal?) : (int?)arg.GetDecimal(%index%);
                        ReadNullableDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(DateTime))
                    {
                        // item.%Property% = arg.GetDateTime(%index%);
                        ReadDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(DateTime?))
                    {
                        // item.%Property% = arg.IsDBNull ? default(DateTime?) : (int?)arg.GetDateTime(%index%);
                        ReadNullableDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else
                    {
                        // item.%Property% = (%PropertyType%)arg[%index%];
                        ReadObject(il, item, columnInfoes, colIndices, i);
                    }
                }
            }

            private static bool IsCompatibleType(Type t1, Type t2)
            {
                if (t1 == t2) { return true; }
                if (t1.IsEnum && Enum.GetUnderlyingType(t1) == t2) { return true; }
                var u1 = Nullable.GetUnderlyingType(t1);
                var u2 = Nullable.GetUnderlyingType(t2);
                if (u1 != null && u2 != null) { return IsCompatibleType(u1, u2); }
                return false;
            }

            private static void ReadInt32(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt32);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadNullableInt32(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                var intNull = il.DefineLabel();
                var intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt32);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadInt64(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt64);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadNullableInt64(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                var intNull = il.DefineLabel();
                var intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordGetInt64);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadDecimal(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDecimal);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadNullableDecimal(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(decimal?));
                var decimalNull = il.DefineLabel();
                var decimalCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, decimalNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDecimal);
                il.Emit(OpCodes.Call, typeof(decimal?).GetConstructor(new Type[] { typeof(decimal) }));
                il.Emit(OpCodes.Br_S, decimalCommon);
                il.MarkLabel(decimalNull);
                il.Emit(OpCodes.Initobj, typeof(decimal?));
                il.MarkLabel(decimalCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadDateTime(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDateTime);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadNullableDateTime(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(DateTime?));
                Label dtNull = il.DefineLabel();
                Label dtCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordIsDbNull);
                il.Emit(OpCodes.Brtrue_S, dtNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecordGetDateTime);
                il.Emit(OpCodes.Call, typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) }));
                il.Emit(OpCodes.Br_S, dtCommon);
                il.MarkLabel(dtNull);
                il.Emit(OpCodes.Initobj, typeof(DateTime?));
                il.MarkLabel(dtCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadObject(ILGenerator il, LocalBuilder item, IList<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var common = il.DefineLabel();
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecordItemGetterInt);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Call, ConvertIsDbNull);
                il.Emit(OpCodes.Brfalse_S, common);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldnull);
                il.MarkLabel(common);
                il.Emit(OpCodes.Unbox_Any, columnInfoes[i].Type);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            #endregion
        }
    }
}