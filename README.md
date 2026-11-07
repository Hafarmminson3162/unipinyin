#Build by Antigravity
# UniPinyin - Bộ Gõ Tiếng Trung Pinyin Siêu Nhẹ Cho Windows (English below)

**UniPinyin** là phần mềm gõ tiếng Trung Pinyin từ bàn phím tiếng Anh (US Keyboard) dành cho hệ điều hành Windows, được thiết kế theo triết lý **siêu nhẹ, mượt mà và trực quan giống hệt UniKey** của Việt Nam.

---

## 🌟 Điểm nổi bật của UniPinyin

1. **Siêu nhẹ & Tiết kiệm tài nguyên (Dung lượng chỉ ~80-95 KB)**:
   - Không cài cắm dịch vụ ngầm, không quảng cáo, không hao pin/RAM (so với các bộ gõ nặng nề hàng trăm MB như Sogou, Baidu, QQ Pinyin).
   - Sử dụng trực tiếp Native Windows Win32 API và .NET Framework tích hợp sẵn trên mọi máy tính Windows 10/11. Không cần cài thêm bất kỳ runtime hay phần mềm phụ trợ nào.

2. **Lấy gợi ý từ Google Input Tools API**:
   - Sử dụng công nghệ nhận diện và đoán từ ngữ cảnh thông minh của Google (`zh-t-i0-pinyin`).
   - Tự động tích hợp cơ chế bộ đệm (Caching) và truy vấn bất đồng bộ (Asynchronous Query) giúp gõ phím tức thì, mượt mà, không giật lag.

3. **Từ điển Offline dự phòng (Offline Fallback)**:
   - Tích hợp sẵn hàng ngàn từ ghép, câu giao tiếp và 400+ âm tiết Pinyin chuẩn trong bộ nhớ.
   - Nếu mất mạng hoặc mạng chậm, bộ gõ vẫn tự động hiển thị gợi ý ngoại tuyến tức thì mà không bị gián đoạn.

4. **Gõ Pinyin có dấu thanh điệu chuẩn TELEX (Mới)**:
   - Hỗ trợ gõ Pinyin có dấu thanh trực tiếp từ bàn phím US:
     - Âm `ü`: gõ phím **`w`** hoặc **`u+w`** (hoặc `v`). Ví dụ: `lw` -> `lü`, `nuw` -> `nü`, `luw` -> `lü`.
     - Thanh 1 (ngang - ā): gõ phím **`x`** (VD: `lax` -> `lā`).
     - Thanh 2 (sắc - á): gõ phím **`s`** (VD: `mas` -> `má`).
     - Thanh 3 (hỏi/caron - ǎ): gõ phím **`r`** (VD: `nir` -> `nǐ`).
     - Thanh 4 (huyền/grave - à): gõ phím **`f`** (VD: `haf` -> `hà`).
   - **Xác nhận chuỗi Pinyin**: Nhấn phím **Enter** để xuất thẳng chuỗi Pinyin có dấu (VD: `nǐhǎo`, `lǜ`, `hànzì`).
   - **Chọn chữ Hán**: Nhấn phím **Space** hoặc phím số **1-9**.
   - **Hủy/hoàn tác dấu**: Gõ lặp lại phím dấu để hủy dấu và lấy chữ cái gốc (VD: `laxx` -> `lax`, `haff` -> `haf`).

5. **Tùy biến linh hoạt & Âm báo (Bảng điều khiển kiểu UniKey)**:
   - **Phím chuyển đổi mặc định**: **`Alt + Z`** (hoặc `Ctrl + Shift`, `Ctrl + Space`).
   - **Âm báo bíp khi chuyển đổi**: Tiếng bíp âm cao (1000 Hz) khi BẬT Tiếng Trung [中], tiếng bíp trầm (600 Hz) khi TẮT sang Tiếng Anh [EN]. Có tùy chọn Bật/Tắt âm báo trong Bảng điều khiển.
   - **Bảng mã**: Chữ Hán Giản thể (Simplified - 简体) hoặc Phồn thể (Traditional - 繁體).
   - **Thanh gợi ý**: Tự động bám theo con trỏ chuột/caret hoặc cố định góc màn hình.
   - **Số lượng từ gợi ý**: 5 từ hoặc 9 từ trên mỗi trang.
   - **Khởi động cùng Windows**: Tùy chọn tự chạy khi bật máy.

