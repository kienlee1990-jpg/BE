KPI Tracker - goi trien khai full he thong

1. Cau truc goi _deploy
- backend\
- frontend\
- database\
- scripts\

2. Backend
- Thu muc publish san: _deploy\backend
- Chinh file:
  _deploy\backend\appsettings.Production.json
- Can cap nhat:
  - ConnectionStrings:DefaultConnection
  - Jwt:Key
  - App:ClientUrl
  - Cors:AllowedOrigins

3. Frontend
- Thu muc deploy san: _deploy\frontend
- Da cau hinh goi API bang duong dan tuong doi /api
- Da co san web.config cho Vue Router history mode

4. Database
- Script tong hop migration:
  _deploy\database\update-database-idempotent.sql
- Script bo sung cot user-don-vi:
  _deploy\database\add-donviid-to-aspnetusers.sql
- Script bo sung cot KPI so sanh:
  _deploy\database\fix-kieu-so-sanh-columns.sql

5. Cach trien khai de xuat tren Windows Server + IIS
- Cai .NET Hosting Bundle dung version
- Cai IIS, URL Rewrite, WebSockets (neu can)
- Tao SQL Server database
- Chay script DB trong _deploy\database
- Copy backend vao vi du:
  C:\inetpub\kpi-api
- Copy frontend vao vi du:
  C:\inetpub\kpi-ui
- Chay script:
  _deploy\scripts\setup-iis-full-system.ps1

6. Kich ban IIS duoc script ho tro
- Site chinh:
  https://YOUR_DOMAIN -> frontend
- Child application:
  /api -> backend

7. Thu tu thuc hien khuyen nghi
1. Chinh appsettings.Production.json
2. Chay script DB
3. Copy backend/frontend len server
4. Chay setup-iis-full-system.ps1 voi quyen Administrator
5. Restart IIS
6. Truy cap domain va kiem tra dang nhap

8. Lenh mau tren server
PowerShell:
  Set-ExecutionPolicy Bypass -Scope Process -Force
  .\setup-iis-full-system.ps1 `
    -SiteName "KPITracker" `
    -Domain "kpi.example.com" `
    -FrontendPath "C:\inetpub\kpi-ui" `
    -BackendPath "C:\inetpub\kpi-api" `
    -Port 80

9. Ghi chu
- Script IIS khong tu tao SSL certificate.
- Neu dung HTTPS, can binding cert trong IIS sau khi tao site.
- Neu server da co site truoc do, doi SiteName/Port cho phu hop.
