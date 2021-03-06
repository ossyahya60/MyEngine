﻿using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace FN_Engine
{
    public class KeyFrame
    {
        public bool PlayAfterFinishing = false;
        public bool FinishedButIsLooping = false;
        public bool DeleteAfterFinishing = false;
        public bool ReverseAfterFinishing = false;
        public bool Finished = true;
        public bool IsLooping = false;
        public string Tag = "Default";

        private float SourceValue = 0;
        private float DestinationValue = 1;
        private float TimeDuration = 1;
        private float TimeCounter = 0;
        private bool Paused = false;
        private Color OriginalColor;
        private bool JustStarted = true;
        private float OriginalSourceValue;
        private float OriginalDestinationValue;

        public KeyFrame(float sourceValue, float destinationValue, float timeDuration, string tag)
        {
            Tag = tag;
            SourceValue = sourceValue;
            DestinationValue = destinationValue;
            TimeDuration = (timeDuration < 0) ? 0 : timeDuration;

            OriginalSourceValue = SourceValue;
            OriginalDestinationValue = DestinationValue;
        }

        public void Update(GameTime gameTime)
        {
            if (!Finished)
            {
                if(!Paused)
                    TimeCounter = MathCompanion.Clamp(TimeCounter + (float)gameTime.ElapsedGameTime.TotalSeconds, 0, TimeDuration);

                if (TimeCounter >= TimeDuration)
                {
                    FinishedButIsLooping = false;
                    Finished = true;

                    if (ReverseAfterFinishing)
                        ReverseKeyFrame();

                    if (PlayAfterFinishing)
                    {
                        Play();
                        PlayAfterFinishing = false;
                    }

                    if (IsLooping)
                    {
                        FinishedButIsLooping = true;
                        Play();
                    }
                }
            }
            else
                if(ReverseAfterFinishing)
                    ReverseKeyFrame();
        }           

        public void Play()
        {
            TimeCounter = 0;
            Finished = false;
            Paused = false;
            JustStarted = true;
        }

        public void GetFeedback(ref int Value)
        {
            if(!Finished)
                Value = (int)((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(ref float Value)
        {
            if (!Finished)
                Value = (TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue;
        }

        public void GetFeedback(ref Vector2 Value)
        {
            if (!Finished)
                Value = Vector2.One * ((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(ref Color Value)
        {
            if (JustStarted)
            {
                OriginalColor = Value;
                JustStarted = false;
            }

            if (!Finished)
                Value = OriginalColor * ((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(Action<float> Value)
        {
            if (!Finished)
                Value((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(Action<Vector2> Value)
        {
            if (!Finished)
                Value(Vector2.One * ((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue));
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        private void ReverseKeyFrame()
        {
            float TempConatiner = SourceValue;
            SourceValue = DestinationValue;
            DestinationValue = TempConatiner;
            ReverseAfterFinishing = false;
            //TimeCounter = TimeDuration - TimeCounter;
        }

        public void ReverseNow()
        {
            float TempConatiner = SourceValue;
            SourceValue = DestinationValue;
            DestinationValue = TempConatiner;
            TimeCounter = TimeDuration - TimeCounter;
        }

        public void ReverseNow(bool PlayIfFinished)
        {
            float TempConatiner = SourceValue;
            SourceValue = DestinationValue;
            DestinationValue = TempConatiner;
            TimeCounter = TimeDuration - TimeCounter;

            if (Finished)
                Play();
        }

        public void Reset()
        {
            SourceValue = OriginalSourceValue;
            DestinationValue = OriginalDestinationValue;
            Finished = true;
        }

        public bool IsReversed()
        {
            return OriginalSourceValue == DestinationValue && OriginalDestinationValue == SourceValue;
        }

        public void Serialize(StreamWriter SW)
        {
            SW.WriteLine(ToString());

            SW.Write("PlayAfterFinishing:\t" + PlayAfterFinishing.ToString() + "\n");
            SW.Write("FinishedButIsLooping:\t" + FinishedButIsLooping.ToString() + "\n");
            SW.Write("DeleteAfterFinishing:\t" + DeleteAfterFinishing.ToString() + "\n");
            SW.Write("ReverseAfterFinishing:\t" + ReverseAfterFinishing.ToString() + "\n");
            SW.Write("IsLooping:\t" + IsLooping.ToString() + "\n");
            SW.Write("Tag:\t" + Tag + "\n");
            SW.Write("SourceValue:\t" + SourceValue.ToString() + "\n");
            SW.Write("DestinationValue:\t" + DestinationValue.ToString() + "\n");
            SW.Write("TimeDuration:\t" + TimeDuration.ToString() + "\n");
            SW.Write("Paused:\t" + Paused.ToString() + "\n");
            SW.Write("OriginalColor:\t" + OriginalColor.R.ToString() + "\t" + OriginalColor.G.ToString() + "\t" + OriginalColor.B.ToString() + "\t" + OriginalColor.A.ToString() + "\n");
            //Original Source and Destination values are assigned the source and destination value in deserialization

            SW.WriteLine("End Of " + ToString());
        }
    }
}
