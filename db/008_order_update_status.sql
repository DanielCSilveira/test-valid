    CREATE OR REPLACE PROCEDURE order_update_status(
        p_order_id UUID,
        p_status TEXT
    )
    LANGUAGE plpgsql
    AS $$
    BEGIN
        UPDATE orders SET status = p_status WHERE id = p_order_id;
    END;
    $$;