6. **Có sẵn 2 phiên bản độc lập**:
   - `UniPinyin.exe`: Phiên bản song ngữ (Việt - Anh, chuyển đổi 1-click).
   - `unipinyin-en.exe`: Phiên bản 100% tiếng Anh độc lập.

7. **Trình gõ thử nghiệm Web (`UniPinyin_Web.html` & `UniPinyin_Web_EN.html`)**:
   - Tích hợp trang Web gõ Pinyin có dấu và chữ Hán độc lập chạy trực tiếp trong trình duyệt.

---

## 📂 Cấu trúc thư mục dự án

```text
C:\Users\p1\.gemini\antigravity\scratch\UniPinyin\
│
├── UniPinyin.exe           # Bản chạy chính (Song ngữ Tiếng Việt & English, ~96 KB)
├── unipinyin-en.exe        # Bản chạy 100% Tiếng Anh độc lập (~87 KB)
├── UniPinyin_Web.html      # Trình gõ Pinyin trên nền trình duyệt web (Song ngữ)
├── UniPinyin_Web_EN.html   # Trình gõ Pinyin trên nền trình duyệt web (100% Tiếng Anh)
├── build.bat               # Script 1-click tự động biên dịch UniPinyin.exe
├── build-en.bat            # Script 1-click tự động biên dịch unipinyin-en.exe
├── HUONG_DAN_SU_DUNG.md    # Tài liệu hướng dẫn chi tiết
│
├── PinyinToneHelper.cs     # Xử lý quy tắc âm ü (w/uw), dấu thanh TELEX (x,s,r,f) & hoàn tác
├── CandidateManager.cs     # Quản lý buffer Pinyin, phân trang & kết hợp Google/Offline
├── GooglePinyinService.cs  # Dịch vụ kết nối Google Input Tools API qua HTTPS
├── LocalDictionary.cs      # Từ điển Pinyin cục bộ ngoại tuyến & chuyển đổi Giản/Phồn
├── FloatingCandidateBar.cs # Thanh gợi ý chữ nổi (không cướp focus con trỏ)
├── KeyboardHookManager.cs  # Bắt phím toàn hệ thống & phân loại phím bấm
├── NativeMethods.cs        # Win32 API Hook (WH_KEYBOARD_LL) & SendInput Unicode (64-bit)
├── Settings.cs             # Quản lý cấu hình & Registry tự khởi động
├── SettingsForm.cs         # Giao diện Bảng điều khiển (Control Panel)
├── Program.cs              # Điểm khởi đầu & cơ chế đơn phiên (Single-Instance Mutex)
│
└── src-en/                 # Mã nguồn độc lập 100% Tiếng Anh cho unipinyin-en.exe
    ├── CandidateManager.cs
    ├── FloatingCandidateBar.cs
    ├── GooglePinyinService.cs
    ├── KeyboardHookManager.cs
    ├── LocalDictionary.cs
    ├── NativeMethods.cs
    ├── PinyinToneHelper.cs
    ├── Program.cs
    ├── Settings.cs
    ├── SettingsForm.cs
    └── TrayContext.cs
```

---

## 🚀 Hướng dẫn sử dụng nhanh

### 1. Khởi động phần mềm
- Nhấp đúp vào file `UniPinyin.exe` (hoặc `unipinyin-en.exe`):
- Một biểu tượng nhỏ sẽ xuất hiện ở **Khay hệ thống (System Tray)** góc dưới bên phải màn hình:
  - `[中]` (Màu xanh dương): Đang bật chế độ gõ Tiếng Trung.
  - `[英]` hoặc `[EN]` (Màu xám): Đang ở chế độ gõ Tiếng Anh bình thường.

### 2. Thao tác gõ phím cơ bản
Mở bất kỳ ứng dụng nào (Word, Excel, Notepad, Chrome, Zalo, Discord, v.v.):

#### A. Gõ chữ Hán (Hanzi):
- Gõ Pinyin thông thường, ví dụ `nihao`.
- Thanh gợi ý xuất hiện:
  ```text
  Pinyin: nihao                           [1/3]
  1.你好   2.拟好   3.您好   4.妮好   5.尼豪
  ```
