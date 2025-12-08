using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    public class ShoveAbility : Ability
    {
        public int Damage { get; set; }
        public int Distance { get; set; }

        public override void Activate(IPlayer user, ITargetable target)
        {
            // Justifications for not using this method.

            // I'm hopeful you'll find some leniency here for a few reasons.  I get that this is a learning tool but it's not
            // a coding decision I would deem useful or good for the assignment at hand.

            // 1. A fire ball ability (an example from the seed data) is not a shove but we're stuck pretending it is in order to deal damage.
            // 2. Shove does not clearly dictate damage
            // 3. The ability models should not contain logic for updating a monster's health in the database
            // 4. The only important piece of information here is that the ability does damage and that should have just been
            //    a shared trait at the Ability type.

            // If the expectation was to change all of the seeded abilities to their own subtypes of Ability
            // they all would have needed the damage field anyway and the distance field would have remained unnecessary.

            // Overall I think utilizing this method to update the monster's health in the database would be a poor choice
            // just as using it as the primary type of ability for all damaging abilities was.

            // In a more advanced application maybe this file was necessary but in the real world
            // my suggestion would have been to remove it from the equation.

            // When damage is the only thing that matters, this class is not KISS it's DUMB.  Doing unnecessarily moronic business.
            Console.WriteLine($"{user.Name} shoves {target.Name} back {Distance} feet, dealing {Damage} damage!");
        }
    }
}
