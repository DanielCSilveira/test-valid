CREATE OR REPLACE FUNCTION order_create_and_return_id(
    p_customer_id UUID,
    p_amount NUMERIC
)
-- Definimos o tipo de retorno como UUID (o ID gerado)
RETURNS UUID 
LANGUAGE plpgsql
AS $$
DECLARE
    new_order_id UUID;
BEGIN
    -- 1. Executa a inserção
    INSERT INTO orders (customer_id, amount, status)
    VALUES (p_customer_id, p_amount, 'PENDING')
    -- 2. Captura o ID gerado pelo banco na variável new_order_id
    RETURNING id INTO new_order_id;
    
    -- 3. Retorna o ID capturado
    RETURN new_order_id;
END;
$$;
