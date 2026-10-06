-- QLTV Database
--DROP DATABASE QLTV
CREATE DATABASE QLTV
go
USE QLTV
go
SET DATEFORMAT dmy
go

-- SCHEM
CREATE TABLE NHOMNGUOIDUNG (id int IDENTITY(1,1) primary key, 
	MaNhomNguoiDung AS CAST('NND' + right('000' + CAST(id as varchar(5)), 3) AS CHAR(6)) persisted, 
	TenNhomNguoiDung nvarchar(max) NOT NULL);
go

CREATE TABLE CHUCNANG (id int primary key IDENTITY(1,1), 
	MaChucNang AS CAST('CN' + right('000' + CAST(id as varchar(3)), 3) as char(5))persisted, 
	TenChucNang NVARCHAR(MAX) NOT NULL, TenManHinh NVARCHAR(MAX) NOT NULL);
go

CREATE TABLE PHANQUYEN (idNhomNguoiDung INT FOREIGN KEY REFERENCES NHOMNGUOIDUNG on delete cascade, 
	idChucNang INT FOREIGN KEY REFERENCES CHUCNANG on delete cascade, 
	PRIMARY KEY (idNhomNguoiDung, idChucNang));
go

CREATE TABLE NGUOIDUNG (id INT IDENTITY PRIMARY KEY, 
	MaNguoiDung AS CAST('ND'+ RIGHT('0000' + CAST(id AS VARCHAR(4)), 4) AS CHAR(6)) PERSISTED, 
	TenNguoiDung NVARCHAR(MAX) NOT NULL, 
	NgaySinh datetime, 
	ChucVu NVARCHAR(MAX), 
	TenDangNhap VARCHAR(256) UNIQUE NOT NULL, 
	MatKhau VARCHAR(MAX) NOT NULL, 
	idNhomNguoiDung INT REFERENCES NHOMNGUOIDUNG on delete cascade NOT NULL, 
	Email VARCHAR(100) NULL, 
	DiaChi NVARCHAR(200) NULL);
go

CREATE TABLE THELOAI (id int IDENTITY(1,1) primary key, 
	MaTheLoai As Cast('TL' + right('0000' + CAST(id as varchar(4)), 4) as char(6)) persisted, 
	TenTheLoai NVARCHAR(MAX) NOT NULL);
go

CREATE TABLE TUASACH (id INT IDENTITY PRIMARY KEY, 
	MaTuaSach AS cast('TS'+ right('0000' + CAST(ID AS VARCHAR(4)), 4) as char(6)) PERSISTED, 
	TenTuaSach NVARCHAR(MAX) NOT NULL, 
	idTheLoai int references THELOAI NOT NULL, 
	DaAn int DEFAULT 0);
go

CREATE TABLE TACGIA (id INT IDENTITY PRIMARY KEY, MATACGIA AS CAST('TG'+ RIGHT('0000' + CAST(ID AS VARCHAR(4)), 4) AS CHAR(6))PERSISTED, 
	TenTacGia NVARCHAR(MAX) NOT NULL, 
	Email VARCHAR(100) NULL, 
	DiaChi NVARCHAR(200) NULL);
go

CREATE TABLE CT_TACGIA (idTacGia int references TACGIA on delete cascade, 
	idTuaSach int references TUASACH on delete cascade, primary key (idTacGia, idTuaSach));
go

CREATE TABLE LOAIDOCGIA (id INT IDENTITY(1,1) PRIMARY KEY, 
	MaLoaiDocGia AS CAST('LDG'+ RIGHT('000' + CAST(ID AS VARCHAR(3)), 3) AS CHAR(6))PERSISTED, 
	TenLoaiDocGia NVARCHAR(MAX) NOT NULL);
go

CREATE TABLE DOCGIA (ID int IDENTITY(1,1) PRIMARY KEY, 
	MaDocGia AS CAST('DG'+ RIGHT('0000' + CAST(ID AS VARCHAR(4)), 4) AS CHAR(6))PERSISTED, 
	TenDocGia NVARCHAR(MAX) NOT NULL, 
	NgaySinh datetime NOT NULL, 
	DiaChi NVARCHAR(MAX), 
	Email VARCHAR(MAX), 
	NgayLapThe Datetime NOT NULL, 
	NgayHetHan Datetime NOT NULL, 
	idLoaiDocGia INT references LOAIDOCGIA NOT NULL, 
	TongNoHienTai int NOT NULL DEFAULT 0, 
	idNguoiDung INT REFERENCES NGUOIDUNG UNIQUE NOT NULL);
go

CREATE TABLE SACH (id int IDENTITY(1,1) primary key, 
	MaSach AS CAST('S'+ RIGHT('00000' + CAST(id AS VARCHAR(5)), 5) AS CHAR(6)) PERSISTED, 
	idTuaSach int references TUASACH NOT NULL, 
	SoLuong int NOT NULL, 
	SoLuongConLai int NOT NULL, 
	DonGia int NOT NULL, 
	NamXB int NOT NULL, 
	NhaXB NVARCHAR(MAX) NOT NULL, 
	DaAn int NOT NULL DEFAULT 0);
go

CREATE TABLE PHIEUNHAPSACH (SoPhieuNhap int IDENTITY(1,1) primary key, 
	TongTien int NOT NULL DEFAULT 0, 
	NgayNhap Datetime NOT NULL);
go

CREATE TABLE CT_PHIEUNHAP (SoPhieuNhap int references PHIEUNHAPSACH(SoPhieuNhap), 
	idSach int references SACH, 
	DonGia int NOT NULL, ThanhTien int NOT NULL, 
	SoLuongNhap int NOT NULL, primary key (SoPhieuNhap, idSach));
go

CREATE TABLE CUONSACH (id int IDENTITY(1,1) primary key, 
	MaCuonSach AS CAST('CS'+ RIGHT('0000' + CAST(id AS VARCHAR(4)), 4) AS CHAR(6)) PERSISTED, 
	idSach int references SACH NOT NULL, 
	TinhTrang INT NOT NULL DEFAULT 1, 
	DaAn int NOT NULL DEFAULT 0);
go

