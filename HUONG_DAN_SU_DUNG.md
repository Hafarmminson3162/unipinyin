# UniPinyin - Hướng Dẫn Sử Dụng Bộ Gõ Tiếng Trung Cho Windows (Phong Cách UniKey)

**UniPinyin** là phần mềm gõ tiếng Trung Pinyin từ bàn phím tiếng Anh (US Keyboard) dành cho hệ điều hành Windows, được thiết kế theo triết lý **siêu nhẹ, mượt mà và trực quan giống hệt UniKey** của Việt Nam.

---

## 🌟 Điểm nổi bật của UniPinyin

1. **Siêu nhẹ & Tiết kiệm tài nguyên (Dung lượng chỉ ~240 KB có logo)**:
   - Không cài cắm dịch vụ ngầm, không quảng cáo, không hao pin/RAM (so với các bộ gõ nặng nề hàng trăm MB như Sogou, Baidu, QQ Pinyin).
   - Sử dụng trực tiếp Native Windows Win32 API và .NET Framework tích hợp sẵn trên mọi máy tính Windows 10/11. Không cần cài thêm bất kỳ runtime hay phần mềm phụ trợ nào.

2. **Lấy gợi ý từ Google Input Tools API**:
   - Sử dụng công nghệ nhận diện và đoán từ ngữ cảnh thông minh của Google (`zh-t-i0-pinyin`).
   - Tự động tích hợp cơ chế bộ đệm (Caching) và truy vấn bất đồng bộ (Asynchronous Query) giúp gõ phím tức thì, mượt mà, không giật lag.

3. **Từ điển Offline dự phòng (Offline Fallback)**:
   - Tích hợp sẵn hàng ngàn từ ghép, câu giao tiếp và 400+ âm tiết Pinyin chuẩn trong bộ nhớ.
   - Nếu mất mạng hoặc mạng chậm, bộ gõ vẫn tự động hiển thị gợi ý ngoại tuyến tức thì mà không bị gián đoạn.

4. **Gõ Pinyin có dấu thanh điệu chuẩn TELEX**:
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
   - **Âm báo thông báo khi chuyển đổi**: Phát âm thanh sạc pin/thông báo Samsung Galaxy êm ái, hiện đại khi chuyển đổi giữa Tiếng Trung [中] và Tiếng Anh [E]. Có tùy chọn Bật/Tắt âm báo trong Bảng điều khiển.
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
UniPinyin/
│
├── UniPinyin.exe           # Bản chạy chính (Song ngữ Tiếng Việt & English, ~246 KB)
├── unipinyin-en.exe        # Bản chạy 100% Tiếng Anh độc lập (~238 KB)
├── app.ico                 # Icon chú gấu trúc (16x16 đến 256x256)
├── app_icon.png            # Logo gấu trúc định dạng PNG
├── UniPinyin_Web.html      # Trình gõ Pinyin trên nền trình duyệt web (Song ngữ)
├── UniPinyin_Web_EN.html   # Trình gõ Pinyin trên nền trình duyệt web (100% Tiếng Anh)
├── build.bat               # Script 1-click tự động biên dịch UniPinyin.exe
├── build-en.bat            # Script 1-click tự động biên dịch unipinyin-en.exe
├── USER_GUIDE.md           # Tài liệu hướng dẫn Tiếng Anh
├── HUONG_DAN_SU_DUNG.md    # Tài liệu hướng dẫn Tiếng Việt (file này)
├── README.md               # Tổng quan dự án GitHub
├── LICENSE                 # Giấy phép nguồn mở MIT
├── .gitignore              # Bộ lọc file tạm Git
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
```

---

## 🚀 Hướng dẫn sử dụng nhanh

### 1. Khởi động phần mềm
- Nhấp đúp vào file `UniPinyin.exe` (hoặc `unipinyin-en.exe`):
- Một biểu tượng nhỏ sẽ xuất hiện ở **Khay hệ thống (System Tray)** góc dưới bên phải màn hình:
  - `[中]` (Màu xanh Apple System Blue): Đang bật chế độ gõ Tiếng Trung (chữ cái trong suốt, không viền ô vuông).
  - `[E]` (Màu bạc/trắng Apple Silver): Đang ở chế độ gõ Tiếng Anh bình thường (chữ cái trong suốt, không viền ô vuông).

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

#### B. Gõ Pinyin có dấu thanh (TELEX-style) & Quy tắc thông minh:
- **Âm `ü`**: Gõ bằng phím **`w`** hoặc **`u+w`** (hoặc `v`). Ví dụ: `lw` -> `lü`, `nuw` -> `nü`, `luw` -> `lü`.
- **Quy tắc gõ `w` và `ww` (Ấn 2 lần `w` ra `w` chứ không phải `uw`)**:
  - Gõ `w` đứng một mình: tạo ra `ü`.
  - Ấn `w` lần 2 (`ww`): tự động đổi thành **`w`** (ấn `w` lần 3 ra `ww`).
  - Gõ `l` + `w` ra `lü`, ấn `w` lần nữa (`lww`) ra **`lw`**.
  - Gõ `u` + `w` ra `ü`, ấn `w` lần nữa (`uww`) ra **`w`**.
  - Gõ các âm bắt đầu bằng `w`: sau khi gõ `w` (đang hiện `ü`), nếu gõ tiếp các nguyên âm `o, a, e, u` (như `wo`, `wa`, `wen`, `wu`), phần mềm tự động hoàn nguyên thành chữ `w` + nguyên âm.
- **Dấu nháy đơn `'` phân cách âm tiết / từ**:
  - Dùng dấu `'` để phân tách rõ các âm tiết tránh dính chữ: `xi'an` -> **`西安`** (khác với `xian` -> `先/线/现`), `tian'an'men` -> **`天安门`**, `wo'shi'zhong'guo'ren` -> **`我是中国人`**, `wo'xiang'xue'zhong'wen` -> **`我想学中文`**.
  - Bỏ dấu thanh điệu theo từng âm tiết sau dấu nháy đơn: `xi'anx` -> **`xi'ān`**.
