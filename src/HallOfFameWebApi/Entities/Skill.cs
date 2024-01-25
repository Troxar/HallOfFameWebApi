using System.Text.Json.Serialization;

namespace HallOfFameWebApi.Entities
{
    public class Skill
    {
        public string Name { get; set; }
        public byte Level { get; set; }

        [JsonIgnore]
        public long PersonId { get; set; }

        [JsonIgnore]
        public Person Person { get; set; }
    }
}
