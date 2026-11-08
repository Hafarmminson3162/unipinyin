# UniPinyin - User Guide & Documentation

**UniPinyin** is a lightweight, high-performance Chinese Pinyin Input Method Editor (IME) for Windows, crafted around the core design philosophy of Vietnam's famous **UniKey**: **ultra-lightweight, blazing fast, intuitive, and distraction-free**.

UniPinyin allows you to type Chinese characters (Simplified & Traditional) and toned Pinyin using a standard English US keyboard on any Windows application (Word, Excel, Chrome, Notepad, Discord, WeChat, Telegram, games, etc.).

---

## 🌟 Key Highlights

1. **Ultra-Lightweight & Zero-Bloat (~240 KB with full high-res icon)**:
   - No background services, telemetry, or intrusive ads (unlike 150+ MB commercial Chinese IMEs like Sogou, Baidu, or QQ Pinyin).
   - Written in native C# targeting Microsoft .NET Framework 4.0/4.8 (pre-installed on 100% of Windows 10/11 PCs). Zero external runtimes or dependencies needed.
   - Memory footprint is typically under 15 MB of RAM.

2. **Dual-Engine Suggestions**:
   - **Google Input Tools API (`zh-t-i0-pinyin`)**: Delivers smart, context-aware sentence and phrase predictions directly from Google Cloud over encrypted HTTPS with non-blocking debounce caching.
   - **Built-in Offline Dictionary**: Contains thousands of essential phrases and all 400+ standard Mandarin syllables. Even without an active internet connection, suggestions appear instantly.

3. **TELEX-Style Pinyin Tone Input**:
   - Type Pinyin tone accents seamlessly with ergonomic keys (`x`, `s`, `r`, `f`), and type `ü` with `w` or `u+w`.
   - Press **`Enter`** to output clean, accented Pinyin text directly into your document (e.g. `nǐhǎo`, `lǜ`, `hànzì`), or press **`Space`** to commit Chinese Hanzi characters (`你好`, `绿`, `汉字`).

4. **Dedicated English & Bilingual Editions**:
   - **`unipinyin-en.exe`**: 100% standalone English edition with an all-English interface.
   - **`UniPinyin.exe`**: Bilingual edition with one-click toggling between English and Vietnamese.
   - **`UniPinyin_Web.html` & `UniPinyin_Web_EN.html`**: Browser-based Web IME editions for instant typing anywhere.

5. **Panda Application Icon**:
   - Both executables feature an adorable panda holding bamboo as their native Windows application icon.

---

## 📂 Project Structure

```text
UniPinyin/
│
├── UniPinyin.exe           # Primary executable (Bilingual VI/EN, ~246 KB)
├── unipinyin-en.exe        # Standalone English executable (100% EN, ~238 KB)
├── app.ico                 # Multi-resolution Windows application icon (16x16 to 256x256)
├── app_icon.png            # High-resolution PNG logo
├── UniPinyin_Web.html      # Browser Web IME (Bilingual)
├── UniPinyin_Web_EN.html   # Browser Web IME (English edition)
├── build.bat               # 1-click batch script to rebuild UniPinyin.exe
├── build-en.bat            # 1-click batch script to rebuild unipinyin-en.exe
├── USER_GUIDE.md           # Complete English user manual (this document)
├── HUONG_DAN_SU_DUNG.md    # Vietnamese user manual
├── LICENSE                 # MIT License
├── .gitignore              # Git ignore rules
│
├── PinyinToneHelper.cs     # TELEX tone rules (x,s,r,f), umlaut ü (w/uw), undo & smart continuation
├── CandidateManager.cs     # Composition buffer, pagination & Google/offline dictionary merge
├── GooglePinyinService.cs  # Google Input Tools API client (HTTPS, JSON)
├── LocalDictionary.cs      # Offline dictionary & Simplified/Traditional converter
├── FloatingCandidateBar.cs # Non-stealing topmost candidate bar (Caret & mouse tracking)
├── KeyboardHookManager.cs  # System-wide low-level keyboard hook (WH_KEYBOARD_LL)
├── NativeMethods.cs        # Win32 API interop (SendInput, GetCaretPos, GetGUIThreadInfo)
├── Settings.cs             # Configuration persistence (config.ini) & Windows auto-start
├── SettingsForm.cs         # Control Panel window
├── Program.cs              # Application entrypoint & Single-Instance Mutex guard
│
└── src-en/                 # Source code for standalone 100% English edition (unipinyin-en.exe)
```

---

## 🚀 Quick Start Guide

