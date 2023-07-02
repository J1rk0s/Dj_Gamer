using System.ComponentModel;
using System.Windows;

namespace Dj_Gamer; 

public partial class Settings : Window {
    public Settings() {
        this.InitializeComponent();
        this.MouseSensitivity.Text = KeyHandler.GetMouseSensitivity().ToString();
    }

    private void CloseSettings(object? sender, CancelEventArgs e) {
        if (string.IsNullOrWhiteSpace(this.MouseSensitivity.Text)) {
            e.Cancel = true;
            return;
        }

        KeyHandler.SetMouseSensitivity(int.Parse(this.MouseSensitivity.Text));
    }
}