-- ========================================
-- Seed data for testing
-- ========================================

-- Extensão para gerar UUID random
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- 1. Generate 1000 customers
INSERT INTO customers (id, name, email, active, created_at)
SELECT 
    gen_random_uuid(),
    'Customer ' || i,
    'customer' || i || '@example.com',
    TRUE,
    NOW() - (random() * interval '365 days')
FROM generate_series(1, 1000) AS s(i);

-- 2. Generate orders
-- Random number of orders per customer, median ~5-7
DO $$
DECLARE
    c RECORD;
    num_orders INT;
    i INT;
BEGIN
    FOR c IN SELECT id FROM customers LOOP
        -- 0~15 orders, mas <1% customers com 0
        IF random() < 0.01 THEN
            num_orders := 0; -- <1% chance
        ELSE
            -- distribuição aproximadamente mediana 6
            num_orders := (floor(random() * 15 + 1))::int; 
        END IF;

        FOR i IN 1..num_orders LOOP
            INSERT INTO orders (id, customer_id, amount, status, created_at)
            VALUES (
                gen_random_uuid(),
                c.id,
                (random() * 1000 + 50)::numeric(12,2), -- valor 50 a 1050
                'PENDING',
                NOW() - (random() * interval '180 days')
            );
        END LOOP;
    END LOOP;
END;
$$;
