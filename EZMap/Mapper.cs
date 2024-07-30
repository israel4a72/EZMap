using System.Reflection;
using EZMap.Attributes;
using EZMap.Exceptions;

namespace EZMap
{
    public class Mapper<TReferenceObject>
    {
        public static TResultType? MapToCreate<TResultType>(TReferenceObject referenceObject)
        {
            var resultObject = GetInstanceOf<TResultType>();

            if (referenceObject is null || resultObject is null) return default;

            SetPropsValues(resultObject, referenceObject, a => a.Create);

            return resultObject;
        }
        public static IEnumerable<TResultType> MapToCreate<TResultType>(IEnumerable<TReferenceObject> entities)
        {
            if (entities is null || !entities.Any()) return [];

            TResultType[] resultObjectArray = new TResultType[entities.Count()];
            resultObjectArray = resultObjectArray.Select(d => GetInstanceOf<TResultType>()).Where(e => e is not null).ToArray()!;

            for(int i = 0; i < resultObjectArray.Length; i++)
                SetPropsValues(resultObjectArray[i], entities.ElementAt(i), a => a.Create);

            return resultObjectArray;
        }
        public static TResultType? MapToUpdate<TResultType>(TReferenceObject referenceObject)
        {
            var resultObject = GetInstanceOf<TResultType>();

            if (referenceObject is null || resultObject is null) return default;

            SetPropsValues(resultObject, referenceObject, a => a.Update);

            return resultObject;
        }

        private static void SetPropsValues<TResultType>(TResultType resultObject, TReferenceObject referenceObject, Func<CrudDefinition, bool> crudSelector)
        {
            IEnumerable<PropertyInfo> referenceObjectProps = GetPropertiesByCRUDCondition(typeof(TReferenceObject), crudSelector);

            foreach (var prop in resultObject!.GetType().GetProperties())
            {
                PropertyInfo? propToMap = referenceObjectProps.FirstOrDefault(p => p.Name == prop.Name && p.PropertyType == prop.PropertyType);
                if (propToMap != null)
                    prop.SetValue(resultObject, propToMap.GetValue(referenceObject));
            }
        }
        private static IEnumerable<PropertyInfo> GetPropertiesByCRUDCondition(Type referenceObjectType, Func<CrudDefinition, bool> crudSelector)
        {
            bool condition(PropertyInfo p) => p.GetCustomAttributes<CrudDefinition>(true).FirstOrDefault(crudSelector) != null;
            return referenceObjectType.GetProperties().Where(condition);
        }
        private static T? GetInstanceOf<T>()
        {
            ConstructorInfo[] constructors = typeof(T).GetConstructors();

            ParameterInfo[]? lowerParamsNumber = constructors.Select(c => c.GetParameters()).OrderBy(p => p.Length).FirstOrDefault();

            if (lowerParamsNumber is null) throw new InvokeObjectException($"Can't get a constructor with lower number of parameters.");

            Dictionary<Type, object?> ctorParams = lowerParamsNumber.Select(p => 
                new KeyValuePair<Type, object?> (p.ParameterType, p.HasDefaultValue ? p.DefaultValue : null)).ToDictionary();

            if (ctorParams.Count != lowerParamsNumber.Length) throw new InvokeObjectException($"Object of type {typeof(T)} has no constructors with {constructors.Length} params.");

            return (T?)(typeof(T).GetConstructor([.. ctorParams.Keys])?.Invoke([.. ctorParams.Values]));
        }
    }
}
