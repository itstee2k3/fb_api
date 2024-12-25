using Microsoft.AspNetCore.Identity;

namespace api.Models;

public class User : IdentityUser
{
    // Bạn có thể thêm các thuộc tính bổ sung tại đây
    public string? FullName { get; set; } // Tên đầy đủ (tùy chọn)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo người dùng
}

// INSERT INTO [User] 
// (
//     [Id], 
//     [UserName], 
//     [NormalizedUserName], 
//     [Email], 
//     [NormalizedEmail], 
//     [EmailConfirmed], 
//     [PasswordHash], 
//     [SecurityStamp], 
//     [ConcurrencyStamp], 
//     [PhoneNumber], 
//     [PhoneNumberConfirmed], 
//     [TwoFactorEnabled], 
//     [LockoutEnd], 
//     [LockoutEnabled], 
//     [CreatedAt], 
//     [FullName], 
//     [AccessFailedCount]
// )
// VALUES 
// (
//     NEWID(),                            -- Tạo một GUID mới cho Id
//     'admin',                            -- UserName
//     UPPER('admin'),                     -- NormalizedUserName (chữ in hoa của UserName)
//     'admin@example.com',                -- Email
//     UPPER('admin@example.com'),         -- NormalizedEmail (chữ in hoa của email)
//     1,                                  -- EmailConfirmed (1: email đã được xác nhận, 0: chưa xác nhận)
//     'AQAAAAEAACcQAAAAEAAk4kYxhNp1W+LXRGHkAKIXeZzOqfOgOqX0/ZZfJWFiQUaeR1jE1rOIoH1uZThIqg==', -- PasswordHash đã mã hóa
//     NEWID(),                            -- SecurityStamp (mã bảo mật ngẫu nhiên)
//     NEWID(),                            -- ConcurrencyStamp (mã đồng bộ hóa ngẫu nhiên)
//     NULL,                               -- PhoneNumber (null nếu không có số điện thoại)
//     0,                                  -- PhoneNumberConfirmed (0: chưa xác nhận số điện thoại)
//     0,                                  -- TwoFactorEnabled (0: chưa kích hoạt xác thực 2 yếu tố)
//     NULL,                               -- LockoutEnd (null nếu không bị khóa tài khoản)
//     0,                                  -- LockoutEnabled (0: tài khoản không bị khóa)
//     GETUTCDATE(),                       -- CreatedAt (Ngày giờ UTC hiện tại)
//     'Administrator',                     -- FullName
//     0                                   -- AccessFailedCount (giá trị mặc định là 0)
// );
