-- MySQL dump 10.13  Distrib 8.0.45, for Linux (x86_64)
--
-- Host: localhost    Database: convenience_store
-- ------------------------------------------------------
-- Server version	8.0.45-0ubuntu0.24.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `audit_logs`
--

DROP TABLE IF EXISTS `audit_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `audit_logs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `action` varchar(50) NOT NULL,
  `table_name` varchar(50) NOT NULL,
  `record_id` int DEFAULT NULL,
  `old_values` json DEFAULT NULL,
  `new_values` json DEFAULT NULL,
  `ip_address` varchar(45) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_audit_logs_user` (`user_id`),
  KEY `idx_audit_logs_table_record` (`table_name`,`record_id`),
  KEY `idx_audit_logs_created_at` (`created_at`),
  CONSTRAINT `fk_audit_logs_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `audit_logs`
--

LOCK TABLES `audit_logs` WRITE;
/*!40000 ALTER TABLE `audit_logs` DISABLE KEYS */;
INSERT INTO `audit_logs` VALUES (1,1,'create','products',12,NULL,'{\"name\": \"Coca Cola lon 330ml\", \"cost_price\": 8000, \"selling_price\": 12000}','192.168.1.100','2026-05-12 08:50:12'),(2,1,'update','products',16,'{\"selling_price\": 55000}','{\"selling_price\": 58000}','192.168.1.100','2026-05-12 08:50:12'),(3,6,'create','purchase_orders',8,NULL,'{\"supplier_id\": 1, \"total_amount\": 4700000}','192.168.1.101','2026-05-12 08:50:12'),(4,6,'update','purchase_orders',8,'{\"status\": \"pending\"}','{\"status\": \"received\"}','192.168.1.101','2026-05-12 08:50:12'),(5,7,'create','orders',5,NULL,'{\"total_amount\": 61500, \"payment_method\": \"cash\"}','192.168.1.102','2026-05-12 08:50:12'),(6,1,'update','promotions',1,'{\"is_active\": false}','{\"is_active\": true}','192.168.1.100','2026-05-12 08:50:12'),(7,6,'delete','customers',NULL,'{\"id\": 99, \"name\": \"Test customer\"}',NULL,'192.168.1.101','2026-05-12 08:50:12'),(8,1,'create','products',12,NULL,'{\"name\": \"Coca Cola lon 330ml\", \"cost_price\": 8000, \"selling_price\": 12000}','192.168.1.100','2026-05-12 08:50:20'),(9,1,'update','products',16,'{\"selling_price\": 55000}','{\"selling_price\": 58000}','192.168.1.100','2026-05-12 08:50:20'),(10,6,'create','purchase_orders',8,NULL,'{\"supplier_id\": 1, \"total_amount\": 4700000}','192.168.1.101','2026-05-12 08:50:20'),(11,6,'update','purchase_orders',8,'{\"status\": \"pending\"}','{\"status\": \"received\"}','192.168.1.101','2026-05-12 08:50:20'),(12,7,'create','orders',5,NULL,'{\"total_amount\": 61500, \"payment_method\": \"cash\"}','192.168.1.102','2026-05-12 08:50:20'),(13,1,'update','promotions',1,'{\"is_active\": false}','{\"is_active\": true}','192.168.1.100','2026-05-12 08:50:20'),(14,6,'delete','customers',NULL,'{\"id\": 99, \"name\": \"Test customer\"}',NULL,'192.168.1.101','2026-05-12 08:50:20');
/*!40000 ALTER TABLE `audit_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categories` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `parent_id` int DEFAULT NULL,
  `icon` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_categories_parent_id` (`parent_id`),
  KEY `idx_categories_is_active` (`is_active`),
  CONSTRAINT `fk_categories_parent` FOREIGN KEY (`parent_id`) REFERENCES `categories` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Danh mục sản phẩm (hỗ trợ danh mục cha-con)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
INSERT INTO `categories` VALUES (1,'Đồ uống',NULL,'drink.png',1,'2026-05-12 01:19:05'),(2,'Thực phẩm',NULL,'food.png',1,'2026-05-12 01:19:05'),(3,'Đồ dùng cá nhân',NULL,'personal.png',1,'2026-05-12 01:19:05'),(4,'Nước ngọt',1,'soda.png',1,'2026-05-12 01:19:05'),(5,'Cà phê',1,'coffee.png',1,'2026-05-12 01:19:05'),(6,'Mì gói',2,'noodle.png',1,'2026-05-12 01:19:05'),(7,'Bánh kẹo',2,'snack.png',1,'2026-05-12 01:19:05'),(8,'Đồ uống',NULL,'drink.png',1,'2026-05-12 01:34:14'),(9,'Thực phẩm',NULL,'food.png',1,'2026-05-12 01:34:14'),(10,'Đồ dùng cá nhân',NULL,'personal.png',1,'2026-05-12 01:34:14'),(25,'Thuốc lá & Phụ kiện',NULL,'cigarette.png',1,'2026-05-12 08:18:16'),(26,'Văn phòng phẩm',NULL,'office.png',1,'2026-05-12 08:18:16');
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customers` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `phone` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `loyalty_points` int NOT NULL DEFAULT '0',
  `total_spent` decimal(14,2) NOT NULL DEFAULT '0.00',
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_customers_phone` (`phone`),
  KEY `idx_customers_name` (`name`),
  KEY `idx_customers_email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Khách hàng thành viên (tích điểm)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (1,'Nguyễn Văn An','0921111111','an.nguyen@gmail.com',166,1671000.00,1,'2026-05-12 08:18:16','2026-05-12 08:46:11'),(2,'Trần Thị Bích','0921111112','bich.tran@yahoo.com',85,850000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(3,'Lê Minh Cường','0921111113',NULL,330,3309500.00,1,'2026-05-12 08:18:16','2026-05-12 08:47:34'),(4,'Phạm Thị Dung','0921111114','dung.pham@gmail.com',45,450000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(5,'Hoàng Văn Em','0921111115',NULL,10,100000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(6,'Vũ Thị Giang','0921111116','giang.vu@hotmail.com',200,2000000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(7,'Đặng Văn Hùng','0921111117',NULL,75,750000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(8,'Bùi Thị Ngọc','0921111118','ngoc.bui@gmail.com',443,4447000.00,1,'2026-05-12 08:18:16','2026-05-12 08:47:34'),(9,'Trịnh Văn Khoa','0921111119',NULL,25,250000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(10,'Mai Thị Lan','0921111120','lan.mai@yahoo.com',180,1800000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(11,'Phan Văn Minh','0921111121',NULL,60,600000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(12,'Đỗ Thị Nga','0921111122','nga.do@gmail.com',95,950000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(13,'Lý Văn Phú','0921111123',NULL,250,2500000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(14,'Trương Thị Quỳnh','0921111124','quynh.truong@gmail.com',35,350000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(15,'Hồ Văn Sơn','0921111125',NULL,155,1550000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(16,'Đinh Thị Tâm','0921111126','tam.dinh@yahoo.com',70,700000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(17,'Lương Văn Út','0921111127',NULL,20,200000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(18,'Cao Thị Vy','0921111128','vy.cao@gmail.com',130,1300000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(19,'Tô Văn Xuân','0921111129',NULL,50,500000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16'),(20,'Hà Thị Yến','0921111130','yen.ha@hotmail.com',90,900000.00,1,'2026-05-12 08:18:16','2026-05-12 08:18:16');
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `daily_cash_register`
--

DROP TABLE IF EXISTS `daily_cash_register`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `daily_cash_register` (
  `id` int NOT NULL AUTO_INCREMENT,
  `staff_id` int NOT NULL,
  `shift_date` date NOT NULL,
  `opening_balance` decimal(14,2) NOT NULL DEFAULT '0.00',
  `closing_balance` decimal(14,2) DEFAULT NULL,
  `expected_cash` decimal(14,2) DEFAULT NULL,
  `cash_difference` decimal(14,2) DEFAULT NULL,
  `opened_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `closed_at` datetime DEFAULT NULL,
  `note` text,
  PRIMARY KEY (`id`),
  KEY `idx_daily_cash_register_date` (`shift_date`),
  KEY `idx_daily_cash_register_staff` (`staff_id`),
  CONSTRAINT `fk_daily_cash_register_staff` FOREIGN KEY (`staff_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `daily_cash_register`
--

LOCK TABLES `daily_cash_register` WRITE;
/*!40000 ALTER TABLE `daily_cash_register` DISABLE KEYS */;
INSERT INTO `daily_cash_register` VALUES (1,7,'2026-05-10',500000.00,815000.00,815000.00,0.00,'2026-05-10 07:00:00','2026-05-10 22:00:00','Ca bình thường, khớp tiền'),(2,2,'2026-05-11',500000.00,736500.00,740500.00,-4000.00,'2026-05-11 07:00:00','2026-05-11 22:00:00','Thiếu 4k - đang kiểm tra'),(3,7,'2026-05-12',500000.00,NULL,NULL,NULL,'2026-05-12 07:00:00',NULL,'Ca đang diễn ra');
/*!40000 ALTER TABLE `daily_cash_register` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inventory` (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` int NOT NULL DEFAULT '0',
  `min_quantity` int NOT NULL DEFAULT '0',
  `expiry_date` date DEFAULT NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_inventory_product_variant` (`product_id`,`variant_id`),
  KEY `idx_inventory_product_id` (`product_id`),
  KEY `idx_inventory_variant_id` (`variant_id`),
  KEY `idx_inventory_expiry_date` (`expiry_date`),
  CONSTRAINT `fk_inventory_product` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_inventory_variant` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `chk_inventory_qty` CHECK (((`quantity` >= 0) and (`min_quantity` >= 0)))
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventory`
--

LOCK TABLES `inventory` WRITE;
/*!40000 ALTER TABLE `inventory` DISABLE KEYS */;
INSERT INTO `inventory` VALUES (33,12,NULL,343,30,'2026-12-15','2026-05-12 08:47:34'),(34,13,NULL,177,20,'2026-12-30','2026-05-12 08:46:11'),(35,14,NULL,246,30,'2026-11-20','2026-05-12 08:47:34'),(36,15,NULL,188,30,'2026-11-25','2026-05-12 08:47:34'),(37,16,NULL,39,10,'2027-06-30','2026-05-12 08:46:11'),(38,17,NULL,58,15,'2026-08-15','2026-05-12 08:47:34'),(39,18,NULL,200,50,'2026-06-10','2026-05-12 08:36:27'),(40,19,NULL,545,50,'2027-03-15','2026-05-12 08:42:21'),(41,20,NULL,380,50,'2027-03-15','2026-05-12 08:38:08'),(42,21,NULL,97,30,'2027-02-20','2026-05-12 08:47:34'),(43,22,NULL,140,40,'2027-02-25','2026-05-12 08:47:34'),(44,23,NULL,177,20,'2026-09-10','2026-05-12 08:46:11'),(45,24,NULL,137,15,'2026-10-15','2026-05-12 08:47:34'),(46,25,NULL,70,20,'2026-08-30','2026-05-12 08:36:27'),(47,26,NULL,268,30,'2027-01-15','2026-05-12 08:47:34'),(48,27,NULL,195,50,'2027-05-20','2026-05-12 08:46:11'),(49,28,NULL,147,40,'2026-12-31','2026-05-12 08:47:34'),(50,29,NULL,90,25,'2027-04-15','2026-05-12 08:36:27'),(51,30,NULL,50,10,'2027-08-30','2026-05-12 08:36:27'),(52,31,NULL,69,15,'2027-06-20','2026-05-12 08:47:34'),(53,32,NULL,39,10,'2028-01-15','2026-05-12 08:47:34'),(54,33,NULL,79,20,'2028-03-30','2026-05-12 08:47:34'),(55,34,NULL,49,15,'2028-02-20','2026-05-12 08:47:34'),(56,35,NULL,100,30,NULL,'2026-05-12 08:36:27'),(57,36,NULL,60,20,NULL,'2026-05-12 08:36:27');
/*!40000 ALTER TABLE `inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `loyalty_transactions`
--

DROP TABLE IF EXISTS `loyalty_transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `loyalty_transactions` (
  `id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int NOT NULL,
  `order_id` int DEFAULT NULL,
  `points` int NOT NULL,
  `transaction_type` enum('earn','redeem','expire','adjust') NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_loyalty_transactions_order` (`order_id`),
  KEY `idx_loyalty_transactions_customer` (`customer_id`),
  KEY `idx_loyalty_transactions_created_at` (`created_at`),
  CONSTRAINT `fk_loyalty_transactions_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_loyalty_transactions_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `loyalty_transactions`
--

LOCK TABLES `loyalty_transactions` WRITE;
/*!40000 ALTER TABLE `loyalty_transactions` DISABLE KEYS */;
INSERT INTO `loyalty_transactions` VALUES (1,1,6,5,'earn','Tích điểm từ đơn #6','2026-05-12 08:46:11'),(2,1,6,5,'earn','Tích điểm từ đơn #6','2026-05-12 08:46:11'),(3,1,6,3,'earn','Tích điểm từ đơn #6','2026-05-12 08:46:11'),(4,1,6,3,'earn','Tích điểm từ đơn #6','2026-05-12 08:46:11'),(5,3,7,6,'earn','Tích điểm từ đơn #7','2026-05-12 08:47:34'),(6,3,7,2,'earn','Tích điểm từ đơn #7','2026-05-12 08:47:34'),(7,3,7,2,'earn','Tích điểm từ đơn #7','2026-05-12 08:47:34'),(8,8,9,6,'earn','Tích điểm từ đơn #9','2026-05-12 08:47:34'),(9,8,9,5,'earn','Tích điểm từ đơn #9','2026-05-12 08:47:34'),(10,8,9,14,'earn','Tích điểm từ đơn #9','2026-05-12 08:47:34'),(11,8,9,4,'earn','Tích điểm từ đơn #9','2026-05-12 08:47:34'),(12,8,9,4,'earn','Tích điểm từ đơn #9','2026-05-12 08:47:34');
/*!40000 ALTER TABLE `loyalty_transactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order_items`
--

DROP TABLE IF EXISTS `order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` int NOT NULL,
  `unit_price` decimal(12,2) NOT NULL,
  `discount` decimal(12,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `idx_order_items_order_id` (`order_id`),
  KEY `idx_order_items_product_id` (`product_id`),
  KEY `idx_order_items_variant_id` (`variant_id`),
  CONSTRAINT `fk_order_items_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_order_items_product` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_order_items_variant` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `chk_order_items_qty` CHECK (((`quantity` > 0) and (`unit_price` >= 0) and (`discount` >= 0)))
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Chi tiết các sản phẩm trong đơn bán hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_items`
--

LOCK TABLES `order_items` WRITE;
/*!40000 ALTER TABLE `order_items` DISABLE KEYS */;
INSERT INTO `order_items` VALUES (1,5,12,NULL,2,12000.00,0.00),(2,5,19,NULL,5,4500.00,0.00),(3,5,23,NULL,1,16000.00,1000.00),(4,6,13,NULL,3,17000.00,0.00),(5,6,16,NULL,1,58000.00,0.00),(6,6,23,NULL,2,16000.00,0.00),(7,6,27,NULL,5,6000.00,0.00),(8,7,12,NULL,5,12000.00,0.00),(9,7,21,NULL,3,8500.00,0.00),(10,7,26,NULL,2,12000.00,0.00),(11,8,14,NULL,4,12000.00,0.00),(12,8,15,NULL,2,12000.00,0.00),(13,8,28,NULL,3,10000.00,0.00),(14,8,31,NULL,1,17000.00,0.00),(15,8,33,NULL,1,22000.00,0.00),(16,9,17,NULL,2,32000.00,0.00),(17,9,22,NULL,10,5500.00,5000.00),(18,9,24,NULL,3,48000.00,4000.00),(19,9,32,NULL,1,48000.00,0.00),(20,9,34,NULL,1,45000.00,0.00);
/*!40000 ALTER TABLE `order_items` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 */ /*!50003 TRIGGER `trg_order_item_inserted` AFTER INSERT ON `order_items` FOR EACH ROW BEGIN
    DECLARE v_staff_id INT;
    DECLARE v_customer_id INT;
    DECLARE v_line_total DECIMAL(14,2);
    DECLARE v_loyalty_rate INT DEFAULT 10000;
    
    SELECT staff_id, customer_id 
    INTO v_staff_id, v_customer_id
    FROM orders WHERE id = NEW.order_id;
    
    -- Trừ inventory (xử lý NULL đúng)
    UPDATE inventory 
    SET quantity = quantity - NEW.quantity
    WHERE product_id = NEW.product_id 
      AND (variant_id = NEW.variant_id 
           OR (variant_id IS NULL AND NEW.variant_id IS NULL));
    
    -- Ghi stock_movements
    INSERT INTO stock_movements 
        (product_id, variant_id, quantity, movement_type, 
         reference_type, reference_id, staff_id, note)
    VALUES 
        (NEW.product_id, NEW.variant_id, -NEW.quantity, 'sale',
         'order', NEW.order_id, v_staff_id, NULL);
    
    -- Tích điểm nếu là khách thành viên
    IF v_customer_id IS NOT NULL THEN
        SET v_line_total = NEW.quantity * NEW.unit_price - NEW.discount;
        
        UPDATE customers 
        SET loyalty_points = loyalty_points + FLOOR(v_line_total / v_loyalty_rate),
            total_spent = total_spent + v_line_total
        WHERE id = v_customer_id;
        
        INSERT INTO loyalty_transactions 
            (customer_id, order_id, points, transaction_type, description)
        VALUES 
            (v_customer_id, NEW.order_id, 
             FLOOR(v_line_total / v_loyalty_rate),
             'earn',
             CONCAT('Tích điểm từ đơn #', NEW.order_id));
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `order_promotions`
--

DROP TABLE IF EXISTS `order_promotions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_promotions` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `promotion_id` int NOT NULL,
  `discount_value` decimal(12,2) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_order_promotions` (`order_id`,`promotion_id`),
  KEY `idx_order_promotions_order` (`order_id`),
  KEY `idx_order_promotions_promotion` (`promotion_id`),
  CONSTRAINT `fk_order_promotions_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_order_promotions_promotion` FOREIGN KEY (`promotion_id`) REFERENCES `promotions` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_promotions`
--

LOCK TABLES `order_promotions` WRITE;
/*!40000 ALTER TABLE `order_promotions` DISABLE KEYS */;
INSERT INTO `order_promotions` VALUES (1,6,7,20000.00,'2026-05-12 08:49:54'),(2,9,6,34700.00,'2026-05-12 08:49:54'),(3,8,7,20000.00,'2026-05-12 08:49:54');
/*!40000 ALTER TABLE `order_promotions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `id` int NOT NULL AUTO_INCREMENT,
  `staff_id` int NOT NULL,
  `customer_id` int DEFAULT NULL,
  `subtotal` decimal(14,2) NOT NULL DEFAULT '0.00',
  `discount_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `tax_amount` decimal(12,2) NOT NULL DEFAULT '0.00',
  `total_amount` decimal(14,2) NOT NULL DEFAULT '0.00',
  `payment_method` enum('cash','card','e_wallet') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'cash',
  `payment_status` enum('paid','pending','refunded') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `order_status` enum('draft','completed','cancelled','returned') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'completed',
  `note` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_orders_staff_id` (`staff_id`),
  KEY `idx_orders_customer_id` (`customer_id`),
  KEY `idx_orders_payment_status` (`payment_status`),
  KEY `idx_orders_created_at` (`created_at`),
  CONSTRAINT `fk_orders_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_orders_staff` FOREIGN KEY (`staff_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `chk_orders_amounts` CHECK (((`subtotal` >= 0) and (`discount_amount` >= 0) and (`tax_amount` >= 0) and (`total_amount` >= 0) and (`discount_amount` <= `subtotal`)))
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Đơn bán hàng tại quầy';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,2,NULL,50000.00,0.00,4000.00,54000.00,'cash','paid','completed',NULL,'2026-05-12 01:53:41'),(2,2,NULL,120000.00,0.00,9600.00,129600.00,'card','paid','completed',NULL,'2026-05-12 01:53:41'),(3,2,NULL,35000.00,0.00,2800.00,37800.00,'e_wallet','paid','completed','Thanh toán Momo','2026-05-12 01:53:41'),(4,2,NULL,200000.00,20000.00,14400.00,194400.00,'cash','paid','completed','Khách VIP giảm 10%','2026-05-12 01:53:41'),(5,7,NULL,61500.00,0.00,0.00,61500.00,'cash','paid','completed','Khách vãng lai','2026-05-10 09:30:00'),(6,7,1,171000.00,0.00,0.00,171000.00,'card','paid','completed',NULL,'2026-05-10 14:15:00'),(7,2,3,109500.00,0.00,0.00,109500.00,'e_wallet','paid','completed','Thanh toán Momo','2026-05-11 10:00:00'),(8,2,NULL,141000.00,0.00,0.00,141000.00,'cash','paid','completed',NULL,'2026-05-11 16:45:00'),(9,7,8,347000.00,0.00,0.00,347000.00,'cash','paid','completed','Khách VIP','2026-05-12 08:20:00');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_variants`
--

DROP TABLE IF EXISTS `product_variants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_variants` (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `variant_name` varchar(100) NOT NULL,
  `barcode` varchar(50) DEFAULT NULL,
  `price_adjustment` decimal(12,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_product_variants_barcode` (`barcode`),
  KEY `idx_product_variants_product_id` (`product_id`),
  CONSTRAINT `fk_product_variants_product` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_variants`
--

LOCK TABLES `product_variants` WRITE;
/*!40000 ALTER TABLE `product_variants` DISABLE KEYS */;
INSERT INTO `product_variants` VALUES (1,12,'330ml','8934567890101',0.00),(2,12,'Lốc 6 lon','8934567890111',60000.00),(3,19,'Gói lẻ',NULL,0.00),(4,19,'Thùng 30 gói','8934567890319',100000.00),(5,16,'Hộp 20 gói','8934567890201',0.00),(6,16,'Hộp 50 gói','8934567890211',60000.00),(7,17,'Có đường',NULL,0.00),(8,17,'Không đường',NULL,0.00),(41,12,'330ml',NULL,0.00),(42,12,'Lốc 6 lon',NULL,60000.00),(43,19,'Gói lẻ',NULL,0.00),(44,19,'Thùng 30 gói',NULL,100000.00),(45,16,'Hộp 20 gói',NULL,0.00),(46,16,'Hộp 50 gói',NULL,60000.00),(47,17,'Có đường',NULL,0.00),(48,17,'Không đường',NULL,0.00);
/*!40000 ALTER TABLE `product_variants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `id` int NOT NULL AUTO_INCREMENT,
  `category_id` int NOT NULL,
  `supplier_id` int DEFAULT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `barcode` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `sku` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `unit` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'cái',
  `cost_price` decimal(12,2) NOT NULL DEFAULT '0.00',
  `selling_price` decimal(12,2) NOT NULL DEFAULT '0.00',
  `image_url` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_products_barcode` (`barcode`),
  UNIQUE KEY `uq_products_sku` (`sku`),
  KEY `idx_products_category_id` (`category_id`),
  KEY `idx_products_supplier_id` (`supplier_id`),
  KEY `idx_products_is_active` (`is_active`),
  KEY `idx_products_name` (`name`),
  CONSTRAINT `fk_products_category` FOREIGN KEY (`category_id`) REFERENCES `categories` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `chk_products_prices` CHECK (((`cost_price` >= 0) and (`selling_price` >= 0)))
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Sản phẩm trong cửa hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,4,1,'Coca Cola lon 330ml','8934567890123','SP001','lon',8000.00,12000.00,NULL,'Nước ngọt có ga',1,'2026-05-12 01:33:33','2026-05-12 08:09:42'),(2,4,1,'Pepsi lon 330ml','8934567890124','SP002','lon',7500.00,11000.00,NULL,'Nước ngọt có ga',1,'2026-05-12 01:33:33','2026-05-12 08:09:42'),(3,5,3,'Cà phê G7 hộp 20 gói','8934567890125','SP003','hộp',45000.00,58000.00,NULL,'Cà phê hòa tan',1,'2026-05-12 01:33:33','2026-05-12 08:09:42'),(4,6,2,'Mì Hảo Hảo tôm chua','8934567890126','SP004','gói',3500.00,4500.00,NULL,'Mì ăn liền',1,'2026-05-12 01:33:33','2026-05-12 08:09:42'),(5,7,NULL,'Bánh Oreo gói 137g','8934567890127','SP005','gói',12000.00,16000.00,NULL,'Bánh quy kem',1,'2026-05-12 01:33:33','2026-05-12 08:09:42'),(12,4,1,'Coca Cola lon 330ml','8934567890101','NG001','lon',8000.00,12000.00,NULL,'Nước ngọt có ga',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(13,4,1,'Coca Cola chai 500ml','8934567890102','NG002','chai',11000.00,17000.00,NULL,'Nước ngọt có ga',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(14,4,1,'Sprite lon 330ml','8934567890103','NG003','lon',8000.00,12000.00,NULL,'Nước ngọt vị chanh',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(15,4,1,'Fanta lon 330ml','8934567890104','NG004','lon',8000.00,12000.00,NULL,'Nước ngọt vị cam',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(16,5,3,'Cà phê G7 hộp 20 gói','8934567890201','CF001','hộp',45000.00,58000.00,NULL,'Cà phê hòa tan 3in1',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(17,5,7,'Sữa Vinamilk hộp 1L','8934567890202','SV001','hộp',25000.00,32000.00,NULL,'Sữa tươi tiệt trùng',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(18,5,7,'Sữa chua Vinamilk 100g','8934567890203','SV002','hộp',5000.00,7000.00,NULL,'Sữa chua có đường',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(19,6,2,'Mì Hảo Hảo tôm chua cay','8934567890301','MG001','gói',3500.00,4500.00,NULL,'Mì ăn liền vị tôm',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(20,6,2,'Mì Hảo Hảo vị bò','8934567890302','MG002','gói',3500.00,4500.00,NULL,'Mì ăn liền vị bò',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(21,6,8,'Mì Omachi sườn hầm','8934567890303','MG003','gói',6500.00,8500.00,NULL,'Mì cao cấp',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(22,6,8,'Mì Kokomi đại','8934567890304','MG004','gói',4000.00,5500.00,NULL,'Mì gói lớn',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(23,7,11,'Bánh Oreo gói 137g','8934567890401','BK001','gói',12000.00,16000.00,NULL,'Bánh quy kem',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(24,7,11,'Bánh Chocopie hộp 12 cái','8934567890402','BK002','hộp',35000.00,48000.00,NULL,'Bánh xốp phủ chocolate',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(25,7,10,'Bánh Goute hộp','8934567890403','BK003','hộp',15000.00,22000.00,NULL,'Bánh quy bơ',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(26,7,11,'Kẹo Mentos','8934567890404','BK004','gói',8000.00,12000.00,NULL,'Kẹo bạc hà',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(27,4,7,'Nước suối Aquafina 500ml','8934567890501','NS001','chai',4000.00,6000.00,NULL,'Nước tinh khiết',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(28,4,7,'Trà xanh không độ chai','8934567890502','TR001','chai',7000.00,10000.00,NULL,'Trà xanh đóng chai',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(29,4,8,'Nước tăng lực Sting','8934567890503','NS002','lon',8500.00,12500.00,NULL,'Nước tăng lực',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(30,9,8,'Nước mắm Nam Ngư 500ml','8934567890601','GV001','chai',18000.00,25000.00,NULL,'Nước mắm cao cấp',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(31,9,8,'Tương ớt Chinsu','8934567890602','GV002','chai',12000.00,17000.00,NULL,'Tương ớt cay',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(32,10,9,'Dầu gội Clear 170ml','8934567890701','DG001','chai',35000.00,48000.00,NULL,'Dầu gội trị gàu',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(33,10,9,'Kem đánh răng PS 100g','8934567890702','KD001','tuýp',15000.00,22000.00,NULL,'Kem đánh răng',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(34,10,9,'Sữa tắm Lifebuoy 250ml','8934567890703','ST001','chai',32000.00,45000.00,NULL,'Sữa tắm diệt khuẩn',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(35,25,8,'Vinataba bao 20 điếu','8934567890801','TL001','bao',18000.00,25000.00,NULL,'Thuốc lá nội địa',1,'2026-05-12 08:22:29','2026-05-12 08:22:29'),(36,25,8,'Marlboro Red','8934567890802','TL002','bao',28000.00,35000.00,NULL,'Thuốc lá nhập khẩu',1,'2026-05-12 08:22:29','2026-05-12 08:22:29');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `promotions`
--

DROP TABLE IF EXISTS `promotions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `promotions` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `type` enum('percent','fixed','buy_x_get_y') COLLATE utf8mb4_unicode_ci NOT NULL,
  `value` decimal(10,2) NOT NULL,
  `min_order_value` decimal(12,2) NOT NULL DEFAULT '0.00',
  `start_date` date NOT NULL,
  `end_date` date NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `idx_promotions_is_active` (`is_active`),
  KEY `idx_promotions_dates` (`start_date`,`end_date`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Các chương trình khuyến mãi';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `promotions`
--

LOCK TABLES `promotions` WRITE;
/*!40000 ALTER TABLE `promotions` DISABLE KEYS */;
INSERT INTO `promotions` VALUES (6,'Giảm 10% cho đơn từ 200k','percent',10.00,200000.00,'2026-05-01','2026-05-31',1),(7,'Giảm 20.000đ cho đơn từ 100k','fixed',20000.00,100000.00,'2026-05-01','2026-12-31',1),(8,'Mua 2 tặng 1 - Mì Hảo Hảo','buy_x_get_y',21.00,0.00,'2026-05-10','2026-05-20',1),(9,'Khuyến mãi hè - giảm 15%','percent',15.00,150000.00,'2026-06-01','2026-08-31',1),(10,'Black Friday - giảm 30%','percent',30.00,500000.00,'2025-11-28','2025-11-30',0),(11,'Tết 2026 - giảm 50.000đ','fixed',50000.00,300000.00,'2026-01-25','2026-02-15',0);
/*!40000 ALTER TABLE `promotions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_order_items`
--

DROP TABLE IF EXISTS `purchase_order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_order_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` int NOT NULL,
  `unit_cost` decimal(12,2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_purchase_order_items_order_id` (`order_id`),
  KEY `idx_purchase_order_items_product_id` (`product_id`),
  KEY `fk_purchase_order_items_variant` (`variant_id`),
  CONSTRAINT `fk_purchase_order_items_order` FOREIGN KEY (`order_id`) REFERENCES `purchase_orders` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_purchase_order_items_product` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_purchase_order_items_variant` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Chi tiết các sản phẩm trong đơn nhập hàng';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_order_items`
--

LOCK TABLES `purchase_order_items` WRITE;
/*!40000 ALTER TABLE `purchase_order_items` DISABLE KEYS */;
INSERT INTO `purchase_order_items` VALUES (1,8,12,NULL,200,8000.00),(2,8,13,NULL,100,11000.00),(3,8,14,NULL,150,8000.00),(4,8,15,NULL,100,8000.00),(5,9,19,NULL,300,3500.00),(6,9,20,NULL,200,3500.00),(7,10,23,NULL,100,12000.00),(8,10,24,NULL,80,35000.00),(9,10,26,NULL,150,8000.00);
/*!40000 ALTER TABLE `purchase_order_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_orders`
--

DROP TABLE IF EXISTS `purchase_orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_orders` (
  `id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int NOT NULL,
  `staff_id` int NOT NULL,
  `total_amount` decimal(14,2) NOT NULL DEFAULT '0.00',
  `status` enum('pending','received','cancelled') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `note` text COLLATE utf8mb4_unicode_ci,
  `ordered_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `received_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_purchase_orders_supplier_id` (`supplier_id`),
  KEY `idx_purchase_orders_staff_id` (`staff_id`),
  KEY `idx_purchase_orders_status` (`status`),
  KEY `idx_purchase_orders_ordered_at` (`ordered_at`),
  CONSTRAINT `fk_purchase_orders_staff` FOREIGN KEY (`staff_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_purchase_orders_supplier` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Đơn nhập hàng từ nhà cung cấp';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_orders`
--

LOCK TABLES `purchase_orders` WRITE;
/*!40000 ALTER TABLE `purchase_orders` DISABLE KEYS */;
INSERT INTO `purchase_orders` VALUES (1,1,1,5000000.00,'received','Đặt thùng Coca và Pepsi','2026-04-01 09:00:00','2026-04-02 14:30:00'),(2,2,2,2500000.00,'received','Nhập mì Hảo Hảo các vị','2026-04-05 10:15:00','2026-04-06 09:00:00'),(3,3,1,1800000.00,'pending','Đặt cà phê G7 hộp','2026-05-10 11:30:00',NULL),(4,1,2,3200000.00,'cancelled','Hủy do nhà cung cấp hết hàng','2026-05-08 14:00:00',NULL),(5,1,6,0.00,'pending','Đặt nước ngọt tháng 5','2026-05-01 09:00:00',NULL),(6,2,6,0.00,'pending','Đặt mì gói các loại','2026-05-03 10:30:00',NULL),(7,11,6,0.00,'pending','Đặt bánh kẹo cho lễ hè','2026-05-05 14:00:00',NULL),(8,1,6,4700000.00,'received','Đặt nước ngọt tháng 5','2026-05-01 09:00:00','2026-05-12 08:31:15'),(9,2,6,1750000.00,'received','Đặt mì gói các loại','2026-05-03 10:30:00','2026-05-12 08:38:08'),(10,11,6,5200000.00,'received','Đặt bánh kẹo cho lễ hè','2026-05-05 14:00:00','2026-05-12 08:38:41');
/*!40000 ALTER TABLE `purchase_orders` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 */ /*!50003 TRIGGER `trg_purchase_received` BEFORE UPDATE ON `purchase_orders` FOR EACH ROW BEGIN
    IF NEW.status = 'received' AND OLD.status != 'received' THEN
        IF NEW.received_at IS NULL THEN
            SET NEW.received_at = NOW();
        END IF;
        
        -- Insert stock_movements
        INSERT INTO stock_movements 
            (product_id, variant_id, quantity, movement_type, 
             reference_type, reference_id, staff_id, note)
        SELECT 
            poi.product_id, poi.variant_id, poi.quantity,
            'purchase', 'purchase_order', NEW.id, NEW.staff_id,
            CONCAT('Nhập hàng từ PO #', NEW.id)
        FROM purchase_order_items poi
        WHERE poi.order_id = NEW.id;
        
        -- UPDATE inventory hiện có
        UPDATE inventory i
        INNER JOIN purchase_order_items poi ON poi.order_id = NEW.id
        SET i.quantity = i.quantity + poi.quantity
        WHERE i.product_id = poi.product_id
          AND (i.variant_id = poi.variant_id 
               OR (i.variant_id IS NULL AND poi.variant_id IS NULL));
        
        -- INSERT dòng mới cho product chưa có
        INSERT INTO inventory (product_id, variant_id, quantity, min_quantity)
        SELECT poi.product_id, poi.variant_id, poi.quantity, 10
        FROM purchase_order_items poi
        WHERE poi.order_id = NEW.id
          AND NOT EXISTS (
              SELECT 1 FROM inventory i
              WHERE i.product_id = poi.product_id
                AND (i.variant_id = poi.variant_id 
                     OR (i.variant_id IS NULL AND poi.variant_id IS NULL))
          );
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `settings`
--

DROP TABLE IF EXISTS `settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `settings` (
  `setting_key` varchar(100) NOT NULL,
  `setting_value` text NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`setting_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `settings`
--

LOCK TABLES `settings` WRITE;
/*!40000 ALTER TABLE `settings` DISABLE KEYS */;
INSERT INTO `settings` VALUES ('currency','VND','Đơn vị tiền tệ','2026-05-12 02:31:25'),('low_stock_alert','true','Bật cảnh báo hàng sắp hết','2026-05-12 02:31:25'),('loyalty_rate','10000','X đồng = 1 điểm','2026-05-12 02:31:25'),('store_name','Cửa hàng Tiện lợi ABC','Tên cửa hàng','2026-05-12 02:31:25'),('tax_rate','8','Thuế VAT (%)','2026-05-12 02:31:25');
/*!40000 ALTER TABLE `settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_movements`
--

DROP TABLE IF EXISTS `stock_movements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_movements` (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `variant_id` int DEFAULT NULL,
  `quantity` int NOT NULL,
  `movement_type` enum('purchase','sale','return','adjustment','damage','transfer') NOT NULL,
  `reference_type` enum('purchase_order','order','manual') DEFAULT NULL,
  `reference_id` int DEFAULT NULL,
  `staff_id` int NOT NULL,
  `note` text,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_stock_movements_variant` (`variant_id`),
  KEY `fk_stock_movements_staff` (`staff_id`),
  KEY `idx_stock_movements_product` (`product_id`),
  KEY `idx_stock_movements_type` (`movement_type`),
  KEY `idx_stock_movements_reference` (`reference_type`,`reference_id`),
  KEY `idx_stock_movements_created_at` (`created_at`),
  CONSTRAINT `fk_stock_movements_product` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_stock_movements_staff` FOREIGN KEY (`staff_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_stock_movements_variant` FOREIGN KEY (`variant_id`) REFERENCES `product_variants` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_movements`
--

LOCK TABLES `stock_movements` WRITE;
/*!40000 ALTER TABLE `stock_movements` DISABLE KEYS */;
INSERT INTO `stock_movements` VALUES (1,12,NULL,200,'purchase','purchase_order',8,6,'Nhập hàng từ PO #8','2026-05-12 08:31:15'),(2,13,NULL,100,'purchase','purchase_order',8,6,'Nhập hàng từ PO #8','2026-05-12 08:31:15'),(3,14,NULL,150,'purchase','purchase_order',8,6,'Nhập hàng từ PO #8','2026-05-12 08:31:15'),(4,15,NULL,100,'purchase','purchase_order',8,6,'Nhập hàng từ PO #8','2026-05-12 08:31:15'),(8,19,NULL,300,'purchase','purchase_order',9,6,'Nhập hàng từ PO #9','2026-05-12 08:38:08'),(9,20,NULL,200,'purchase','purchase_order',9,6,'Nhập hàng từ PO #9','2026-05-12 08:38:08'),(11,23,NULL,100,'purchase','purchase_order',10,6,'Nhập hàng từ PO #10','2026-05-12 08:38:41'),(12,24,NULL,80,'purchase','purchase_order',10,6,'Nhập hàng từ PO #10','2026-05-12 08:38:41'),(13,26,NULL,150,'purchase','purchase_order',10,6,'Nhập hàng từ PO #10','2026-05-12 08:38:41'),(14,12,NULL,-2,'sale','order',5,7,NULL,'2026-05-12 08:42:21'),(15,19,NULL,-5,'sale','order',5,7,NULL,'2026-05-12 08:42:21'),(16,23,NULL,-1,'sale','order',5,7,NULL,'2026-05-12 08:42:21'),(17,13,NULL,-3,'sale','order',6,7,NULL,'2026-05-12 08:46:11'),(18,16,NULL,-1,'sale','order',6,7,NULL,'2026-05-12 08:46:11'),(19,23,NULL,-2,'sale','order',6,7,NULL,'2026-05-12 08:46:11'),(20,27,NULL,-5,'sale','order',6,7,NULL,'2026-05-12 08:46:11'),(21,12,NULL,-5,'sale','order',7,2,NULL,'2026-05-12 08:47:34'),(22,21,NULL,-3,'sale','order',7,2,NULL,'2026-05-12 08:47:34'),(23,26,NULL,-2,'sale','order',7,2,NULL,'2026-05-12 08:47:34'),(24,14,NULL,-4,'sale','order',8,2,NULL,'2026-05-12 08:47:34'),(25,15,NULL,-2,'sale','order',8,2,NULL,'2026-05-12 08:47:34'),(26,28,NULL,-3,'sale','order',8,2,NULL,'2026-05-12 08:47:34'),(27,31,NULL,-1,'sale','order',8,2,NULL,'2026-05-12 08:47:34'),(28,33,NULL,-1,'sale','order',8,2,NULL,'2026-05-12 08:47:34'),(29,17,NULL,-2,'sale','order',9,7,NULL,'2026-05-12 08:47:34'),(30,22,NULL,-10,'sale','order',9,7,NULL,'2026-05-12 08:47:34'),(31,24,NULL,-3,'sale','order',9,7,NULL,'2026-05-12 08:47:34'),(32,32,NULL,-1,'sale','order',9,7,NULL,'2026-05-12 08:47:34'),(33,34,NULL,-1,'sale','order',9,7,NULL,'2026-05-12 08:47:34');
/*!40000 ALTER TABLE `stock_movements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `contact_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `phone` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `address` text COLLATE utf8mb4_unicode_ci,
  `tax_code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_suppliers_tax_code` (`tax_code`),
  KEY `idx_suppliers_name` (`name`),
  KEY `idx_suppliers_phone` (`phone`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Nhà cung cấp sản phẩm';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers` VALUES (1,'Công ty TNHH Coca-Cola Việt Nam','Nguyễn Văn A','0901234567','contact@coca-cola.vn','485 Hà Nội, Phường Linh Trung, TP. Thủ Đức, TP.HCM','0300792451'),(2,'Công ty CP Acecook Việt Nam','Trần Thị B','0908765432','info@acecookvietnam.vn','Lô II-3, Đường số 11, KCN Tân Bình, Q. Tân Phú, TP.HCM','0301002854'),(3,'Công ty CP Tập đoàn Trung Nguyên','Lê Văn C','0912345678','trungnguyen@coffee.vn','82-84 Bùi Thị Xuân, P. Bến Thành, Q.1, TP.HCM','5900298776'),(4,'Cửa hàng tạp hóa Cô Bảy','Bảy','0987654321',NULL,'123 Lê Lợi, Quận 1, TP.HCM',NULL),(7,'Công ty CP Vinamilk','Lê Văn Cường','0903333333','cskh@vinamilk.com.vn','10 Tân Trào, Q.7, TP.HCM','0300100003'),(8,'Công ty CP Masan Consumer','Phạm Thị Dung','0904444444','masan@masan.com.vn','23 Lê Duẩn, Q.1, TP.HCM','0300100004'),(9,'Công ty Unilever Việt Nam','Hoàng Văn Em','0905555555','unilever@unilever.vn','156 Nguyễn Lương Bằng, Q.7','0300100005'),(10,'Công ty CP Bibica','Vũ Thị Giang','0906666666','bibica@bibica.com.vn','443 Lý Thường Kiệt, Q.10','0300100006'),(11,'Công ty Mondelez Kinh Đô','Đặng Văn Hùng','0907777777','kinhdo@mondelez.vn','141 Nguyễn Du, Q.1, TP.HCM','0300100007'),(12,'Hộ kinh doanh Cô Tám','Tám','0908888888',NULL,'12 Nguyễn Thị Minh Khai, Q.1',NULL);
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `full_name` varchar(100) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `role` enum('Admin','Manager','Cashier','Staff') NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin01','123456','Nguyễn Thanh Quang','0777010738','Admin',1,'2026-05-12 02:26:42'),(2,'staff01','123456','Nguyễn Thành Nhân','0978882622','Staff',1,'2026-05-12 02:26:42'),(6,'manager01','$2y$10$hashedpwd_manager','Lê Văn Quản Lý','0911111111','Manager',1,'2026-05-12 08:18:16'),(7,'cashier03','$2y$10$hashedpwd_cashier','Nguyễn Thị Thu','0912222222','Cashier',1,'2026-05-12 08:18:16');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'convenience_store'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-12  9:06:42
