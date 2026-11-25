# Game API + Game Windows Forms (ASM 1 GAM106)

Solution gồm 2 project:

- `GameApi_ASM_NguyenTriThinh` — ASP.NET Core Web API, net10.0, SQL Server 2022, EF Core, JWT, Swagger.
- `GameClient_WinForms` — Windows Forms desktop client, net10.0-windows.

## Chạy trong Visual Studio 2022

1. Mở `GameApi_ASM_NguyenTriThinh.slnx`.
2. Chuột phải Solution → **Set Startup Projects...** → chọn **Multiple startup projects**:
   - `GameApi_ASM_NguyenTriThinh`: **Start**
   - `GameClient_WinForms`: **Start**
3. Nhấn F5. API chạy, WinForms mở form đăng nhập.

Nếu chỉ muốn chạy từng project: chọn project tương ứng làm Startup Project.

## Cấu hình API

`GameApi_ASM_NguyenTriThinh/appsettings.json`:

```
Server=.;Database=GameApiDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Migration đã apply: `20261125080742_InitialCreate`. Không xóa dữ liệu có sẵn.

Swagger: chạy API → `http://localhost:5086/swagger` (profile `http`) hoặc `https://localhost:7272/swagger` (profile `https`). Nút **Authorize** nhận JWT (không gõ "Bearer").

## Cấu hình WinForms → API

`GameClient_WinForms/appsettings.json` → `ApiBaseUrl`. Mặc định `http://localhost:5086/`.
Nếu chạy API profile `https`, đổi thành `https://localhost:7272/`.

Token JWT lưu trong bộ nhớ (`Session`), xóa khi đăng xuất. Không gửi AccountId từ client.

## API

| Method | Endpoint | Auth | Mô tả |
|---|---|---|---|
| POST | `/api/auth/register` | Không | Đăng ký (201 / 400 / 409) |
| POST | `/api/auth/login` | Không | Đăng nhập, trả JWT (200 / 401) |
| POST | `/api/characters` | Bearer | Tạo nhân vật (201 / 400 / 401) |
| PUT | `/api/characters/{id}` | Bearer | Sửa nhân vật của mình (200 / 400 / 401 / 404) |
| GET | `/api/characters/mine` | Bearer | Danh sách nhân vật của mình (bổ sung cho WinForms) |

## 12 test case ASM 1 (đã kiểm chứng bằng HTTP thực)

| TC | Kết quả |
|---|---|
| TC01 Đăng ký tài khoản mới | 201 |
| TC02 Đăng ký trùng username | 409 |
| TC03 Password 123 | 400 |
| TC04 Sai mật khẩu | 401 |
| TC05 Đăng nhập đúng | 200, có JWT |
| TC06 Tạo nhân vật không có token | 401 |
| TC07 Authorize bằng token | 200 |
| TC08 Tạo Hiep Si Rong / Warrior | 201, Level 1, HP 100 |
| TC09 Name rỗng | 400 |
| TC10 Sửa Level 5, HP 250 | 200 |
| TC11 Level 0 | 400 |
| TC12 Dùng token account này sửa nhân vật account khác | 404 |

Ghi chú: để chạy lại TC01 cần username chưa tồn tại. Test đã tạo account `asmcheck01` và nhân vật `Hiep Si Rong` trong DB; không xóa dữ liệu cũ.

## Kiểm thử WinForms

Đã chạy thật process WinForms với API đang chạy, dùng UI Automation:

- Mở form đăng nhập: OK.
- Sai mật khẩu → hiện "Sai tên đăng nhập hoặc mật khẩu." OK.
- Đăng nhập đúng → mở Dashboard, hiện username + BaseUrl API. OK.
- Danh sách nhân vật tải từ API qua `GET /api/characters/mine`. OK.
- Chọn dòng → tự điền Character ID/Name/Class/Level/HP. OK.
- Tạo nhân vật → "Tạo nhân vật thành công: ... | Level 1 | HP 100". OK.
- Sửa nhân vật → cập nhật thành công, thông báo giữ lại sau khi reload danh sách. OK.
- Đăng xuất → token xóa, về lại form đăng nhập. OK.

Chưa tự kiểm chứng bằng automation: luồng đăng ký trong form (modal `ShowDialog` chặn automation), thông báo khi API tắt, token hết hạn. Đây là các bước bạn nên click tay trong VS.

Giao diện đã chỉnh: tông tối xanh đen/tím, nhấn vàng ở tiêu đề, thanh accent, DataGridView màu xen kẽ, nút đổi màu khi hover. Không dùng ảnh ngoài, không HTML/Blazor.
