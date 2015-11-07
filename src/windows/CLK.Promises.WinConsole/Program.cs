using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises.WinConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Deferred
            var x = new Deferred();
            x.Resolve();
            x.Promise

                // Result
                .Then<string>(delegate ()
                {
                    return "AAA";
                })
                .Then(delegate (string message)
                {
                    Console.WriteLine(message);
                })

                // Throw
                .Then(delegate ()
                {
                    throw new Exception("BBB");
                })
                .Catch(delegate(Exception error)
                {
                    throw error;
                })
                .Catch(delegate (Exception error)
                {
                    Console.WriteLine(error.Message);
                })

                // Result - Promise
                .Then<string>(delegate ()
                {
                    return Promise.Resolve("CCC");
                })
                .Then(delegate (string message)
                {
                    Console.WriteLine(message);
                })

                // Throw
                .Then(delegate ()
                {
                    return Promise.Reject(new Exception("DDD"));
                })
                .Catch(delegate (Exception error)
                {
                    throw error;
                })
                .Catch(delegate (Exception error)
                {
                    Console.WriteLine(error.Message);
                })
                
                // End
                .Then(delegate ()
                {
                    Console.WriteLine("End");
                })
            ;

            Console.ReadLine();
        }
    }
}
