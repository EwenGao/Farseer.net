using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using FS.Core.Model;
using FS.Extend;
using FS.Mapping.Table;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库表达式树解析器
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    public abstract class DbVisit<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     实体类映射
        /// </summary>
        internal TableMap Map = typeof(TInfo);

        /// <summary>
        ///     参数列表
        /// </summary>
        protected List<DbParameter> ParamsList;

        /// <summary>
        ///     条件堆栈
        /// </summary>
        protected Stack<string> SqlList;

        /// <summary>
        ///     数据库提供者
        /// </summary>
        protected DbProvider dbProvider;

        /// <summary>
        ///     参数个数（标识）
        /// </summary>
        protected int m_ParamsCount;

        protected eumVisitType VisitType;

        /// <summary>
        ///     解释T-SQL(Where)
        /// </summary>
        /// <param name="exp">Lambda表达式</param>
        /// <param name="lstParams">输出参数列表</param>
        public virtual string WhereTranslator(Expression exp, out List<DbParameter> lstParams)
        {
            VisitType = eumVisitType.Where;
            SqlList = new Stack<string>();
            ParamsList = new List<DbParameter>();

            if (exp != null) { Visit(exp); }

            lstParams = ParamsList;

            var lstSql = SqlList.ToList();
            lstSql.Reverse();
            return lstSql.Count == 0 ? "1=1" : lstSql.ToString(" ");
        }

        /// <summary>
        ///     解释T-SQL(Select)
        /// </summary>
        /// <param name="exp">Lambda表达式</param>
        public virtual List<string> SelectTranslator(Expression exp)
        {
            VisitType = eumVisitType.Select;
            SqlList = new Stack<string>();

            if (exp != null) { Visit(exp); }

            var lstSql = SqlList.ToList();
            lstSql.Reverse();
            return lstSql;
        }

        /// <summary>
        ///     解释T-SQL(Order)
        /// </summary>
        /// <param name="exp">Lambda表达式</param>
        public virtual List<string> OrderTranslator(Expression exp)
        {
            VisitType = eumVisitType.Order;
            SqlList = new Stack<string>();

            if (exp != null) { Visit(exp); }

            var lstSql = SqlList.ToList();
            lstSql.Reverse();
            return lstSql;
        }

        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null) { return null; }
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess: exp = VisitConvert(exp); break;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return VisitBinary((BinaryExpression)exp);
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                    return Visit(((UnaryExpression)exp).Operand);
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)exp);

                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp);

                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp);

                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp);

                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp);

                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp);

                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp);

                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp);

                case ExpressionType.New:
                    return VisitNew((NewExpression)exp);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp);

                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp);
            }
            throw new Exception(string.Format("类型：(ExpressionType){0}，不存在。", exp.NodeType));
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected virtual Expression VisitBinary(BinaryExpression bexp)
        {
            if (bexp == null)
            {
                return null;
            }

            #region 操作符号

            string opr;
            switch (bexp.NodeType)
            {
                case ExpressionType.Equal:
                    opr = "=";
                    break;
                case ExpressionType.NotEqual:
                    opr = "<>";
                    break;
                case ExpressionType.GreaterThan:
                    opr = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    opr = ">=";
                    break;
                case ExpressionType.LessThan:
                    opr = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    opr = "<=";
                    break;
                case ExpressionType.AndAlso:
                    opr = "AND";
                    break;
                case ExpressionType.OrElse:
                    opr = "OR";
                    break;
                case ExpressionType.Add:
                    opr = "+";
                    break;
                case ExpressionType.Subtract:
                    opr = "-";
                    break;
                case ExpressionType.Multiply:
                    opr = "*";
                    break;
                case ExpressionType.Divide:
                    opr = "/";
                    break;
                case ExpressionType.And:
                    opr = "&";
                    break;
                case ExpressionType.Or:
                    opr = "|";
                    break;
                default:
                    throw new NotSupportedException(bexp.NodeType + "的类型，未定义操作符号！");
            }

            #endregion

            Visit(bexp.Left);
            Visit(bexp.Right);

            var right = SqlNot(SqlList.Pop());
            var left = SqlNot(SqlList.Pop());

            if (opr == "AND" || opr == "OR")
            {
                right = SqlTrue(right);
                left = SqlTrue(left);
            }

            SqlList.Push(String.Format("({0} {1} {2})", left, opr, right));

            return bexp;
        }

        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected virtual Expression VisitConstant(ConstantExpression cexp)
        {
            if (cexp == null) return null;
            m_ParamsCount++;
            var paramName = String.Format("{0}Parms_{1}", dbProvider.ParamsPrefix, m_ParamsCount.ToString());
            SqlList.Push(paramName);
            ParamsList.Add(dbProvider.CreateDbParam(paramName.Substring(dbProvider.ParamsPrefix.Length), cexp.Value));

            return cexp;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return null;

            switch (m.NodeType)
            {
                //局部变量
                case ExpressionType.Constant: return Visit(VisitConvert(m));
                default:
                    {
                        var keyValue = Map.GetModelInfo(m.Member.Name);
                        if (keyValue.Key == null)
                        {
                            switch (m.Member.Name)
                            {
                                case "Length":
                                    {
                                        var exp = VisitMemberAccess((MemberExpression)m.Expression);
                                        SqlList.Push(string.Format("LEN({0})", SqlList.Pop()));
                                        return exp;
                                    }
                            }
                            return VisitMemberAccess((MemberExpression)m.Expression);
                        }

                        // 加入Sql队列

                        string filedName;
                        if (VisitType == eumVisitType.Select && !dbProvider.IsField(keyValue.Value.Column.Name))
                        {
                            filedName = keyValue.Value.Column.Name + " as " + keyValue.Key.Name;
                        }
                        else
                        {
                            filedName = dbProvider.CreateTableAegis(keyValue.Value.Column.Name);
                        }
                        SqlList.Push(filedName);
                        return m;
                    }
            }
        }

        /// <summary>
        ///     数组值
        /// </summary>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            foreach (var ex in na.Expressions)
            {
                Visit(ex);
            }
            return null;
        }

        /// <summary>
        ///     值类型的转换
        /// </summary>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Not)
            {
                SqlList.Push("Not");
            }
            return Visit((u).Operand);
        }

        /// <summary>
        ///     将变量转换成值
        /// </summary>
        protected virtual Expression VisitConvert(Expression exp)
        {
            if (exp is BinaryExpression) { return exp; }
            //if (!IsCanCompile(exp)) { return exp; }

            var lambda = Expression.Lambda(exp);
            return !IsCanCompile(lambda) ? exp : Expression.Constant(lambda.Compile().DynamicInvoke(null), exp.Type);
        }

        /// <summary>
        ///     解析方法
        /// </summary>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Object == null)
            {
                for (var i = m.Arguments.Count - 1; i > 0; i--)
                {
                    var exp = m.Arguments[i];
                    //while (exp != null && exp.NodeType == ExpressionType.Call)
                    //{
                    //    exp = ((MethodCallExpression)exp).Object;
                    //}
                    Visit(exp);
                }
                Visit(m.Arguments[0]);
            }
            else
            {
                Visit(m.Object);
                for (var i = 0; i < m.Arguments.Count; i++)
                {
                    var exp = m.Arguments[i];
                    //while (exp != null && exp.NodeType == ExpressionType.Call)
                    //{
                    //    exp = ((MethodCallExpression)exp).Object;
                    //}
                    Visit(exp);
                }
            }
            return m;
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);

                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);

                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);
            }
            throw new Exception(string.Format("类型：(MemberBindingType){0}，不存在。", binding.BindingType));
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            var num = 0;
            var count = original.Count;
            while (num < count)
            {
                var item = VisitBinding(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<MemberBinding>(count);
                    for (var i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test = Visit(c.Test);
            var ifTrue = Visit(c.IfTrue);
            var ifFalse = Visit(c.IfFalse);
            if (((test == c.Test) && (ifTrue == c.IfTrue)) && (ifFalse == c.IfFalse))
            {
                return c;
            }
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.Arguments);
            return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            var num = 0;
            var count = original.Count;
            while (num < count)
            {
                var item = VisitElementInitializer(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<ElementInit>(count);
                    for (var i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> sequence = null;
            var num = 0;
            var count = original.Count;
            while (num < count)
            {
                var item = Visit(original[num]);
                if (sequence != null)
                {
                    sequence.Add(item);
                }
                else if (item != original[num])
                {
                    sequence = new List<Expression>(count);
                    for (var i = 0; i < num; i++)
                    {
                        sequence.Add(original[i]);
                    }
                    sequence.Add(item);
                }
                num++;
            }
            if (sequence != null)
            {
                return (ReadOnlyCollection<Expression>)(IEnumerable)sequence;
            }
            return original;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> arguments = VisitExpressionList(iv.Arguments);
            var expression = Visit(iv.Expression);
            if ((arguments == iv.Arguments) && (expression == iv.Expression))
            {
                return iv;
            }
            return Expression.Invoke(expression, arguments);
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            var body = Visit(lambda.Body);
            if (body != lambda.Body)
            {
                try
                {
                    return Expression.Lambda(lambda.Type, Expression.Convert(body, typeof(object)), lambda.Parameters);
                }
                catch
                {
                    return lambda.Body;
                }
            }
            return lambda;
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            var newExpression = VisitNew(init.NewExpression);
            var initializers = VisitElementInitializerList(init.Initializers);
            if ((newExpression == init.NewExpression) && (initializers == init.Initializers))
            {
                return init;
            }
            return Expression.ListInit(newExpression, initializers);
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var expression = Visit(assignment.Expression);
            return expression != assignment.Expression ? Expression.Bind(assignment.Member, expression) : assignment;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            var newExpression = VisitNew(init.NewExpression);
            var bindings = VisitBindingList(init.Bindings);
            if ((newExpression == init.NewExpression) && (bindings == init.Bindings))
            {
                return init;
            }
            return Expression.MemberInit(newExpression, bindings);
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.Initializers);
            return initializers != binding.Initializers ? Expression.ListBind(binding.Member, initializers) : binding;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.Bindings);
            return bindings != binding.Bindings ? Expression.MemberBind(binding.Member, bindings) : binding;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> arguments = VisitExpressionList(nex.Arguments);
            if (arguments == nex.Arguments)
            {
                return nex;
            }
            return nex.Members != null ? Expression.New(nex.Constructor, arguments, nex.Members) : Expression.New(nex.Constructor, arguments);
        }

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expression = Visit(b.Expression);
            return expression != b.Expression ? Expression.TypeIs(expression, b.TypeOperand) : b;
        }

        /// <summary>
        ///     清除值为空的条件，并给与1!=1的SQL
        /// </summary>
        protected virtual bool ClearCallSql()
        {
            if (ParamsList != null && ParamsList.Count > 0 && ParamsList.GetLast().Value.ToString().IsNullOrEmpty())
            {
                ParamsList.RemoveAt(ParamsList.Count - 1);
                SqlList.Pop();
                SqlList.Pop();
                SqlList.Push("1<>1");
                return true;
            }
            return false;
        }

        /// <summary>
        ///     当存在Not 时，特殊处理
        /// </summary>
        protected virtual string SqlNot(string sql)
        {
            var lst = new List<string> { sql };
            // 当存在Not 时，特殊处理
            while (SqlList.Count > 0 && SqlList.First().IsEquals("Not"))
            {
                lst.Add(SqlList.Pop());
            }
            lst.Reverse();
            return lst.ToString(" ");
        }

        /// <summary>
        ///     当存在true 时，特殊处理
        /// </summary>
        protected virtual string SqlTrue(string sql)
        {
            var dbParam = ParamsList.FirstOrDefault(o => o.ParameterName == sql);
            if (dbParam != null)
            {
                var result = dbParam.Value.ToString().IsEquals("true");
                ParamsList.RemoveAll(o => o.ParameterName == sql);
                return result ? "1=1" : "1<>1";
            }
            return sql;
        }

        /// <summary>
        ///     是否允许执行转换
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsCanCompile(Expression exp)
        {
            if (exp == null)
            {
                return false;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return IsCanCompile(((LambdaExpression)exp).Body);
                case ExpressionType.Call:
                    {
                        var callExp = (MethodCallExpression)exp;
                        if (callExp.Object != null && !IsCanCompile(callExp.Object)) { return false; }
                        foreach (var item in callExp.Arguments) { if (!IsCanCompile(item)) { return false; } }
                        return true;
                    }
                case ExpressionType.MemberAccess:
                    {
                        var memExp = (MemberExpression)exp;
                        return memExp.Expression == null || IsCanCompile(memExp.Expression);
                        //if (memExp.Expression.NodeType == ExpressionType.Constant) { return true; }
                        //if (memExp.Expression.NodeType == ExpressionType.MemberAccess) { return IsCanCompile(memExp.Expression); }
                        //break;
                    }
                case ExpressionType.Parameter: return !exp.Type.IsClass;
                case ExpressionType.Convert: return IsCanCompile(((UnaryExpression)exp).Operand);
                case ExpressionType.ArrayIndex:
                case ExpressionType.ListInit:
                case ExpressionType.Constant: { return true; }
            }
            return false;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct Buffer<TElement>
        {
            internal TElement[] items;
            internal int count;

            internal Buffer(IEnumerable<TElement> source)
            {
                TElement[] array = null;
                var length = 0;
                var is2 = source as ICollection<TElement>;
                if (is2 != null)
                {
                    length = is2.Count;
                    if (length > 0)
                    {
                        array = new TElement[length];
                        is2.CopyTo(array, 0);
                    }
                }
                else
                {
                    foreach (var local in source)
                    {
                        if (array == null)
                        {
                            array = new TElement[4];
                        }
                        else if (array.Length == length)
                        {
                            var destinationArray = new TElement[length * 2];
                            Array.Copy(array, 0, destinationArray, 0, length);
                            array = destinationArray;
                        }
                        array[length] = local;
                        length++;
                    }
                }
                items = array;
                count = length;
            }

            internal TElement[] ToArray()
            {
                if (count == 0)
                {
                    return new TElement[0];
                }
                if (items.Length == count)
                {
                    return items;
                }
                var destinationArray = new TElement[count];
                Array.Copy(items, 0, destinationArray, 0, count);
                return destinationArray;
            }
        }
    }

    /// <summary>
    /// 访问时的类型
    /// </summary>
    public enum eumVisitType
    {
        Select,
        Order,
        Where
    }
}