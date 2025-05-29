using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using NAudio.Wave;
using NAudio;

namespace RPG_game
{
    public class AudioManager
    {
        private static AudioManager instance;
        private Dictionary<string, string> musicTracks;
        private IWavePlayer musicPlayer;
        private AudioManager currentMusicReader;
        private string currentTrack;
        private bool isMusicEnabled = true;
        private float volume = 0.5f;

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioManager();
                }
                return instance;
            }
        }

        private AudioManager()
        {
            musicTracks = new Dictionary<string, string>();

            try
            {
                musicPlayer = new WaveOutEvent();
                InitializeAudio();
                Console.WriteLine("Аудио система успешно синхронизирована");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Ошибка при инициализации аудио: {ex.Message}");
                Console.WriteLine("Игра будет работать без звука");
            }
        }

        private void InitializeAudio()
        {
            string musicFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");

            if (!Directory.Exists(musicFolder))
            {
                Directory.CreateDirectory(musicFolder);
                Console.WriteLine($"Создана папка для музыки: {musicFolder}");
            }

            RegisterTrack("Деревня", musicFolder, new[] { "village.mp3", "village.wav" });
            RegisterTrack("Лес", musicFolder, new[] { "forest.mp3", "forest.wav" });
            RegisterTrack("Пещера", musicFolder, new[] { "cave.mp3", "cave.wav" });
            RegisterTrack("Подземелье", musicFolder, new[] { "dungeon.mp3", "dungeon.wav" });
            RegisterTrack("Битва", musicFolder, new[] { "battle.mp3", "battle.wav" });
            RegisterTrack("Победа", musicFolder, new[] { "victory.mp3", "victory.wav" });
            RegisterTrack("Поражение", musicFolder, new[] { "defeat.mp3", "defeat.wav" });
            RegisterTrack("Меню", musicFolder, new[] { "menu.mp3", "menu.wav" });

            var availableTracks = GetAvailableTracks();
            
            if (availableTracks.Count > 0)
            {
                Console.WriteLine($"Найдено {availableTracks.Count} аудиотреков");
            }
            else
            {
                Console.WriteLine("Аудиофайлы не найдены. Игра будет работать без звука.");
            }
        }

        private void RegisterTrack(string trackName, string folder, string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(folder, fileName);
                if (File.Exists(filePath))
                {
                    musicTracks[fileName] = filePath;
                    return;
                }
            }

            musicTracks[trackName] = Path.Combine(folder, fileNames[0]);
        }

        private List<string> GetAvailableTracks()
        {
            var availableTracks = new List<string>();

            foreach (var track in musicTracks)
            {
                if (File.Exists(track.Value))
                {
                    availableTracks.Add(track.Key);
                }
            }

            return availableTracks;
        }

        public void PlayMusic(string nameTrack)
        {
            if (!isMusicEnabled) {  return; }

            if (currentTrack == nameTrack && musicPlayer?.PlaybackState == PlaybackState.Playing) { return; } // ?. - musicPlayer = null (условие не проверяется) 

            StopMusic();

            if (!musicTracks.ContainsKey(nameTrack)) { return; }

            try
            {
                string filePath = musicTracks[nameTrack];

                if (!File.Exists(filePath))
                {
                    DisplayMusicStatus($"{nameTrack}", Console.ForegroundColor = ConsoleColor.DarkGray);
                    currentTrack = nameTrack;
                    return;
                }

                currentMusicReader = new AudioFileReader(filePath);
                currentMusicReader.Volume = volume;

                musicPlayer.Init(currentMusicReader);
                musicPlayer.Play();
                currentTrack = nameTrack;

                musicPlayer.PlaybackStopped += OnPlaybackStopped;

                DisplayMusicStatus($"{nameTrack}", ConsoleColor.Cyan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка воспроизведения: {ex.Message}");
                currentTrack = null;
            }
        }

        public void StopMusic()
        {
            try
            {
                musicPlayer.PlaybackStopped -= OnPlaybackStopped;

                if (musicPlayer?.PlaybackState == PlaybackState.Playing) { musicPlayer.Stop(); }

                if (currentMusicReader != null)
                {
                    currentMusicReader.Dispose();
                    currentMusicReader = null;
                }

                currentTrack = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при остановке музыки: {ex.Message}");
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (currentMusicReader != null && currentTrack != null)
            {
                try
                {
                    currentMusicReader.Position = 0;
                    musicPlayer.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при зацикливании: {ex.Message}");
                }
            }     
        }

        private void DisplayMusicStatus(string message, ConsoleColor color)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            ConsoleColor consoleColor = Console.ForegroundColor;

            try
            {
                Console.SetCursorPosition(Console.WindowWidth - message.Length - 2, 0);
                Console.ForegroundColor = color;
                Console.Write(message);
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = consoleColor;
            }
            catch ( Exception ex )
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ForegroundColor = consoleColor;
            }
        }

        public void PlaySoundEffect(string effectName)
        {
            if (!isMusicEnabled) { return; }
            if (!musicTracks.ContainsKey(effectName)) { return; }

            try
            {
                string filePath = musicTracks[effectName];
                if (!File.Exists(filePath))
                {
                    DisplaySoundEffect($"{effectName}", ConsoleColor.DarkYellow);
                    return;
                }

                var effectReader = new AudioFileReader(filePath);
                var effectPlayer = new WaveOutEvent();

                effectReader.Volume = volume;
                effectPlayer.Init(effectReader);
                effectPlayer.Play();

                effectPlayer.PlaybackStopped += (sender, args) =>
                {
                    effectReader.Dispose();
                    effectPlayer.Dispose();
                };

                DisplaySoundEffect($"{effectName}", ConsoleColor.Yellow);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка воспроизвдения эффекта: {ex.Message}");
            }
        }

        private void DisplaySoundEffect(string message, ConsoleColor color)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = consoleColor;
        }

        public void SetMusicEnabled(bool enabled)
        {
            isMusicEnabled = enabled;

            if (!enabled)
            {
                StopMusic();
                DisplayMusicStatus("Музыка отключена", ConsoleColor.Gray);
            }
            else
            {
                DisplayMusicStatus("Музыка включена", ConsoleColor.Green);
            }
        }

        public void SetVolume(float newVolume)
        {
            volume = Math.Clamp(newVolume, 0.0f, 1.0f);

            if (currentMusicReader != null)
            {
                currentMusicReader.Volume = volume;
            }

            Console.WriteLine($"Громкость установлена:{(int)(volume * 100)}%");
        }

        public float GetVolume()
        {
            return volume;  
        }

        public bool IsMusicEnabled()
        {
            return isMusicEnabled;
        }

        public string GetCurrentTrack()
        {
            return currentTrack;
        }

        public void ShowAudioStatus()
        {
            Console.WriteLine("\n=== Статус аудио системы ===");
            Console.WriteLine($"Состояние: {(isMusicEnabled ? "Включено" : "Отключено")}");
            Console.WriteLine($"Громкость: {(int)(volume * 100)}%");
            Console.WriteLine($"Текущий трек: {currentTrack ?? "Нет"}");

            var availableTracks = GetAvailableTracks();
            Console.WriteLine($"Доступно треков: {availableTracks.Count}/{musicTracks.Count}");

            if (availableTracks.Count > 0)
            {
                Console.WriteLine("Найденные треки: ");
                foreach (var track in availableTracks)
                {
                    Console.WriteLine($"- {track}");
                }
            }

            if (availableTracks.Count < musicTracks.Count)
            {
                Console.WriteLine("Отсутствующие треки: ");
                foreach (var track in musicTracks)
                {
                    if (!File.Exists(track.Value))
                    {
                        Console.WriteLine($"{track.Key} - ({Path.GetFileName(track.Value)})");
                    }
                }

                Console.WriteLine($"\nПоместить в папку: (для себя)");
                Console.WriteLine($"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music")}");
            }
        }

        public void Dispose()
        {
            StopMusic();

            if (musicPlayer != null)
            {
                musicPlayer.Dispose();
                musicPlayer = null;
            }
        }
    }
}
