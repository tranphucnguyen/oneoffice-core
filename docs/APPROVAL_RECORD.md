# ONEOFFICE CORE — OFFICIAL IMPLEMENTATION DIRECTIVE

**Directive ID:** ONEOFFICE-C1-APPROVAL-001
**Ngày:** 05/11/2026
**Project:** OneOffice Core
**Trạng thái:** Owner Approved — Execute C1-01 Closeout & C1-02

Bạn là Senior .NET Software Architect, Technical Lead và AI Coding Agent chịu trách nhiệm triển khai dự án OneOffice Core.

Project Owner đã phê duyệt các quyết định kiến trúc và 5 blocker còn lại của C1-01.

Từ thời điểm này, bạn được phép cập nhật tài liệu dự án và thực hiện các thay đổi mã nguồn thuộc phạm vi C1-02.

Không cần yêu cầu xác nhận lại đối với những quyết định đã được phê duyệt dưới đây.

---

# PHẦN I — KIẾN TRÚC ĐÃ CHỐT

## 1. Công nghệ

- Backend: ASP.NET Core 10 LTS, C#.
- Database: PostgreSQL 16, EF Core 10.
- Frontend: React, TypeScript, Vite, Tailwind CSS, shadcn/ui.
- Kiến trúc: Clean Modular Monolith.
- Phạm vi: Hệ thống nội bộ cho một doanh nghiệp.
- Thời gian phát triển: 15–20 giờ/tuần.
- Ngân sách Chặng 1: 120–150 giờ, kế hoạch tham chiếu 135 giờ.
- Ưu tiên giao diện chuyên nghiệp và code dễ bảo trì.

## 2. Các ADR được phê duyệt

### ADR-015 — Core Architecture

Đổi project `OneOffice.SharedKernel` thành `OneOffice.Core`.

Cấu trúc mục tiêu:

```text
OneOffice/
├── OneOffice.slnx
├── OneOffice.Api/
├── OneOffice.Core/
│   ├── SharedKernel/
│   └── Modules/
│       ├── Identity/
│       │   ├── Domain/
│       │   └── Application/
│       ├── Organization/
│       │   ├── Domain/
│       │   └── Application/
│       └── AccessControl/
│           ├── Domain/
│           └── Application/
├── OneOffice.Infrastructure/
├── docs/
├── frontend/
└── README.md
```

Project references:

- `OneOffice.Api` → `OneOffice.Core`
- `OneOffice.Api` → `OneOffice.Infrastructure`
- `OneOffice.Infrastructure` → `OneOffice.Core`
- `OneOffice.Core` không phụ thuộc API hoặc Infrastructure.

Không tạo project thứ tư trong C1-02.

### ADR-016 — Role Assignment Scope

Scope gắn tường minh với từng `UserRoleAssignment`.

Hỗ trợ:
- All
- Department
- DepartmentTree
- Self

Effective permission phải được tính từ từng cặp permission/scope hợp lệ.

Không ghép permission của một role với scope của role khác nếu role thứ hai không cấp permission đó.

### ADR-018 — Authentication

- Access JWT có TTL 15 phút.
- Refresh Token Family tối đa 7 ngày từ login.
- Rotation không gia hạn absolute expiry.
- Lưu token hash, không lưu token gốc.
- Có revocation và reuse detection.

### ADR-023 — Cross-Department Manager

- Manager có thể khác department.
- Manager phải active.
- Không được tự quản lý mình.
- Không tạo reporting cycle.
- Quan hệ DirectManager không cấp RBAC permissions.

### ADR-024 — Multiple Department Leadership

- Một User có thể làm Leader của nhiều Department.
- Một Department có tối đa một Leader chính.
- Leader phải active nhưng không bắt buộc thuộc cùng Department.
- Không có unique index trên `Departments.LeaderId`.
- Quan hệ Leader không tự cấp RBAC.
- Không cần bảng nhiều Leader trong MVP.

### PLAN-001 — Delivery Budget

- Tổng ngân sách 120–150 giờ.
- Tham chiếu 135 giờ.
- Điều chỉnh backlog và sửa sai lệch tổng giờ.
- Ưu tiên chất lượng bảo mật và UI, không cắt bỏ các kiểm thử P0.

---

# PHẦN II — 5 BLOCKER ĐƯỢC OWNER PHÊ DUYỆT

## B01 — Organization Rules

APPROVED:

- Cho phép nhiều Department root.
- `Departments.ParentId` nullable.
- User bắt buộc thuộc một Department và có một Position.
- `DirectManagerId` có thể null khi người dùng thuộc nhóm ngoại lệ do SuperAdmin chỉ định.
- Ngoại lệ không có manager phải được lưu tường minh và kiểm tra được.
- Không dùng Position.Level hoặc chức danh để ngầm suy ra ngoại lệ.
- Khi chuyển phòng ban, có thể giữ manager cũ nếu manager vẫn hợp lệ.

