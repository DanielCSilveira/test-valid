import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { orderService } from "../services/orderService";
import { customerService } from "../services/customerService";
import { type CustomerDto } from "../types/customer";
import AutocompleteSelect from "../components/AutocompleteSelect";
import "./Cliente.css";

const PedidoNew = () => {
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState({
    customerId: "",
    amount: "",
  });
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadCustomers();
  }, []);

  const loadCustomers = async () => {
    try {
      const data = await customerService.getAll();
      setCustomers(data);
    } catch (err) {
      setError("Erro ao carregar clientes");
      console.error(err);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      setLoading(true);
      setError(null);

      await orderService.create({
        customerId: formData.customerId,
        amount: parseFloat(formData.amount),
      });

      navigate("/pedido");
    } catch (err) {
      setError("Erro ao criar pedido");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleCustomerChange = (value: string) => {
    setFormData((prev) => ({
      ...prev,
      customerId: value,
    }));
  };

  const customerOptions = customers.map((customer) => ({
    value: customer.id,
    label: `${customer.name} - ${customer.email}`,
  }));

  return (
    <div className="cliente-container">
      <h2 className="white">Novo Pedido</h2>

      {error && <div className="error">{error}</div>}

      <form onSubmit={handleSubmit} className="customer-form">
        <div className="form-group">
          <label htmlFor="customerId">Cliente:</label>
          <AutocompleteSelect
            options={customerOptions}
            value={formData.customerId}
            onChange={handleCustomerChange}
            placeholder="Selecione um cliente"
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="amount">Valor:</label>
          <input
            type="number"
            id="amount"
            name="amount"
            value={formData.amount}
            onChange={handleChange}
            step="0.01"
            min="0"
            required
          />
        </div>

        <div className="form-actions">
          <button 
            type="button" 
            onClick={() => navigate("/pedido")}
            className="btn-cancel"
            disabled={loading}
          >
            Cancelar
          </button>
          <button 
            type="submit" 
            className="btn-submit"
            disabled={loading}
          >
            {loading ? "Salvando..." : "Salvar"}
          </button>
        </div>
      </form>
    </div>
  );
};

export default PedidoNew;
