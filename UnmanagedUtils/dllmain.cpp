#include <windows.h>

#define KEYEVENTF_KEYDOWN 0;

INPUT in[2] = { 0 };

bool run = true;
bool runMouse = true;
bool rightRun = true;
BYTE vk_key;

extern "C" __declspec(dllexport) void InitKeys(const unsigned short key) {
    vk_key = key;
}

extern "C" __declspec(dllexport) void PushDown(int delay = 1) {
    run = true;
    while (run) {
        keybd_event(vk_key, 0, 0, 0);
        Sleep(delay);
        keybd_event(vk_key, 0, KEYEVENTF_KEYUP, 0);
    }
}

extern "C" __declspec(dllexport) void ClickLeftDown(int delay = 1) {
    runMouse = true;
    while (runMouse) {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        Sleep(delay);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}

extern "C" __declspec(dllexport) void ClickLeftUp() {
    runMouse = false;
}

extern "C" __declspec(dllexport) void ClickRightDown(int delay = 1) {
    rightRun = true;
    while (rightRun) {
        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        Sleep(delay);
        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
    }
}

extern "C" __declspec(dllexport) void ClickRightUp() {
    rightRun = false;
}

extern "C" __declspec(dllexport) void PushUp() {
    run = false;
}

extern "C" __declspec(dllexport) void ShowMessage(void) { // Testing
    MessageBoxA(nullptr, "Ahojky", "SUS", MB_OK);
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }

    return TRUE;
}