### 1. Launching UniPinyin
- Double-click **`unipinyin-en.exe`** (or `UniPinyin.exe`).
- A status icon appears in the **Windows System Tray** (notification area at the bottom-right of your taskbar):
  - **`[中]` (Apple System Blue)**: Chinese IME is **ACTIVE** (clean transparent glyph, no box).
  - **`[E]` (Apple Silver / White)**: English Mode is **ACTIVE** (Direct US keyboard pass-through, no box).

### 2. Switching Between Chinese and English
- **Default Hotkey**: Press **`Alt + Z`**.
- **Mouse**: Single **left-click** on the system tray icon (just like clicking the `V`/`E` icon in UniKey).
- **Audio Feedback**:
  - Plays a crisp, modern Samsung Galaxy notification chime when toggling between Chinese `[中]` and English `[E]`.
  - *Note: Audio chime can be toggled on/off anytime in the Control Panel.*

---

## ⌨️ Typing Guide

### 1. Typing Chinese Characters (Hanzi)
Open any document or text field (Google Chrome, Word, Notepad, WeChat, Discord):

1. **Type standard Pinyin**, e.g.: `nihao`:
   A sleek candidate bar floats right beneath your cursor:
   ```text
   Pinyin: nihao                           [1/3]
   1.你好   2.拟好   3.您好   4.妮好   5.尼豪
   ```
2. **Selecting Candidates**:
   - Press **`Spacebar`**: Commits candidate **#1** immediately (`你好`).
   - Press **`1` – `9`**: Commits the corresponding numbered candidate.
   - Press **`.`** (Period) or **`PageDown`**: Next page of candidates.
   - Press **`,`** (Comma) or **`PageUp`**: Previous page of candidates.
   - Press **`Backspace`**: Deletes the last Pinyin character.
   - Press **`Esc`**: Clears composition and closes the bar.

---

### 2. Typing Pinyin with Tone Accents (TELEX Mode)

UniPinyin supports typing Mandarin tone marks on Pinyin using familiar TELEX-style keys:

