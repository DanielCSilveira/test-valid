import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { customerService } from '../services/customerService';
import './Cliente.css';

const ClienteForm = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditMode = id !== 'new';

  const [formData, setFormData] = useState({
    name: '',
    email: '',
    isActive: true,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isEditMode && id) {
      loadCustomer(id);
    }
  }, [id, isEditMode]);

  const loadCustomer = async (customerId: string) => {
    try {
      setLoading(true);
      const customer = await customerService.getById(customerId);
      setFormData({
        name: customer.name || '',
        email: customer.email || '',
        isActive: customer.isActive,
      });
      setError(null);
    } catch (err) {
      setError('Erro ao carregar cliente');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      setLoading(true);
      setError(null);

      if (isEditMode && id) {
        await customerService.update(id, formData);
      } else {
        await customerService.create(formData);
      }

      navigate('/cliente');
    } catch (err) {
      setError(`Erro ao ${isEditMode ? 'atualizar' : 'criar'} cliente`);
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  if (loading && isEditMode) {
    return <div className="loading">Carregando cliente...</div>;
  }

  return (
    <div className="cliente-container">
      <h2 className="white">
        {isEditMode ? 'Editar Cliente' : 'Novo Cliente'}
      </h2>

      {error && <div className="error">{error}</div>}

      <form onSubmit={handleSubmit} className="customer-form">
        <div className="form-group">
          <label htmlFor="name">Nome:</label>
          <input
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="email">E-mail:</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="isActive">
            <input
              type="checkbox"
              id="isActive"
              name="isActive"
              checked={formData.isActive}
              onChange={handleChange}
              disabled={true}
            />
            Ativo
          </label>
        </div>

        <div className="form-actions">
          <button 
            type="button" 
            onClick={() => navigate('/cliente')}
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
            {loading ? 'Salvando...' : 'Salvar'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default ClienteForm;
