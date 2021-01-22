using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Sem1Project
{
    class PlatformSpriteRotate : Sprite
    {
        public PlatformSpriteRotate(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 0f);//Controlling the starting point of the image being used 
            isColliding = true; //Setting collision to true
            //controlling the animations for rotated platform/setting up the image/animation to be drawn
            animations = new List<List<Rectangle>>();
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 100));
        }
    }
}
