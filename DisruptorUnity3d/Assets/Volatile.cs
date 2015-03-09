using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace DisruptorUnity3d
{
	public static class Volatile
	{
	    private const int CacheLineSize = 64;

	    [StructLayout(LayoutKind.Explicit, Size = CacheLineSize * 2)]
	    public struct PaddedLong
	    {
	        [FieldOffset(CacheLineSize)]
	        private long _value;

	        /// <summary>
	        /// Create a new <see cref="PaddedLong"/> with the given initial value.
	        /// </summary>
	        /// <param name="value">Initial value</param>
	        public PaddedLong(long value)
	        {
	            _value = value;
	        }

	        /// <summary>
	        /// Read the value without applying any fence
	        /// </summary>
	        /// <returns>The current value</returns>
	        public long ReadUnfenced()
	        {
	            return _value;
	        }

	        /// <summary>
	        /// Read the value applying acquire fence semantic
	        /// </summary>
	        /// <returns>The current value</returns>
	        public long ReadAcquireFence()
	        {
	            var value = _value;
	            Thread.MemoryBarrier();
	            return value;
	        }

	        /// <summary>
	        /// Read the value applying full fence semantic
	        /// </summary>
	        /// <returns>The current value</returns>
	        public long ReadFullFence()
	        {
	            Thread.MemoryBarrier();
	            return _value;
	        }

	        /// <summary>
	        /// Read the value applying a compiler only fence, no CPU fence is applied
	        /// </summary>
	        /// <returns>The current value</returns>
	        [MethodImpl(MethodImplOptions.NoOptimization)]
	        public long ReadCompilerOnlyFence()
	        {
	            return _value;
	        }

	        /// <summary>
	        /// Write the value applying release fence semantic
	        /// </summary>
	        /// <param name="newValue">The new value</param>
	        public void WriteReleaseFence(long newValue)
	        {
	            Thread.MemoryBarrier();
	            _value = newValue;
	        }

	        /// <summary>
	        /// Write the value applying full fence semantic
	        /// </summary>
	        /// <param name="newValue">The new value</param>
	        public void WriteFullFence(long newValue)
	        {
	            Thread.MemoryBarrier();
	            _value = newValue;
	        }

	        /// <summary>
	        /// Write the value applying a compiler fence only, no CPU fence is applied
	        /// </summary>
	        /// <param name="newValue">The new value</param>
	        [MethodImpl(MethodImplOptions.NoOptimization)]
	        public void WriteCompilerOnlyFence(long newValue)
	        {
	            _value = newValue;
	        }

	        /// <summary>
	        /// Write without applying any fence
	        /// </summary>
	        /// <param name="newValue">The new value</param>
	        public void WriteUnfenced(long newValue)
	        {
	            _value = newValue;
	        }

	        /// <summary>
	        /// Atomically set the value to the given updated value if the current value equals the comparand
	        /// </summary>
	        /// <param name="newValue">The new value</param>
	        /// <param name="comparand">The comparand (expected value)</param>
	        /// <returns></returns>
	        public bool AtomicCompareExchange(long newValue, long comparand)
	        {
	            return Interlocked.CompareExchange(ref _value, newValue, comparand) == comparand;
	        }

	        /// <summary>
	        /// Atomically set the value to the given updated value
	        /// </summary>
	        /// <param name="newValue">The new value</param>
	        /// <returns>The original value</returns>
	        public long AtomicExchange(long newValue)
	        {
	            return Interlocked.Exchange(ref _value, newValue);
	        }

	        /// <summary>
	        /// Atomically add the given value to the current value and return the sum
	        /// </summary>
	        /// <param name="delta">The value to be added</param>
	        /// <returns>The sum of the current value and the given value</returns>
	        public long AtomicAddAndGet(long delta)
	        {
	            return Interlocked.Add(ref _value, delta);
	        }

	        /// <summary>
	        /// Atomically increment the current value and return the new value
	        /// </summary>
	        /// <returns>The incremented value.</returns>
	        public long AtomicIncrementAndGet()
	        {
	            return Interlocked.Increment(ref _value);
	        }

	        /// <summary>
	        /// Atomically increment the current value and return the new value
	        /// </summary>
	        /// <returns>The decremented value.</returns>
	        public long AtomicDecrementAndGet()
	        {
	            return Interlocked.Decrement(ref _value);
	        }

	        /// <summary>
	        /// Returns the string representation of the current value.
	        /// </summary>
	        /// <returns>the string representation of the current value.</returns>
	        public override string ToString()
	        {
	            var value = ReadFullFence();
	            return value.ToString();
	        }
	    }
	}
}