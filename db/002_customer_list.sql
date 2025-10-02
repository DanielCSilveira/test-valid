CREATE OR REPLACE PROCEDURE customer_list(  )
LANGUAGE plpgsql
AS $$
BEGIN
        SELECT id, name, email, active, created_at
    FROM customers
    WHERE active = TRUE;
END;
$$;