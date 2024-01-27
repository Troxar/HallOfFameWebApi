using HallOfFameWebApi.Entities;

namespace HallOfFameWebApi.Models
{
    public class UpdatePersonCommand : PersonCommandBase<UpdateSkillCommand>
    {
        public void UpdatePerson(Person person)
        {
            person.Name = Name;
            person.DisplayName = DisplayName;
            person.Skills = Skills
                .Select(s => s.ToSkill())
                .ToList();
        }
    }
}
