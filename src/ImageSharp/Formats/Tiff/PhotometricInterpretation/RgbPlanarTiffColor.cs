// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff
{
    /// <summary>
    /// Implements the 'RGB' photometric interpretation with 'Planar' layout (for all bit depths).
    /// </summary>
    internal static class RgbPlanarTiffColor
    {
        /// <summary>
        /// Decodes pixel data using the current photometric interpretation.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="data">The buffers to read image data from.</param>
        /// <param name="bitsPerSample">The number of bits per sample for each pixel.</param>
        /// <param name="pixels">The image buffer to write pixels to.</param>
        /// <param name="left">The x-coordinate of the left-hand side of the image block.</param>
        /// <param name="top">The y-coordinate of the  top of the image block.</param>
        /// <param name="width">The width of the image block.</param>
        /// <param name="height">The height of the image block.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decode<TPixel>(byte[][] data, uint[] bitsPerSample, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
            where TPixel : struct, IPixel<TPixel>
        {
            TPixel color = default(TPixel);

            BitReader rBitReader = new BitReader(data[0]);
            BitReader gBitReader = new BitReader(data[1]);
            BitReader bBitReader = new BitReader(data[2]);
            float rFactor = (float)Math.Pow(2, bitsPerSample[0]) - 1.0f;
            float gFactor = (float)Math.Pow(2, bitsPerSample[1]) - 1.0f;
            float bFactor = (float)Math.Pow(2, bitsPerSample[2]) - 1.0f;

            for (int y = top; y < top + height; y++)
            {
                for (int x = left; x < left + width; x++)
                {
                    float r = ((float)rBitReader.ReadBits(bitsPerSample[0])) / rFactor;
                    float g = ((float)gBitReader.ReadBits(bitsPerSample[1])) / gFactor;
                    float b = ((float)bBitReader.ReadBits(bitsPerSample[2])) / bFactor;
                    color.PackFromVector4(new Vector4(r, g, b, 1.0f));
                    pixels[x, y] = color;
                }

                rBitReader.NextRow();
                gBitReader.NextRow();
                bBitReader.NextRow();
            }
        }
    }
}