- Nhấn **Phím Cách (Space)**: Chọn từ số 1 (`你好`).
- Hoặc bấm các phím số **1, 2, 3, 4, 5** để chọn từ tương ứng.
- Nhấn phím dấu chấm `.` hoặc `PageDown`: Sang trang tiếp theo.
- Nhấn phím dấu phẩy `,` hoặc `PageUp`: Lùi về trang trước.
- Nhấn phím **Esc**: Hủy bỏ phiên soạn thảo.

#### B. Gõ Pinyin có dấu thanh (TELEX-style):
- **Âm `ü`**: Gõ bằng phím **`w`** hoặc **`u+w`** (hoặc `v`). Ví dụ: `lw` -> `lü`, `nuw` -> `nü`, `luw` -> `lü`.
- **Thanh 1 (¯)**: Gõ phím **`x`**. Ví dụ: `lax` -> `lā`.
- **Thanh 2 (´)**: Gõ phím **`s`**. Ví dụ: `mas` -> `má`.
- **Thanh 3 (ˇ)**: Gõ phím **`r`**. Ví dụ: `nir` -> `nǐ`.
- **Thanh 4 (`)**: Gõ phím **`f`**. Ví dụ: `haf` -> `hà`.
- **Gõ cụm Pinyin có dấu**:
  - `nirhaor` -> `nǐhǎo`
  - `hanfzif` -> `hànzì`
  - `lwf` -> `lǜ`
- **Xác nhận lấy chuỗi Pinyin có dấu**: Nhấn phím **Enter**! Phần mềm sẽ xuất thẳng chuỗi Pinyin có dấu vào văn bản.

### 3. Phím tắt Bật / Tắt Tiếng Trung
- **Mặc định**: Nhấn tổ hợp phím **`Alt + Z`**.
- Hoặc chỉ cần **nhấp chuột trái 1 lần** vào biểu tượng `[中]` / `[EN]` ở khay hệ thống.
- Khi chuyển đổi, phần mềm phát âm báo bíp (1000 Hz khi BẬT, 600 Hz khi TẮT) giúp nhận biết tức thì mà không cần nhìn xuống khay màn hình.

### 4. Mở Bảng điều khiển (Settings)
- **Nhấp đúp chuột trái** hoặc **nhấp chuột phải** vào icon khay hệ thống > Chọn **"Bảng điều khiển... [Mở cấu hình]"** (hoặc *"Control Panel... [Settings]"*).
- Tại đây bạn có thể thay đổi:
  - Ngôn ngữ giao diện: Tiếng Việt hoặc English.
  - Bảng mã Giản thể / Phồn thể.
  - Phím tắt chuyển đổi (`Alt+Z`, `Ctrl+Shift`, `Ctrl+Space`).
  - Bật / Tắt âm báo bíp khi chuyển đổi.
  - Đổi số lượng từ gợi ý (5 từ hoặc 9 từ).
  - Bật/Tắt tự khởi động cùng Windows.
  - Thử gõ trực tiếp trong ô kiểm tra.

---

## 🌐 Hướng dẫn sử dụng bản Web (Browser)
Nếu muốn gõ trực tuyến trong trình duyệt:
1. Nhấp đúp mở file `UniPinyin_Web.html` hoặc `UniPinyin_Web_EN.html`.
2. Gõ Pinyin có dấu hoặc chữ Hán vào ô soạn thảo.
3. Bấm **"Sao Chép Văn Bản"** để dán sang ứng dụng khác.

---

## 💡 Mẹo gõ Pinyin tiếng Trung hiệu quả
- **Gõ cụm từ dài**: Hãy gõ liền nguyên từ/cụm từ (ví dụ `zhongwen`, `xiexie`, `weishenme`, `duibuqi`), Google sẽ gợi ý chính xác hơn rất nhiều so với gõ từng chữ rời rạc.
- **Ký tự ü (dấu hai chấm)**: Gõ `w` hoặc `u+w` giống chữ `ư` trong UniKey (hoặc dùng `v`). Ví dụ: `lw` / `luw` -> `lü` (绿), `nw` / `nuw` -> `nü` (女).
- **Lấy chữ Pinyin có dấu**: Nhấn Enter sau khi gõ dấu bằng các phím `x, s, r, f`.

# UniPinyin - User Guide & Documentation

**UniPinyin** is a lightweight, high-performance Chinese Pinyin Input Method Editor (IME) for Windows, crafted around the core design philosophy of Vietnam's famous **UniKey**: **ultra-lightweight, blazing fast, intuitive, and distraction-free**.

UniPinyin allows you to type Chinese characters (Simplified & Traditional) and toned Pinyin using a standard English US keyboard on any Windows application (Word, Excel, Chrome, Notepad, Discord, WeChat, Telegram, games, etc.).

---

## 🌟 Key Highlights

1. **Ultra-Lightweight & Zero-Bloat (~240 KB with full high-res icon)**:
   - No background background services, telemetry, or intrusive ads (unlike 150+ MB commercial Chinese IMEs like Sogou, Baidu, or QQ Pinyin).
   - Written in native C# targeting Microsoft .NET Framework 4.0/4.8 (pre-installed on 100% of Windows 10/11 PCs). Zero external runtimes or dependencies needed.
   - Memory footprint is typically under 15 MB of RAM.

2. **Dual-Engine Suggestions**:
   - **Google Input Tools API (`zh-t-i0-pinyin`)**: Delivers smart, context-aware sentence and phrase predictions directly from Google Cloud over encrypted HTTPS with non-blocking debounce caching.
   - **Built-in Offline Dictionary**: Contains thousands of essential phrases and all 400+ standard Mandarin syllables. Even without an active internet connection, suggestions appear instantly.

3. **TELEX-Style Pinyin Tone Input (New Feature)**:
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
C:\Users\p1\.gemini\antigravity\scratch\UniPinyin\
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
  - **`[中]` (Blue icon)**: Chinese IME is **ACTIVE**.
  - **`[英]` or `[EN]` (Gray icon)**: English Mode is **ACTIVE** (Direct US keyboard pass-through).

### 2. Switching Between Chinese and English
- **Default Hotkey**: Press **`Alt + Z`**.
- **Mouse**: Single **left-click** on the system tray icon (just like clicking the `V`/`E` icon in UniKey).
- **Audio Feedback**:
  - A high-pitched beep (**1000 Hz**) sounds when switching **ON** to Chinese `[中]`.
  - A low-pitched beep (**600 Hz**) sounds when switching **OFF** to English `[EN]`.
  - *Note: Audio beeps can be toggled on/off in the Control Panel.*

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

#### A. Multi-Syllable Toned Pinyin
Type syllables continuously with their tone marks:
- `nirhaor` $\rightarrow$ **`nǐhǎo`**
- `hanfzif` $\rightarrow$ **`hànzì`**
- `lwf` or `luwf` $\rightarrow$ **`lǜ`**
- `zhongxwenx` $\rightarrow$ **`zhōngwén`**

#### B. Outputting Pinyin vs. Hanzi
- **Press `Enter`**: Commits the **toned Pinyin text** directly into your application (e.g. typing `nirhaor` then pressing `Enter` inserts **`nǐhǎo`**).
- **Press `Spacebar`** or **`1-9`**: Commits the **Chinese Hanzi characters** (e.g. typing `nirhaor` then pressing `Spacebar` inserts **`你好`**).

#### C. Undo / Repeating Tone Keys
- Pressing the same tone key twice cancels the tone and preserves the original letter:
  - `lax` $\rightarrow$ `lā` $\rightarrow$ pressing `x` again $\rightarrow$ **`lax`**
  - `mas` $\rightarrow$ `má` $\rightarrow$ pressing `s` again $\rightarrow$ **`mas`**
  - `haf` $\rightarrow$ `hà` $\rightarrow$ pressing `f` again $\rightarrow$ **`haf`**
  - `lw` $\rightarrow$ `lü` $\rightarrow$ pressing `w` again $\rightarrow$ **`luw`**

#### D. Smart Continuation for Common Words
- If you are typing standard words without tones that contain `x`, `s`, `r`, or `f` as consonants (e.g. `xiexie`, `nishi`, `chifan`, `haode`), UniPinyin automatically recognizes subsequent characters and avoids unintended tone locks.

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
