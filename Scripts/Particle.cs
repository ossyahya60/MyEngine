﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FN_Engine
{
    public class Particle
    {
        public bool NoLifeTime = false;
        public Vector2 Position;
        public Vector2 Size = Vector2.One;
        public float LifeTime = 1f;
        public Color Color = Color.White;
        public Color FadeColor = Color.Black;
        public float Layer = 0f;
        public bool Expired = false;
        public bool ShrinkMode = true;
        public float Rotation = 0;
        public float RotationalVelocity = 0;

        //Specific for trail renderer
        public int Length;
        public int Height
        {
            set
            {
                height = (value >= 0) ? value : 0;
                OriginalSize.Y = height;
            }
            get
            {
                return height;
            }
        }

        public bool ConstantVelocity = false; //For Particle effect
        public Vector2 Direction;
        public float MinimumSize = 1f;
        public float Speed = 5f;
        public bool Accelerate = false;
        public float AccelerationMagnitude = 2f;

        private float LifeTimeCounter = 0;
        private Vector2 OriginalSize;
        private bool OneTime = false;
        private bool ForParticleEffect = false;
        private float AccelerationCounter = 0;
        private int height;

        public Particle(bool ForParticleEffect)
        {
            OriginalSize = Size;
            this.ForParticleEffect = ForParticleEffect;
            Length = 2;
            Height = 10;
        }

        public void Update(GameTime gameTime)
        {
            if(!OneTime)
            {
                if (ForParticleEffect)
                    OriginalSize = Size;
                else
                    OriginalSize.Y = height;
                OneTime = true;
            }

            if (!Expired)
            {
                if(ForParticleEffect)
                {
                    if (RotationalVelocity != 0)
                        Rotation += RotationalVelocity;

                    if (ConstantVelocity)
                        Position += Speed * (Vector2.Normalize(Direction));
                    else
                    {
                        AccelerationCounter = (Accelerate)? AccelerationCounter + AccelerationMagnitude: AccelerationCounter - AccelerationMagnitude;
                        Position += (Speed + AccelerationCounter) * Vector2.Normalize(Direction);
                    }
                }

                if (ShrinkMode)
                {
                    if(ForParticleEffect)
                        Size = ((LifeTime - LifeTimeCounter * MinimumSize) / LifeTime) * OriginalSize;
                    else
                        height = (int)(((LifeTime - LifeTimeCounter * MinimumSize) / LifeTime) * OriginalSize.Y);
                }

                if (!NoLifeTime)
                {
                    Color = Color.Lerp(Color, FadeColor, LifeTimeCounter / LifeTime);

                    if (LifeTimeCounter >= LifeTime)
                        Expired = true;
                    else
                        LifeTimeCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        public void Draw()
        {
            if(!Expired)
            {
                Rectangle HandyRectangle = Rectangle.Empty;
                HandyRectangle.X = (int)Position.X;
                HandyRectangle.Y = (int)Position.Y;
                HandyRectangle.Width = (int)Size.X;
                HandyRectangle.Height = (int)Size.Y;

                HitBoxDebuger.DrawRectangle(HandyRectangle, Color, Rotation, Layer);
            }
        }

        public void Draw(Texture2D texture)
        {
            if (!Expired)
            {
                Rectangle HandyRectangle = Rectangle.Empty;
                HandyRectangle.X = (int)Position.X;
                HandyRectangle.Y = (int)Position.Y;
                HandyRectangle.Width = (int)Size.X;
                HandyRectangle.Height = (int)Size.Y;

                HitBoxDebuger.DrawRectangle(HandyRectangle, Color, Rotation, texture, Layer, Vector2.Zero);
            }
        }

        ///For Trail Renderer, Segments not particles
        public void DrawSegment()
        {
            if (!Expired)
            {
                Rectangle HandyRectangle = Rectangle.Empty;
                HandyRectangle.X = (int)Position.X;
                HandyRectangle.Y = (int)Position.Y;
                HandyRectangle.Width = Length;
                HandyRectangle.Height = Height;

                HitBoxDebuger.DrawLine(HandyRectangle, Color, Rotation, Layer, Vector2.Zero);
            }
        }

        public Particle DeepCopy()
        {
            return this.MemberwiseClone() as Particle;
        }
    }
}
