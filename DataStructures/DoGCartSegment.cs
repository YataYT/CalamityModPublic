using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.DataStructures
{
    public class DoGCartSegment
    {
        public Vector2 Center;
        public float Rotation;
        public int OldDirection;
        public void Update(Player player, Vector2 aheadPosition, float idealRotation)
        {
            int direction = (player.velocity.SafeNormalize(Vector2.UnitX * player.direction).X > 0f).ToDirectionInt();
            if (player.velocity.X == 0f)
                direction = player.direction;
            if (OldDirection != direction)
            {
                // Flip the segments if the player flips their direction.
                if (OldDirection != 0)
                    Center = player.Center - Center + player.Center;

                OldDirection = direction;
            }

            Rotation = (aheadPosition - Center).ToRotation();
            Center = aheadPosition - (aheadPosition - Center).SafeNormalize(Vector2.Zero) * 24f;
        }
    }
}
