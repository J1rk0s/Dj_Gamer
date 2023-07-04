namespace Dj_Gamer; 

[System.Serializable]
public class KeyStruct {
    public int MidiKey { get; init; }
    public VirtualKeys Key { get; set; }
    public int Delay { get; set; }
    public string Description { get; set; } = "";
}