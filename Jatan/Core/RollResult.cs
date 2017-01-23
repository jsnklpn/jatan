using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core
{
    /// <summary>
    /// Class which represents the result of a dice roll.
    /// </summary>
    public class RollResult
    {
        /// <summary>
        /// The values for each die.
        /// </summary>
        public int[] Dice { get; private set; }
        /// <summary>
        /// The sum of all die.
        /// </summary>
        public int Total { get; private set; }
        /// <summary>
        /// Constructor.
        /// </summary>
        public RollResult(IEnumerable<int> dice)
        {
            var diceList = new List<int>(dice);
            this.Dice = diceList.ToArray();
            this.Total = diceList.Sum();
        }

        /// <summary>
        /// Equals override
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            RollResult r = (RollResult) obj;
            if (this.Total != r.Total || this.Dice.Length != r.Dice.Length)
                return false;

            for (int i = 0; i < this.Dice.Length; i++)
            {
                if (this.Dice[i] != r.Dice[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a hashcode for use in hash sets
        /// </summary>
        public override int GetHashCode()
        {
            int result = 0;
            foreach (var d in Dice)
            {
                result ^= d;
            }
            return result;
        }
    }
}
