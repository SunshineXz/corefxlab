﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Sequences;

namespace System.Buffers
{
    public interface ISlicable
    {
        ReadOnlyBuffer<byte> Slice(Position start, Position end);
    }

    // TODO: the TryReadUntill methods are very inneficient. We need to fix that.
    public static partial class BufferReaderExtensions
    {
        public static bool TryReadUntill<TSequence>(ref BufferReader<TSequence> reader, out ReadOnlyBuffer<byte> bytes, byte delimiter)
            where TSequence : ISequence<ReadOnlyMemory<byte>>, ISlicable
        {
            var copy = reader;
            var start = reader.Position;
            while (!reader.End)
            {
                Position end = reader.Position;
                if (reader.Read() == delimiter)
                {
                    bytes = reader.Sequence.Slice(start, end);
                    return true;
                }
            }
            reader = copy;
            bytes = default;
            return false;
        }

        public static bool TryReadUntill<TSequence>(ref BufferReader<TSequence> reader, out ReadOnlyBuffer<byte> bytes, ReadOnlySpan<byte> delimiter)
            where TSequence : ISequence<ReadOnlyMemory<byte>>, ISlicable
        {
            if (delimiter.Length == 0)
            {
                bytes = default;
                return true;
            }

            int matched = 0;
            var copy = reader;
            var start = reader.Position;
            var end = reader.Position;
            while (!reader.End)
            {
                if (reader.Read() == delimiter[matched])
                {
                    matched++;
                }
                else
                {
                    end = reader.Position;
                    matched = 0;
                }
                if (matched >= delimiter.Length)
                {
                    bytes = reader.Sequence.Slice(start, end);
                    return true;
                }
            }
            reader = copy;
            bytes = default;
            return false;
        }
    }
}
