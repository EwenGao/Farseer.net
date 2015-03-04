using System;
using System.Data;
using System.Linq.Expressions;
using FS.Core.Model;
using FS.Extend;
using FS.Utils.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     Oracle数据库持久化
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    internal class OracleVisit<TInfo> : DbVisit<TInfo> where TInfo : ModelInfo, new()
    {
        internal OracleVisit()
        {
            dbProvider = new OracleProvider();
        }

        /// <summary>
        ///     将方法转换成T-SQL特殊函数名
        /// </summary>
        /// <param name="m">自定义特殊的函数</param>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            base.VisitMethodCall(m);
            if (ClearCallSql()) { return m; }

            #region 字段、参数、值类型
            Type fieldType;
            Type paramType;
            string fieldName, paramName;

            if (m.Object == null)
            {
                if (!m.Arguments[0].Type.IsGenericType || m.Arguments[0].Type.GetGenericTypeDefinition() == typeof(Nullable<>)) { fieldType = m.Arguments[0].Type; paramType = m.Arguments[1].Type; fieldName = SqlList.Pop(); paramName = SqlList.Pop(); }
                else { paramType = m.Arguments[0].Type; fieldType = m.Arguments[1].Type; paramName = SqlList.Pop(); fieldName = SqlList.Pop(); }
            }
            else
            {
                if (!m.Object.Type.IsGenericType || m.Object.Type.GetGenericTypeDefinition() == typeof(Nullable<>)) { fieldType = m.Object.Type; paramType = m.Arguments[0].Type; paramName = SqlList.Pop(); fieldName = SqlList.Pop(); }
                else { paramType = m.Object.Type; fieldType = m.Arguments[0].Type; fieldName = SqlList.Pop(); paramName = SqlList.Pop(); }
            }
            #endregion

            switch (m.Method.Name.ToUpper())
            {
                case "CONTAINS":
                    {
                        if (!paramType.IsGenericType || paramType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            #region 搜索值串的处理
                            var param = ParamsList.Find(o => o.ParameterName == paramName);
                            if (param != null && IsType.IsDigital(param.Value.ToString()) && (Type.GetTypeCode(fieldType) == TypeCode.Int16 || Type.GetTypeCode(fieldType) == TypeCode.Int32 || Type.GetTypeCode(fieldType) == TypeCode.Decimal || Type.GetTypeCode(fieldType) == TypeCode.Double || Type.GetTypeCode(fieldType) == TypeCode.Int64 || Type.GetTypeCode(fieldType) == TypeCode.UInt16 || Type.GetTypeCode(fieldType) == TypeCode.UInt32 || Type.GetTypeCode(fieldType) == TypeCode.UInt64))
                            {
                                param.Value = "," + param.Value + ",";
                                param.DbType = DbType.String;
                                if (dbProvider.CreateTableAegis("").Length > 0) { fieldName = "','+" + fieldName.Substring(1, fieldName.Length - 2) + "+','"; }
                                else { fieldName = "','+" + fieldName + "+','"; }
                            }
                            #endregion

                            SqlList.Push(String.Format("INSTR({1},{0}) > 0", paramName, fieldName));
                        }
                        else
                        {
                            if (Type.GetTypeCode(fieldType) == TypeCode.String) { base.ParamsList.GetLast().Value = "'" + base.ParamsList.GetLast().Value.ToString().Replace(",", "','") + "'"; }
                            SqlList.Push(String.Format("{0} IN ({1})", fieldName, base.ParamsList.GetLast().Value));
                            base.ParamsList.Remove(base.ParamsList.GetLast());
                        }

                        break;
                    }
                case "STARTSWITH":
                    {
                        SqlList.Push(String.Format("INSTR({0},{1}) = 1", fieldName, paramName));
                        break;
                    }
                case "ENDSWITH":
                    {
                        SqlList.Push(String.Format("{0} LIKE {1}", fieldName, paramName));
                        ParamsList.GetLast().Value = string.Format("%{0}", ParamsList.GetLast().Value);
                        break;
                    }
                case "ISEQUALS":
                    {
                        SqlList.Push(String.Format("{0} = {1}", fieldName, paramName));
                        break;
                    }
                default:
                    {
                        if (m.Arguments.Count == 0 && m.Object != null) { return m; }
                        else { return VisitConvert(m); throw new Exception(string.Format("暂不支持该方法的SQL转换：" + m.Method.Name.ToUpper())); }
                    }
            }
            return m;
        }
    }
}