- **Các phím thanh điệu TELEX**:
  - **Thanh 1 (¯)**: Gõ phím **`x`**. Ví dụ: `lax` -> `lā`.
  - **Thanh 2 (´)**: Gõ phím **`s`**. Ví dụ: `mas` -> `má`.
  - **Thanh 3 (ˇ)**: Gõ phím **`r`**. Ví dụ: `nir` -> `nǐ`.
  - **Thanh 4 (`)**: Gõ phím **`f`**. Ví dụ: `haf` -> `hà`.
- **Gõ cụm Pinyin có dấu**:
  - `nirhaor` -> `nǐhǎo`
  - `hanfzif` -> `hànzì`
  - `lwf` -> `lǜ`
- **Xác nhận lấy chuỗi Pinyin có dấu**: Nhấn phím **Enter**! Phần mềm sẽ xuất thẳng chuỗi Pinyin có dấu vào văn bản.
- **Xử lý câu dài thông minh khi Offline**:
  - Tích hợp thuật toán đối sánh từ dài nhất (Forward Maximum Matching - FMM) phân tích câu nhiều âm tiết ngay cả khi không có mạng Internet.
  - Hỗ trợ câu dài lên tới 100 ký tự cả ở dạng có dấu nháy đơn (`wo'shi'zhong'guo'ren`, `wo'shi'yue'nan'ren`) lẫn dạng viết liền (`woshizhongguoren`, `nihaoma`).

### 3. Phím tắt Bật / Tắt Tiếng Trung
- **Mặc định**: Nhấn tổ hợp phím **`Alt + Z`**.
- Hoặc chỉ cần **nhấp chuột trái 1 lần** vào biểu tượng `[中]` / `[E]` ở khay hệ thống.
- Khi chuyển đổi, phần mềm phát âm thanh thông báo Samsung Galaxy êm ái, giúp nhận biết tức thì mà không cần nhìn xuống khay màn hình.

### 4. Mở Bảng điều khiển (Settings)
- **Nhấp đúp chuột trái** hoặc **nhấp chuột phải** vào icon khay hệ thống > Chọn **"Bảng điều khiển... [Mở cấu hình]"** (hoặc *"Control Panel... [Settings]"*).
- Tại đây bạn có thể thay đổi:
  - Ngôn ngữ giao diện: Tiếng Việt hoặc English.
  - Bảng mã Giản thể / Phồn thể.
  - Phím tắt chuyển đổi (`Alt+Z`, `Ctrl+Shift`, `Ctrl+Space`).
  - Bật / Tắt âm báo khi chuyển đổi.
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
