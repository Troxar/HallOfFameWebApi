using HallOfFameWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace HallOfFameWebApi.Models
{
    public class CreatePersonCommand
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public string DisplayName { get; set; }

        public IList<CreateSkillCommand> Skills { get; set; } = new List<CreateSkillCommand>();

        public Person ToPerson()
        {
            return new Person
            {
                Name = Name,
                DisplayName = DisplayName,
                Skills = Skills
                    .Select(s => s.ToSkill())
                    .ToList()
            };
        }
    }
}
