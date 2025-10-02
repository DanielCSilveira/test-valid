CREATE OR REPLACE PROCEDURE customer_delete(
    p_id UUID
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE customers SET active = FALSE WHERE id = p_id;
END;
$$;