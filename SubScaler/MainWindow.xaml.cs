/*
 * SubScaler Pro - Native Subtitle Rescaler & Encoder
 * License: BSD 3-Clause License
 * Copyright (c) 2026 NIIcK. All rights reserved.
 * License URL: https://opensource.org/license/bsd-3-clause
 */

using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SubScaler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Registering code pages for legacy encodings support (1250, 1251, etc.)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();
        }

        private void Log(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => Log(message));
                return;
            }
            TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            TxtLog.ScrollToEnd();
        }

        private void Theme_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (ComboTheme == null || !this.IsInitialized) return;

            bool isLight = ComboTheme.SelectedIndex == 1;

            Color backColor = isLight ? Color.FromRgb(240, 240, 240) : Color.FromRgb(18, 18, 18);
            Color foreColor = isLight ? Colors.Black : Colors.White;
            Color inputBack = isLight ? Colors.White : Color.FromRgb(45, 45, 45);
            Color inputFore = isLight ? Colors.Black : Colors.White;
            Color ctrlBack = isLight ? Color.FromRgb(225, 225, 225) : Color.FromRgb(200, 200, 200);

            this.Resources["AppBack"] = new SolidColorBrush(backColor);
            this.Resources["AppFore"] = new SolidColorBrush(foreColor);
            this.Resources["InputBack"] = new SolidColorBrush(inputBack);
            this.Resources["InputFore"] = new SolidColorBrush(inputFore);
            this.Resources["ControlBack"] = new SolidColorBrush(ctrlBack);
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            TxtLog.Clear();
            Log("Engine initialized. Starting subtitle processing...");

            string srcPath = TxtSource.Text.Trim();
            string bkpDir = TxtBackup.Text.Trim();
            string sourceFpsInput = TxtSubFPS.Text.Replace(',', '.');
            string targetFpsInput = TxtVideoFPS.Text.Replace(',', '.');
            bool isEncodingOverrideEnabled = ChkChangeEncoding.IsChecked ?? false;

            // Fixed: Added null-coalescing to prevent null warnings
            string inExtension = (ComboInExt.SelectedItem as ComboBoxItem)?.Content?.ToString()?.ToLower() ?? ".sub";
            string outExtension = (ComboOutExt.SelectedItem as ComboBoxItem)?.Content?.ToString()?.ToLower() ?? ".srt";
            string inEncLabel = (ComboInEnc.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "UTF-8 (65001)";
            string outEncLabel = (ComboOutEnc.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "UTF-8 (65001)";

            if (string.IsNullOrEmpty(srcPath) || !Directory.Exists(srcPath))
            {
                Log("[ERROR] Invalid source folder path.");
                return;
            }

            BtnStart.IsEnabled = false;
            try
            {
                double sourceFps = double.Parse(sourceFpsInput, CultureInfo.InvariantCulture);
                double targetFps = double.Parse(targetFpsInput, CultureInfo.InvariantCulture);
                double scalingFactor = sourceFps / targetFps; // Frame-perfect scaling ratio

                Encoding encodingIn = isEncodingOverrideEnabled ? MapMpcEncoding(inEncLabel) : Encoding.UTF8;
                Encoding encodingOut = isEncodingOverrideEnabled ? MapMpcEncoding(outEncLabel) : Encoding.UTF8;

                string fullBackupPath = Path.IsPathRooted(bkpDir) ? bkpDir : Path.Combine(srcPath, bkpDir);
                if (!Directory.Exists(fullBackupPath)) Directory.CreateDirectory(fullBackupPath);

                var filesToProcess = Directory.GetFiles(srcPath, "*" + inExtension);
                Log($"Found {filesToProcess.Length} files to convert.");

                foreach (var file in filesToProcess)
                {
                    string nameOnly = Path.GetFileName(file);
                    string outputFilePath = Path.Combine(srcPath, Path.GetFileNameWithoutExtension(file) + outExtension);

                    Log($"Task: {nameOnly} rescaled to {targetFps} FPS");

                    File.Copy(file, Path.Combine(fullBackupPath, nameOnly), true);
                    string rawContent = await Task.Run(() => File.ReadAllText(file, encodingIn));

                    string processedContent = await Task.Run(() => {
                        bool isSourceFrameBased = (inExtension == ".sub" || inExtension == ".txt");
                        bool isTargetFrameBased = (outExtension == ".sub" || outExtension == ".txt");

                        if (isSourceFrameBased && outExtension == ".srt")
                            return ConvertFrameToTime(rawContent, sourceFps, targetFps);

                        if (inExtension == ".srt" && isTargetFrameBased)
                            return ConvertTimeToFrame(rawContent, sourceFps, targetFps);

                        if (isSourceFrameBased && isTargetFrameBased)
                            return RescaleFrames(rawContent, sourceFps, targetFps);

                        return RescaleSrtTimes(rawContent, scalingFactor);
                    });

                    await Task.Run(() => File.WriteAllText(outputFilePath, processedContent, encodingOut));
                    if (!string.Equals(file, outputFilePath, StringComparison.OrdinalIgnoreCase)) File.Delete(file);
                    Log("    Status: [COMPLETED]");
                }
            }
            catch (Exception ex) { Log($"[CRITICAL ERROR]: {ex.Message}"); }
            finally { BtnStart.IsEnabled = true; Log("\n--- BATCH PROCESSING FINISHED ---"); }
        }

        // Fixed: Added nullable string support and internal null check to resolve warnings
        private Encoding MapMpcEncoding(string? label)
        {
            if (string.IsNullOrEmpty(label)) return Encoding.UTF8;

            if (label.Contains("1250")) return Encoding.GetEncoding(1250);
            if (label.Contains("1251")) return Encoding.GetEncoding(1251);
            if (label.Contains("1252")) return Encoding.GetEncoding(1252);
            if (label.Contains("1253")) return Encoding.GetEncoding(1253);
            if (label.Contains("1254")) return Encoding.GetEncoding(1254);
            if (label.Contains("1255")) return Encoding.GetEncoding(1255);
            if (label.Contains("1256")) return Encoding.GetEncoding(1256);
            if (label.Contains("1257")) return Encoding.GetEncoding(1257);
            if (label.Contains("1258")) return Encoding.GetEncoding(1258);
            if (label.Contains("65001")) return Encoding.UTF8;
            return Encoding.Default;
        }

        private string ConvertFrameToTime(string content, double inFps, double outFps)
        {
            StringBuilder result = new StringBuilder();
            int counter = 1;
            string clean = Regex.Replace(content, @"^\{1\}\{1\}(\d+\.?\d*)[\r\n]*", "");
            var matches = Regex.Matches(clean, @"\{(\d+)\}\{(\d+)\}(.*)");
            double factor = inFps / outFps;

            foreach (Match m in matches)
            {
                double frameStart = double.Parse(m.Groups[1].Value);
                double frameEnd = double.Parse(m.Groups[2].Value);
                string text = m.Groups[3].Value.Replace("|", Environment.NewLine).Trim();

                double secondsStart = (frameStart / inFps) * factor;
                double secondsEnd = (frameEnd / inFps) * factor;

                result.AppendLine((counter++).ToString());
                result.AppendLine($"{FormatSrtTimestamp(secondsStart)} --> {FormatSrtTimestamp(secondsEnd)}");
                result.AppendLine(text);
                result.AppendLine();
            }
            return result.ToString();
        }

        private string ConvertTimeToFrame(string content, double inFps, double outFps)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine($"{{1}}{{1}}{outFps.ToString("F3", CultureInfo.InvariantCulture)}");
            double factor = inFps / outFps;
            var blocks = Regex.Split(content, @"\r?\n\r?\n");

            foreach (var block in blocks)
            {
                var m = Regex.Match(block, @"(\d{2}):(\d{2}):(\d{2}),(\d{3}) --> (\d{2}):(\d{2}):(\d{2}),(\d{3})[\r\n]+(.*)", RegexOptions.Singleline);
                if (m.Success)
                {
                    TimeSpan t1 = new TimeSpan(0, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
                    TimeSpan t2 = new TimeSpan(0, int.Parse(m.Groups[5].Value), int.Parse(m.Groups[6].Value), int.Parse(m.Groups[7].Value), int.Parse(m.Groups[8].Value));
                    string text = m.Groups[9].Value.Replace("\r", "").Replace("\n", "|").TrimEnd('|');

                    long fStart = (long)Math.Round(t1.TotalSeconds * outFps * factor);
                    long fEnd = (long)Math.Round(t2.TotalSeconds * outFps * factor);
                    result.AppendLine($"{{{fStart}}}{{{fEnd}}}{text}");
                }
            }
            return result.ToString();
        }

        private string RescaleFrames(string content, double inFps, double outFps)
        {
            double factor = inFps / outFps;
            string headersAdjusted = Regex.Replace(content, @"^\{1\}\{1\}(\d+\.?\d*)", $"{{1}}{{1}}{outFps.ToString("F3", CultureInfo.InvariantCulture)}");
            return Regex.Replace(headersAdjusted, @"\{(\d+)\}\{(\d+)\}", m => {
                if (m.Groups[1].Value == "1" && m.Groups[2].Value == "1") return m.Value;
                long f1 = (long)Math.Round(double.Parse(m.Groups[1].Value) * factor);
                long f2 = (long)Math.Round(double.Parse(m.Groups[2].Value) * factor);
                return $"{{{f1}}}{{{f2}}}";
            });
        }

        private string RescaleSrtTimes(string content, double factor)
        {
            return Regex.Replace(content, @"(\d{2}):(\d{2}):(\d{2}),(\d{3})", m => {
                TimeSpan ts = new TimeSpan(0, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
                return FormatSrtTimestamp(ts.TotalSeconds * factor);
            });
        }

        private string FormatSrtTimestamp(double seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return $"{(int)Math.Floor(t.TotalHours):00}:{t.Minutes:00}:{t.Seconds:00},{t.Milliseconds:000}";
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e) { var d = new OpenFolderDialog(); if (d.ShowDialog() == true) TxtSource.Text = d.FolderName; }
        private void BrowseBackup_Click(object sender, RoutedEventArgs e) { var d = new OpenFolderDialog(); if (d.ShowDialog() == true) TxtBackup.Text = d.FolderName; }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string info = "SubScaler Pro v1.2\n" +
                          "Copyright (c) 2026 DraNick. All rights reserved.\n\n" +
                          "Distributed under BSD 3-Clause License\n" +
                          "License Details: https://opensource.org/license/bsd-3-clause\n" +
                          "Visit: https://selfnet.org";
            MessageBox.Show(info, "About SubScaler");
        }
    }
}