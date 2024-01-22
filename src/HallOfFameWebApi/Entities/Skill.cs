namespace HallOfFameWebApi.Entities
{
    public class Skill
    {
        public string Name { get; set; }
        public byte Level { get; set; }

        public long PersonId { get; set; }
        public Person Person { get; set; }
    }
}
