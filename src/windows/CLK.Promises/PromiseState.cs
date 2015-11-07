using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises
{
    public enum PromiseState
    {
        Pending,  // unresolved  
        Resolved, // has-resolution
        Rejected, // has-rejection
    };
}
