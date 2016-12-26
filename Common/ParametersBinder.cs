using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Ado.Common
{
    public class ParametersBinder
    {
        public ParametersBinder(string parameterIdentifier, object[] objectParameters)
        {
            ParameterIdentifier = parameterIdentifier;
            ObjectParameters = objectParameters;
        }

        public string ParameterIdentifier { get; private set; }

        public object[] ObjectParameters { get; private set; }

        public void Bind(IDbCommand command)
        {
            if (ObjectParameters == null || ObjectParameters.Length == 0) return;

            var values = CreateValuesList(ObjectParameters);

            foreach (var item in values)
            {
                var parameterName = string.Concat(ParameterIdentifier, item.Key);
                if (!command.CommandText.Contains(parameterName)) continue;
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = item.Value ?? DBNull.Value;
                if (item.Value != null)
                {
                    parameter.DbType = GetDataType(item.Value);
                }

                command.Parameters.Add(parameter);
            }
        }

        private DbType GetDataType(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Decimal:
                    return DbType.Decimal;

                case TypeCode.Boolean:
                    return DbType.Boolean;

                case TypeCode.Byte:
                    return DbType.Byte;

                case TypeCode.DateTime:
                    return DbType.DateTime;

                case TypeCode.Int32:
                    return DbType.Int32;

                case TypeCode.Int64:
                    return DbType.Int64;

                case TypeCode.String:
                    return DbType.String;

                default:

                    return DbType.String;
            }
        }

        public IEnumerable<KeyValuePair<string, object>> CreateValuesList(object[] target)
        {
            if (target.Length == 1 && IsComplexType(target.First()))
            {
                var firstItem = target.First();
                var dictionary = firstItem as Dictionary<string, object>;
                if (dictionary != null) return dictionary.Select(i => i).ToList();

                var properties = firstItem.GetType().GetProperties();
                var notIgnoredProperties = properties.Where(p => !HasIgnoreAttribute(p));
                return notIgnoredProperties.Select(p => CreateKeyValuePair(firstItem, p)).ToList();
            }
            return target.Select((value, index) => CreateKeyValuePair(value, index));
        }

        private bool IsComplexType(object target)
        {
            return !(target.GetType().GetTypeInfo().IsValueType || target is string);
        }

        private KeyValuePair<string, object> CreateKeyValuePair(object target, PropertyInfo property)
        {
            object value;
            if (property.PropertyType.GetTypeInfo().IsEnum) value = (int)property.GetValue(target, null);
            else value = property.GetValue(target, null);
            return new KeyValuePair<string, object>(property.Name, value);
        }

        private KeyValuePair<string, object> CreateKeyValuePair(object value, int index)
        {
            return new KeyValuePair<string, object>(Convert.ToString(index), value);
        }

        private bool HasIgnoreAttribute(PropertyInfo propertyInfo)
        {
            return false;
        }
    }
}
