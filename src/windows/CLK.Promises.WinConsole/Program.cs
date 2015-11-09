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
            // Tests
            PromiseTests();

            // End
            Console.ReadLine();
        }

        static void PromiseTests()
        {
            // Promise
            var promise = new EmptyPromise();

            promise

                // ========== Then ==========
                .Then(delegate ()
                {
                    Console.WriteLine("AAA");
                })

                // ThenPromise - Resolve
                .ThenPromise(delegate ()
                {
                    var newPromise = new EmptyPromise();
                    newPromise.Resolve();
                    return newPromise;
                })
                .Then(delegate ()
                {
                    Console.WriteLine("BBB");
                })

                // ThenPromise - Reject
                .ThenPromise(delegate ()
                {
                    var newPromise = new EmptyPromise();
                    newPromise.Reject(new Exception("CCC"));
                    return newPromise;
                })
                .Fail(delegate (Exception error)
                {
                    Console.WriteLine(error.Message);
                })


                // ========== ThenNew ==========
                .ThenNew<string>(delegate ()
                {
                    return "DDD";
                })
                .Then(delegate (string message)
                {
                    Console.WriteLine(message);
                })

                // ThenNewPromise - Resolve
                .ThenNewPromise<string>(delegate ()
                {
                    var newPromise = new ResultPromise<string>();
                    newPromise.Resolve("EEE");
                    return newPromise;
                })
                .Then(delegate (string message)
                {
                    Console.WriteLine(message);
                })

                // ThenNewPromise - Reject
                .ThenNewPromise<string>(delegate ()
                {
                    var newPromise = new ResultPromise<string>();
                    newPromise.Reject(new Exception("FFF"));
                    return newPromise;
                })
                .Fail(delegate (Exception error)
                {
                    Console.WriteLine(error.Message);
                })


                // ========== Throw ==========
                .Then(delegate ()
                {
                    throw new Exception("GGG");
                })
                .Fail(delegate (Exception error)
                {
                    throw error;
                })
                .Fail(delegate (Exception error)
                {
                    Console.WriteLine(error.Message);
                })


                // ========== End ==========
                .Progress(delegate (Progress progress)
                {
                    Console.WriteLine("Progress:" + progress.Description);
                })
                .Fail(delegate (Exception error)
                {
                    Console.WriteLine("Fail:" + error.Message);
                })
                .Then(delegate ()
                {
                    Console.WriteLine("End");
                })
            ;

            // Operate 
            promise.Notify(new Progress(0, 100, "0%"));
            promise.Notify(new Progress(50, 100, "50%"));
            promise.Notify(new Progress(100, 100, "100%"));
            promise.Resolve();
        }
    }
}
