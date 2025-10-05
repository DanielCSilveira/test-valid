import { useEffect, useState, useRef } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { orderService } from "../services/orderService";
import { type OrderDto } from "../types/order";
import "./Cliente.css";

const PedidoDetail = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const printRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (id) {
      loadOrder();
    }
  }, [id]);

  const loadOrder = async () => {
    try {
      setLoading(true);
      const data = await orderService.getById(id!);
      setOrder(data);
      setError(null);
    } catch (err) {
      setError("Erro ao carregar detalhes do pedido");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handlePrint = () => {
    if (printRef.current) {
      const printWindow = window.open('', '', 'width=800,height=600');
      if (printWindow) {
        printWindow.document.write('<html><head><title>Detalhes do Pedido</title>');
        printWindow.document.write('<style>body { font-family: Arial, sans-serif; padding: 20px; }</style>');
        printWindow.document.write('</head><body>');
        printWindow.document.write(printRef.current.innerHTML);
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        printWindow.focus();
        printWindow.print();
        printWindow.close();
      }
    }
  };

  if (loading) {
    return <div className="loading">Carregando detalhes do pedido...</div>;
  }

  if (error || !order) {
    return <div className="error">{error || "Pedido não encontrado"}</div>;
  }

  return (
    <div className="cliente-container">
      <h2 className="white">Detalhes do Pedido</h2>

      <div ref={printRef} style={{ backgroundColor: "#fff", padding: "20px", borderRadius: "8px", marginTop: "20px", color: "#333" }}>
        <div style={{ marginBottom: "15px" }}>
          <strong>ID:</strong> {order.id}
        </div>
        <div style={{ marginBottom: "15px" }}>
          <strong>Cliente:</strong> {order.customerName || "-"}
        </div>
        <div style={{ marginBottom: "15px" }}>
          <strong>ID do Cliente:</strong> {order.customerId}
        </div>
        <div style={{ marginBottom: "15px" }}>
          <strong>Valor:</strong>{" "}
          {new Intl.NumberFormat("pt-BR", {
            style: "currency",
            currency: "BRL",
          }).format(order.amount)}
        </div>
        <div style={{ marginBottom: "15px" }}>
          <strong>Status:</strong> {order.status || "-"}
        </div>
        <div style={{ marginBottom: "15px" }}>
          <strong>Data de Criação:</strong>{" "}
          {new Date(order.createdAt).toLocaleDateString("pt-BR", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
          })}
        </div>
      </div>

      <div style={{ marginTop: "20px", display: "flex", gap: "10px" }}>
        <button className="btn-edit" onClick={() => navigate("/pedido")}>
          Voltar
        </button>
        <button className="btn-edit" onClick={handlePrint}>
          Imprimir
        </button>
      </div>
    </div>
  );
};

export default PedidoDetail;
