using XColor = Microsoft.Xna.Framework.Color;
using SColor = System.Drawing.Color;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AuroraMod {
	public static partial class AuroraUtils {
		public static XColor ColorFromPacked(uint packed) {
			return new((byte)packed, (byte)(packed >> 8), (byte)(packed >> 16), (byte)(packed >> 24));
		}

		public static SColor ToSystemColor(this XColor color) => SColor.FromArgb(color.A, color.R, color.G, color.B);
		public static XColor ToXnaColor(this SColor color) => new(color.R, color.G, color.B, color.A);

		public static float GetHue(this XColor c) => c.ToSystemColor().GetHue();
		public static float GetSaturation(this XColor c) => c.ToSystemColor().GetSaturation();
		public static float GetBrightness(this XColor c) => (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f;

		public static int GetColorDifference(this XColor c1, XColor c2) {
			return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) + (c1.G - c2.G) * (c1.G - c2.G) + (c1.B - c2.B) * (c1.B - c2.B));
		}
		public static int GetColorDifference(this XColor c1, SColor c2) {
			return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) + (c1.G - c2.G) * (c1.G - c2.G) + (c1.B - c2.B) * (c1.B - c2.B));
		}
		public static int GetColorDifference(this SColor c1, SColor c2) {
			return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) + (c1.G - c2.G) * (c1.G - c2.G) + (c1.B - c2.B) * (c1.B - c2.B));
		}
		public static int GetColorDifference(this SColor c1, XColor c2) {
			return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) + (c1.G - c2.G) * (c1.G - c2.G) + (c1.B - c2.B) * (c1.B - c2.B));
		}
		public static float ColorNum(this XColor c, float factorSat = 1f, float factorBri = 1f) {
			return c.GetSaturation() * factorSat + c.GetBrightness() * factorBri;
		}
		public static float ColorNum(this SColor c, float factorSat = 1f, float factorBri = 1f) {
			return c.GetSaturation() * factorSat + c.GetBrightness() * factorBri;
		}
		public static float GetHueDistance(float hue1, float hue2) {
			float d = Math.Abs(hue1 - hue2);
			return d > 180 ? 360 - d : d;
		}

		public static int ClosestColorByHue(this XColor target, List<XColor> colors) {
			var hue1 = target.GetHue();
			var diffs = colors.Select(n => GetHueDistance(n.GetHue(), hue1));
			var diffMin = diffs.Min(n => n);
			return diffs.ToList().FindIndex(n => n == diffMin);
		}
		public static int ClosestColorByRGB(this XColor target, List<XColor> colors) {
			var colorDiffs = colors.Select(n => n.GetColorDifference(target)).Min(n => n);
			return colors.FindIndex(n => n.GetColorDifference(target) == colorDiffs);
		}
		public static int ClosetsColorByWeight(List<XColor> colors, XColor target) {
			float hue1 = target.GetHue();
			var num1 = ColorNum(target);
			var diffs = colors.Select(n => Math.Abs(ColorNum(n) - num1) + GetHueDistance(n.GetHue(), hue1));
			var diffMin = diffs.Min(x => x);
			return diffs.ToList().FindIndex(n => n == diffMin);
		}
	}
}
