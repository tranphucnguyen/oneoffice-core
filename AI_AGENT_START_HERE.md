# START HERE — Prompt for coding AI agent

Bạn đang là AI coding agent của dự án **OneOffice Core**. Developer làm một mình, phát triển hệ thống nội bộ cho **một doanh nghiệp**, 15–20 giờ/tuần, **ưu tiên UI chỉn chu ngay từ đầu**.

**BẮT BUỘC đọc theo thứ tự:**
1. `docs/ONEOFFICE_MASTER_PLAN.md` — kiến trúc, ERD, rule, API, UX, backlog, kiểm thử và quy tắc phối hợp.
2. `docs/DECISIONS.md` — ADR phê duyệt và điều còn mở.
3. `docs/TASKS.md` — ticket theo thứ tự.
4. `docs/PROJECT_STATE.md` — hiện trạng thực sự; đừng giả định đã có phần mềm chạy.
5. PDF gốc nếu có; nếu không có, coi mục A của master plan là bản tổng hợp có truy xuất nguồn và hỏi developer khi phát hiện mâu thuẫn.

## Cách làm việc
- Chỉ nhận **một ticket** được chỉ định. Trước code, tóm tắt mục tiêu, phụ thuộc, file cần thay đổi, command xác minh, risk.
- Nếu thiếu quyết định ảnh hưởng bảo mật/dữ liệu, dừng phần liên quan và ghi ADR; có thể tiếp tục phần độc lập.
- Viết source hoàn chỉnh, file paths rõ, không TODO ở business/security logic. Mọi thay đổi phải kèm tests.
- Sau mỗi ticket, sửa `docs/TASKS.md`, `docs/PROJECT_STATE.md` theo **kết quả thực sự**, không điền đã xong nếu chưa chạy.
- Tạo handoff dạng:

```text
[ONEOFFICE HANDOFF]
Ticket:
Commit:
Changed files:
Commands actually executed:
Test results (pass/fail/not run):
Acceptance evidence:
Risks/ADRs:
Next step:
[/ONEOFFICE HANDOFF]
```

## Nhiệm vụ đầu tiên
C1-01: rà soát và chốt business rules còn mở trong `docs/ONEOFFICE_MASTER_PLAN.md` Section K, sau đó finalize ERD, constraints, DTOs và API contract. Không viết Authentication controller hoặc tự coi C1-01 hoàn thành khi quy tắc nghiệp vụ vẫn chưa được owner duyệt.
