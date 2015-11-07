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
            var x = new Deferred();            
            x.Promise
                .Then<string>(delegate ()
                {
                    return "AAA";
                })
                .Then(delegate (string message)
                {
                    Console.WriteLine(message);
                })
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
                .Then(delegate ()
                {
                    Console.WriteLine("CCC");
                })
            ;
            x.Resolve();

            //x.Reject(new Exception("GGG"));

            Console.ReadLine();
        }
    }
}
