using System.Reflection;
using System.Security.AccessControl;
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

            MapReferenceToResult(resultObject, referenceObject, a => a.Create);

            return resultObject;
        }
        public static IEnumerable<TResultType> MapToCreate<TResultType>(IEnumerable<TReferenceObject> references)
        {
            if (references is null || !references.Any()) return [];

            TResultType[] resultObjectArray = new TResultType[references.Count()];
            resultObjectArray = resultObjectArray.Select(d => GetInstanceOf<TResultType>()).Where(e => e is not null).ToArray()!;

            for(int i = 0; i < resultObjectArray.Length; i++)
                MapReferenceToResult(resultObjectArray[i], references.ElementAt(i), a => a.Create);

            return resultObjectArray;
        }
        public static TResultType? MapToUpdate<TResultType>(TReferenceObject referenceObject)
        {
            var resultObject = GetInstanceOf<TResultType>();

            if (referenceObject is null || resultObject is null) return default;

            MapReferenceToResult(resultObject, referenceObject, a => a.Update);

            return resultObject;
        }
        public static TResultType? MapToRead<TResultType>(TReferenceObject referenceObject)
        {
            var resultObject = GetInstanceOf<TResultType>();

            if (referenceObject is null || resultObject is null) return default;

            MapReferenceToResult(resultObject, referenceObject, a => a.Read);

            return resultObject;
        }
        public static IEnumerable<TResultType> MapToRead<TResultType>(IEnumerable<TReferenceObject> references)
        {
            if (references is null || !references.Any()) return [];

            TResultType[] resultObjectArray = new TResultType[references.Count()];
            resultObjectArray = resultObjectArray.Select(d => GetInstanceOf<TResultType>()).Where(e => e is not null).ToArray()!;

            for (int i = 0; i < resultObjectArray.Length; i++)
                MapReferenceToResult(resultObjectArray[i], references.ElementAt(i), a => a.Read);

            return resultObjectArray;
        }

        private static void MapReferenceToResult<TResultType>(TResultType resultObject, TReferenceObject referenceObject, Func<CrudDefinition, bool> crudSelector)
        {
            IEnumerable<PropertyInfo> enabledProps = GetPropertiesByCRUDCondition(typeof(TReferenceObject), crudSelector);

            if (!enabledProps.Any())
                enabledProps = GetPropertiesByCRUDCondition(typeof(TResultType), crudSelector);

            IEnumerable<PropertyInfo> referencePropsEnabled = referenceObject!.GetType().GetProperties().Where(p => enabledProps.Any(ep => ep.Name == p.Name && ep.PropertyType == p.PropertyType));

            foreach (var resultObjectProp in resultObject!.GetType().GetProperties())
            {
                PropertyInfo? referenceProps = referencePropsEnabled.FirstOrDefault(p => p.Name == resultObjectProp.Name && p.PropertyType == resultObjectProp.PropertyType);
                if (referenceProps != null)
                    resultObjectProp.SetValue(resultObject, referenceProps.GetValue(referenceObject));
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