CREATE TABLE PHIEUMUONTRA (SoPhieuMuonTra int IDENTITY(1,1) primary key, 
	idDocGia int references DOCGIA NOT NULL, 
	idCuonSach int references CUONSACH NOT NULL, 
	NgayMuon Datetime NOT NULL, 
	NgayTra Datetime, 
	HanTra Datetime NOT NULL, 
	SoTienPhat int DEFAULT 0);
go

CREATE TABLE PHIEUTHU (SoPhieuThu int IDENTITY(1,1) primary key, 
	idDocGia int references DOCGIA NOT NULL, 
	SoTienThu int NOT NULL DEFAULT 0, 
	NgayLap datetime NOT NULL);
go

CREATE TABLE BCLUOTMUONTHEOTHELOAI (id INT IDENTITY(1,1) PRIMARY KEY, 
	Thang int NOT NULL, 
	Nam int NOT NULL, 
	MaBaoCao AS CAST('BCLM' + RIGHT('0' + CAST(THANG AS CHAR(2)), 2) + CAST(NAM AS CHAR(4)) AS CHAR(10)) PERSISTED, 
	TongSoLuotMuon int NOT NULL DEFAULT 0);
go

CREATE TABLE CT_BCLUOTMUONTHEOTHELOAI (idBaoCao INT references BCLUOTMUONTHEOTHELOAI, 
	idTheLoai int references THELOAI, 
	SoLuotMuon int NOT NULL DEFAULT 0, TiLe numeric(4,2) DEFAULT 0, primary key (idBaoCao, idTheLoai));
go

CREATE TABLE BCSACHTRATRE (Ngay datetime not null, 
	idCuonSach int references CUONSACH on delete cascade, 
	NgayMuon datetime NOT NULL, 
	SoNgayTre int NOT NULL DEFAULT 0, primary key(Ngay, idCuonSach));
go

CREATE TABLE THAMSO (id int identity(1,1) primary key, 
	TuoiToiThieu int NOT NULL, 
	TuoiToiDa int NOT NULL, 
	ThoiHanThe int NOT NULL, 
	KhoangCachXuatBan int NOT NULL, 
	SoSachMuonToiDa int NOT NULL, 
	SoNgayMuonToiDa int NOT NULL, 
	DonGiaPhat int NOT NULL, AD_QDKTTienThu int NOT NULL);
go

CREATE TABLE NHUCAUDOC (id int IDENTITY(1,1) primary key, 
	MaNhuCau AS CAST('NC' + RIGHT('0000' + CAST(id AS VARCHAR(4)), 4) AS CHAR(6)) PERSISTED, 
	idDocGia INT NOT NULL REFERENCES DOCGIA(ID) ON DELETE CASCADE, 
	idSach INT NOT NULL REFERENCES SACH(id), 
	NgayThem DATETIME NOT NULL DEFAULT GETDATE(), 
	TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Đang chờ',
	GhiChu NVARCHAR(MAX) NULL);
go

CREATE TABLE AUDITLOG (id INT IDENTITY(1,1) PRIMARY KEY, 
	idNguoiDung INT NOT NULL REFERENCES NGUOIDUNG(id), 
	HanhDong NVARCHAR(20) NOT NULL, 
	Bang NVARCHAR(50) NOT NULL, 
	idBanGhi INT NOT NULL, 
	ChiTiet NVARCHAR(MAX) NULL, 
	ThoiGian DATETIME NOT NULL DEFAULT GETDATE());
go

CREATE PROC sp_AuditLog @idNguoiDung INT, @HanhDong NVARCHAR(20), @Bang NVARCHAR(50), @idBanGhi INT, @ChiTiet NVARCHAR(MAX) = NULL AS INSERT INTO AUDITLOG(idNguoiDung, HanhDong, Bang, idBanGhi, ChiTiet) VALUES (@idNguoiDung, @HanhDong, @Bang, @idBanGhi, @ChiTiet);
go
CREATE VIEW v_LichSuMuonTra AS
SELECT pmt.SoPhieuMuonTra, dg.MaDocGia, dg.TenDocGia, cs.MaCuonSach, ts.TenTuaSach AS TenSach,
    pmt.NgayMuon, pmt.HanTra, pmt.NgayTra,
    CASE WHEN pmt.NgayTra IS NULL AND pmt.HanTra < GETDATE() THEN N'Quá hạn'
         WHEN pmt.NgayTra IS NULL THEN N'Đang mượn' ELSE N'Đã trả' END AS TrangThai,
    ISNULL(pmt.SoTienPhat, 0) AS SoTienPhat
FROM PHIEUMUONTRA pmt JOIN CUONSACH cs ON pmt.idCuonSach = cs.id
    JOIN SACH s ON cs.idSach = s.id
    JOIN TUASACH ts ON s.idTuaSach = ts.id
    JOIN DOCGIA dg ON pmt.idDocGia = dg.ID;
go

--  DATA 
INSERT INTO NHOMNGUOIDUNG(TenNhomNguoiDung) VALUES (N'Quản Lý'), (N'Thủ Thư'), (N'Độc Giả');
go

INSERT INTO CHUCNANG(TenChucNang, TenManHinh) VALUES
('QLDG', N'Quản Lý Độc Giả'), ('QLS', N'Quản Lý Sách'), ('QLPM', N'Quản Lý Phiếu Mượn Trả'),
('QLPT', N'Quản Lý Phiếu Thu'), ('BCTK', N'Báo Cáo Thống Kê'), ('QLND', N'Quản Lý Người Dùng'),
('TDQD', N'Thay Đổi Quy Định'), ('DG', N'Là Độc Giả');
go

INSERT INTO PHANQUYEN(idNhomNguoiDung, idChucNang) VALUES
(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),
(2,1),(2,2),(2,3),(2,4),(2,5),(2,6),(2,7),
(3,8);
go

