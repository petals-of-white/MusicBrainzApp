namespace MusicBrainz.BLL.DbEntitySerialization.Serialization
{
    public interface ISerializationManager
    {
        T? Deserialize<T>(string serializedObject);
        string Serialize(object objectToSerialize);
    }
}