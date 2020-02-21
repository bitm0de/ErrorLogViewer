using System;

namespace ErrorLogViewerLibrary.SimplPlus
{
    /// <summary>
    /// Generic array wrapper class for SIMPL+ compatibility.
    /// </summary>
    /// <remarks>Inheritors of this type for typed array wrappers can be
    /// passed back to SIMPL+ using callbacks but cannot be returned from
    /// functions back to SIMPL+ for some reason.</remarks>
    /// <typeparam name="T">Array element type</typeparam>
    public abstract class CrestronArray<T>
    {
        protected readonly T[] Data;
        public T[] Array { get { return Data; } }

        public int ContainsData { get { return Data != null ? 1 : 0; } }
    
        public int Length { get { return Data.Length; } }
    
        protected CrestronArray() { }
        protected CrestronArray(T[] array) { Data = array; }
    
        public virtual int IndexOf(T item)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i].Equals(item))
                    return i;
            }
            return -1;
        }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// String array for SIMPL+ compatibility.
    /// </summary>
    public sealed class StringArray : CrestronArray<string>
    {
        [Obsolete("Provided only for S+ compatibility", false)]
        public StringArray() { }
        public StringArray(string[] array) : base(array) { }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// Integer array for SIMPL+ compatibility.
    /// </summary>
    public sealed class IntegerArray : CrestronArray<ushort>
    {
        [Obsolete("Provided only for S+ compatibility", false)]
        public IntegerArray() { }
        public IntegerArray(ushort[] array) : base(array) { }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// SignedInteger array for SIMPL+ compatibility.
    /// </summary>
    public sealed class SignedIntegerArray : CrestronArray<short>
    {
        [Obsolete("Provided only for S+ compatibility", false)]
        public SignedIntegerArray() { }
        public SignedIntegerArray(short[] array) : base(array) { }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// LongInteger array for SIMPL+ compatibility.
    /// </summary>
    public sealed class LongIntegerArray : CrestronArray<uint>
    {
        [Obsolete("Provided only for S+ compatibility", false)]
        public LongIntegerArray() { }
        public LongIntegerArray(uint[] array) : base(array) { }
    }
    
    
    /// <inheritdoc />
    /// <summary>
    /// SignedLongInteger array for SIMPL+ compatibility.
    /// </summary>
    public sealed class SignedLongInteger : CrestronArray<int>
    {
        [Obsolete("Provided only for S+ compatibility", false)]
        public SignedLongInteger() { }
        public SignedLongInteger(int[] array) : base(array) { }
    }
}