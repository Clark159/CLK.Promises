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
        public Progress(string localizedDescription, int completedCount, int totalCount)
        {
            #region Contracts

            if (string.IsNullOrEmpty(localizedDescription) == true) throw new ArgumentNullException();

            #endregion

            // Default           
            this.LocalizedDescription = localizedDescription;
            this.CompletedCount = completedCount;
            this.TotalCount = totalCount;            
        }


        // Properties 
        public string LocalizedDescription { get; }

        public int CompletedCount { get; }

        public int TotalCount { get; }      
    }
}
