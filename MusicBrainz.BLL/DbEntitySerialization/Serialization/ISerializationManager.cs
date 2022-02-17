namespace MusicBrainz.BLL.DbEntitySerialization.Serialization
{
    internal interface ISerializationManager
    {
        T? Deserialize<T>(string serializedObject);
        string Serialize(object objectToSerialize);
    }
}