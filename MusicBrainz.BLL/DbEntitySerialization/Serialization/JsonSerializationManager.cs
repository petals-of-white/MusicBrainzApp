using Newtonsoft.Json;
namespace MusicBrainz.BLL.DbEntitySerialization.Serialization
{
    public class JsonSerializationManager : ISerializationManager
    {
        public T? Deserialize<T>(string serializedObject) =>
            JsonConvert.DeserializeObject<T>(serializedObject);

        public string Serialize(object objectToSerialize) =>
            JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
    }
}