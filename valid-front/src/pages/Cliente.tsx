import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { customerService } from "../services/customerService";
import { type CustomerDto } from "../types/customer";
import "./Cliente.css";

const Cliente = () => {
  const navigate = useNavigate();
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [filterText, setFilterText] = useState("");

  useEffect(() => {
    loadCustomers();
  }, []);

  const loadCustomers = async () => {
    try {
      setLoading(true);
      const data = await customerService.getAll();
      setCustomers(data);
      setError(null);
    } catch (err) {
      setError("Erro ao carregar clientes");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (id: string) => {
    navigate(`/cliente/${id}`);
  };

  const handleDelete = async (id: string) => {
    if (window.confirm("Tem certeza que deseja excluir este cliente?")) {
      try {
        await customerService.delete(id);
        loadCustomers();
      } catch (err) {
        console.error("Erro ao excluir cliente:", err);
        alert("Erro ao excluir cliente");
      }
    }
  };

  const filteredCustomers = customers.filter((customer) => {
    if (!filterText) return true;
    const searchText = filterText.toLowerCase();
    const name = (customer.name || "").toLowerCase();
    const email = (customer.email || "").toLowerCase();
    return name.includes(searchText) || email.includes(searchText);
  });

  const paginatedCustomers = filteredCustomers.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize
  );

  const totalPages = Math.ceil(filteredCustomers.length / pageSize);

  const handlePageSizeChange = (value: number) => {
    const newSize = Math.min(Math.max(value, 5), 1000);
    setPageSize(newSize);
    setCurrentPage(1);
  };

  if (loading) {
    return <div className="loading">Carregando clientes...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="cliente-container">
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: "20px",
        }}
      >
        <h2 className="white">Clientes</h2>
        <button className="btn-edit" onClick={() => navigate("/cliente/new")}>
          Novo Cliente
        </button>
      </div>

      <div style={{ marginBottom: "20px" }}>
        <input
          type="text"
          placeholder="Filtrar por nome ou e-mail..."
          value={filterText}
          onChange={(e) => {
            setFilterText(e.target.value);
            setCurrentPage(1);
          }}
          style={{
            width: "100%",
            padding: "8px 12px",
            fontSize: "14px",
            border: "1px solid #ccc",
            borderRadius: "4px",
          }}
        />
      </div>

      <table className="customer-table">
        <thead>
          <tr>
            <th>Nome</th>
            <th>E-mail</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {customers.length === 0 ? (
            <tr>
              <td colSpan={3} className="empty-message">
                Nenhum cliente encontrado
              </td>
            </tr>
          ) : (
            paginatedCustomers.map((customer) => (
              <tr key={customer.id}>
                <td>{customer.name || "-"}</td>
                <td>{customer.email || "-"}</td>
                <td className="actions">
                  <button
                    className="btn-edit"
                    onClick={() => handleEdit(customer.id)}
                  >
                    Editar
                  </button>
                  <button
                    className="btn-delete"
                    onClick={() => handleDelete(customer.id)}
                  >
                    Deletar
                  </button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      {customers.length > 0 && (
        <div className="pagination-controls">
          <div className="page-size-control">
            <label>
              Itens por página:
              <input
                type="number"
                min="5"
                max="1000"
                value={pageSize}
                onChange={(e) => handlePageSizeChange(Number(e.target.value))}
              />
            </label>
          </div>

          <div className="pagination-buttons">
            <button
              onClick={() => setCurrentPage(1)}
              disabled={currentPage === 1}
            >
              Primeira
            </button>
            <button
              onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
              disabled={currentPage === 1}
            >
              Anterior
            </button>

            <span className="page-info">
              Página {currentPage} de {totalPages}
            </span>
            <button
              onClick={() =>
                setCurrentPage((prev) => Math.min(prev + 1, totalPages))
              }
              disabled={currentPage === totalPages}
            >
              Próxima
            </button>
            <button
              onClick={() => setCurrentPage(totalPages)}
              disabled={currentPage === totalPages}
            >
              Última
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default Cliente;
