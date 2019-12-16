using System;
using System.Collections.Generic;
using System.Text;

namespace Xmas2019.Library.Infrastructure.Toys
{
    [Serializable]
    public class ToyDistributionProblem
    {
        public List<Toy> Toys { get; set; }

        public List<Child> Children { get; set; }

        public ToyDistributionProblem()
        {

        }
    }
}
