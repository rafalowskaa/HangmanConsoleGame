using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using WisielecConsole.Resources;

namespace WisielecConsole.Utilities
{
    public static class SoundManager
    {
        // Słownik do śledzenia aktywnych odtwarzaczy dźwięku
        private static readonly ConcurrentDictionary<string, IWavePlayer> _activeSounds = new ConcurrentDictionary<string, IWavePlayer>();

        public static void PlaySound(string soundFileName, string language)
        {
            Task.Run(() =>
            {
                try
                {
                    string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", soundFileName);
                    if (!File.Exists(soundFilePath))
                    {
                        // Logowanie błędu, jeśli plik nie istnieje
                        string message = LanguageResources.GetResource("SoundFileNotFound", language).Replace("{FileName}", soundFileName);
                        ExceptionHandler.LogError(message);
                        return;
                    }

                    var audioFile = new AudioFileReader(soundFilePath);
                    var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    // Dodanie odtwarzacza do słownika
                    _activeSounds.TryAdd(soundFileName, outputDevice);

                    // Monitorowanie zakończenia odtwarzania
                    outputDevice.PlaybackStopped += (sender, args) =>
                    {
                        outputDevice.Dispose();
                        audioFile.Dispose();
                        _activeSounds.TryRemove(soundFileName, out _);
                    };
                }
                catch (Exception ex)
                {
                    // Logowanie wyjątku z lokalizowanym komunikatem
                    string message = LanguageResources.GetResource("SoundPlayError", language)
                        .Replace("{FileName}", soundFileName)
                        .Replace("{Error}", ex.Message);
                    ExceptionHandler.LogError(message);
                }
            });
        }

        public static void StopSound(string soundFileName)
        {
            if (_activeSounds.TryGetValue(soundFileName, out IWavePlayer player))
            {
                player.Stop();
                player.Dispose();
                _activeSounds.TryRemove(soundFileName, out _);
            }
        }
    }
}
