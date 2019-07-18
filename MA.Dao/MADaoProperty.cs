using System;
using System.Reflection;
using System.Linq;
using System.Data;
using MA.Dao.Attributes;

namespace MA.Dao
{
    public class MADaoProperty
    {
        #region Constructs
        internal MADaoProperty(PropertyInfo property)
        {
            this.Property = property;
            this.Attributes = this.Property.GetCustomAttributes().ToArray();
        }
        #endregion

        #region Variables
        public Attribute[] Attributes { get; private set; }
        public PropertyInfo Property { get; private set; }

        bool? isNullable = null;
        public bool IsNullable
        {
            get
            {
                if (isNullable == null)
                    isNullable = (Property.PropertyType.IsGenericType &&
                                Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) || Property.PropertyType.IsClass;
                return isNullable.Value;
            }
        }

        string columnName = null;
        public string ColumnName
        {
            get
            {
                if (columnName == null)
                    columnName = AttributeColumn.Name == null ? Property.Name : AttributeColumn.Name;
                return columnName;
            }
        }

        MADataColumnAttribute attributeColumn = null;
        public MADataColumnAttribute AttributeColumn
        {
            get
            {
                if (attributeColumn == null)
                    attributeColumn = (MADataColumnAttribute)Attributes.FirstOrDefault(attr => attr is MADataColumnAttribute);
                return attributeColumn;
            }
        }

        MADataKeyAttribute attributeKey = null;
        public MADataKeyAttribute AttributeKey
        {
            get
            {
                if (attributeKey == null)
                    attributeKey = (MADataKeyAttribute)Attributes.FirstOrDefault(attr => attr is MADataKeyAttribute);
                return attributeKey;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
