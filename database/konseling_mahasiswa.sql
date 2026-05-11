CREATE DATABASE IF NOT EXISTS sistem_konseling_mahasiswa
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE sistem_konseling_mahasiswa;

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS sesi_konseling;
DROP TABLE IF EXISTS mahasiswa;
DROP TABLE IF EXISTS konselor;
DROP TABLE IF EXISTS kategori_masalah;
DROP TABLE IF EXISTS users;

SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE mahasiswa (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nim VARCHAR(20) NOT NULL UNIQUE,
    nama VARCHAR(100) NOT NULL,
    program_studi VARCHAR(100) NOT NULL,
    angkatan YEAR NULL,
    jenis_kelamin ENUM('L','P') NULL,
    no_hp VARCHAR(20) NULL,
    email VARCHAR(120) NULL,
    alamat TEXT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE konselor (
    id INT AUTO_INCREMENT PRIMARY KEY,
    kode_konselor VARCHAR(20) NOT NULL UNIQUE,
    nama VARCHAR(100) NOT NULL,
    jabatan VARCHAR(100) NOT NULL,
    no_hp VARCHAR(20) NULL,
    email VARCHAR(120) NULL,
    bidang_keahlian VARCHAR(150) NULL,
    aktif TINYINT(1) NOT NULL DEFAULT 1,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Users table for authentication/authorization
-- Roles: Admin, Mahasiswa, Konselor
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role ENUM('Admin','Mahasiswa','Konselor') NOT NULL,
    mahasiswa_id INT NULL,
    konselor_id INT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_users_mahasiswa FOREIGN KEY (mahasiswa_id) REFERENCES mahasiswa(id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_users_konselor FOREIGN KEY (konselor_id) REFERENCES konselor(id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE INDEX idx_users_username ON users(username);

CREATE TABLE kategori_masalah (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nama_kategori VARCHAR(100) NOT NULL UNIQUE,
    deskripsi TEXT NULL,
    aktif TINYINT(1) NOT NULL DEFAULT 1,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE sesi_konseling (
    id INT AUTO_INCREMENT PRIMARY KEY,
    kode_sesi VARCHAR(30) NOT NULL UNIQUE,
    mahasiswa_id INT NOT NULL,
    konselor_id INT NOT NULL,
    kategori_masalah_id INT NULL,
    tanggal_sesi DATE NOT NULL,
    waktu_mulai TIME NULL,
    waktu_selesai TIME NULL,
    status ENUM('Dijadwalkan','Berlangsung','Selesai','Dibatalkan') NOT NULL DEFAULT 'Dijadwalkan',
    topik VARCHAR(150) NOT NULL,
    catatan TEXT NULL,
    tindak_lanjut TEXT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_sesi_mahasiswa FOREIGN KEY (mahasiswa_id) REFERENCES mahasiswa(id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT fk_sesi_konselor FOREIGN KEY (konselor_id) REFERENCES konselor(id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT fk_sesi_kategori FOREIGN KEY (kategori_masalah_id) REFERENCES kategori_masalah(id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE INDEX idx_mahasiswa_nama ON mahasiswa(nama);
CREATE INDEX idx_mahasiswa_nim ON mahasiswa(nim);
CREATE INDEX idx_konselor_nama ON konselor(nama);
CREATE INDEX idx_sesi_tanggal ON sesi_konseling(tanggal_sesi);
CREATE INDEX idx_sesi_status ON sesi_konseling(status);
CREATE INDEX idx_sesi_mahasiswa ON sesi_konseling(mahasiswa_id);
CREATE INDEX idx_sesi_konselor ON sesi_konseling(konselor_id);

INSERT INTO mahasiswa (nim, nama, program_studi, angkatan, jenis_kelamin, no_hp, email, alamat) VALUES
('2311001', 'Aisyah Putri', 'Sistem Informasi', 2023, 'P', '081234567801', 'aisyah@example.com', 'Bandung'),
('2311002', 'Dimas Pratama', 'Keperawatan', 2023, 'L', '081234567802', 'dimas@example.com', 'Jakarta'),
('2311003', 'Siti Rahma', 'Psikologi', 2022, 'P', '081234567803', 'siti@example.com', 'Yogyakarta'),
('2311004', 'Raka Maulana', 'Teknik Informatika', 2024, 'L', '081234567804', 'raka@example.com', 'Surabaya'),
('2311005', 'Nadia Zahra', 'Manajemen', 2024, 'P', '081234567805', 'nadia@example.com', 'Semarang'),
('2311006', 'Bayu Saputra', 'Hukum', 2023, 'L', '081234567806', 'bayu@example.com', 'Medan');

INSERT INTO konselor (kode_konselor, nama, jabatan, no_hp, email, bidang_keahlian) VALUES
('K-001', 'dr. Maya Sari', 'Konselor Akademik', '081234567811', 'maya@example.com', 'Akademik dan adaptasi studi'),
('K-002', 'Arif Nugroho', 'Konselor Karier', '081234567812', 'arif@example.com', 'Karier dan magang'),
('K-003', 'Nanda Putri', 'Konselor Pribadi', '081234567813', 'nanda@example.com', 'Pribadi dan sosial');

INSERT INTO kategori_masalah (nama_kategori, deskripsi) VALUES
('Akademik', 'Masalah yang berkaitan dengan nilai, tugas, dan jadwal kuliah'),
('Karier', 'Masalah yang berkaitan dengan rencana karier dan magang'),
('Pribadi', 'Masalah pribadi, emosional, dan sosial');

INSERT INTO sesi_konseling (kode_sesi, mahasiswa_id, konselor_id, kategori_masalah_id, tanggal_sesi, waktu_mulai, waktu_selesai, status, topik, catatan, tindak_lanjut) VALUES
('S-2026-0001', 1, 1, 1, '2026-04-12', '09:00:00', '09:45:00', 'Selesai', 'Penyesuaian kuliah', 'Mahasiswa butuh dukungan adaptasi perkuliahan', 'Follow-up 2 minggu'),
('S-2026-0002', 2, 2, 2, '2026-04-13', '10:00:00', '10:30:00', 'Selesai', 'Rencana studi', 'Membahas mata kuliah pilihan dan magang', 'Kirim daftar peluang magang'),
('S-2026-0003', 3, 3, 3, '2026-04-14', '13:00:00', '13:30:00', 'Dijadwalkan', 'Manajemen stres', 'Topik awal dari mahasiswa', 'Konfirmasi kehadiran sebelum sesi');

INSERT IGNORE INTO users (username, password_hash, role, mahasiswa_id, konselor_id) VALUES
('admin', '$2b$12$2.xgvE3auPGBnW7eATk7..QGsCT8JNDKZk.obpexy/gZFJxjVxTJW', 'Admin', NULL, NULL),
('konselor', '$2b$12$wAEgxJ2SvTwzkOM/bhVldOv7dXPSmK949Bv1Goi4oG2walPXWUPA2', 'Konselor', NULL, 1),
('konselor2', '$2b$12$wAEgxJ2SvTwzkOM/bhVldOv7dXPSmK949Bv1Goi4oG2walPXWUPA2', 'Konselor', NULL, 2),
('konselor3', '$2b$12$wAEgxJ2SvTwzkOM/bhVldOv7dXPSmK949Bv1Goi4oG2walPXWUPA2', 'Konselor', NULL, 3);
