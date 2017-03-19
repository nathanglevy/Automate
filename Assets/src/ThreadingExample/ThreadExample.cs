using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Assets.src.ThreadingExample {
    public class ThreadExample
    {
        public static void Main()
        {
            // example scenario:
            // we need more information before continuing calculation:
            bool isCompleted = false;
            int theResult = 0;

            // we make a wait handle 'signal' event, and set it to initial signal state false
            AutoResetEvent waitHandle = new AutoResetEvent(false);

            //create a thread what will do the work for us
            Thread thread = new Thread(delegate() {
                // calculate the stuff we need to know
                // this might take some time...
                Thread.Sleep(10000);

                // calculation is ready:
                theResult = 100;
                isCompleted = true;

                // we set the handle to signal that we are done
                // ONE of the threads waiting on this signal will wake up
                // since we have only one, we know who will wake up
                waitHandle.Set();
            });

            //we need this information to continue, so we send thread
            thread.Start();
            //then wait on the signal that it is done (we can put a timeout here as an argument)
            Console.Out.WriteLine("We are waiting!");
            waitHandle.WaitOne();
            //once this returns, the data is ready, we can keep working
            Console.Out.WriteLine("We got a reply!");
            Console.Out.WriteLine("Data is: " + theResult);
        }
    }
}
