using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises
{
    public sealed class Progress
    {
        // Constructors
        public Progress(int totalCount, int completedCount, string description = null)
        {
            // Default
            this.TotalCount = totalCount;
            this.CompletedCount = completedCount;
            this.Description = description;
        }


        // Properties
        public int TotalCount { get; }

        public int CompletedCount { get; }

        public string Description { get; }
    }
}
