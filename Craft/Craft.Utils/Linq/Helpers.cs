using System.Collections.Generic;

namespace Craft.Utils.Linq
{
    public static class Helpers
    {
        // Partitions a sequence into arrays of desired size
        // From: https://stackoverflow.com/questions/4461367/linq-to-objects-return-pairs-of-numbers-from-list-of-numbers
        public static IEnumerable<T[]> Partition<T>(
            this IEnumerable<T> sequence, 
            int partitionSize)
        {
            var buffer = new T[partitionSize];
            var n = 0;
            foreach (var item in sequence)
            {
                buffer[n] = item;
                n += 1;
                if (n != partitionSize) continue;

                yield return buffer;
                buffer = new T[partitionSize];
                n = 0;
            }

            // Partial leftovers
            if (n > 0) yield return buffer;
        }
    }
}
