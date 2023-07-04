using System.ComponentModel;
using System.Windows;

namespace Dj_Gamer; 

public partial class Settings : Window {
    public Settings() {
        this.InitializeComponent();
        this.MouseSensitivity.Text = KeyHandler.MouseSensitivity.ToString();
        this.MouseDelay.Text = KeyHandler.MouseDelay.ToString();
    }

    private void CloseSettings(object? sender, CancelEventArgs e) {
        if (string.IsNullOrWhiteSpace(this.MouseSensitivity.Text) || string.IsNullOrWhiteSpace(this.MouseDelay.Text)) {
            e.Cancel = true;
            MessageBox.Show("You must provide valid values!", "DJ Gamer");
            return;
        }

        KeyHandler.MouseSensitivity = int.Parse(this.MouseSensitivity.Text);
        KeyHandler.MouseDelay = int.Parse(this.MouseDelay.Text);
    }
}