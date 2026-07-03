START TRANSACTION;

ALTER TABLE `ba_user` MODIFY COLUMN `password` varchar(500) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `ba_user` MODIFY COLUMN `login` varchar(100) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `ba_role` MODIFY COLUMN `role_name` varchar(100) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `ba_refresh_token` MODIFY COLUMN `token` varchar(500) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `ba_customer` MODIFY COLUMN `email` varchar(100) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `ba_bank_account` MODIFY COLUMN `currency` varchar(50) CHARACTER SET utf8mb4 NOT NULL;

CREATE TABLE `ba_authentification` (
    `id_authentification` bigint NOT NULL AUTO_INCREMENT,
    `login` datetime(6) NOT NULL,
    CONSTRAINT `PK_ba_authentification` PRIMARY KEY (`id_authentification`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260703081654_NewTableAuthentification', '8.0.0');

COMMIT;

