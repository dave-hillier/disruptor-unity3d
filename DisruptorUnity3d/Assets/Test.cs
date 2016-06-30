using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace DisruptorUnity3d
{
    public class Test : MonoBehaviour
    {
        static readonly Random Rng = new Random();
        static readonly RingBuffer<int> Queue = new RingBuffer<int>(1000);
        //static readonly ConcurrentQueue<int> Queue = new ConcurrentQueue<int>();

        static readonly Stopwatch sw = new Stopwatch();
        private const long Count = 5000000;//100000000;
        private long _queued = 0;
        private const long BatchSize = 20000;
        private Thread _consumerThread;
        private bool _printed = false;

        private int numberToEnqueue;

        public void Start()
        {
            Debug.Log("Started Test");
            _consumerThread = new Thread(() =>
            {
                Debug.Log("Started consumer");
                int expectedNumber = 0;
                int previousNumber = 0;
                for (long i = 0; i < Count; )
                {
                    int val;
                    var dequeued = Queue.TryDequeue(out val);
                    if (dequeued)
                    {
                        if (expectedNumber != val)
                            Debug.Log("wrong value " + val + " ,correct: " + i + " ,previous: " + previousNumber);
                        previousNumber = val;
                        expectedNumber++;
                        ++i;
                    }
                        
                }
                Debug.Log(string.Format("Consumer done {0}", sw.Elapsed));
            });
            _consumerThread.Start();
            sw.Start();
        }

        

        // Update is called once per frame
        public void Update()
        {
            if (_queued >= Count && !_printed)
            {
                sw.Stop();
                Debug.Log(string.Format("Producer done {0} {1}", sw.Elapsed, _queued));
                _printed = true;

            }
            else
            {
                for (long i = 0; i < BatchSize && _queued < Count; ++i)
                {
                    Queue.Enqueue(numberToEnqueue++);
                    ++_queued;
                }
            }
        }
    }
}
