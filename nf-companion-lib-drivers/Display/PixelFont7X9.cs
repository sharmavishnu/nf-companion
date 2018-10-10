﻿/***************************************************
Font Map Generator
(c) 2018 Vishnu Sharma, vishnusharma.com, All rights reserved
This file is distributed under MIT license (https://opensource.org/licenses/MIT)

WARNING !!
This file was generated using the Font Map Genertor tool.
Any manual editing of this file may result in a situation
where this file can no longer be recognized by the tool
and associated elements. 
****************************************************/
using nanoFramework.Companion.Drivers.Display;
using System;

//7ede8a4c-223c-4389-a683-cb2c5de448d5
//7;9
//7ede8a4c-223c-4389-a683-cb2c5de448d5

namespace nanoFramework.Companion.Drivers.Display
{
    /// <summary>
    /// Represents a 7 X 9 (W x H) pixel font object to be used with nanoFramework display device drivers.
    /// </summary>
    public class PixelFont7X9 : IPixelFont
    {
        private static readonly UInt32[] _charmap = {
		//f3b635d2-e762-4110-b202-488f5b6ca5df
            // Space
0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
// !
0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x00000000,0x10000000,0x10000000,
// "
0x28000000,0x28000000,0x28000000,0x28000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
// #
0x28000000,0x28000000,0x28000000,0xFE000000,0x28000000,0xFE000000,0x28000000,0x28000000,0x28000000,
// $
0x10000000,0x38000000,0x54000000,0x50000000,0x38000000,0x14000000,0x54000000,0x38000000,0x10000000,
// %
0xC2000000,0xC6000000,0x0C000000,0x18000000,0x30000000,0x60000000,0xC0000000,0x86000000,0x06000000,
// &
0x30000000,0x48000000,0x28000000,0x10000000,0x28000000,0x46000000,0x86000000,0x88000000,0x70000000,
// '
0x10000000,0x10000000,0x10000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
// (
0x18000000,0x20000000,0x20000000,0x20000000,0x20000000,0x20000000,0x20000000,0x20000000,0x18000000,
// )
0x30000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x30000000,
// *
0x00000000,0x10000000,0x54000000,0x38000000,0xFE000000,0x38000000,0x54000000,0x10000000,0x00000000,
// +
0x00000000,0x00000000,0x10000000,0x10000000,0x7C000000,0x10000000,0x10000000,0x00000000,0x00000000,
// ,
0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x18000000,0x08000000,0x10000000,
// -
0x00000000,0x00000000,0x00000000,0x00000000,0x7C000000,0x00000000,0x00000000,0x00000000,0x00000000,
// .
0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x10000000,0x00000000,0x00000000,
// /
0x02000000,0x06000000,0x0C000000,0x18000000,0x30000000,0x60000000,0xC0000000,0x80000000,0x00000000,
// 0
0x7C000000,0x82000000,0x86000000,0x8A000000,0x92000000,0xA2000000,0xC2000000,0x82000000,0x7C000000,
// 1
0x10000000,0x30000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x7C000000,
// 2
0x7C000000,0x82000000,0x82000000,0x02000000,0x04000000,0x08000000,0x10000000,0x60000000,0xFE000000,
// 3
0x7C000000,0x82000000,0x02000000,0x02000000,0x1C000000,0x02000000,0x02000000,0x82000000,0x7C000000,
// 4
0x84000000,0x84000000,0x84000000,0x84000000,0x84000000,0x7E000000,0x04000000,0x04000000,0x04000000,
// 5
0xFE000000,0x80000000,0x80000000,0x7C000000,0x02000000,0x02000000,0x02000000,0x82000000,0x7C000000,
// 6
0x7C000000,0x82000000,0x80000000,0x80000000,0xFC000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// 7
0xFE000000,0x02000000,0x06000000,0x04000000,0x08000000,0x18000000,0x10000000,0x20000000,0x20000000,
// 8
0x7C000000,0x82000000,0x82000000,0x82000000,0x7C000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// 9
0x7C000000,0x82000000,0x82000000,0x82000000,0x7E000000,0x02000000,0x02000000,0x82000000,0x7C000000,
// :
0x00000000,0x00000000,0x10000000,0x10000000,0x00000000,0x00000000,0x10000000,0x10000000,0x00000000,
// ;
0x00000000,0x00000000,0x10000000,0x10000000,0x00000000,0x18000000,0x08000000,0x10000000,0x00000000,
// <
0x08000000,0x10000000,0x20000000,0x40000000,0x80000000,0x40000000,0x20000000,0x10000000,0x08000000,
// =
0x00000000,0x00000000,0x00000000,0xFE000000,0x00000000,0xFE000000,0x00000000,0x00000000,0x00000000,
// >
0x20000000,0x10000000,0x08000000,0x04000000,0x02000000,0x04000000,0x08000000,0x10000000,0x20000000,
// ?
0x7C000000,0x82000000,0x82000000,0x02000000,0x04000000,0x08000000,0x10000000,0x00000000,0x10000000,
// @
0x7C000000,0x80000000,0xBC000000,0xA2000000,0xBA000000,0xAA000000,0xBA000000,0x82000000,0x7C000000,
// A
0x10000000,0x28000000,0x28000000,0x44000000,0x44000000,0xFE000000,0x82000000,0x82000000,0x82000000,
// B
0xFC000000,0x82000000,0x82000000,0x82000000,0xFC000000,0x82000000,0x82000000,0x82000000,0xFC000000,
// C
0x7C000000,0x82000000,0x80000000,0x80000000,0x80000000,0x80000000,0x80000000,0x82000000,0x7C000000,
// D
0xFC000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0xFC000000,
// E
0xFE000000,0x80000000,0x80000000,0x80000000,0xF8000000,0x80000000,0x80000000,0x80000000,0xFE000000,
// F
0xFE000000,0x80000000,0x80000000,0x80000000,0xF8000000,0x80000000,0x80000000,0x80000000,0x80000000,
// G
0x7C000000,0x82000000,0x80000000,0x80000000,0xBE000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// H
0x82000000,0x82000000,0x82000000,0x82000000,0xFE000000,0x82000000,0x82000000,0x82000000,0x82000000,
// I
0x7C000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x7C000000,
// J
0xFE000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x48000000,0x48000000,0x30000000,
// K
0x84000000,0x88000000,0x90000000,0xA0000000,0xC0000000,0xA0000000,0x90000000,0x88000000,0x84000000,
// L
0x80000000,0x80000000,0x80000000,0x80000000,0x80000000,0x80000000,0x80000000,0x80000000,0xFE000000,
// M
0x82000000,0xC6000000,0xAA000000,0x92000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,
// N
0x82000000,0x82000000,0xC2000000,0xA2000000,0x92000000,0x8A000000,0x86000000,0x82000000,0x82000000,
// O
0x7C000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// P
0xFC000000,0x82000000,0x82000000,0x82000000,0x82000000,0xFC000000,0x80000000,0x80000000,0x80000000,
// Q
0x7C000000,0x82000000,0x82000000,0x82000000,0x82000000,0x92000000,0x8A000000,0x86000000,0x7E000000,
// R
0xFC000000,0x82000000,0x82000000,0x82000000,0xFC000000,0x90000000,0x88000000,0x84000000,0x82000000,
// S
0x7C000000,0x82000000,0x80000000,0x80000000,0x7C000000,0x02000000,0x02000000,0x82000000,0x7C000000,
// T
0xFE000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,
// U
0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// V
0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x44000000,0x28000000,0x10000000,
// W
0x82000000,0x82000000,0x82000000,0x82000000,0x82000000,0x92000000,0xAA000000,0xC6000000,0x82000000,
// X
0x82000000,0x82000000,0x44000000,0x28000000,0x10000000,0x28000000,0x44000000,0x82000000,0x82000000,
// Y
0x82000000,0x82000000,0x44000000,0x28000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,
// Z
0xFE000000,0x04000000,0x08000000,0x10000000,0x10000000,0x20000000,0x20000000,0x40000000,0xFE000000,
// [
0x38000000,0x20000000,0x20000000,0x20000000,0x20000000,0x20000000,0x20000000,0x20000000,0x38000000,
// \
0x00000000,0x80000000,0xC0000000,0x60000000,0x30000000,0x18000000,0x0C000000,0x06000000,0x00000000,
// ]
0x38000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x38000000,
// ^
0x10000000,0x28000000,0x44000000,0x82000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
// _
0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x7C000000,0x00000000,
// `
0x20000000,0x30000000,0x10000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
// a
0x00000000,0x7C000000,0x82000000,0x02000000,0x7E000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// b
0x80000000,0x80000000,0x80000000,0x80000000,0xFC000000,0x82000000,0x82000000,0x82000000,0xFC000000,
// c
0x00000000,0x00000000,0x7C000000,0x82000000,0x80000000,0x80000000,0x80000000,0x82000000,0x7C000000,
// d
0x00000000,0x02000000,0x02000000,0x02000000,0x7E000000,0x82000000,0x82000000,0x82000000,0x7E000000,
// e
0x00000000,0x00000000,0x7C000000,0x82000000,0x82000000,0xFC000000,0x80000000,0x82000000,0x7C000000,
// f
0x30000000,0x48000000,0x40000000,0x40000000,0xE0000000,0x40000000,0x40000000,0x40000000,0x40000000,
// g
0x00000000,0x7C000000,0x82000000,0x82000000,0x82000000,0x7E000000,0x02000000,0x82000000,0x7C000000,
// h
0x80000000,0x80000000,0x80000000,0x80000000,0xFC000000,0x82000000,0x82000000,0x82000000,0x82000000,
// i
0x00000000,0x00000000,0x10000000,0x00000000,0x10000000,0x10000000,0x10000000,0x10000000,0x18000000,
// j
0x08000000,0x00000000,0x08000000,0x08000000,0x08000000,0x08000000,0x08000000,0x48000000,0x30000000,
// k
0x20000000,0x20000000,0x20000000,0x24000000,0x28000000,0x30000000,0x28000000,0x24000000,0x22000000,
// l
0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x18000000,
// m
0x00000000,0x00000000,0x00000000,0x00000000,0x7C000000,0x92000000,0x92000000,0x92000000,0x92000000,
// n
0x00000000,0x00000000,0x00000000,0x00000000,0x5C000000,0x62000000,0x42000000,0x42000000,0x42000000,
// o
0x00000000,0x00000000,0x00000000,0x7C000000,0x82000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// p
0x00000000,0x00000000,0xFC000000,0x82000000,0x82000000,0x82000000,0xFC000000,0x80000000,0x80000000,
// q
0x7C000000,0x84000000,0x84000000,0x84000000,0x7C000000,0x04000000,0x04000000,0x06000000,0x04000000,
// r
0x00000000,0x00000000,0x00000000,0x18000000,0xA0000000,0x40000000,0x40000000,0x40000000,0x40000000,
// s
0x00000000,0x00000000,0x7C000000,0x82000000,0x80000000,0x7C000000,0x02000000,0x82000000,0x7C000000,
// t
0x10000000,0x10000000,0x38000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x18000000,
// u
0x00000000,0x00000000,0x00000000,0x00000000,0x82000000,0x82000000,0x82000000,0x82000000,0x7C000000,
// v
0x00000000,0x00000000,0x00000000,0x00000000,0x82000000,0x82000000,0x44000000,0x28000000,0x10000000,
// w
0x00000000,0x00000000,0x00000000,0x00000000,0x92000000,0x92000000,0x92000000,0x92000000,0x7C000000,
// x
0x00000000,0x00000000,0x00000000,0x00000000,0xC6000000,0x28000000,0x10000000,0x28000000,0xC6000000,
// y
0x00000000,0x00000000,0x84000000,0x84000000,0x44000000,0x3C000000,0x04000000,0x84000000,0x78000000,
// z
0x00000000,0x00000000,0x00000000,0xFE000000,0x0C000000,0x18000000,0x30000000,0x60000000,0xFE000000,
// {
0x18000000,0x10000000,0x10000000,0x10000000,0x20000000,0x10000000,0x10000000,0x10000000,0x18000000,
// |
0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,0x10000000,
// }
0x30000000,0x10000000,0x10000000,0x10000000,0x08000000,0x10000000,0x10000000,0x10000000,0x30000000,
// ~
0x00000000,0x00000000,0x00000000,0x60000000,0x92000000,0x0C000000,0x00000000,0x00000000,0x00000000
		//f3b635d2-e762-4110-b202-488f5b6ca5df
        };
		/// <summary>
		/// Accessor for the LCD font height (in pixels)
		/// </summary>
		public byte Height { get { return 9; } }
        /// <summary>
        /// Accessor for the LCD font width (in pixels)
        /// </summary>
        public byte Width { get { return 7; } }
        /// <summary>
        /// The character map
        /// </summary>
        public UInt32[] CharMap{ get { return _charmap;}}
    }
}

