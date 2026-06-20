USE convenience_store;

START TRANSACTION;

-- Seed data had duplicate products with the same name/unit but different barcodes.
-- Keep the newer product rows used by the complete sample dataset:
--   Coca Cola lon 330ml: keep 12, remove 1
--   Ca phe G7 hop 20 goi: keep 16, remove 3
--   Banh Oreo goi 137g: keep 23, remove 5

-- Merge inventory quantities into the kept product rows.
UPDATE inventory keep_i
JOIN inventory dup_i ON dup_i.product_id = 1 AND dup_i.variant_id IS NULL
SET keep_i.quantity = keep_i.quantity + dup_i.quantity,
    keep_i.min_quantity = GREATEST(keep_i.min_quantity, dup_i.min_quantity)
WHERE keep_i.product_id = 12 AND keep_i.variant_id IS NULL;

UPDATE inventory keep_i
JOIN inventory dup_i ON dup_i.product_id = 3 AND dup_i.variant_id IS NULL
SET keep_i.quantity = keep_i.quantity + dup_i.quantity,
    keep_i.min_quantity = GREATEST(keep_i.min_quantity, dup_i.min_quantity)
WHERE keep_i.product_id = 16 AND keep_i.variant_id IS NULL;

UPDATE inventory keep_i
JOIN inventory dup_i ON dup_i.product_id = 5 AND dup_i.variant_id IS NULL
SET keep_i.quantity = keep_i.quantity + dup_i.quantity,
    keep_i.min_quantity = GREATEST(keep_i.min_quantity, dup_i.min_quantity)
WHERE keep_i.product_id = 23 AND keep_i.variant_id IS NULL;

-- Re-point historical and stock reference rows to the kept products.
UPDATE order_items SET product_id = 12 WHERE product_id = 1;
UPDATE purchase_order_items SET product_id = 12 WHERE product_id = 1;
UPDATE stock_movements SET product_id = 12 WHERE product_id = 1;
UPDATE product_variants SET product_id = 12 WHERE product_id = 1;

UPDATE order_items SET product_id = 16 WHERE product_id = 3;
UPDATE purchase_order_items SET product_id = 16 WHERE product_id = 3;
UPDATE stock_movements SET product_id = 16 WHERE product_id = 3;
UPDATE product_variants SET product_id = 16 WHERE product_id = 3;

UPDATE order_items SET product_id = 23 WHERE product_id = 5;
UPDATE purchase_order_items SET product_id = 23 WHERE product_id = 5;
UPDATE stock_movements SET product_id = 23 WHERE product_id = 5;
UPDATE product_variants SET product_id = 23 WHERE product_id = 5;

-- Remove duplicate inventory/product rows after references have been moved.
DELETE FROM inventory WHERE product_id IN (1, 3, 5);
DELETE FROM products WHERE id IN (1, 3, 5);

COMMIT;

