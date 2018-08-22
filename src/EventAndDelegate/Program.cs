using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventAndDelegate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Clock clock = new Clock();

            DisplayTime display = new DisplayTime();
            display.Subscribe(clock);

            LogClock lc = new LogClock();
            lc.Subscribe(clock);

            clock.Run();
        }
    }



    public class Clock
    {
        private int _second;
        private int _minute;
        private int _hour;


        // The delegate named SecondChangeHandler, which will encapsulate
        // any method that takes a clock object and a TimeInfoEventArgs
        // object as the parameter and returns no value. It's the
        // delegate the subscribers must implement.
        public delegate void SecondsEventHandler(Object clock, TimeInfoEventArgs timeInfoArgs);


        //The event we publish
        public event SecondsEventHandler SecondChange;

        //Method which fires the event
        protected void OnSecondChange(object clock,TimeInfoEventArgs timeInfo)
        {
            SecondChange?.Invoke(clock, timeInfo);
        }

        public void Run()
        {
            for (;;)
            {
                Thread.Sleep(1000);
                var currentTime = DateTime.Now;
                if (currentTime.Second != _second)
                {
                    TimeInfoEventArgs timeArgs = new TimeInfoEventArgs(currentTime.Second, currentTime.Minute, currentTime.Hour);
                    OnSecondChange(this, timeArgs);
                }
                _second = currentTime.Second;
                _minute = currentTime.Minute;
                _hour = currentTime.Hour;
            }
        }

    }

    public class TimeInfoEventArgs : EventArgs
    {
        public TimeInfoEventArgs(int second,int minute,int hour) 
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }
        public readonly int hour;
        public readonly int minute;
        public readonly int second;
    }


    //Subscriber 1, whose job is to display currnet time
    public class DisplayTime
    {
        public void Subscribe(Clock clock)
        {
            clock.SecondChange += new Clock.SecondsEventHandler(TimeChanged);
        }

        public void TimeChanged(Object clock, TimeInfoEventArgs args)
        {
            Console.WriteLine("Current Time " + args.hour +":"+args.minute+":" +args.second);
        }
    }

    // A second subscriber whose job is to write to a file
    public class LogClock
    {
        public void Subscribe(Clock theClock)
        {
            theClock.SecondChange +=
               new Clock.SecondsEventHandler(WriteLogEntry);
        }

        // This method should write to a file
        // we write to the console to see the effect
        // this object keeps no state
        public void WriteLogEntry(
           object theClock, TimeInfoEventArgs ti)
        {
            Console.WriteLine("Logging to file: {0}:{1}:{2}",
               ti.hour.ToString(),
               ti.minute.ToString(),
               ti.second.ToString());
        }
    }

}