INSERT INTO NGUOIDUNG(TenNguoiDung, NgaySinh, TenDangNhap, MatKhau, idNhomNguoiDung, Email, DiaChi) VALUES
(N'Admin Hệ Thống', NULL, 'admin', '123', 1, 'admin@uit.edu.vn', N'Phòng Quản Lý, Tầng 1, UIT'),
(N'Thủ Thư', NULL, 'lib', '123', 2, 'lib@uit.edu.vn', N'Quầy Thư Viện, Tầng 2, UIT'),
(N'Danh Trường Hưng', '11/06/2005', 'docgia1', '123', 3, 'hung.dt@gmail.com', N'12 Nguyễn Huệ, Q1, HCM'),
(N'Nguyễn Huỳnh Phát', '08/01/2005', 'docgia2', '123', 3, 'phat.nh@gmail.com', N'45 Lê Lợi, Q1, HCM'),
(N'Trần Ngọc Kiều Duyên', '24/02/2005', 'docgia3', '123', 3, 'duyen.ntk@gmail.com', N'78 Trần Hưng Đạo, Q5, HCM'),
(N'Đinh Vũ Thành', '03/01/2005', 'docgia4', '123', 3, 'thanh.dv@gmail.com', N'23 Hai Bà Trưng, Q3, HCM'),
(N'Trần Ngọc Vinh', '22/06/2005', 'docgia5', '123', 3, 'vinh.tn@gmail.com', N'56 Điện Biên Phủ, Bình Thạnh, HCM'),
(N'K'' Khoa', '25/08/2005', 'docgia6', '123', 3, 'khoa.k@gmail.com', N'90 Nguyễn Thị Minh Khai, Q1, HCM'),
(N'Trần Bửu Ngọc', '11/04/2005', 'docgia7', '123', 3, 'ngoc.tb@gmail.com', N'34 Võ Văn Tần, Q3, HCM'),
(N'Trần Trung Nguyên', '02/11/2005', 'docgia8', '123', 3, 'nguyen.tt@gmail.com', N'67 Cách Mạng Tháng 8, Q10, HCM'),
(N'Nguyễn Đức Quang', '03/07/2005', 'docgia9', '123', 3, 'quang.nd@gmail.com', N'89 Lý Tự Trọng, Q1, HCM'),
(N'Nguyễn Huy Thiện', '25/09/2005', 'docgia10', '123', 3, 'thien.nh@gmail.com', N'12 Phạm Ngũ Lão, Q1, HCM'),
(N'Phạm Duy Tự', '05/12/2005', 'docgia11', '123', 3, 'tu.pd@gmail.com', N'45 Nguyễn Trãi, Q5, HCM'),
(N'Lê Thiên Nga', '09/04/2005', 'docgia12', '123', 3, 'nga.lt@gmail.com', N'78 Lê Văn Sỹ, Q3, HCM'),
(N'Đoàn Văn Quang Đại', '05/09/2005', 'docgia13', '123', 3, 'dai.dvq@gmail.com', N'23 Hoàng Diệu, Q4, HCM'),
(N'Lê Dương Tuấn Anh', '30/07/2005', 'docgia14', '123', 3, 'anh.ldt@gmail.com', N'56 Nguyễn Đình Chiểu, Q3, HCM'),
(N'Nguyễn Đức Bảo', '18/08/2005', 'docgia15', '123', 3, 'bao.nd@gmail.com', N'90 Xô Viết Nghệ Tĩnh, Bình Thạnh, HCM'),
(N'Nguyễn Đức Đạt', '23/10/2005', 'docgia16', '123', 3, 'dat.nd@gmail.com', N'34 Ba Tháng Hai, Q10, HCM'),
(N'Nguyễn Minh Luân', '05/05/2005', 'docgia17', '123', 3, 'luan.nm@gmail.com', N'67 Nguyễn Văn Cừ, Q5, HCM'),
(N'Phạm Hữu Lợi', '24/11/2005', 'docgia18', '123', 3, 'loi.ph@gmail.com', N'89 Trần Quốc Thảo, Q3, HCM'),
(N'Lưu Tuấn Thanh', '19/05/2005', 'docgia19', '123', 3, 'thanh.lt@gmail.com', N'12 Lê Duẩn, Q1, HCM'),
(N'Lý Phước Lợi', '03/01/2005', 'docgia20', '123', 3, 'loi.lp@gmail.com', N'45 Đinh Tiên Hoàng, Bình Thạnh, HCM'),
(N'Nguyễn Nhật Duy', '01/07/2005', 'docgia21', '123', 3, 'duy.nn@gmail.com', N'78 Phan Đình Phùng, Phú Nhuận, HCM'),
(N'Đỗ Lê Tuấn Đạt', '24/12/2005', 'docgia22', '123', 3, 'dat.dlt@gmail.com', N'23 Nguyễn Kiệm, Gò Vấp, HCM'),
(N'Ngô Thế Đức', '14/05/2005', 'docgia23', '123', 3, 'duc.nt@gmail.com', N'56 Quang Trung, Gò Vấp, HCM'),
(N'Nguyễn Thanh Phong', '12/05/2005', 'docgia24', '123', 3, 'phong.nt@gmail.com', N'90 Lê Quang Định, Bình Thạnh, HCM'),
(N'Nguyễn Việt Quang', '23/01/2005', 'docgia25', '123', 3, 'quang.nv@gmail.com', N'34 Phan Xích Long, Phú Nhuận, HCM'),
(N'Nguyễn Phạm Chí Thành', '01/01/2005', 'docgia26', '123', 3, 'thanh.npc@gmail.com', N'67 Nguyễn Văn Trỗi, Phú Nhuận, HCM'),
(N'Lê Phúc Thịnh', '28/10/2005', 'docgia27', '123', 3, 'thinh.lp@gmail.com', N'89 Hoàng Văn Thụ, Tân Bình, HCM'),
(N'Nguyễn Tuấn Kiệt', '21/05/2005', 'docgia28', '123', 3, 'kiet.nt@gmail.com', N'12 Cộng Hòa, Tân Bình, HCM'),
(N'Trịnh Lâm Bảo Ngọc', '03/11/2005', 'docgia29', '123', 3, 'ngoc.tlb@gmail.com', N'45 Trường Chinh, Tân Bình, HCM'),
(N'Lê Trọng Phong', '04/05/2005', 'docgia30', '123', 3, 'phong.lt@gmail.com', N'78 Âu Cơ, Tân Phú, HCM');
go

INSERT INTO THELOAI(TenTheLoai) VALUES (N'Công nghệ thông tin'), (N'Kinh tế'), (N'Giáo trình'), (N'Tài liệu tham khảo'), (N'Văn học'), (N'Khác');
go

