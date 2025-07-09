#include <windows.h>
#include <mmdeviceapi.h>
#include <endpointvolume.h>
#include <atomic>

typedef void(__stdcall* VolumeChangedCallback)(float newVolume);

static VolumeChangedCallback g_callback = nullptr;

static IMMDeviceEnumerator* g_deviceEnumerator = nullptr;
static IAudioEndpointVolume* g_endpointVolume = nullptr;

class VolumeCallback : public IAudioEndpointVolumeCallback {
  LONG _refCount = 1;

public:
  VolumeCallback() {
  }

  // IUnknown
  ULONG STDMETHODCALLTYPE AddRef() override {
    return InterlockedIncrement(&_refCount);
  }

  ULONG STDMETHODCALLTYPE Release() override {
    ULONG ulRef = InterlockedDecrement(&_refCount);
    if (0 == ulRef)
      delete this;
    return ulRef;
  }

  HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, VOID** ppvInterface) override {
    if (IID_IUnknown == riid) {
      AddRef();
      *ppvInterface = (IUnknown*) this;
    } else if (__uuidof(IAudioEndpointVolumeCallback) == riid) {
      AddRef();
      *ppvInterface = (IAudioEndpointVolumeCallback*) this;
    } else {
      *ppvInterface = nullptr;
      return E_NOINTERFACE;
    }
    return S_OK;
  }

  // IAudioEndpointVolumeCallback
  HRESULT STDMETHODCALLTYPE OnNotify(PAUDIO_VOLUME_NOTIFICATION_DATA pNotify) override {
    if (g_callback) {
      g_callback(pNotify->fMasterVolume);
    }
    return S_OK;
  }
};

static VolumeCallback* g_volumeCallback = nullptr;

// Exports

extern "C" __declspec(dllexport) HRESULT __stdcall InitializeAudioMonitor() {
  HRESULT hr = CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
  if (FAILED(hr)) return hr;

  hr = CoCreateInstance(
    __uuidof(MMDeviceEnumerator), nullptr,
    CLSCTX_ALL, __uuidof(IMMDeviceEnumerator),
    (void**) &g_deviceEnumerator);

  if (FAILED(hr)) return hr;

  IMMDevice* pDevice = nullptr;
  hr = g_deviceEnumerator->GetDefaultAudioEndpoint(eRender, eConsole, &pDevice);
  if (FAILED(hr)) return hr;

  hr = pDevice->Activate(__uuidof(IAudioEndpointVolume),
    CLSCTX_ALL, nullptr, (void**) &g_endpointVolume);

  pDevice->Release();
  if (FAILED(hr)) return hr;

  g_volumeCallback = new VolumeCallback();
  hr = g_endpointVolume->RegisterControlChangeNotify(g_volumeCallback);

  return hr;
}

extern "C" __declspec(dllexport) void __stdcall SetVolumeChangedCallback(VolumeChangedCallback cb) {
  g_callback = cb;
}

extern "C" __declspec(dllexport) void __stdcall CleanupAudioMonitor() {
  if (g_endpointVolume && g_volumeCallback) {
    g_endpointVolume->UnregisterControlChangeNotify(g_volumeCallback);
  }
  if (g_volumeCallback) {
    g_volumeCallback->Release();
    g_volumeCallback = nullptr;
  }
  if (g_endpointVolume) {
    g_endpointVolume->Release();
    g_endpointVolume = nullptr;
  }
  if (g_deviceEnumerator) {
    g_deviceEnumerator->Release();
    g_deviceEnumerator = nullptr;
  }

  CoUninitialize();
}

// Set master volume level
extern "C" __declspec(dllexport) HRESULT __stdcall SetMasterVolume(float level) {
  if (!g_endpointVolume) return E_FAIL;
  return g_endpointVolume->SetMasterVolumeLevelScalar(level, nullptr);
}

// Get master volume level
extern "C" __declspec(dllexport) HRESULT __stdcall GetMasterVolume(float* outLevel) {
  if (!g_endpointVolume || !outLevel) return E_FAIL;
  return g_endpointVolume->GetMasterVolumeLevelScalar(outLevel);
}

// Set mute state (1 = mute, 0 = unmute)
extern "C" __declspec(dllexport) HRESULT __stdcall SetMute(BOOL mute) {
  if (!g_endpointVolume) return E_FAIL;
  return g_endpointVolume->SetMute(mute, nullptr);
}

// Get mute state (returns 1 if muted, 0 if not)
extern "C" __declspec(dllexport) HRESULT __stdcall GetMute(BOOL* outMute) {
  if (!g_endpointVolume || !outMute) return E_FAIL;
  return g_endpointVolume->GetMute(outMute);
}
