using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Inaba.Framework
{
    public static class ExtensionMethodsStream
    {
        public static void EnsureRead(this Stream input, Span<Byte> span)
        {
            if (input.Read(span) != span.Length)
                throw new EndOfStreamException();
        }
        
        public static void EnsureRead<T>(this Stream input, Span<T> span) where T: unmanaged
        {
            EnsureRead(input, MemoryMarshal.Cast<T, Byte>(span));
        }

        public static T EnsureRead<T>(this Stream input) where T : unmanaged
        {
            unsafe
            {
                T result = default;
                Span<Byte> headerSpan = new Span<Byte>(&result, sizeof(T));
                EnsureRead(input, headerSpan);
                return result;
            }
        }
        
        public static T[] EnsureRead<T>(this Stream input, Int32 count) where T : unmanaged
        {
            T[] result = new T[count];
            Span<T> span = new Span<T>(result);
            EnsureRead(input, span);
            return result;
        }

        public static void Write<T>(this Stream input, in T value) where T : unmanaged
        {
            unsafe
            {
                fixed (T* ptr = &value)
                {
                    Span<Byte> span = new Span<Byte>(ptr, sizeof(T));
                    input.Write(span);
                }
            }
        }
        
        public static void Write<T>(this Stream input, T[] value) where T : unmanaged
        {
            unsafe
            {
                fixed (T* ptr = value)
                {
                    Span<Byte> span = new Span<Byte>(ptr, sizeof(T) * value.Length);
                    input.Write(span);
                }
            }
        }
        
        public static void SetPosition(this Stream input, Int64 position)
        {
            if (input.Position != position)
                input.Position = position;
        }
    }
}