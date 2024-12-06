using NAudio.Wave;

namespace WisielecConsole.Utilities
{
    public static class BackgroundMusicPlayer
    {
        private static IWavePlayer? _wavePlayer;
        private static AudioFileReader? _audioFileReader;
        private static LoopStream? _loopStream;

        public static void StartBackgroundMusic()
        {
            try
            {
                string soundFilePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Sounds", "background.mp3");
                if (!File.Exists(soundFilePath))
                {
                    Console.WriteLine("Plik muzyki w tle nie został znaleziony: " + soundFilePath);
                    return;
                }

                _audioFileReader = new AudioFileReader(soundFilePath);
                _loopStream = new LoopStream(_audioFileReader); // Tworzymy strumień zapętlający
                _wavePlayer = new WaveOutEvent();
                _wavePlayer.Init(_loopStream);
                _wavePlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nie można odtworzyć muzyki w tle: " + ex.Message);
            }
        }

        public static void StopBackgroundMusic()
        {
            if (_wavePlayer != null)
            {
                _wavePlayer.Stop();
                _wavePlayer.Dispose();
                _wavePlayer = null;
            }
            if (_loopStream != null)
            {
                _loopStream.Dispose();
                _loopStream = null;
            }
            if (_audioFileReader != null)
            {
                _audioFileReader.Dispose();
                _audioFileReader = null;
            }
        }
    }

    // Klasa pomocnicza do zapętlania strumienia audio
    public class LoopStream : WaveStream
    {
        private readonly WaveStream _sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            _sourceStream = sourceStream;
            EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

        public override long Length => _sourceStream.Length;

        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = _sourceStream.Read(
                    buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (_sourceStream.Position == 0 || !EnableLooping)
                    {
                        // Jeśli nie ma więcej danych do odczytu lub zapętlanie jest wyłączone, przerywamy
                        break;
                    }
                    // Resetujemy pozycję do początku, aby zapętlić
                    _sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
