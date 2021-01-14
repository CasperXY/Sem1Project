using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace Sem1Project
{
    class EnemySprite : Sprite
    {
        public bool hasCollided = false;
       public bool dead = false;
        public bool levelAdvance = false;
        SoundEffect advanceSound, deathSound;
        
        public EnemySprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation, SoundEffect newAdvanceSound, SoundEffect newDeathSound) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            advanceSound = newAdvanceSound;
            deathSound = newDeathSound;
            spriteOrigin = new Vector2(0.5f, 1f);
            spriteScale = new Vector2(2, 2);
            isColliding = true;
            drawCollision = false;
            flipped = true;


            animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(10, 5, 48, 50));





        }

        public void Update(GameTime gameTime, List<KiBlastSprite> blastSprites, PlayerSprite playerSprite)
        {





            if (spritePos.Y > 120) spriteScale = new Vector2(1.5f, 1.5f);



            foreach (KiBlastSprite blastSprite in blastSprites)
            {
                if (!dead && checkCollision(blastSprite))
                {
                    hasCollided = true;
                    spriteVelocity.Y = 0;
                    dead = true;
                    deathSound.Play();
                }
                    


                if (dead && spritePos.Y < 200f) 
                {
                    levelAdvance = true;
                    advanceSound.Play();
                }
            }

        }
    }
}
