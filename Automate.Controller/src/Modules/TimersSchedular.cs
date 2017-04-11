using System;
using System.Collections.Generic;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Modules
{
    public class TimersSchedular<T> : ITimerScheduler<T>
    {
        private DateTime _lastUpdate;

        //        private readonly SortedDictionary<long,T> _ackEvents = new SortedDictionary<long , T>();
        private readonly SortedDictionary<long, Queue<T>> _ackEvents = new SortedDictionary<long, Queue<T>>();
        private readonly Object dictLocker = new Object();
        private int _itemsCount = 0;

        ///  EVENTS SECTION
//        public event PushToTimerQ<T> GetEnqueueDelegate;

        // Get and Set Proprties
        public TimedOut<T> OnTimeOut { get; }


        /// <summary>
        /// Constructor, gets a delegate to call whenever TimedOut of an item needed
        /// </summary>
        /// <param name="onTimeOut"></param>
        public TimersSchedular(TimedOut<T> onTimeOut)
        {
            OnTimeOut = onTimeOut;
        }


        /// <summary>
        /// the method will go over the internal data structure and fire the TimedOut Event for all items with "past" time
        /// </summary>
        /// <param name="currentTime"> the time value which needed to decide what is past and what is future</param>
        private void InvokeTimedOut(DateTime currentTime)
        {
//            Console.Out.WriteLine("Event Of Timer AT: " + System.DateTime.Now.ToFileTime());
            lock (dictLocker)
            {
                var enumerator = _ackEvents.GetEnumerator();
                var forDisposal = new List<long>();
                while (enumerator.MoveNext() && currentTime.Ticks > enumerator.Current.Key)
                {
                    forDisposal.Add(enumerator.Current.Key);
                    var queue = enumerator.Current.Value;

                    while (queue.Count > 0)
                    {
                        T action = queue.Dequeue();
                        _itemsCount--;

                        OnTimeOut(action);
                    }


                }

                enumerator.Dispose();

                foreach (var disposeKey in forDisposal)
                {
                    lock (dictLocker)
                    {
                        _ackEvents.Remove(disposeKey);
                    }
                }
            }

        }

        /// <summary>
        /// returns the number of items reside in the internal data structure
        /// </summary>
        public int ItemsCount
        {
            get { return _itemsCount; }
        }

        /// <summary>
        ///  returns a boolean to indicate if the data structure has any items
        /// </summary>
        public bool HasItems
        {
            get { return ItemsCount > 0; }
        }

        /// <summary>
        /// the method used to add an item to the internal data structure
        /// </summary>
        /// <param name="timeoutTime">the timeout value to set</param>
        /// <param name="item">the object value to fire on timeout</param>
        public void Enqueue(DateTime timeoutTime, T item)
        {
            // check if we have a valid update time
            if (_lastUpdate == null)
                throw new ArgumentException("LastUpdate Time Still Null/Not Initiazied, therfore, cannot calculate the timeout Slot");

            // calculate the slot
            var timeoutTicks = timeoutTime.Ticks;
            var delta = timeoutTicks - _lastUpdate.Ticks;

            // check if the slot is in the past
            if (delta < 0)
            {
                //todo: consult Naph if it's the needed behaviur (can we have a race and the Update is faster?
                OnTimeOut(item);
                return;
            }

            lock (dictLocker)
            {
                if (!_ackEvents.ContainsKey(timeoutTicks))
                {
                    _ackEvents.Add(timeoutTicks, new Queue<T>());
                }
                _ackEvents[timeoutTicks].Enqueue(item);
                _itemsCount++;
            }

        }

        /// <summary>
        /// method to be used to force an update to the Q, 
        /// </summary>
        /// <param name="args">arguments which contain the Time to use to fire timedout items</param>
        public void Update(TimerSchudulerUpdateArgs args)
        {
            // invoke all timedout actions
            InvokeTimedOut(args.Time);

            // update LastUpdate Point
            _lastUpdate = args.Time;

        }


    }
}