INSERT INTO TUASACH(TenTuaSach, idTheLoai, DaAn) VALUES
-- TL1: CNTT (10)
(N'Lập trình C# từ cơ bản đến nâng cao', 1, 0),
(N'Nhập môn lập trình Python', 1, 0),
(N'Cấu trúc dữ liệu và giải thuật', 1, 0),
(N'Mạng máy tính căn bản', 1, 0),
(N'Hệ quản trị cơ sở dữ liệu SQL Server', 1, 0),
(N'Lập trình Java hướng đối tượng', 1, 0),
(N'An toàn và bảo mật thông tin', 1, 0),
(N'Phát triển ứng dụng Web với ASP.NET', 1, 0),
(N'Trí tuệ nhân tạo cơ bản', 1, 0),
(N'Điện toán đám mây và ứng dụng', 1, 0),
-- TL2: Kinh tế (10)
(N'Kinh tế học vi mô', 2, 0),
(N'Kinh tế học vĩ mô', 2, 0),
(N'Nguyên lý kế toán', 2, 0),
(N'Quản trị tài chính doanh nghiệp', 2, 0),
(N'Marketing căn bản', 2, 0),
(N'Kinh tế lượng ứng dụng', 2, 0),
(N'Thương mại điện tử', 2, 0),
(N'Quản trị nhân lực', 2, 0),
(N'Khởi nghiệp và đổi mới sáng tạo', 2, 0),
(N'Kinh tế quốc tế', 2, 0),
-- TL3: Giáo trình (10)
(N'Thực hành Hệ điều hành', 3, 0),
(N'Các kỹ thuật trong xử lý ngôn ngữ tự nhiên', 3, 0),
(N'Các hệ cơ sở tri thức', 3, 0),
(N'Các hệ suy diễn mờ', 3, 0),
(N'Xử lý ngôn ngữ tự nhiên', 3, 0),
(N'Lập trình C++ nâng cao', 3, 0),
(N'Hệ điều hành Linux', 3, 0),
(N'Thiết kế phần mềm hướng đối tượng', 3, 0),
(N'Kiến trúc máy tính', 3, 0),
(N'Toán rời rạc ứng dụng', 3, 0),
-- TL4: Tài liệu tham khảo (10)
(N'Cơ sở dữ liệu nâng cao', 4, 0),
(N'Khai phá dữ liệu (Data Mining)', 4, 0),
(N'Phân tích mạng xã hội và ứng dụng', 4, 0),
(N'Hướng dẫn giải bài tập xác suất và thống kê toán học', 4, 0),
(N'Học máy cơ bản và ứng dụng', 4, 0),
(N'Phân tích thiết kế giải thuật', 4, 0),
(N'Lập trình di động với Flutter', 4, 0),
(N'Quản lý dự án phần mềm', 4, 0),
(N'Tối ưu hóa và ứng dụng', 4, 0),
(N'Xử lý ảnh số và thị giác máy tính', 4, 0),
-- TL5: Văn học (10)
(N'Giết Con Chim Nhại', 5, 0),
(N'Ông Trăm Tuổi Trèo Qua Cửa Sổ Và Biến Mất', 5, 0),
(N'5 Centimet Trên Giây', 5, 0),
(N'Ông Già Và Biển Cả', 5, 0),
(N'Điều Kỳ Diệu Của Tiệm Tạp Hóa Namiya', 5, 0),
(N'Không Gia Đình', 5, 0),
(N'Bắt Trẻ Đồng Xanh', 5, 0),
(N'Nhà Giả Kim', 5, 0),
(N'Hoàng Tử Bé', 5, 0),
(N'Rừng Na Uy', 5, 0),
-- TL6: Khác (10)
(N'Kỹ năng thuyết trình hiệu quả', 6, 0),
(N'Phương pháp nghiên cứu khoa học', 6, 0),
(N'Kỹ năng làm việc nhóm', 6, 0),
(N'Tư duy phản biện', 6, 0),
(N'Kỹ năng quản lý thời gian', 6, 0),
(N'Viết báo cáo khoa học', 6, 0),
(N'Tiếng Anh chuyên ngành CNTT', 6, 0),
(N'Kỹ năng phỏng vấn xin việc', 6, 0),
(N'Đạo đức nghề nghiệp trong CNTT', 6, 0),
(N'Tâm lý học đại cương', 6, 0);
go

INSERT INTO TACGIA(TenTacGia, Email, DiaChi) VALUES
(N'Phạm Huy Hoàng', 'hoang.ph@gmail.com', N'ĐH Bách Khoa Hà Nội'),
(N'Robert Pindyck', 'pindyck@mit.edu', N'MIT, Cambridge, MA, USA'),
(N'Nguyễn Gia Tuấn Anh', 'anh.ngt@gmail.com', N'ĐH Công Nghệ Thông Tin, HCM'),
(N'Phan Đình Duy', 'duy.pd@gmail.com', N'ĐH Khoa Học Tự Nhiên, HCM'),
(N'Nguyễn Tuấn Đăng', 'dang.nt@gmail.com', N'ĐH Công Nghệ Thông Tin, HCM'),
(N'Đỗ Văn Nhơn', 'nhon.dv@gmail.com', N'ĐH Quốc Gia TP.HCM'),
(N'Trương Hải Bằng', 'bang.th@gmail.com', N'ĐH Khoa Học Tự Nhiên, HCM'),
(N'Đỗ Phúc', 'phuc.d@hcmut.edu.vn', N'ĐH Bách Khoa TP.HCM'),
(N'Dương Tôn Đảm', 'dam.dt@gmail.com', N'ĐH Công Nghệ Thông Tin, HCM'),
(N'Nguyễn Đình Thúc', 'thuc.nd@gmail.com', N'ĐH Khoa Học, Huế'),
(N'Harper Lee', 'harperlee@aol.com', N'Monroeville, Alabama, USA'),
(N'Jonas Jonasson', 'jonas@sweden.se', N'Vasteras, Thụy Điển'),
(N'Shinkai Makoto', 'makoto@shinkai.jp', N'Tokyo, Nhật Bản'),
(N'Ernest Hemingway', 'hemingway@cuba.com', N'Key West, Florida, USA'),
(N'Higashino Keigo', 'keigo@higashino.jp', N'Osaka, Nhật Bản'),
(N'Hector Malot', 'malot@france.fr', N'Paris, Pháp'),
(N'J. D. Salinger', 'salinger@jd.com', N'New York, USA'),
(N'Nguyễn Văn An', 'an.nv@gmail.com', N'ĐH Khoa Học Tự Nhiên, HCM'),
(N'Trần Thị Bình', 'binh.tt@gmail.com', N'ĐH Bách Khoa Hà Nội'),
(N'Lê Hoàng Dũng', 'dung.lh@gmail.com', N'ĐH Công Nghệ Thông Tin, HCM'),
(N'Phạm Thị Hương', 'huong.pt@gmail.com', N'ĐH Kinh Tế TP.HCM'),
(N'Vũ Đức Minh', 'minh.vd@gmail.com', N'ĐH Quốc Gia TP.HCM'),
(N'Đoàn Ngọc Phương', 'phuong.dn@gmail.com', N'ĐH Sư Phạm Kỹ Thuật, HCM'),
(N'Hoàng Thanh Sơn', 'son.ht@gmail.com', N'ĐH Bách Khoa Đà Nẵng'),
(N'Ngô Thị Thu', 'thu.nt@gmail.com', N'ĐH Ngoại Thương, Hà Nội'),
(N'Bùi Quốc Tuấn', 'tuan.bq@gmail.com', N'ĐH Cần Thơ'),
(N'Đặng Kim Xuân', 'xuan.dk@gmail.com', N'ĐH Khoa Học, Huế'),
(N'Lý Văn Hải', 'hai.lv@gmail.com', N'ĐH Hàng Hải, Hải Phòng'),
(N'Mai Thanh Hoa', 'hoa.mt@gmail.com', N'ĐH Nông Lâm, HCM');
go