| Tone | Description | Key | Example Key Sequence | Output Pinyin |
| :--- | :--- | :---: | :--- | :--- |
| **Tone 1** | Flat / Macron (`¯`) | **`x`** | `lax` $\rightarrow$ `lā` <br> `max` $\rightarrow$ `mā` | **`lā`**, **`mā`** |
| **Tone 2** | Rising / Acute (`´`) | **`s`** | `mas` $\rightarrow$ `má` <br> `shis` $\rightarrow$ `shí` | **`má`**, **`shí`** |
| **Tone 3** | Caron / Hook (`ˇ`) | **`r`** | `nir` $\rightarrow$ `nǐ` <br> `haor` $\rightarrow$ `hǎo` | **`nǐ`**, **`hǎo`** |
| **Tone 4** | Falling / Grave (`` ` ``) | **`f`** | `haf` $\rightarrow$ `hà` <br> `zif` $\rightarrow$ `zì` | **`hà`**, **`zì`** |
| **Umlaut ü** | Umlaut (`ü`) | **`w`** or **`u+w`** | `lw` / `luw` $\rightarrow$ `lü` <br> `nw` / `nuw` $\rightarrow$ `nü` <br> `lwf` $\rightarrow$ `lǜ` | **`lü`**, **`nü`**, **`lǜ`** |

#### A. Multi-Syllable Toned Pinyin & Apostrophe `'` Separator
- **Continuous syllables**: Type syllables continuously with their tone marks:
  - `nirhaor` $\rightarrow$ **`nǐhǎo`**
  - `hanfzif` $\rightarrow$ **`hànzì`**
  - `lwf` or `luwf` $\rightarrow$ **`lǜ`**
  - `zhongxwenx` $\rightarrow$ **`zhōngwén`**
- **Apostrophe `'` Syllable Separator**: Use single quote **`'`** to explicitly separate ambiguous syllables and phrases:
  - `xi'an` $\rightarrow$ **`西安`** (prevents merging into `xian` $\rightarrow$ `先/线/现`)
  - `tian'an'men` $\rightarrow$ **`天安门`**
  - `wo'shi'zhong'guo'ren` $\rightarrow$ **`我是中国人`**
  - `wo'xiang'xue'zhong'wen` $\rightarrow$ **`我想学中文`**
  - Tone marks apply specifically to the syllable after the apostrophe: e.g. `xi'anx` $\rightarrow$ **`xi'ān`**.

#### B. The `w` and `ww` Rules
- Typing `w` by itself produces **`ü`**; typing `lw` or `nw` produces **`lü`**, **`nü`**; typing `u+w` produces **`ü`**.
- **Double `w` (`ww`) produces `w` (NOT `uw`)**:
  - `w` $\rightarrow$ `ü` $\rightarrow$ press `w` again $\rightarrow$ **`w`** (press `w` a third time $\rightarrow$ `ww`).
  - `lw` $\rightarrow$ `lü` $\rightarrow$ press `w` again $\rightarrow$ **`lw`**.
  - `uw` $\rightarrow$ `ü` $\rightarrow$ press `w` again $\rightarrow$ **`w`**.
- Starting an initial syllable with `w`: typing `w` followed by `o`, `a`, `e`, or `u` automatically converts to initial `w` (e.g. `w` + `o` $\rightarrow$ **`wo`**, `w` + `a` $\rightarrow$ **`wa`**, `w` + `e` $\rightarrow$ **`wen`**).

#### C. Outputting Pinyin vs. Hanzi
- **Press `Enter`**: Commits the **toned Pinyin text** directly into your application (e.g. typing `nirhaor` then pressing `Enter` inserts **`nǐhǎo`**).
- **Press `Spacebar`** or **`1-9`**: Commits the **Chinese Hanzi characters** (e.g. typing `nirhaor` then pressing `Spacebar` inserts **`你好`**).

#### D. Undo / Repeating Tone Keys
- Pressing the same tone key twice cancels the tone and preserves the original letter:
  - `lax` $\rightarrow$ `lā` $\rightarrow$ pressing `x` again $\rightarrow$ **`lax`**
  - `mas` $\rightarrow$ `má` $\rightarrow$ pressing `s` again $\rightarrow$ **`mas`**
  - `haf` $\rightarrow$ `hà` $\rightarrow$ pressing `f` again $\rightarrow$ **`haf`**

#### E. Smart Continuation & Offline Long-Sentence Composition
- **Smart Consonants**: If you type standard words containing `x`, `s`, `r`, or `f` as consonants (e.g. `xiexie`, `nishi`, `chifan`, `haode`), UniPinyin automatically recognizes subsequent characters and avoids unintended tone locks.
- **Offline Forward Maximum Matching (FMM)**:
  - Even without internet connection, the built-in dictionary engine segments and composes multi-word expressions and long sentences (up to 100 characters in buffer).
  - Works with separated syllables (`wo'shi'zhong'guo'ren`, `wo'shi'yue'nan'ren`) as well as unsegmented syllables (`woshizhongguoren`, `nihaoma`).

---

## ⚙️ Control Panel (Settings)

To open the Control Panel:
- **Double-click** the system tray icon.
- Or **Right-click** the tray icon $\rightarrow$ select **"Control Panel... [Settings]"**.

### Configurable Options

1. **Character Set**:
   - `Simplified Chinese (简体)`: Mainland China standard.
   - `Traditional Chinese (繁體)`: Taiwan, Hong Kong, and Macau standard.
2. **Toggle Hotkey**:
   - `Alt + Z` (Default)
   - `Ctrl + Shift` (Classic UniKey style)
   - `Ctrl + Space`
3. **Sound Alert**:
   - `Play sound beep on toggle`: Check to enable audible tone indicator when switching modes.
4. **Suggestion Source**:
   - `Google Cloud Suggestion + Offline Dict (Recommended)`: Best prediction accuracy.
   - `Offline Dictionary Only`: Operates 100% offline with zero network calls.
5. **Candidates Per Page**:
   - `5 candidates (Compact)`: Minimalistic on-screen bar.
   - `9 candidates (Full)`: Shows more choices per page.
6. **Candidate Bar Behavior**:
   - `Press Spacebar to select 1st candidate`: Checked by default.
   - `Candidate bar automatically follows caret/cursor`: Floats near current text input location.
7. **System Startup**:
   - `Start with Windows`: Launches UniPinyin minimized to system tray upon Windows login.
8. **Live Typing Test Box**:
   - Test typing phrases directly inside the settings window before switching to other apps.

---

## 🌐 Web Browser Edition

Need to type Chinese on a computer where you cannot run `.exe` files?
1. Open **`UniPinyin_Web_EN.html`** (or `UniPinyin_Web.html`) in any browser (Chrome, Edge, Firefox, Safari).
2. Type Pinyin or accented Pinyin in the online editor.
3. Use the **"Copy Text"** button to copy your characters to the clipboard.

---

## 🛠️ Building from Source

Both executables can be built in seconds on any Windows PC without installing Visual Studio or external tools:

1. Double-click **`build-en.bat`** to build `unipinyin-en.exe`.
2. Double-click **`build.bat`** to build `UniPinyin.exe`.

Both batch scripts invoke the standard .NET Framework C# compiler (`csc.exe`) located at:
`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`

---

## 📄 License & Credits
- **License**: MIT License. Free for personal, educational, and commercial use.
- **Engine**: Powered by Windows Win32 API and Google Input Tools Cloud API.
