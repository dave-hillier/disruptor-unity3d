using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Collections;

namespace DisruptorUnity3d
{
	public class Test : MonoBehaviour {
    static readonly System.Random Rng = new System.Random();
    static readonly RingBuffer<int> Queue = new RingBuffer<int>(1000);
    //static readonly ConcurrentQueue<int> Queue = new ConcurrentQueue<int>();

    static readonly Stopwatch sw = new Stopwatch();
	private const long Count = 5000000 ;//100000000;
    private long Queued = 0;
    private const long BatchSize = 20000;
	private Thread consumerThread;
	private bool printed = false;
	// Use this for initialization
	void Start () {
		UnityEngine.Debug.Log("Started Test");
		consumerThread = new Thread(() =>
        {
			UnityEngine.Debug.Log("Started consumer");
			for (long i = 0; i < Count; )
            {
                int val;
                var dequeued = Queue.TryDequeue(out val);
                if (dequeued)
                    ++i;
            }
			UnityEngine.Debug.Log(string.Format("Consumer done {0}", sw.Elapsed));
        });	
		consumerThread.Start ();
		sw.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if (Queued >= Count && !printed) {
			sw.Stop ();
			UnityEngine.Debug.Log (string.Format ("Producer done {0} {1}", sw.Elapsed, Queued));
			printed = true;
			
		} else {
			for (long i = 0; i < BatchSize && Queued < Count; ++i) {
				Queue.Enqueue (Rng.Next ());
				++Queued;
			}
		}
	}
}
}
