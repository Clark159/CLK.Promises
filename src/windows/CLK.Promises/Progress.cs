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
        public Progress(int completedCount, int totalCount, string description = null)
        {
            // Default            
            this.CompletedCount = completedCount;
            this.TotalCount = totalCount;
            this.Description = description;
        }


        // Properties
        public int CompletedCount { get; }

        public int TotalCount { get; }       

        public string Description { get; }
    }
}
