# SubScaler Pro v1.3

**SubScaler Pro** is a lightweight, high-precision desktop utility designed to batch process subtitle files. It specializes in rescaling timestamps or frame numbers based on frame rate (FPS) differences and converting between various character encodings to ensure perfect compatibility with modern and legacy media players.

## 🚀 Features

* **Frame-Perfect Rescaling:** Uses high-precision scaling factors to sync subtitles perfectly between different video sources (e.g., 25 FPS to 23.976 FPS).
* **Multi-Format Support:** Full support for `.srt` (SubRip), `.sub` (MicroDVD), `.mdvd` and `.txt` formats for both input and output.
* **MPC-Style Encodings:** Includes a comprehensive list of character encodings identical to Media Player Classic (MPC), resolving "mojibake" or broken character issues in Central European, Cyrillic, Greek, and other scripts.
* **Intelligent Conversion:** Automatically handles conversions between time-based (SRT) and frame-based (SUB/TXT) formats.
* **Safety First:** Automatically creates backups of your original files before processing.
* **Modern UI:** Native Dark Mode interface by default for a comfortable user experience.
* **Single File Portable:** No installation required. Runs as a standalone executable.

## 🛠 Usage

1.  **Select Source:** Point to the folder containing your subtitle files.
2.  **Configure Backup:** Define a subfolder name where original files will be safely stored.
3.  **Set FPS:** Enter the source subtitle FPS and the target video FPS.
4.  **Encoding (Optional):** Check "Override character encoding" if you need to fix broken characters or convert to UTF-8.
5.  **Process:** Hit **START PROCESSING** and watch the real-time console log.

## 📋 Supported Encodings

Matches the Media Player Classic standard:
* Western (1252)
* Central European (1250)
* Cyrillic (1251)
* Greek (1253)
* Turkish (1254)
* Hebrew (1255)
* Arabic (1256)
* Baltic (1257)
* Vietnamese (1258)
* UTF-8 (65001)

## ⚖️ License

Distributed under the **BSD 3-Clause License**. See the [official license text](https://opensource.org/license/bsd-3-clause) for more details.

Copyright (c) 2026 **DraNick**. All rights reserved.

---
**Project Website:** [selfnet.org](https://selfnet.org)