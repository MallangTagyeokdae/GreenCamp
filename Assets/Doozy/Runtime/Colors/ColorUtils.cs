// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//https://stackoverflow.com/questions/6615002/given-an-rgb-value-how-do-i-create-a-tint-or-shade
//https://en.wikipedia.org/wiki/HSL_and_HSV
//https://github.com/edelstone/tints-and-shades
//https://medium.com/@donatbalipapp/colours-maths-90346fb5abda
//https://rip94550.wordpress.com/2009/02/26/color-hsb-and-tint-tone-shade/
//https://en.wikipedia.org/wiki/Color_space
//https://www.easyrgb.com/en/math.php
//https://www.cs.rit.edu/~ncs/color/t_convert.html
//https://www.programmingalgorithms.com/algorithm/rgb-to-hsv/

using System;
using Doozy.Runtime.Colors.Models;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Doozy.Runtime.Colors
{
	/// <summary>
	/// Methods used to convert and modify color classes
	/// </summary>
#pragma warning disable 0219
	public static class ColorUtils
	{
		#region Hue

		/// <summary> Get the hue value from r,g,b values </summary>
		/// <param name="r"> Red </param>
		/// <param name="g"> Green </param>
		/// <param name="b"> Blue </param>
		/// <param name="factorize"> If TRUE it returns a value between 0 and 360, otherwise it returns a value between 0 and 1 </param>
		/// <returns> Hue value either factorized [0, 360] or not [0,1]</returns>
		public static float Hue(float r, float g, float b, bool factorize = false)
		{
			if (r == g && g == b) return 0;
			float hue = 0;
			if (r >= g && g >= b) hue = 60 * ((g - b) / (r - b));         //(A) If R ≥ G ≥ B  |  H = 60° x [(G-B)/(R-B)]
			else if (g > r && r >= b) hue = 60 * (2 - (r - b) / (g - b)); //(B) If G > R ≥ B  |  H = 60° x [2 - (R-B)/(G-B)]
			else if (g >= b && b > r) hue = 60 * (2 + (b - r) / (g - r)); //(C) If G ≥ B > R  |  H = 60° x [2 + (B-R)/(G-R)]
			else if (b > g && g > r) hue = 60 * (4 - (g - r) / (b - r));  //(D) If B > G > R  |  H = 60° x [4 - (G-R)/(B-R)]
			else if (b > r && r >= g) hue = 60 * (4 + (r - g) / (b - g)); //(E) If B > R ≥ G  |  H = 60° x [4 + (R-G)/(B-G)]
			else if (r >= b && b > g) hue = 60 * (6 - (b - g) / (r - g)); //(F) If R ≥ B > G  |  H = 60° x [6 - (B-G)/(R-G)]

			if (factorize) Mathf.RoundToInt(hue);
			return (float) Math.Round(hue / 360, 2);

			// if (factorize) return hue;                              
			// return hue / 360;                                       
		}

		/// <summary> Get the hue value from RGB </summary>
		/// <param name="target"> Target RGB </param>
		/// <param name="factorize"> If TRUE it returns a value between 0 and 360, otherwise it returns a value between 0 and 1 </param>
		/// <returns> Hue value either factorized [0, 360] or not [0,1]</returns>
		public static float RGBtoHUE(RGB target, bool factorize = false) =>
			Hue(target.r, target.g, target.b, factorize);

		/// <summary> Get RGB from hue </summary>
		/// <param name="hue"> HUE value </param>
		/// <returns> A new RGB </returns>
		public static RGB HUEtoRGB(float hue)
		{
			float R = Mathf.Abs(hue * 6 - 3) - 1;
			float G = 2 - Mathf.Abs(hue * 6 - 2);
			float B = 2 - Mathf.Abs(hue * 6 - 4);
			return new RGB(R, G, B);
		}

		#endregion

		#region Color

		/// <summary> Convert RGB to Color </summary>
		/// <param name="rgb"> RGB value </param>
		/// <returns> A new Color </returns>
		public static Color RGBtoCOLOR(RGB rgb) =>
			new Color(rgb.r, rgb.g, rgb.g);

		/// <summary> Convert HSL to Color </summary>
		/// <param name="hsl"> HSL value </param>
		/// <returns> A new Color </returns>
		public static Color HSLtoCOLOR(HSL hsl) =>
			RGBtoCOLOR(hsl.ToRGB());

		/// <summary> Convert HSV to Color </summary>
		/// <param name="hsv"> HSV value </param>
		/// <returns> A new Color </returns>
		public static Color HSVtoCOLOR(HSV hsv) =>
			RGBtoCOLOR(hsv.ToRGB());

		#endregion

		#region HSL

		/// <summary> Convert r,g,b values to HSL </summary>
		/// <param name="r"> Red </param>
		/// <param name="g"> Green </param>
		/// <param name="b"> Blue </param>
		/// <returns> A new HSL </returns>
		public static HSL RGBtoHSL(float r, float g, float b)
		{
			//http://www.rapidtables.com/convert/color/rgb-to-hsl.htm
			float Cmax = Mathf.Max(r, g, b);
			float Cmin = Mathf.Min(r, g, b);
			float delta = Cmax - Cmin;
			float H = 0;
			float S = 0;
			float L = (Cmax + Cmin) / 2;
			if (delta == 0) return new HSL(H, S, L).Validate();
			H = Hue(r, g, b);
			S = L < 0.5f
				    ? delta / (Cmax + Cmin)
				    : delta / (2 - Cmax - Cmin);
			return new HSL(H, S, L).Validate();
		}

		/// <summary> Convert RGB to HSL </summary>
		/// <param name="rgb"> RGB value </param>
		/// <returns> A new HSL </returns>
		public static HSL RGBtoHSL(RGB rgb) =>
			RGBtoHSL(rgb.r, rgb.g, rgb.b);

		/// <summary> Convert Color to HSL </summary>
		/// <param name="color"> Color value </param>
		/// <returns> A new HSL </returns>
		public static HSL COLORtoHSL(Color color) =>
			RGBtoHSL(color.r, color.g, color.b);

		#endregion

		#region HSV / HSB

		/// <summary> Convert r,g,b values to HSV </summary>
		/// <param name="r"> Red </param>
		/// <param name="g"> Green </param>
		/// <param name="b"> Blue </param>
		/// <returns> A new HSV </returns>
		public static HSV RGBtoHSV(float r, float g, float b)
		{
			//http://www.rapidtables.com/convert/color/rgb-to-hsv.htm //http://www.easyrgb.com/en/math.php#text20
			float Cmax = Mathf.Max(r, g, b);
			float Cmin = Mathf.Min(r, g, b);
			float delta = Cmax - Cmin;
			float H = 0;
			float S = 0;
			float V = Cmax;
			if (delta == 0) return new HSV(H, S, V).Validate();
			H = Hue(r, g, b);
			S = delta / Cmax;
			return new HSV(H, S, V).Validate();
		}

		/// <summary> Convert RGB to HSV </summary>
		/// <param name="value"> RGB value </param>
		/// <returns> A new HSV </returns>
		public static HSV RGBtoHSV(RGB value) =>
			RGBtoHSV(value.r, value.g, value.g);

		/// <summary> Convert Color to HSV </summary>
		/// <param name="color"> Color value </param>
		/// <returns> A new HSV </returns>
		public static HSV COLORtoHSV(Color color) =>
			RGBtoHSV(color.r, color.g, color.b);

		#endregion

		#region RGB

		/// <summary> Convert Color to RGB </summary>
		/// <param name="color"> Color value </param>
		/// <returns> A new RGB </returns>
		public static RGB COLORtoRGB(Color color) =>
			new RGB(color.r, color.g, color.b);

		/// <summary> Convert HSL to RGB </summary>
		/// <param name="value"> HSL value </param>
		/// <returns> A new RGB </returns>
		public static RGB HSLtoRGB(HSL value)
		{
			//http://www.rapidtables.com/convert/color/hsl-to-rgb.htm
			HSL hsl = new HSL(value.h, value.s, value.l).Validate();
			float H = hsl.Factorize().x;
			float S = hsl.s;
			float L = hsl.l;
			float C = (1 - Mathf.Abs(2 * L - 1)) * S;
			float X = C * (1 - Mathf.Abs(H / 60 % 2 - 1));
			float m = L - C / 2;
			float r = 0, g = 0, b = 0;
			if (0 <= H && H < 60)
			{
				r = C;
				g = X;
				b = 0;
			}
			else if (60 <= H && H < 120)
			{
				r = X;
				g = C;
				b = 0;
			}
			else if (120 <= H && H < 180)
			{
				r = 0;
				g = C;
				b = X;
			}
			else if (180 <= H && H < 240)
			{
				r = 0;
				g = X;
				b = C;
			}
			else if (240 <= H && H < 300)
			{
				r = X;
				g = 0;
				b = C;
			}
			else if (300 <= H && H < 360)
			{
				r = C;
				g = 0;
				b = X;
			}

			return new RGB(r + m, g + m, b + m).Validate();
		}

		/// <summary> Convert HSV to RGB </summary>
		/// <param name="value"> HSV value </param>
		/// <returns> A new RGB </returns>
		public static RGB HSVtoRGB(HSV value) //http://www.rapidtables.com/convert/color/hsv-to-rgb.htm
		{
			var hsv = new HSV(value.h, value.s, value.v);

			float H = hsv.Factorize().x;
			float S = hsv.s;
			float V = hsv.v;

			float C = V * S;
			float X = C * (1 - Mathf.Abs(H / 60 % 2 - 1));
			float m = V - C;

			float r = 0, g = 0, b = 0;

			if (0 <= H && H < 60)
			{
				r = C;
				g = X;
				b = 0;
			}
			else if (60 <= H && H < 120)
			{
				r = X;
				g = C;
				b = 0;
			}
			else if (120 <= H && H < 180)
			{
				r = 0;
				g = C;
				b = X;
			}
			else if (180 <= H && H < 240)
			{
				r = 0;
				g = X;
				b = C;
			}
			else if (240 <= H && H < 300)
			{
				r = X;
				g = 0;
				b = C;
			}
			else if (300 <= H && H < 360)
			{
				r = C;
				g = 0;
				b = X;
			}

			return new RGB(r + m, g + m, b + m);
		}

		#endregion
	}
#pragma warning restore 0219
}