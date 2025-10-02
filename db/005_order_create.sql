CREATE OR REPLACE PROCEDURE order_create(
    p_customer_id UUID,
    p_amount NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO orders (customer_id, amount, status)
    VALUES (p_customer_id, p_amount, 'PENDING');
END;
$$;