INSERT INTO CT_TACGIA(idTacGia, idTuaSach) VALUES
-- TL1: CNTT (tựa 1-10, tác giả 1,18-20)
(1,1),(18,2),(18,3),(18,4),(19,5),(19,6),(20,7),(20,8),(20,9),(20,10),
-- TL2: Kinh tế (tựa 11-20, tác giả 2,21-22)
(2,11),(21,12),(21,13),(21,14),(22,15),(22,16),(22,17),(22,18),(22,19),(21,20),
-- TL3: Giáo trình (tựa 21-30, tác giả 4,5,6,7,9,10,23)
(4,21),(5,22),(6,23),(7,24),(10,25),(23,26),(23,27),(23,28),(23,29),(23,30),
-- TL4: Tài liệu tham khảo (tựa 31-40, tác giả 3,8,9,24-25)
(3,31),(8,32),(8,33),(9,34),(24,35),(24,36),(25,37),(25,38),(25,39),(25,40),
-- TL5: Văn học (tựa 41-50, tác giả 11-17,26)
(11,41),(12,42),(13,43),(14,44),(15,45),(16,46),(17,47),(26,48),(26,49),(26,50),
-- TL6: Khác (tựa 51-60, tác giả 27-29)
(27,51),(28,52),(28,53),(28,54),(29,55),(29,56),(29,57),(29,58),(29,59),(29,60);
go

INSERT INTO LOAIDOCGIA(TenLoaiDocGia) VALUES (N'Giảng viên'), (N'Sinh viên'), (N'Khác');
go

