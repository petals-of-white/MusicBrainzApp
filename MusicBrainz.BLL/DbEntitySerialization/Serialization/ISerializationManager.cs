namespace MusicBrainz.BLL.DbEntitySerialization.Serialization
{
    public interface ISerializationManager
    {
        string Format { get; }

        T? Deserialize<T>(string serializedObject);

        string Serialize(object objectToSerialize);
    }
}
