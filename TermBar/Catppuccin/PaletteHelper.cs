using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TermBar.Catppuccin.Json;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace TermBar.Catppuccin {
  /// <summary>
  /// The Catppuccin color palette.
  /// </summary>
  internal static class PaletteHelper {
    private const string remotePaletteJsonFile = @"https://github.com/catppuccin/palette/raw/refs/heads/main/palette.json";
    private const string localPaletteJsonFile = @"Assets\Catppuccin\palette.json";

    private static HttpClient? httpClient;
    private static Palette? palette;

    /// <summary>
    /// The palette.
    /// </summary>
    internal static Palette Palette {
      get {
        bool paletteFileError = false;

        if (palette is null) {
          string _localPaletteJsonFile = $@"{Windows.ApplicationModel.Package.Current.InstalledLocation.Path}\{localPaletteJsonFile}";

          if (!File.Exists(_localPaletteJsonFile)) {
            httpClient ??= new();

            try {
              Directory.CreateDirectory(Path.GetDirectoryName(_localPaletteJsonFile)!);

              using (Task<Stream> httpStream = httpClient.GetStreamAsync(remotePaletteJsonFile)) {
                using (FileStream fileStream = new(_localPaletteJsonFile, FileMode.OpenOrCreate)) {
                  httpStream.Result.CopyTo(fileStream);
                }
              }
            } catch (AggregateException e) {
              foreach (Exception _e in e.InnerExceptions) {
                if (_e is HttpRequestException) {
                  PaletteFileError(remotePaletteJsonFile, _localPaletteJsonFile, (_e as HttpRequestException)!);
                  paletteFileError = true;
                }
              }
            } finally {
              if (paletteFileError) {
                File.Delete(_localPaletteJsonFile);
                Environment.Exit(1);
              }
            }
          }

          using (StreamReader reader = new(_localPaletteJsonFile)) {
            palette = JsonSerializer.Deserialize(reader.BaseStream, PaletteContext.Default.Palette)!;
          }
        }

        return palette;
      }
    }

    /// <summary>
    /// Displays a message indicating that the download failed.
    /// </summary>
    /// <param name="e">A <see cref="HttpRequestException"/>.</param>
    private static void PaletteFileError(string remotePaletteJsonFile, string localPaletteJsonFile, HttpRequestException e) {
      const string caption = "Catppuccin palette.json Download Failed";
      string text = $"Failed to download {remotePaletteJsonFile}:\r\n\r\n{e.Message}\r\n\r\nYou will need to download this file manually and place it at {localPaletteJsonFile}.";

      PInvoke.MessageBox(HWND.Null, text, caption, Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_ICONERROR);
    }
  }
}
