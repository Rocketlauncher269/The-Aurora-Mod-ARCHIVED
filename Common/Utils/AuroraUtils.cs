using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace AuroraMod 
{
	public static partial class AuroraUtils 
	{
        public static Vector2 Normalized(this Vector2 vector)
        {
            vector.Normalize();
            return vector;
        }
    }
}
