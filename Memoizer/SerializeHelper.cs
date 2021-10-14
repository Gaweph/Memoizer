using System.Text.Json;
using System.Text.Json.Serialization;

namespace Memoizer
{
    public static class SerializeHelper
    {        
        public static string Serialize<T>(T thing)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return JsonSerializer.Serialize(thing, options);
        }
    }

}
