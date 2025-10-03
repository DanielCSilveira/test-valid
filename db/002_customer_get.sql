CREATE OR REPLACE PROCEDURE customer_get(  p_id UUID)
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT id, name, email, active, created_at
    FROM customers
    WHERE active = TRUE
    AND id = p_id;
END;
$$;