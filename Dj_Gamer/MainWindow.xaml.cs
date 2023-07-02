using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NAudio.Midi;
using System.Windows;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace Dj_Gamer; 

public partial class MainWindow : Window {
    private MidiIn? _midiIn;
    private volatile List<KeyStruct> _keys = new();
    private bool _isDebug = false;
    private bool _add = false;

    public MainWindow() {
        this.InitializeComponent();

        for (int i = 0; i < MidiIn.NumberOfDevices; i++) {
            MidiInCapabilities cap = MidiIn.DeviceInfo(i);
            this.Devices.Items.Add(cap.ProductName);
        }
        
        this.Inputs.ItemsSource = this._keys;
    }

    private void ConnectBtn(object sender, RoutedEventArgs e) {
        if (this.Devices.SelectedIndex == -1) {
            MessageBox.Show("You must select valid device!", "ERROR");
            return;
        }
        
        if(this._midiIn != null) return;

        this._midiIn = new MidiIn(this.Devices.SelectedIndex);
        this._midiIn.MessageReceived += this.OnMessageRecieve;
        this._midiIn.Start();
    }

    private void DisconnectBtn(object sender, RoutedEventArgs e) {
        if(this._midiIn == null) return;
        
        this._midiIn.Stop();
        this._midiIn.Dispose();
    }

    private void OnMessageRecieve(object? sender, MidiInMessageEventArgs args) {
        int message = args.MidiEvent.GetAsShortMessage();

        if (this._isDebug) {
            Console.WriteLine("Midi: " + message);
            return;
        }

        if (this._keys.Any(a => a.MidiKey == message)) { // TODO: Get all messages from dj controller and put them in enum or struct to enumerate over later
            KeyStruct keyN = this._keys.Find(i => i.MidiKey == message)!;
            KeyHandler.SendInput(keyN);
            return;
        }

        if (this._add) {
            KeyStruct keyS = new() {
                MidiKey = message,
                Key = VirtualKeys.VK_KEY_A,
            };
            
            this._keys.Add(keyS);
            this.Inputs.Dispatcher.Invoke(this.Inputs.Items.Refresh);
        }
    }

    private void SaveBtn(object sender, RoutedEventArgs e) {
        SaveFileDialog dialog = new() {
            Title = "Save preset",
            DefaultExt = ".json",
            AddExtension = true,
            Filter = "Json files|*.json",
        };

        if (dialog.ShowDialog() == true) {
            using (FileStream stream = (FileStream)dialog.OpenFile()) {
                JsonSerializer.Serialize(stream, this._keys);   
            }
        }
    }

    private void DebugBtn(object sender, RoutedEventArgs e) {
        this._isDebug = !this._isDebug;
        this.Debug.Content = "Is debug: " + this._isDebug;
    }

    private void LoadBtn(object sender, RoutedEventArgs e) {
        OpenFileDialog dialog = new() {
            Filter = "json files|*.json|everything|*.*",
            Title = "Select your preset",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
        };

        if (dialog.ShowDialog() == true) {
            string json = File.ReadAllText(dialog.FileName);
            this._keys = JsonSerializer.Deserialize<List<KeyStruct>>(json)!;
            Console.WriteLine(this._keys.Count);
            this.Inputs.ItemsSource = this._keys;
        }
    }

    private void AddBtn(object sender, RoutedEventArgs e) {
        if(this._add) return;
        
        this._add = true;
        MessageBox.Show("Click some keys");
    }

    private void StopBtn(object sender, RoutedEventArgs e) {
        if(!this._add) return;
        
        this._add = false;
        MessageBox.Show("Stopped recording!");
    }

    private void SettingsBtn(object sender, RoutedEventArgs e) {
        Settings settings = new();
        settings.ShowDialog();
    }
}