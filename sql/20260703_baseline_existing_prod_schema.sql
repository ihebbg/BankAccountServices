-- One-time baseline for an existing production database.
-- Use this only if the schema already exists in RDS but __EFMigrationsHistory is empty or missing.

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20250803152912_Initial', '8.0.0'),
('20250810122143_userRole', '8.0.0'),
('20250821123607_ajouterRefreshToken', '8.0.0'),
('20250821123818_ajouterRefreshTokenUser', '8.0.0'),
('20250821124100_ajouterRefreshTokenUserForeign', '8.0.0'),
('20250821124448_ajouterRefreshTokenUserForeignKey', '8.0.0');
