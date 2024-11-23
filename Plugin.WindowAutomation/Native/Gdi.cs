using System;
using System.Runtime.InteropServices;

namespace Plugin.WindowAutomation.Native
{
	internal static class Gdi
	{
		public enum RopMode : Int32
		{
			R2_NOT = 6
		}

		[DllImport("gdi32.dll")]
		public static extern Int32 SetROP2(IntPtr hdc, Int32 fnDrawMode);

		public enum PenStyles : Int32
		{
			PS_INSIDEFRAME = 6
		}

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreatePen(Int32 fnPenStyle, Int32 nWidth, UInt32 crColor);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		/// <summary>The type of stock object</summary>
		public enum StockObjects : Int32
		{
			WHITE_BRUSH = 0,
			LTGRAY_BRUSH = 1,
			GRAY_BRUSH = 2,
			DKGRAY_BRUSH = 3,
			BLACK_BRUSH = 4,
			/// <summary>Null brush (equivalent to HOLLOW_BRUSH).</summary>
			NULL_BRUSH = 5,
			WHITE_PEN = 6,
			BLACK_PEN = 7,
			NULL_PEN = 8,
			OEM_FIXED_FONT = 10,
			ANSI_FIXED_FONT = 11,
			ANSI_VAR_FONT = 12,
			SYSTEM_FONT = 13,
			DEVICE_DEFAULT_FONT = 14,
			DEFAULT_PALETTE = 15,
			SYSTEM_FIXED_FONT = 16,
		}

		/// <summary>The GetStockObject function retrieves a handle to one of the stock pens, brushes, fonts, or palettes.</summary>
		/// <param name="i">The type of stock object</param>
		/// <returns>If the function succeeds, the return value is a handle to the requested logical object.</returns>
		[DllImport("gdi32.dll")]
		public static extern IntPtr GetStockObject(StockObjects i);

		[DllImport("gdi32.dll")]
		public static extern UInt32 Rectangle(IntPtr hdc, Int32 nLeftRect, Int32 nTopRect, Int32 nRightRect, Int32 nBottomRect);

		[DllImport("gdi32.dll")]
		public static extern Boolean DeleteObject(IntPtr hObject);

		/// <summary>
		/// These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color.
		/// </summary>
		public enum RasterOperationCode : UInt32
		{
			/// <summary>Copies the source rectangle directly to the destination rectangle.</summary>
			SRCCOPY = 0x00CC0020,
			/// <summary>
			/// Includes any windows that are layered on top of your window in the resulting image.
			/// By default, the image only contains your window.
			/// Note that this generally cannot be used for printing device contexts.
			/// </summary>
			CAPTUREBLT = 0x40000000,
			/// <summary>Combines the colors of the source and destination rectangles by using the Boolean OR operator.</summary>
			SRCPAINT = 0x00EE0086,
			/// <summary>Combines the colors of the source and destination rectangles by using the Boolean AND operator.</summary>
			SRCAND = 0x008800C6,
			/// <summary>Combines the colors of the source and destination rectangles by using the Boolean XOR operator.</summary>
			SRCINVERT = 0x00660046,
			/// <summary>Combines the inverted colors of the destination rectangle with the colors of the source rectangle by using the Boolean AND operator.</summary>
			SRCERASE = 0x00440328,
			/// <summary>Copies the inverted source rectangle to the destination</summary>
			NOTSRCCOPY = 0x00330008,
			/// <summary>Combines the colors of the source and destination rectangles by using the Boolean OR operator and then inverts the resultant color.</summary>
			NOTSRCERASE = 0x001100A6,
			/// <summary>Prevents the bitmap from being mirrored</summary>
			NOMIRRORBITMAP = 0x80000000,
			/// <summary>Merges the colors of the source rectangle with the brush currently selected in hdcDest, by using the Boolean AND operator.</summary>
			MERGECOPY = 0x00C000CA,
			/// <summary>Merges the colors of the inverted source rectangle with the colors of the destination rectangle by using the Boolean OR operator.</summary>
			MERGEPAINT = 0x00BB0226,
			/// <summary>Copies the brush currently selected in hdcDest, into the destination bitmap</summary>
			PATCOPY = 0x00F00021,
			/// <summary>
			/// Combines the colors of the brush currently selected in hdcDest, with the colors of the inverted source rectangle by using the Boolean OR operator.
			/// The result of this operation is combined with the colors of the destination rectangle by using the Boolean OR operator.
			/// </summary>
			PATPAINT = 0x00FB0A09,
			/// <summary>Combines the colors of the brush currently selected in hdcDest, with the colors of the destination rectangle by using the Boolean XOR operator</summary>
			PATINVERT = 0x005A0049,
			/// <summary>Inverts the destination rectangle</summary>
			DSTINVERT = 0x00550009,
			/// <summary>
			/// Fills the destination rectangle using the color associated with index 0 in the physical palette.
			/// (This color is black for the default physical palette.)
			/// </summary>
			BLACKNESS = 0x00000042,
			/// <summary>
			/// Fills the destination rectangle using the color associated with index 1 in the physical palette.
			/// (This color is white for the default physical palette.)
			/// </summary>
			WHITENESS = 0x00FF0062,
		}

