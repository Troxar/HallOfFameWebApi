using HallOfFameWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace HallOfFameWebApi.Models
{
    public abstract class SkillCommandBase
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, Range(1, 10)]
        public byte Level { get; set; }

        public Skill ToSkill()
        {
            return new Skill
            {
                Name = Name,
                Level = Level
            };
        }
    }
}
