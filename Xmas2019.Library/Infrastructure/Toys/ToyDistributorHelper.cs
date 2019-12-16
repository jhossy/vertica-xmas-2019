using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xmas2019.Library.Infrastructure.Toys
{
    public class ToyDistributorHelper
    {
        private readonly List<Toy> _toys;

        public ToyDistributorHelper(List<Toy> toys)
        {
            if (toys == null) throw new ArgumentOutOfRangeException(nameof(toys));

            _toys = toys;
        }

        public void RemoveToy(Toy toy)
        {
            _toys.Remove(toy);
        }

        public bool TryResolve(Child child, out Toy foundToy)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));

            foundToy = null;

            int matches = 0;
            foreach(Toy toy in _toys)
            {
                if(child.WishList.Toys.Any(toyWish => toyWish.Name.Equals(toy.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    matches++;
                    foundToy = toy;
                }
            }

            if (matches == 1)
            {
                Console.WriteLine($"Found match: {child.Name}-{foundToy.Name}");
                return true;
            }

            return false;
        }
    }
}
