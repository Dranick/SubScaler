# 🎬 SubScaler Pro v1.3

**SubScaler Pro** is a lightweight, high-performance desktop utility designed for precision subtitle synchronization and batch processing. It is built as a **Visual Studio project in C# using .NET 10 (LTS)**, ensuring long-term support and top-tier performance.

---

## 🚀 Key Features

| Feature | Description |
| :--- | :--- |
| **High-Precision Scaling** | Uses a core algorithm that calculates scaling factors dynamically for every timestamp, effectively **eliminating the cumulative error effect** (drift). |
| **Native Batch Processing** | Designed to handle entire folders at once. Performs encoding and conversion across all files in seconds. |
| **Modern Stack** | Developed in **C# / .NET 10 LTS**. It does **not** require `ffmpeg` or any other external binaries to function. |
| **Intelligent Formatting** | Automatically handles conversions between time-based (SRT) and frame-based (SUB/TXT) formats on the fly. |
| **MPC-Style Encodings** | Identical character encoding scripts to **Media Player Classic (MPC)**, fixing "mojibake" or broken characters. |
| **Portable (Single EXE)** | Self-contained executable with all dependencies included. No installation required. |

---

## 🛠 Usage

1. **Select Source:** Point to the folder containing your subtitle files.
2. **Configure Backup:** Define a subfolder name where original files will be safely stored.
3. **Set FPS:** Enter the source subtitle FPS and the target video FPS.
4. **Encoding (Optional):** Check "Override character encoding" if you need to fix broken characters.
5. **Process:** Hit **START PROCESSING** and watch the real-time log.

---

## 📋 Supported Encodings

Matches the standard Media Player Classic scripts:

* **Western** (1252)
* **Central European** (1250)
* **Cyrillic** (1251)
* **Greek** (1253)
* **Turkish** (1254)
* **Hebrew** (1255)
* **Arabic** (1256)
* **Baltic** (1257)
* **Vietnamese** (1258)
* **UTF-8** (65001)

---

## 🛡 Safety & Design

* **Automatic Backups:** Every file is backed up before modification.
* **Modern UI:** Native Dark and Light Mode support for a comfortable experience.
* **Development:** Built with **Visual Studio** using modern C# patterns.

---

## ⚖️ License

Distributed under the **BSD 3-Clause License**. See the [official license text](https://opensource.org/license/bsd-3-clause) for details.

Copyright (c) 2026 **DraNick**. All rights reserved.

---
**Project Website:** [selfnet.org](https://selfnet.org)
