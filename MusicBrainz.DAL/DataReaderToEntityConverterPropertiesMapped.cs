using System.Data;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;

namespace MusicBrainz.DAL
{
    /// <summary>
    /// Contains logic that helps with converting SqlReader to corresponding entity type.
    /// </summary>
    internal static class DataReaderToEntityConverterPropertiesMapped
    {
        public delegate object? ForeignKeySubstitutioner(int id, Tables table);

        private static T? ConvertValue<T>(object raw)
            =>
            (raw != DBNull.Value) ? (T) raw : default(T);

        /// <summary>
        /// Return IEnumerable<T>; foreign keys will be provided by foreignKeySubstitutioner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="foreignKeySubstitutioner"></param>
        /// <returns></returns>
        public static IEnumerable<T> Select<T>(this IDataReader reader, ForeignKeySubstitutioner foreignKeySubstitutioner) where T : TableEntity
        {
            Type entityType = typeof(T);

            while (reader.Read())
            {
                T? entity = Activator.CreateInstance<T>();

                foreach (var property in entityType.GetProperties())
                {
                    // check if the property type is one of custom table entity types
                    // (classes inherited from TableEntity)
                    bool isForeignKey = typeof(TableEntity).IsAssignableFrom(property.PropertyType);

                    object? rawValue = (reader [property.Name] is DBNull) ? null : reader [property.Name];

                    //object? foreignRecord = isForeignKey ? foreignKeySubstitutioner((int) rawValue, (Tables) Enum.Parse(typeof(Tables), property.Name));

                    //
                    object? realValue =
                        (rawValue is null)
                        ? null
                        : (isForeignKey ? foreignKeySubstitutioner((int) rawValue, (Tables) Enum.Parse(typeof(Tables), property.Name)) : rawValue);

                    property.SetValue(entity, realValue);
                }
                yield return entity;
            }
        }
    }
}