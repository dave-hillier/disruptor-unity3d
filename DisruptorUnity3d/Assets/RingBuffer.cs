using System.Threading;

namespace DisruptorUnity3d
{
	public class RingBuffer<T> 
	{
	    private readonly T[] _entries;
	    private readonly int _modMask;
	    private Volatile.PaddedLong _consumerCursor = new Volatile.PaddedLong();
	    private Volatile.PaddedLong _producerCursor = new Volatile.PaddedLong();

	    public RingBuffer(int size)
	    {
	        size = NextPowerOfTwo(size);
	        _modMask = size - 1;
	        _entries = new T[size];
	    }

	    public int Capacity
	    {
	        get { return _entries.Length; }
	    }

	    public T this[long index]
	    {
	        get { unchecked { return _entries[index & _modMask]; } }
	        set { unchecked { _entries[index & _modMask] = value; } }
	    }

	    public T Dequeue()
	    {
	        var next = _consumerCursor.ReadFullFence() + 1;
	        while (_producerCursor.ReadFullFence() < next)
	        {
	            Thread.SpinWait(1); 
	        }
	        _consumerCursor.WriteFullFence(next);
	        return this[next];
	    }

	    public bool TryDequeue(out T obj)
	    {
	        var next = _consumerCursor.ReadFullFence() + 1;

	        if (_producerCursor.ReadFullFence() < next)
	        {
	            obj = default(T);
	            return false;
	        }
	        obj = Dequeue();
	        return true;
	    }

	    public void Enqueue(T element)
	    {
	        var next = _producerCursor.ReadFullFence() + 1;

	        long wrapPoint = next- _entries.Length;
	        long min = _consumerCursor.ReadFullFence();
	        
	        while (wrapPoint > min)
	        {
	            min = _consumerCursor.ReadFullFence();
	            Thread.SpinWait(1);
	        }

	        _producerCursor.WriteUnfenced(next);
	        this[next] = element;
	    }

	    public int Count { get { return (int)(_producerCursor.ReadFullFence() - _consumerCursor.ReadFullFence()); } }

	    private static int NextPowerOfTwo(int x)
	    {
	        var result = 2;
	        while (result < x)
	        {
	            result <<= 1;
	        }
	        return result;
	    }
	}
}
