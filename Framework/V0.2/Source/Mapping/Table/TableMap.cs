using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using FS.Configs;
using FS.Core.Bean;
using FS.Core.Data;
using FS.Extend;

namespace FS.Mapping.Table
{
    /// <summary>
    ///     ORM 映射关系
    /// </summary>
    public class TableMap
    {
        /// <summary>
        ///     获取所有属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, FieldMapState> ModelList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public TableMap(Type type)
        {
            ModelList = new Dictionary<PropertyInfo, FieldMapState>();
            object[] attrs;

            //变量属性
            FieldMapState fieldMapState;

            #region 类属性

            //类属性
            ClassInfo = new DBAttribute();

            attrs = type.GetCustomAttributes(typeof(DBAttribute), false);
            ClassInfo = attrs.Length == 0 ? new DBAttribute() : ((DBAttribute)attrs[0]);

            if (ClassInfo.Name.IsNullOrEmpty())
            {
                ClassInfo.Name = type.Name;
            }

            #region 自动创建数据库配置文件

            if (DbConfigs.ConfigInfo.DbList.Count == 0 && ClassInfo.DbIndex == 0)
            {
                var db = new DbConfig();
                db.DbList.Add(new DbInfo
                                  {
                                      Catalog = "数据库名称",
                                      CommandTimeout = 60,
                                      ConnectTimeout = 30,
                                      DataType = DataBaseType.SqlServer,
                                      DataVer = "2005",
                                      PassWord = "123456",
                                      PoolMaxSize = 100,
                                      PoolMinSize = 16,
                                      Server = ".",
                                      UserID = "sa"
                                  });
                DbConfigs.SaveConfig(db);
            }
            else if (DbConfigs.ConfigInfo.DbList.Count - 1 < ClassInfo.DbIndex)
            {
                throw new Exception("数据库配置(索引项：" + ClassInfo.DbIndex + ")不存在！");
            }

            #endregion

            #region 获取DbConfig的配置

            DbInfo dbInfo = ClassInfo.DbIndex;
            ClassInfo.ConnStr = DbFactory.CreateConnString(ClassInfo.DbIndex);
            ClassInfo.DataType = dbInfo.DataType;
            ClassInfo.DataVer = dbInfo.DataVer;
            ClassInfo.CommandTimeout = dbInfo.CommandTimeout;

            #endregion

            #endregion

            #region 变量属性

            //遍历所有属性变量,取得对应使用标记名称
            //无加标记时，则为不使用该变量。
            foreach (var propertyInfo in type.GetProperties())
            {
                fieldMapState = new FieldMapState();

                // 是否带属性
                attrs = propertyInfo.GetCustomAttributes(false);
                foreach (var item in attrs)
                {
                    // 加入属性
                    fieldMapState.IsDbField = !(item is NotJoinAttribute);
                    // 数据类型
                    if (item is DataTypeAttribute) { fieldMapState.DataType = (DataTypeAttribute)item; continue; }
                    // 字段映射
                    if (item is ColumnAttribute) { fieldMapState.Column = (ColumnAttribute)item; continue; }
                    // 属性扩展
                    if (item is PropertyExtendAttribute) { fieldMapState.PropertyExtend = ((PropertyExtendAttribute)item).PropertyExtend; continue; }
                }
                //if (fieldMapState.Display == null) { fieldMapState.Display = new DisplayAttribute { Name = propertyInfo.Name }; }
                //if (fieldMapState.Display.Name.IsNullOrEmpty()) { fieldMapState.Display.Name = propertyInfo.Name; }

                if (fieldMapState.Column == null) { fieldMapState.Column = new ColumnAttribute { Name = propertyInfo.Name }; }
                if (fieldMapState.Column.Name.IsNullOrEmpty()) { fieldMapState.Column.Name = propertyInfo.Name; }

                if (fieldMapState.IsDbField && fieldMapState.Column.IsDbGenerated) { IndexName = fieldMapState.Column.Name; } else { fieldMapState.Column.IsDbGenerated = false; }

                //添加属变量标记名称
                ModelList.Add(propertyInfo, fieldMapState);
            }
            #endregion

            Type = type;
        }

        /// <summary>
        ///     类属性
        /// </summary>
        public DBAttribute ClassInfo { get; set; }

        /// <summary>
        ///     自增ID
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator TableMap(Type type)
        {
            return TableMapCache.GetMap(type);
        }

        /// <summary>
        ///     获取当前属性（通过使用的fieldName）
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        public KeyValuePair<PropertyInfo, FieldMapState> GetModelInfo(string fieldName = "")
        {
            return fieldName.IsNullOrEmpty() ? ModelList.FirstOrDefault(oo => oo.Value.Column.IsDbGenerated) : ModelList.FirstOrDefault(oo => oo.Key.Name == fieldName);
        }

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="propertyInfo">属性变量</param>
        /// <returns></returns>
        public string GetFieldName(PropertyInfo propertyInfo)
        {
            return ModelList[propertyInfo].Column.Name;
        }
    }
}