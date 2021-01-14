using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Sem1Project
{
    class KiBlastSprite : Sprite
    {
        public bool dead = false;
        public bool hasCollided = false;

        public KiBlastSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation, Vector2 newVelocity) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 1f);
            spriteScale = new Vector2(1, 1);
            isColliding = true;
            drawCollision = false;
            spriteVelocity = newVelocity;

            collisionInsetMin = new Vector2(0.25f, 0.3f);
            collisionInsetMax = new Vector2(0.25f, 0.03f);

            frameTime = 0.23f;
            

            animations = new List<List<Rectangle>>();


            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(30, 128, 32, 32));
            animations[0].Add(new Rectangle(320, 228, 32, 32));
            animations[0].Add(new Rectangle(64, 128, 32, 32));



        }

        public void Update(GameTime gameTime, List<EnemySprite>enemySprites, PlayerSprite playerSprite)
        {

            spritePos += spriteVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;


            

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
