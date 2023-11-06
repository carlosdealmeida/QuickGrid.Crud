using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuickGrid.Crud
{
    public class FilterGenericState
    {
        public Dictionary<string, string> StringFilters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, (DateTime? Start, DateTime? End)> DateFilters { get; set; } = new Dictionary<string, (DateTime?, DateTime?)>();
        public Dictionary<string, bool> BoolFilters { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, int?> IntFilters { get; set; } = new Dictionary<string, int?>();
        public Dictionary<string, Guid?> GuidFilters { get; set; } = new Dictionary<string, Guid?>();
        public Dictionary<string, byte?> ByteFilters { get; set; } = new Dictionary<string, byte?>();

        public void InitializeWith<T>()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    StringFilters[prop.Name] = string.Empty;
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    DateFilters[prop.Name] = (null, null);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    BoolFilters[prop.Name] = false;
                }
                else if (prop.PropertyType == typeof(int))
                {
                    IntFilters[prop.Name] = null;
                }
                else if (prop.PropertyType == typeof(Guid))
                {
                    GuidFilters[prop.Name] = null;
                }
                else if (prop.PropertyType == typeof(byte))
                {
                    ByteFilters[prop.Name] = null;
                }
            }
        }

        public string GetStringFilter(string key)
        {
            if (StringFilters.TryGetValue(key, out var value))
            {
                return value;
            }
            return string.Empty;
        }

        public void SetStringFilter(string key, string value)
        {
            StringFilters[key] = value;
        }

        public (DateTime? Start, DateTime? End) GetDateFilter(string key)
        {
            if (DateFilters.TryGetValue(key, out var value))
            {
                return value;
            }
            return (null, null);
        }

        public void SetDateFilterStart(string key, DateTime? start)
        {
            if (DateFilters.ContainsKey(key))
            {
                var current = DateFilters[key];
                DateFilters[key] = (start, current.End);
            }
            else
            {
                DateFilters[key] = (start, null);
            }
        }

        public void SetDateFilterEnd(string key, DateTime? end)
        {
            if (DateFilters.ContainsKey(key))
            {
                var current = DateFilters[key];
                DateFilters[key] = (current.Start, end);
            }
            else
            {
                DateFilters[key] = (null, end);
            }
        }

        public void SetDateIfValid(ChangeEventArgs e, Action<DateTime> setDate)
        {
            if (DateTime.TryParse(e.Value.ToString(), out var date))
            {
                setDate(date);
            }
        }

        public bool GetBoolFilter(string key)
        {
            if (BoolFilters.TryGetValue(key, out var value))
            {
                return value;
            }
            return false; // Default value for bool
        }

        public void SetBoolFilter(string key, bool value)
        {
            BoolFilters[key] = value;
        }

        public int? GetIntFilter(string key)
        {
            if (IntFilters.TryGetValue(key, out var value))
            {
                return value ?? 0;
            }
            return 0;
        }

        public void SetIntFilter(string key, int? value)
        {
            IntFilters[key] = value;
        }
        public Guid? GetGuidFilter(string key)
        {
            if (GuidFilters.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        public void SetGuidFilter(string key, Guid? value)
        {
            GuidFilters[key] = value;
        }

        public byte? GetByteFilter(string key)
        {
            if (ByteFilters.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        public void SetByteFilter(string key, byte? value)
        {
            ByteFilters[key] = value;
        }

        public Expression<Func<TEntity, bool>> GenerateFilterExpression<TEntity>()
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression finalExpression = null;

            foreach (var filter in StringFilters)
            {
                if (!string.IsNullOrEmpty(filter.Value))
                {
                    var property = Expression.Property(parameter, filter.Key);
                    var targetValue = Expression.Constant(filter.Value);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var stringComparisonValue = Expression.Constant(StringComparison.CurrentCultureIgnoreCase);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                    var propertyToLower = Expression.Call(property, toLowerMethod);
                    var targetValueToLower = Expression.Call(targetValue, toLowerMethod);

                    var containsExpression = Expression.Call(propertyToLower, containsMethod, targetValueToLower);

                    finalExpression = finalExpression == null ? containsExpression : Expression.AndAlso(finalExpression, containsExpression);
                }
            }

            foreach (var filter in DateFilters)
            {
                var property = Expression.Property(parameter, filter.Key);

                if (filter.Value.Start.HasValue)
                {
                    var startValue = Expression.Constant(filter.Value.Start.Value.Date);
                    var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, startValue);

                    finalExpression = finalExpression == null ? (Expression)greaterThanOrEqual : Expression.AndAlso(finalExpression, greaterThanOrEqual);
                }

                if (filter.Value.End.HasValue)
                {
                    var endOfDay = filter.Value.End.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var endValue = Expression.Constant(endOfDay);
                    var lessThanOrEqual = Expression.LessThanOrEqual(property, endValue);

                    finalExpression = finalExpression == null ? (Expression)lessThanOrEqual : Expression.AndAlso(finalExpression, lessThanOrEqual);
                }
            }

            foreach (var filter in BoolFilters)
            {
                var property = Expression.Property(parameter, filter.Key);
                var targetValue = Expression.Constant(filter.Value);

                var equalsExpression = Expression.Equal(property, targetValue);

                finalExpression = finalExpression == null
                    ? (Expression)equalsExpression
                    : Expression.AndAlso(finalExpression, equalsExpression);
            }

            foreach (var filter in IntFilters)
            {
                if (filter.Value.HasValue)
                {
                    var property = Expression.Property(parameter, filter.Key);
                    var targetValue = Expression.Constant(filter.Value.Value);

                    var equalsExpression = Expression.Equal(property, targetValue);

                    finalExpression = finalExpression == null
                        ? (Expression)equalsExpression
                        : Expression.AndAlso(finalExpression, equalsExpression);
                }
            }

            foreach (var filter in GuidFilters)
            {
                if (filter.Value.HasValue)
                {
                    var property = Expression.Property(parameter, filter.Key);
                    var targetValue = Expression.Constant(filter.Value.Value);

                    var equalsExpression = Expression.Equal(property, targetValue);

                    finalExpression = finalExpression == null
                        ? (Expression)equalsExpression
                        : Expression.AndAlso(finalExpression, equalsExpression);
                }
            }

            foreach (var filter in ByteFilters)
            {
                if (filter.Value.HasValue)
                {
                    var property = Expression.Property(parameter, filter.Key);
                    var targetValue = Expression.Constant(filter.Value.Value);

                    var equalsExpression = Expression.Equal(property, targetValue);

                    finalExpression = finalExpression == null
                        ? (Expression)equalsExpression
                        : Expression.AndAlso(finalExpression, equalsExpression);
                }
            }


            if (finalExpression == null)
            {
                return entity => true;
            }

            return Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);
        }
    }
}
