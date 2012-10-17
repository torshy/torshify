using System;

namespace Torshify
{
    public class MusicDeliveryEventArgs : EventArgs
    {
        public MusicDeliveryEventArgs(int channels, int rate, byte[] samples, int frames)
        {
            Channels = channels;
            Rate = rate;
            Samples = samples;
            Frames = frames;
            ConsumedFrames = 0;
        }

        public int Channels
        {
            get; 
            private set;
        }

        public int Rate
        {
            get; 
            private set;
        }

        public byte[] Samples
        {
            get; 
            private set;
        }

        public int Frames
        {
            get; 
            private set;
        }

        public int ConsumedFrames
        {
            get; 
            set;
        }
    }
}