Hãy đề xuất chi tiết cách lưu ngoại lệ manager-null. Ưu tiên một mô hình đơn giản, có audit và tránh bổ sung bảng không cần thiết.

## B02 — Authorization

APPROVED:

- Chỉ SuperAdmin quản lý các tài khoản và grant đặc quyền.
- HRAdmin không được tự nâng quyền.
- HRAdmin không được quản lý tài khoản HRAdmin/SuperAdmin khác.
- Data scope phải được cấp tường minh.
- DepartmentManager chỉ đọc dữ liệu nhân sự trong phạm vi được cấp.
- Áp dụng field-level authorization.
- Không trả dữ liệu ngoài scope qua API response, search, filter hoặc count.
- Không tự tạo quyền từ quan hệ Leader, DirectManager hoặc Position.Level.
- Phải bảo vệ SuperAdmin active cuối cùng.

Hãy hoàn thiện permission catalog, role mapping và chính sách đọc metadata tổ chức theo nguyên tắc least privilege.

## B03 — Disable & Concurrency

APPROVED:

- Không hard delete dữ liệu nghiệp vụ trong Chặng 1.
- Sử dụng IsActive và status endpoints.
- Không vô hiệu hóa User, Position hoặc Department khi còn phụ thuộc chưa xử lý.
- Các invariant liên quan nhiều bản ghi phải kiểm tra trong transaction.
- Chống manager cycle, department cycle và concurrency race.
- Optimistic concurrency dùng Version.
- Các thao tác bảo vệ invariant phải sử dụng locking phù hợp.
- Không tự thay Leader, manager hay role assignments khi disable.

Hãy xác định thứ tự khóa nhất quán nhằm hạn chế deadlock.

## B04 — Authentication Security

APPROVED:

- Refresh Token rotation nghiêm ngặt.
- Backend phát hiện token replay và thu hồi token family theo chính sách.
- Frontend điều phối refresh giữa các tab.
- Bảo vệ cookie bằng HttpOnly, Secure và SameSite phù hợp.
- Kiểm tra Origin và CSRF.
- Bootstrap SuperAdmin bằng cơ chế an toàn, secret không nằm trong repository.
- Có phương án khôi phục quyền quản trị an toàn.
- Không tự động bỏ qua replay chỉ vì lỗi mạng hoặc nhiều tab.

Hãy đặc tả recovery procedure, secret management và các test bảo mật liên quan.

## B05 — Durable Audit

APPROVED:

- Sử dụng bảng `AuditLogs` trong PostgreSQL.
- Ghi audit cho các thao tác quản trị và sự kiện bảo mật quan trọng.
- Các audit event gắn với thay đổi nghiệp vụ phải được ghi trong cùng transaction.
- Không lưu password, access token, refresh token hoặc secret trong audit.
- Structured logs dùng riêng cho monitoring và debugging.
- Phải có chính sách phân quyền xem audit, tránh để người dùng thông thường truy cập.

Hãy thiết kế AuditLog entity, data dictionary, index và quy tắc retention đề xuất. Chưa tự ý chọn retention cuối cùng nếu Owner chưa phê duyệt.

---

# PHẦN III — NHIỆM VỤ 1: HOÀN TẤT C1-01

Bạn được phép chỉnh sửa các tài liệu trong `docs/`.

Thực hiện:

1. Rà soát toàn bộ tài liệu hiện tại và bản Design Specification v0.2.
2. Tạo bản thiết kế chính thức `docs/C1-01_DESIGN_SPEC.md`.
3. Cập nhật `docs/DECISIONS.md`.
4. Cập nhật ERD và Data Dictionary với B01–B05.
5. Cập nhật Permission Matrix, API Contract và Security Design.
6. Bổ sung `AuditLogs` và mô hình ngoại lệ manager-null.
7. Sửa backlog: tổng giờ của bản v0.2 đang không khớp 135 giờ. Kiểm tra lại từng ticket và công bố tổng chính xác.
8. Cập nhật `docs/TASKS.md` và `docs/PROJECT_STATE.md` theo bằng chứng.
9. Tạo approval record ghi nhận sáu quyết định ADR/PLAN và B01–B05 do Owner phê duyệt.
10. Đối chiếu tài liệu gốc và ghi rõ những quyết định thay thế baseline trước đây.

Chỉ đánh dấu C1-01 là Done khi:

- Các ADR không mâu thuẫn.
- ERD và dữ liệu phân quyền nhất quán.
- API Contract thống nhất.
- Các blocker được giải quyết hoặc có đề xuất kỹ thuật cụ thể, không còn vấn đề ảnh hưởng schema/security chưa được xử lý.
- Tài liệu trên đĩa đã đồng bộ.
- Có approval record.

Nếu phát sinh một quyết định nghiệp vụ mới không thể suy ra từ các quyết định Owner đã duyệt, không tự phê duyệt; đánh dấu Pending và báo cáo để Owner xem xét.

---

# PHẦN IV — NHIỆM VỤ 2: THỰC HIỆN C1-02

Chỉ bắt đầu C1-02 sau khi C1-01 đạt điều kiện nghiệm thu.

Bạn được phép:

1. Khảo sát và ghi nhận trạng thái Git, Solution, SDK trước khi sửa.
2. Khởi tạo Git repository nếu chưa có, tạo `.gitignore` chuẩn .NET/Node.
3. Đổi tên project, folder, `.csproj`, AssemblyName và namespace từ `OneOffice.SharedKernel` sang `OneOffice.Core`.
4. Cập nhật `OneOffice.slnx` tại thư mục gốc để khai báo đủ ba project.
5. Thêm project references theo Dependency Graph đã duyệt.
6. Tạo cấu trúc thư mục Core/SharedKernel/Modules và Infrastructure/Persistence.
7. Dọn file mẫu `WeatherForecast*`, `Class1.cs` và endpoint demo sau khi xác minh chúng chỉ là template.
8. Giữ hoặc điều chỉnh OpenAPI theo framework hiện tại.
9. Chạy `dotnet restore` và `dotnet build`.
10. Kiểm tra toàn bộ solution build bằng .NET 10 SDK.
11. Cập nhật tài liệu trạng thái và bằng chứng build.

Không được:

- Tạo bảng nghiệp vụ hoặc migrations.
- Tự triển khai JWT, RBAC hoặc CRUD nhân sự.
- Tạo project thứ tư.
- Triển khai frontend.
- Thực hiện C1-03 khi chưa được giao.
- Thay đổi kiến trúc nền tảng.

## Tiêu chí nghiệm thu C1-02

- `OneOffice.slnx` khai báo đúng ba project.
- Không còn tên assembly `OneOffice.SharedKernel`.
- Project references đúng chiều, không có circular dependency.
- `OneOffice.Core` độc lập API và Infrastructure.
- `dotnet sln list` hiển thị đủ ba project.
- `dotnet restore` hoàn thành.
- `dotnet build` thành công, không có compile error.
- Không còn WeatherForecast demo.
- Các file cấu hình không chứa secret.
- Git diff có thể review và rollback.
- `TASKS.md` và `PROJECT_STATE.md` phản ánh đúng trạng thái.

---

# PHẦN V — QUY TRÌNH BÁO CÁO

Sau khi hoàn thành, bắt buộc báo cáo bằng tiếng Việt:

## A. C1-01

- Tài liệu đã tạo/sửa.
- Danh sách ADR và blocker.
- ERD, RBAC, Auth và Audit đã được đồng bộ ra sao.
- Những quyết định còn Pending.
- Có đủ điều kiện đóng C1-01 hay không.

## B. C1-02

- Danh sách file tạo/sửa/xóa/đổi tên.
- Cấu trúc Solution sau thay đổi.
- Project references.
- Kết quả thực tế của `dotnet sln list`.
- Kết quả `dotnet restore`.
- Kết quả `dotnet build`.
- Cảnh báo và lỗi còn lại.
- Git diff/status.
- Các điều kiện nghiệm thu đã đạt và chưa đạt.

## C. Tiến độ

- Ticket đã hoàn thành.
- Ticket đang thực hiện.
- Ngân sách giờ đã sử dụng nếu có dữ liệu thực tế.
- Rủi ro còn lại.
- Đề xuất bước tiếp theo.

Bắt buộc phân biệt:

- DONE — Có bằng chứng kiểm tra.
- IN PROGRESS — Đang thực hiện.
- BLOCKED — Có vấn đề chưa giải quyết.
- NOT STARTED — Chưa thực hiện.

Không được báo cáo rằng build, test hoặc migration thành công nếu chưa thực sự chạy.

---

# PHẦN VI — ĐIỂM DỪNG

Sau khi hoàn thành C1-01 và C1-02:

1. Dừng triển khai.
2. Không tự chuyển sang C1-03.
3. Trình bày kết quả và bằng chứng.
4. Chờ Project Owner và Architecture Reviewer kiểm tra.

Mục tiêu là đưa OneOffice Core từ trạng thái ba project template sang một Solution .NET 10 chuẩn hóa, có kiến trúc và tài liệu thống nhất, đủ điều kiện triển khai database ở C1-03.
