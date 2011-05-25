using System;
using Un4seen.Bass;

namespace Torshify.Server
{
    public class BassPlayer
    {
        private BASSBuffer _basbuffer = null;
        private STREAMPROC _streamproc = null;

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            int consumed = 0;
            if (_basbuffer == null)
            {
                Bass.BASS_Init(-1, rate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                _basbuffer = new BASSBuffer(0.5f, rate, channels, 2);
                _streamproc = new STREAMPROC(Reader);
                Bass.BASS_ChannelPlay(
                    Bass.BASS_StreamCreate(rate, channels, BASSFlag.BASS_DEFAULT, _streamproc, IntPtr.Zero),
                    false
                    );
            }

            if (_basbuffer.Space(0) > samples.Length)
            {
                _basbuffer.Write(samples, samples.Length);
                consumed = frames;
            }

            return consumed;
        }

        private int Reader(int handle, IntPtr buffer, int length, IntPtr user)
        {
            return _basbuffer.Read(buffer, length, user.ToInt32());
        }

        public void Stop()
        {
            // In real world usage you must remember to free the BASS stream if not reusing it!
            _basbuffer.Clear();
        }

        public float Volume
        {
            get
            {
                return Bass.BASS_GetVolume();
            }
            set
            {
                Bass.BASS_SetVolume(value);
            }
        }
    }
}
