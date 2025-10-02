CREATE OR REPLACE PROCEDURE order_delete(
    p_id UUID
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE orders SET status = 'CANCELLED' WHERE id = p_id;
END;
$$;