using HallOfFameWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace HallOfFameWebApi.Models
{
    public abstract class PersonCommandBase<TSkillCommand>
        where TSkillCommand : SkillCommandBase
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public string DisplayName { get; set; }

        public IList<TSkillCommand> Skills { get; set; } = new List<TSkillCommand>();

        public Person ToPerson(long id = 0)
        {
            return new Person
            {
                Id = id,
                Name = Name,
                DisplayName = DisplayName,
                Skills = Skills
                    .Select(s => s.ToSkill())
                    .ToList()
            };
        }
    }
}