INSERT INTO DOCGIA(TenDocGia, NgaySinh, DiaChi, Email, NgayLapThe, NgayHetHan, idLoaiDocGia, TongNoHienTai, idNguoiDung) VALUES
-- 3 d?c gi? th? h?t h?n (NgayLapThe d?u 2025, NgayHetHan gi?a 2025)
(N'Danh Trường Hưng', '11/06/2005', N'12 Nguyễn Huệ, Q1, HCM', 'hung.dt@gmail.com', '02/01/2025', '02/07/2025', 2, 0, 3),
(N'Nguyễn Huỳnh Phát', '08/01/2005', N'45 Lê Lợi, Q1, HCM', 'phat.nh@gmail.com', '15/01/2025', '15/07/2025', 2, 0, 4),
(N'Trần Ngọc Kiều Duyên', '24/02/2005', N'78 Trần Hưng Đạo, Q5, HCM', 'duyen.ntk@gmail.com', '01/02/2025', '01/08/2025', 2, 0, 5),
-- 27 d?c gi? c�n h?n d?n 2027 (NgayLapThe 01/07/2026, NgayHetHan 01/01/2027)
(N'Đinh Vũ Thành', '03/01/2005', N'23 Hai Bà Trưng, Q3, HCM', 'thanh.dv@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 6),
(N'Trần Ngọc Vinh', '22/06/2005', N'56 Điện Biên Phủ, Bình Thạnh, HCM', 'vinh.tn@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 7),
(N'K'' Khoa', '25/08/2005', N'90 Nguyễn Thị Minh Khai, Q1, HCM', 'khoa.k@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 8),
(N'Trần Bửu Ngọc', '11/04/2005', N'34 Võ Văn Tần, Q3, HCM', 'ngoc.tb@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 9),
(N'Trần Trung Nguyên', '02/11/2005', N'67 Cách Mạng Tháng 8, Q10, HCM', 'nguyen.tt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 10),
(N'Nguyễn Đức Quang', '03/07/2005', N'89 Lý Tự Trọng, Q1, HCM', 'quang.nd@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 11),
(N'Nguyễn Huy Thiện', '25/09/2005', N'12 Phạm Ngũ Lão, Q1, HCM', 'thien.nh@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 12),
(N'Phạm Duy Tự', '05/12/2005', N'45 Nguyễn Trãi, Q5, HCM', 'tu.pd@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 13),
(N'Lê Thiên Nga', '09/04/2005', N'78 Lê Văn Sỹ, Q3, HCM', 'nga.lt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 14),
(N'Đoàn Văn Quang Đại', '05/09/2005', N'23 Hoàng Diệu, Q4, HCM', 'dai.dvq@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 15),
(N'Lê Dương Tuấn Anh', '30/07/2005', N'56 Nguyễn Đình Chiểu, Q3, HCM', 'anh.ldt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 16),
(N'Nguyễn Đức Bảo', '18/08/2005', N'90 Xô Viết Nghệ Tĩnh, Bình Thạnh, HCM', 'bao.nd@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 17),
(N'Nguyễn Đức Đạt', '23/10/2005', N'34 Ba Tháng Hai, Q10, HCM', 'dat.nd@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 18),
(N'Nguyễn Minh Luân', '05/05/2005', N'67 Nguyễn Văn Cừ, Q5, HCM', 'luan.nm@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 19),
(N'Phạm Hữu Lợi', '24/11/2005', N'89 Trần Quốc Thảo, Q3, HCM', 'loi.ph@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 20),
(N'Lưu Tuấn Thanh', '19/05/2005', N'12 Lê Duẩn, Q1, HCM', 'thanh.lt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 21),
(N'Lý Phước Lợi', '03/01/2005', N'45 Đinh Tiên Hoàng, Bình Thạnh, HCM', 'loi.lp@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 22),
(N'Nguyễn Nhật Duy', '01/07/2005', N'78 Phan Đình Phùng, Phú Nhuận, HCM', 'duy.nn@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 23),
(N'Đỗ Lê Tuấn Đạt', '24/12/2005', N'23 Nguyễn Kiệm, Gò Vấp, HCM', 'dat.dlt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 24),
(N'Ngô Thế Đức', '14/05/2005', N'56 Quang Trung, Gò Vấp, HCM', 'duc.nt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 25),
(N'Nguyễn Thanh Phong', '12/05/2005', N'90 Lê Quang Định, Bình Thạnh, HCM', 'phong.nt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 26),
(N'Nguyễn Việt Quang', '23/01/2005', N'34 Phan Xích Long, Phú Nhuận, HCM', 'quang.nv@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 27),
(N'Nguyễn Phạm Chí Thành', '01/01/2005', N'67 Nguyễn Văn Trỗi, Phú Nhuận, HCM', 'thanh.npc@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 28),
(N'Lê Phúc Thịnh', '28/10/2005', N'89 Hoàng Văn Thụ, Tân Bình, HCM', 'thinh.lp@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 29),
(N'Nguyễn Tuấn Kiệt', '21/05/2005', N'12 Cộng Hòa, Tân Bình, HCM', 'kiet.nt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 30),
(N'Trịnh Lâm Bảo Ngọc', '03/11/2005', N'45 Trường Chinh, Tân Bình, HCM', 'ngoc.tlb@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 31),
(N'Lê Trọng Phong', '04/05/2005', N'78 Âu Cơ, Tân Phú, HCM', 'phong.lt@gmail.com', '01/07/2026', '01/01/2027', 2, 0, 32);
go

-- SACH: 60 tua, moi tua 20 cuon
INSERT INTO SACH(idTuaSach, SoLuong, SoLuongConLai, DonGia, NamXB, NhaXB, DaAn) VALUES
(1,20,20,120000,2022,N'NXB Bách Khoa Hà Nội',0),(2,20,20,110000,2023,N'NXB Đại Học Quốc Gia',0),
(3,20,20,130000,2022,N'NXB Khoa Học Kỹ Thuật',0),(4,20,20,120000,2023,N'NXB Thông Tin Truyền Thông',0),
(5,20,20,140000,2022,N'NXB Đại Học Quốc Gia',0),(6,20,20,115000,2021,N'NXB Bách Khoa Hà Nội',0),
(7,20,20,125000,2023,N'NXB Khoa Học Kỹ Thuật',0),(8,20,20,105000,2022,N'NXB Thông Tin Truyền Thông',0),
(9,20,20,135000,2023,N'NXB Đại Học Quốc Gia',0),(10,20,20,100000,2021,N'NXB Bách Khoa Hà Nội',0),
(11,20,20,150000,2021,N'NXB Kinh Tế TP.HCM',0),(12,20,20,150000,2022,N'NXB Kinh Tế TP.HCM',0),
(13,20,20,135000,2023,N'NXB Tài Chính',0),(14,20,20,145000,2022,N'NXB Kinh Tế TP.HCM',0),
(15,20,20,120000,2021,N'NXB Đại Học Quốc Gia',0),(16,20,20,160000,2023,N'NXB Tài Chính',0),
(17,20,20,110000,2022,N'NXB Kinh Tế TP.HCM',0),(18,20,20,140000,2023,N'NXB Đại Học Quốc Gia',0),
(19,20,20,130000,2021,N'NXB Kinh Tế TP.HCM',0),(20,20,20,155000,2022,N'NXB Tài Chính',0),
(21,20,20,80000,2021,N'NXB Đại Học Quốc Gia',0),(22,20,20,110000,2019,N'NXB Khoa Học Kỹ Thuật',0),
(23,20,20,130000,2020,N'NXB Thông Tin Truyền Thông',0),(24,20,20,95000,2020,N'NXB Đại Học Quốc Gia',0),
(25,20,20,120000,2022,N'NXB Bách Khoa Hà Nội',0),(26,20,20,125000,2023,N'NXB Khoa Học Kỹ Thuật',0),
(27,20,20,110000,2022,N'NXB Đại Học Quốc Gia',0),(28,20,20,135000,2023,N'NXB Bách Khoa Hà Nội',0),
(29,20,20,120000,2021,N'NXB Đại Học Quốc Gia',0),(30,20,20,105000,2022,N'NXB Khoa Học Kỹ Thuật',0),
(31,20,20,95000,2020,N'NXB Đại Học Quốc Gia',0),(32,20,20,130000,2020,N'NXB Thông Tin Truyền Thông',0),
(33,20,20,90000,2019,N'NXB Văn Học',0),(34,20,20,105000,2020,N'NXB Trẻ',0),
(35,20,20,145000,2023,N'NXB Thông Tin Truyền Thông',0),(36,20,20,130000,2022,N'NXB Khoa Học Kỹ Thuật',0),
(37,20,20,115000,2023,N'NXB Đại Học Quốc Gia',0),(38,20,20,140000,2022,N'NXB Bách Khoa Hà Nội',0),
(39,20,20,125000,2021,N'NXB Khoa Học Kỹ Thuật',0),(40,20,20,150000,2023,N'NXB Đại Học Quốc Gia',0),
(41,20,20,90000,2019,N'NXB Văn Học',0),(42,20,20,105000,2020,N'NXB Trẻ',0),
(43,20,20,70000,2018,N'NXB Hội Nhà Văn',0),(44,20,20,65000,2017,N'NXB Văn Học',0),
(45,20,20,140000,2021,N'NXB Hội Nhà Văn',0),(46,20,20,88000,2019,N'NXB Văn Học',0),
(47,20,20,80000,2020,N'NXB Trẻ',0),(48,20,20,95000,2022,N'NXB Văn Học',0),
(49,20,20,85000,2021,N'NXB Trẻ',0),(50,20,20,105000,2023,N'NXB Hội Nhà Văn',0),
(51,20,20,80000,2022,N'NXB Đại Học Quốc Gia',0),(52,20,20,75000,2021,N'NXB Khoa Học Kỹ Thuật',0),
(53,20,20,85000,2023,N'NXB Trẻ',0),(54,20,20,80000,2022,N'NXB Đại Học Quốc Gia',0),
(55,20,20,70000,2021,N'NXB Bách Khoa Hà Nội',0),(56,20,20,75000,2023,N'NXB Khoa Học Kỹ Thuật',0),
(57,20,20,90000,2022,N'NXB Đại Học Quốc Gia',0),(58,20,20,80000,2021,N'NXB Trẻ',0),
(59,20,20,75000,2023,N'NXB Bách Khoa Hà Nội',0),(60,20,20,85000,2022,N'NXB Đại Học Quốc Gia',0);
go

-- PHIEU NHAP SACH
INSERT INTO PHIEUNHAPSACH(TongTien, NgayNhap) VALUES (0, '10/09/2024');
DECLARE @soPN INT = SCOPE_IDENTITY();
DECLARE @s INT = 1;
WHILE @s <= 60
BEGIN
    DECLARE @dg INT, @tt INT;
    SELECT @dg = DonGia FROM SACH WHERE id = @s;
    SET @tt = @dg * 20;
    INSERT INTO CT_PHIEUNHAP(SoPhieuNhap, idSach, DonGia, ThanhTien, SoLuongNhap)
    VALUES (@soPN, @s, @dg, @tt, 20);
    SET @s = @s + 1;
END
UPDATE PHIEUNHAPSACH SET TongTien = (SELECT SUM(ThanhTien) FROM CT_PHIEUNHAP WHERE SoPhieuNhap = @soPN)
WHERE SoPhieuNhap = @soPN;
go

-- CUON SACH: 20 cuon / sach (60 x 20 = 1200)
DECLARE @sid INT = 1;
WHILE @sid <= 60
BEGIN
    DECLARE @i INT = 0;
    WHILE @i < 20
    BEGIN
        INSERT INTO CUONSACH(idSach, TinhTrang, DaAn) VALUES (@sid, 1, 0);
        SET @i = @i + 1;
    END
    SET @sid = @sid + 1;
END
go

INSERT INTO PHIEUTHU(idDocGia, SoTienThu, NgayLap) VALUES (3, 4000, '20/02/2025');
go

UPDATE DOCGIA SET TongNoHienTai = 0 WHERE ID = 3;
go

INSERT INTO BCLUOTMUONTHEOTHELOAI(Thang, Nam, TongSoLuotMuon) VALUES (1, 2025, 2), (5, 2025, 3);
go

INSERT INTO CT_BCLUOTMUONTHEOTHELOAI(idBaoCao, idTheLoai, SoLuotMuon, TiLe) VALUES
(1, 1, 1, 50.00), (1, 3, 1, 50.00), (2, 4, 1, 33.33), (2, 5, 2, 66.67);
go

INSERT INTO THAMSO(TuoiToiThieu, TuoiToiDa, ThoiHanThe, KhoangCachXuatBan, SoSachMuonToiDa, SoNgayMuonToiDa, DonGiaPhat, AD_QDKTTienThu) VALUES (18, 55, 6, 8, 5, 4, 1000, 1);
go

-- Dữ liệu mượn PHIEUMUONTRA + NHUCAUDOC
INSERT INTO PHIEUMUONTRA(idDocGia, idCuonSach, NgayMuon, HanTra, NgayTra, SoTienPhat) VALUES
(1, 1, '05/01/2025', '09/01/2025', '08/01/2025', 0),
(2, 21, '10/01/2025', '14/01/2025', '13/01/2025', 0),
(3, 41, '12/02/2025', '16/02/2025', '20/02/2025', 4000),
(4, 61, '01/03/2025', '05/03/2025', NULL, 0),
(5, 81, '10/05/2025', '14/05/2025', NULL, 0);
go


USE QLTV;
GO

-- Thêm cột TrangThai nếu chưa tồn tại
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('NHUCAUDOC') AND name='TrangThai')
    ALTER TABLE NHUCAUDOC ADD TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Đang chờ';
GO

-- 2. Tạo lại view với ISNULL (chống NULL)
IF OBJECT_ID('v_LichSuMuonTra','V') IS NOT NULL DROP VIEW v_LichSuMuonTra
go
CREATE VIEW v_LichSuMuonTra AS
SELECT pmt.SoPhieuMuonTra, dg.MaDocGia, dg.TenDocGia, cs.MaCuonSach, ts.TenTuaSach AS TenSach,
    pmt.NgayMuon, pmt.HanTra, pmt.NgayTra,
    CASE WHEN pmt.NgayTra IS NULL AND pmt.HanTra < GETDATE() THEN N'Quá hạn'
         WHEN pmt.NgayTra IS NULL THEN N'Đang mượn' ELSE N'Đã trả' END AS TrangThai,
    ISNULL(pmt.SoTienPhat, 0) AS SoTienPhat
FROM PHIEUMUONTRA pmt JOIN CUONSACH cs ON pmt.idCuonSach = cs.id
    JOIN SACH s ON cs.idSach = s.id
    JOIN TUASACH ts ON s.idTuaSach = ts.id
    JOIN DOCGIA dg ON pmt.idDocGia = dg.ID
go

-- 3. Thêm dữ liệu mẫu nếu bảng trống
IF NOT EXISTS (SELECT 1 FROM PHIEUMUONTRA)
INSERT INTO PHIEUMUONTRA(idDocGia, idCuonSach, NgayMuon, HanTra, NgayTra, SoTienPhat) VALUES
(1,1,'05/01/2025','09/01/2025','08/01/2025',0),
(2,21,'10/01/2025','14/01/2025','13/01/2025',0),
(3,41,'12/02/2025','16/02/2025','20/02/2025',4000),
(4,61,'01/03/2025','05/03/2025',NULL,0),
(5,81,'10/05/2025','14/05/2025',NULL,0)
go

IF NOT EXISTS (SELECT 1 FROM NHUCAUDOC)
INSERT INTO NHUCAUDOC(idDocGia, idSach, GhiChu) VALUES
(1,1,N'Muốn đọc cuốn này để ôn thi'),
(1,5,N'Sách hay, nên đọc trong hè'),
(2,11,N'Tài liệu tham khảo cho môn Kinh tế vi mô'),
(3,41,N'Tiểu thuyết yêu thích')
go

USE QLTV
DELETE FROM NHUCAUDOC
go
INSERT INTO NHUCAUDOC(idDocGia, idSach, GhiChu) VALUES
(1, 1, N'Muốn đọc cuốn này để ôn thi'),
(1, 5, N'Sách hay, nên đọc trong hè'),
(2, 11, N'Tài liệu tham khảo cho môn Kinh tế vi mô'),
(3, 41, N'Tiểu thuyết yêu thích')
go

-- Tạo phiếu mượn quá hạn cho độc giả DG0001
INSERT INTO PHIEUMUONTRA(idDocGia, idCuonSach, NgayMuon, HanTra, NgayTra, SoTienPhat) 
VALUES (1, 27, '07/10/2026', '07/11/2026', NULL, 0)

-- Tìm CUONSACH còn trống
SELECT TOP 5 cs.id, cs.MaCuonSach, s.MaSach, ts.TenTuaSach
FROM CUONSACH cs JOIN SACH s ON cs.idSach = s.id JOIN TUASACH ts ON s.idTuaSach = ts.id
WHERE cs.TinhTrang = 1

USE QLTV;
GO

-- Xóa phiếu mượn của độc giả có ID = 1 và idCuonSach = 27
DELETE FROM PHIEUMUONTRA 
WHERE idDocGia = 1 AND idCuonSach = 27;
GO

UPDATE DOCGIA SET TongNoHienTai = 
    ISNULL((SELECT SUM(pmt.SoTienPhat) FROM PHIEUMUONTRA pmt WHERE pmt.idDocGia = DOCGIA.ID), 0)
  - ISNULL((SELECT SUM(pt.SoTienThu)   FROM PHIEUTHU pt     WHERE pt.idDocGia = DOCGIA.ID), 0)
GO

-- Lost Book feature: add DaMat column to PHIEUMUONTRA
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('PHIEUMUONTRA') AND name='DaMat')
    ALTER TABLE PHIEUMUONTRA ADD DaMat INT NOT NULL DEFAULT 0;
GO

-- Lost Book feature: add HeSoPhatMatSach column to THAMSO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('THAMSO') AND name='HeSoPhatMatSach')
    ALTER TABLE THAMSO ADD HeSoPhatMatSach INT NOT NULL DEFAULT 3;
GO

-- Recreate v_LichSuMuonTra view with lost status + DonGia
IF OBJECT_ID('v_LichSuMuonTra','V') IS NOT NULL DROP VIEW v_LichSuMuonTra;
GO
CREATE VIEW v_LichSuMuonTra AS
SELECT pmt.SoPhieuMuonTra, dg.MaDocGia, dg.TenDocGia, cs.MaCuonSach, ts.TenTuaSach AS TenSach,
    pmt.NgayMuon, pmt.HanTra, pmt.NgayTra,
    CASE WHEN pmt.DaMat = 1 THEN N'Đã mất'
         WHEN pmt.NgayTra IS NULL AND pmt.HanTra < GETDATE() THEN N'Quá hạn'
         WHEN pmt.NgayTra IS NULL THEN N'Đang mượn'
         ELSE N'Đã trả' END AS TrangThai,
    ISNULL(pmt.SoTienPhat, 0) AS SoTienPhat,
    ISNULL(s.DonGia, 0) AS DonGia,
    ISNULL(pmt.DaMat, 0) AS DaMat
FROM PHIEUMUONTRA pmt JOIN CUONSACH cs ON pmt.idCuonSach = cs.id
    JOIN SACH s ON cs.idSach = s.id
    JOIN TUASACH ts ON s.idTuaSach = ts.id
    JOIN DOCGIA dg ON pmt.idDocGia = dg.ID;
GO

-- Reservation (Đặt trước) feature
CREATE TABLE DATTRUOC (
    id INT IDENTITY(1,1) PRIMARY KEY,
    idDocGia INT NOT NULL REFERENCES DOCGIA(ID),
    idSach INT NOT NULL REFERENCES SACH(id),
    NgayDat DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Đang chờ',
    GhiChu NVARCHAR(MAX) NULL
);
GO

-- Sample reservation data
INSERT INTO DATTRUOC(idDocGia, idSach, GhiChu) VALUES
(4, 1, N'Cần gấp để ôn thi'),
(4, 11, N'Mượn cho học kỳ sau'),
(5, 41, N'Sách hay muốn đọc');
GO

-- NHAXUATBAN entity (Publisher)
CREATE TABLE NHAXUATBAN (
    id INT IDENTITY(1,1) PRIMARY KEY,
    MaNXB AS CAST('NXB' + RIGHT('000' + CAST(id AS VARCHAR(3)), 3) AS CHAR(6)) PERSISTED,
    TenNXB NVARCHAR(MAX) NOT NULL,
    DiaChi NVARCHAR(200) NULL,
    Email VARCHAR(100) NULL,
    DienThoai VARCHAR(20) NULL
);
GO

INSERT INTO NHAXUATBAN(TenNXB, DiaChi) VALUES
(N'Đại Học Quốc Gia', N'TP. Hồ Chí Minh'),
(N'Bách Khoa Hà Nội', N'Hà Nội'),
(N'Khoa Học Kỹ Thuật', N'Hà Nội'),
(N'Thông Tin Truyền Thông', N'TP. Hồ Chí Minh'),
(N'Kinh Tế TP.HCM', N'TP. Hồ Chí Minh'),
(N'Tài Chính', N'Hà Nội'),
(N'Văn Học', N'TP. Hồ Chí Minh'),
(N'Trẻ', N'TP. Hồ Chí Minh'),
(N'Hội Nhà Văn', N'Hà Nội');
GO

USE QLTV;
GO

INSERT INTO PHANQUYEN(idNhomNguoiDung, idChucNang) VALUES (2,6), (2,7);
GO