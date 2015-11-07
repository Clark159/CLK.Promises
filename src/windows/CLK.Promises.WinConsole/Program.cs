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
                    return "Clark1111";
                }, delegate (Exception error) { Console.WriteLine("Error:" + error.Message); }, delegate (Progress progress) { })
                .Then(delegate (string aaa)
                {
                    Console.WriteLine(aaa);
                }, delegate (Exception error) { Console.WriteLine("Error:" + error.Message); }, delegate (Progress progress) { })
            ;

            x.Resolve();
            //x.Reject(new Exception("GGG"));

            Console.ReadLine();
        }
    }
}
