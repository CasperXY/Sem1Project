using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Sem1Project
{
    class KiBlastSprite : Sprite
    {
        public bool dead = false;//Setting the starting value of dead to false
        public bool hasCollided = false; //Setting the starting value of collision

        public KiBlastSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation, Vector2 newVelocity) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 1f);//Controlling the starting point of the image being used
            spriteScale = new Vector2(1, 1);//Controlling the size of the image
            isColliding = true; //Setting collision to true
            spriteVelocity = newVelocity; //Giving the velocity a variable
            //Setting Collision values
            collisionInsetMin = new Vector2(0.25f, 0.3f);
            collisionInsetMax = new Vector2(0.25f, 0.03f);

            //controlling the animations for the blast sprite/setting up the image/animation to be drawn
            animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(30, 128, 32, 32));
            animations[0].Add(new Rectangle(320, 228, 32, 32));
            animations[0].Add(new Rectangle(64, 128, 32, 32));
        }

        public void Update(GameTime gameTime, List<EnemySprite>enemySprites, PlayerSprite playerSprite)
        {
            spritePos += spriteVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds; //Movement of the blast
            //Decaliring the collision on an enemy and declairing the blast as dead to be removed
            foreach (EnemySprite enemySprite in enemySprites)
            {
                if (!enemySprite.dead && checkCollisionBelow(enemySprite))
                {
                    hasCollided = true;
                    spriteVelocity.Y = 0;
                    dead = true;
                }
                else if (!enemySprite.dead && checkCollisionAbove(enemySprite))
                {
                    hasCollided = true;
                    spriteVelocity.Y = 0;
                    dead = true;
                }

                if (!enemySprite.dead && checkCollisionLeft(enemySprite))
                {
                    hasCollided = true;
                    spriteVelocity.X = 0;
                    dead = true;
                }
                else if (!enemySprite.dead && checkCollisionRight(enemySprite))
                {
                    hasCollided = true;
                    spriteVelocity.X = 0;
                    dead = true;
                }
            }
        }
    }
}