		/// <summary>The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context.</summary>
		/// <param name="hdc">A handle to the destination device context</param>
		/// <param name="x">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle</param>
		/// <param name="y">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle</param>
		/// <param name="cx">The width, in logical units, of the source and destination rectangles</param>
		/// <param name="cy">The height, in logical units, of the source and the destination rectangles</param>
		/// <param name="hdcSrc">A handle to the source device context</param>
		/// <param name="x1">The x-coordinate, in logical units, of the upper-left corner of the source rectangle</param>
		/// <param name="y1">The y-coordinate, in logical units, of the upper-left corner of the source rectangle</param>
		/// <param name="rop">
		/// These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color.
		/// </param>
		/// <returns>If the function succeeds, the return value is nonzero</returns>
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern Boolean BitBlt(IntPtr hdc, Int32 x, Int32 y, Int32 cx, Int32 cy, IntPtr hdcSrc, Int32 x1, Int32 y1, RasterOperationCode rop);

		/// <summary>
		/// The StretchBlt function copies a bitmap from a source rectangle into a destination rectangle, stretching or compressing the bitmap to fit the dimensions of the destination rectangle, if necessary.
		/// The system stretches or compresses the bitmap according to the stretching mode currently set in the destination device context.
		/// </summary>
		/// <remarks>
		/// StretchBlt stretches or compresses the source bitmap in memory and then copies the result to the destination rectangle.
		/// This bitmap can be either a compatible bitmap (DDB) or the output from CreateDIBSection.
		/// The color data for pattern or destination pixels is merged after the stretching or compression occurs.
		/// </remarks>
		/// <param name="hdcDest">A handle to the destination device context</param>
		/// <param name="xDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle</param>
		/// <param name="yDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle</param>
		/// <param name="wDest">The width, in logical units, of the destination rectangle</param>
		/// <param name="hDest">The height, in logical units, of the destination rectangle</param>
		/// <param name="hdcSrc">A handle to the source device context</param>
		/// <param name="xSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle</param>
		/// <param name="ySrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle</param>
		/// <param name="wSrc">The width, in logical units, of the source rectangle</param>
		/// <param name="hSrc">The height, in logical units, of the source rectangle</param>
		/// <param name="rop">The raster operation to be performed</param>
		/// <returns>If the function succeeds, the return value is nonzero</returns>
		[DllImport("gdi32.dll",SetLastError = true)]
		public static extern Boolean StretchBlt(IntPtr hdcDest, Int32 xDest, Int32 yDest, Int32 wDest, Int32 hDest, IntPtr hdcSrc, Int32 xSrc, Int32 ySrc, Int32 wSrc, Int32 hSrc, RasterOperationCode rop);
	}
}