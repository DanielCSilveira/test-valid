CREATE OR REPLACE PROCEDURE customer_get_by_mail(  p_mail varchar(100))
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT id, name, email, active, created_at
    FROM customers
    WHERE email = p_mail;
    
END;
$$;