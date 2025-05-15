using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using ReactiveUI;
using PacketRedirector.Models;
using PacketRedirector.Services;

namespace PacketRedirector.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly RedirectorService _redirectorService;
        private const string ConfigPath = "config.json";

        public MainWindowViewModel()
        {
            StartCommand = ReactiveCommand.Create(Start);
            StopCommand = ReactiveCommand.Create(Stop);

            AvailableProtocols = new ObservableCollection<string> { "UDP", "TCP" };

            _redirectorService = new RedirectorService();
            _redirectorService.OnPacketReceived += packet => LastPacket = packet;
            _redirectorService.OnPacketRateUpdate += rate => PacketRate = $"Packets/sec: {rate}";

            LoadLocales();
            LoadConfig();
        }

        // Language localization
        public ObservableCollection<LocalizationEntry> AvailableLanguages { get; } = new();
        private LocalizationEntry? _selectedLanguage;
        public LocalizationEntry? SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
                this.RaisePropertyChanged(nameof(Text));
                SaveConfig();
            }
        }
        public Dictionary<string, string> Text => SelectedLanguage?.Text ?? new();

        private void LoadLocales()
        {
            var folder = Path.Combine(AppContext.BaseDirectory, "Locales");
            if (!Directory.Exists(folder)) return;

            foreach (var file in Directory.GetFiles(folder, "*.json"))
            {
                try
                {
                    var content = File.ReadAllText(file);
                    var entry = JsonSerializer.Deserialize<LocalizationEntry>(content);
                    if (entry != null)
                        AvailableLanguages.Add(entry);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to load {file}: {ex.Message}");
                }
            }
        }

        private void LoadConfig()
        {
            if (!File.Exists(ConfigPath)) return;

            try
            {
                var config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(ConfigPath));
                if (config is null) return;

                ReceiveProtocol = config.ReceiveProtocol;
                ReceiveIP = config.ReceiveIP;
                ReceivePort = config.ReceivePort;
                SendProtocol = config.SendProtocol;
                SendIP = config.SendIP;
                SendPort = config.SendPort;

                SelectedLanguage = AvailableLanguages.FirstOrDefault(l => l.Lang == config.Language)
                                   ?? AvailableLanguages.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load config: {ex.Message}");
            }
        }

        private void SaveConfig()
        {
            try
            {
                var config = new AppConfig
                {
                    ReceiveProtocol = ReceiveProtocol,
                    ReceiveIP = ReceiveIP,
                    ReceivePort = ReceivePort,
                    SendProtocol = SendProtocol,
                    SendIP = SendIP,
                    SendPort = SendPort,
                    Language = SelectedLanguage?.Lang ?? "en"
                };

                File.WriteAllText(ConfigPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save config: {ex.Message}");
            }
        }

        public ObservableCollection<string> AvailableProtocols { get; }

        private string _receiveProtocol = "UDP";
        public string ReceiveProtocol
        {
            get => _receiveProtocol;
            set
            {
                this.RaiseAndSetIfChanged(ref _receiveProtocol, value);
                SaveConfig();
            }
        }

        private string _receiveIp = "127.0.0.1";
        public string ReceiveIP
        {
            get => _receiveIp;
            set
            {
                this.RaiseAndSetIfChanged(ref _receiveIp, value);
                SaveConfig();
            }
        }

        private int _receivePort = 39539;
        public int ReceivePort
        {
            get => _receivePort;
            set
            {
                this.RaiseAndSetIfChanged(ref _receivePort, value);
                SaveConfig();
            }
        }

        private string _sendProtocol = "UDP";
        public string SendProtocol
        {
            get => _sendProtocol;
            set
            {
                this.RaiseAndSetIfChanged(ref _sendProtocol, value);
                SaveConfig();
            }
        }

        private string _sendIp = "127.0.0.1";
        public string SendIP
        {
            get => _sendIp;
            set
            {
                this.RaiseAndSetIfChanged(ref _sendIp, value);
                SaveConfig();
            }
        }

        private int _sendPort = 39539;
        public int SendPort
        {
            get => _sendPort;
            set
            {
                this.RaiseAndSetIfChanged(ref _sendPort, value);
                SaveConfig();
            }
        }

        private string _packetRate = "Packets/sec: 0";
        public string PacketRate
        {
            get => _packetRate;
            set => this.RaiseAndSetIfChanged(ref _packetRate, value);
        }

        private string _lastPacket = string.Empty;
        public string LastPacket
        {
            get => _lastPacket;
            set => this.RaiseAndSetIfChanged(ref _lastPacket, value);
        }

        
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        private void Start()
        {
            var settings = new RedirectSettings
            {
                ReceiveProtocol = ReceiveProtocol,
                ReceiveIP = ReceiveIP,
                ReceivePort = ReceivePort,
                SendProtocol = SendProtocol,
                TargetIP = SendIP,
                TargetPort = SendPort
            };

            _redirectorService.Start(settings);
            PacketRate = "Listening...";
            SaveConfig();
            Logger.LogEvent("Start clicked");
        }

        private void Stop()
        {
            _redirectorService.Stop();
            PacketRate = "Stopped";
            SaveConfig();
            Logger.LogEvent("Stop clicked");
        }
    }
}
