CREATE OR REPLACE PROCEDURE order_list(
)
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT id, customer_id, amount, status, created_at
    FROM orders;
END;
$$;