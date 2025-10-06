import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { orderService } from "../services/orderService";
import { type OrderDto } from "../types/order";
import "./Cliente.css";

const Pedido = () => {
  const navigate = useNavigate();
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [filterText, setFilterText] = useState("");

  useEffect(() => {
    loadOrders();
  }, []);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = await orderService.getAll();
      setOrders(data);
      setError(null);
    } catch (err) {
      setError("Erro ao carregar pedidos");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleView = (id: string) => {
    navigate(`/pedido/detail/${id}`);
  };

  const handleCancel = async (id: string) => {
    if (window.confirm("Tem certeza que deseja cancelar este pedido?")) {
      try {
        await orderService.cancelar(id);
        loadOrders();
      } catch (err) {
        console.error("Erro ao cancelar pedido:", err);
        alert("Erro ao cancelar pedido");
      }
    }
  };

  const filteredOrders = orders.filter((order) => {
    if (!filterText) return true;
    const searchText = filterText.toLowerCase();
    const customerName = (order.customerName || "").toLowerCase();
    const status = (order.status || "").toLowerCase();
    return customerName.includes(searchText) || status.includes(searchText);
  });

  const paginatedOrders = filteredOrders.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize
  );

  const totalPages = Math.ceil(filteredOrders.length / pageSize);

  const handlePageSizeChange = (value: number) => {
    const newSize = Math.min(Math.max(value, 5), 1000);
    setPageSize(newSize);
    setCurrentPage(1);
  };

  if (loading) {
    return <div className="loading">Carregando pedidos...</div>;
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
        <h2 className="white">Pedidos</h2>
        <button className="btn-edit" onClick={() => navigate("/pedido/new")}>
          Novo Pedido
        </button>
      </div>

      <div style={{ marginBottom: "20px" }}>
        <input
          type="text"
          placeholder="Filtrar por cliente ou status..."
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
            <th>Cliente</th>
            <th>Data de Criação</th>
            <th>Valor</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {orders.length === 0 ? (
            <tr>
              <td colSpan={5} className="empty-message">
                Nenhum pedido encontrado
              </td>
            </tr>
          ) : (
            paginatedOrders.map((order) => (
              <tr key={order.id}>
                <td>{order.customerName || "-"}</td>
                <td>{new Date(order.createdAt).toLocaleDateString("pt-BR")}</td>
                <td>
                  {new Intl.NumberFormat("pt-BR", {
                    style: "currency",
                    currency: "BRL",
                  }).format(order.amount)}
                </td>
                <td>{order.status || "-"}</td>
                <td className="actions">
                  <button
                    className="btn-edit"
                    onClick={() => handleView(order.id)}
                  >
                    View
                  </button>
                  <button
                    className="btn-delete"
                    onClick={() => handleCancel(order.id)}
                  >
                    Cancelar
                  </button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      {orders.length > 0 && (
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

export default Pedido;
