CREATE OR REPLACE PROCEDURE customer_create(
    p_name VARCHAR,
    p_email VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO customers (name, email) VALUES (p_name, p_email);
END;
$$;