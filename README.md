# UniPinyin 🐼

> **A Lightweight, Fast & Distraction-Free Chinese Pinyin IME for Windows (UniKey Style)**

UniPinyin brings the simplicity and responsiveness of Vietnam's legendary **UniKey** to Chinese Pinyin input on Windows. Powered by Google Input Tools suggestions with a built-in offline dictionary fallback, UniPinyin operates seamlessly across all Windows applications without bloated backgrounds or telemetry.

[**English Documentation (USER_GUIDE.md)**](USER_GUIDE.md) | [**Hướng Dẫn Tiếng Việt (HUONG_DAN_SU_DUNG.md)**](HUONG_DAN_SU_DUNG.md)

---

## ✨ Features at a Glance

* **Ultra-Lightweight**: Only ~240 KB single binary, minimal RAM footprint (< 15 MB), zero installation required.
* **Apple Modern Design Language**:
  * **System Tray Icons**: Clean glyphs `[中]` (Apple System Blue) and `[E]` (Apple Silver) rendered directly with transparency — strictly **no bounding boxes or squares**.
  * **Floating Candidate Bar**: Sleek macOS Dark Mode glass capsule with rounded corners, `PINYIN` pill badge, and soft blue candidate pill indicator.
  * **Settings Control Panel**: macOS System Settings grouped-inset card layout with generous spacing and zero cramped or obscured controls.
* **Dual-Engine Suggestions**: Google Input Tools API Cloud predictions + Built-in offline dictionary fallback.
* **TELEX-Style Pinyin Tone Input & Smart Rules**:
  * `w` or `u+w` $\rightarrow$ `ü` (e.g. `lw` $\rightarrow$ `lü`, `nuw` $\rightarrow$ `nü`).
  * Double `w` (`ww`) $\rightarrow$ `w` (e.g. `ww` $\rightarrow$ `w`, `lww` $\rightarrow$ `lw`, `uww` $\rightarrow$ `w`).
  * Syllable Separator **`'`** (Apostrophe) $\rightarrow$ explicitly split ambiguous syllables and phrases (e.g. `xi'an` $\rightarrow$ `西安`, `tian'an'men` $\rightarrow$ `天安门`, `wo'shi'zhong'guo'ren` $\rightarrow$ `我是中国人`).
  * `x` $\rightarrow$ Tone 1 (`¯` - ā)
  * `s` $\rightarrow$ Tone 2 (`´` - á)
  * `r` $\rightarrow$ Tone 3 (`ˇ` - ǎ)
  * `f` $\rightarrow$ Tone 4 (``` ` ``` - à)
  * Press **`Enter`** to commit **toned Pinyin text** (e.g. `nǐhǎo`, `lǜ`, `hànzì`).
  * Press **`Space`** or **`1-9`** to commit **Chinese Hanzi** (`你好`, `绿`, `汉字`).
* **Offline Long-Sentence Composition**:
  * Intelligent Forward Maximum Matching (FMM) engine with multi-syllable segmentation.
  * Handles complete long sentences offline both with apostrophes (`wo'shi'zhong'guo'ren`) and continuous Pinyin (`woshizhongguoren`).
* **Ergonomic Switching**:
  * Default Hotkey: **`Alt + Z`** (or `Ctrl + Shift`, `Ctrl + Space`).
  * Tray icon 1-click toggle.
  * Audio chime alert: Modern Samsung Galaxy notification chime when toggling between Chinese and English (can be toggled on/off in Control Panel).
* **Simplified & Traditional Chinese**: Full support for both character sets.
* **Two Standalone Editions**:
  * `UniPinyin.exe`: Bilingual edition (English / Vietnamese).
  * `unipinyin-en.exe`: 100% English edition.
* **Browser Web IME**: `UniPinyin_Web.html` and `UniPinyin_Web_EN.html` for typing directly in any browser.

---

## 🚀 Quick Start

1. Download or clone this repository.
2. Run **`unipinyin-en.exe`** (or `UniPinyin.exe`).
3. Press **`Alt + Z`** to switch between Chinese `[中]` and English `[E]`.
4. Start typing Pinyin anywhere on Windows!

---

## 🛠️ Building from Source

Build directly on any Windows 10/11 PC without installing Visual Studio:

```cmd
# Build 100% English edition
build-en.bat

# Build Bilingual edition
build.bat
```

Uses the built-in Microsoft .NET Framework C# compiler (`csc.exe`).

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).
