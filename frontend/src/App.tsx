// import React from 'react';

// const App: React.FC = () => {
//     return (
//         <div className="flex items-center justify-center h-screen">
//             <h1 className="text-2xl font-bold">Welcome to the React App with TailwindCSS!</h1>
//         </div>
//     );
// };

// export default App;



import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';

const Home: React.FC = () => (
  <div className="flex flex-col items-center justify-center h-screen gap-4">
    <h1 className="text-2xl font-bold mb-4">TMB Educação e Serviços rocks!</h1>
    <Link to="/add" className="text-blue-500 underline">Add Order</Link>
    <Link to="/list" className="text-blue-500 underline">List Orders</Link>
  </div>
);

const AddOrder: React.FC = () => 
{
  const [cliente, setCliente] = React.useState('');
  const [produto, setProduto] = React.useState('');
  const [valor, setValor] = React.useState('');
  const [status, setStatus] = React.useState('');  
  const [message, setMessage] = React.useState('');

  const handleSubmit = async (e: React.FormEvent) => 
  {
    e.preventDefault();
    setMessage('');
    const response = await fetch('http://localhost:8080/api/Orders/', 
    {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ cliente, produto, valor: parseFloat(valor), status }),
    });

    if (response.ok) 
    {
      setMessage('Order added successfully!');
      setCliente('');
      setProduto('');
      setValor('');
      setStatus('');
    } 
    else 
    {
      setMessage('Failed to add order.');
    }
  };

  return (
    <div className="flex flex-col items-center justify-center h-screen">
      <form className="flex flex-col gap-2 w-80" onSubmit={handleSubmit}>
        <input className="border p-2" placeholder="Cliente" value={cliente} onChange={e => setCliente(e.target.value)} required />
        <input className="border p-2" placeholder="Produto" value={produto} onChange={e => setProduto(e.target.value)} required />
        <input className="border p-2" placeholder="Valor" type="number" value={valor} onChange={e => setValor(e.target.value)} required />
        {/* <select className="border p-2" value={status} onChange={e => setStatus(e.target.value)} required>
          <option value="">Selecione o status</option>
          <option value="Pendente">Pendente</option>
          <option value="Processando">Processando</option>
          <option value="Finalizado">Finalizado</option>
        </select> */}
        <button className="bg-blue-500 text-white p-2 rounded" type="submit">Add Order</button>
        {message && <div className="text-green-600 mt-2">{message}</div>}
        <Link to="/" className="text-blue-500 underline mt-2">Back</Link>
      </form>
    </div>
  );
};

const ListOrders: React.FC = () => 
{
  const [orders, setOrders] = React.useState<any[]>([]);

  React.useEffect(() => 
  {
    fetch('http://localhost:8080/api/Orders/')
      .then(res => res.json())
      .then(data => setOrders(data));
  }, []);

  return (
    <div className="flex flex-col items-center justify-center h-screen table-auto">
      <h2 className="text-xl font-bold mb-4">Orders List</h2>
      <table className="border-collapse border w-96">
        <thead>
          <tr>
            <th className="border p-2">ID</th>
            <th className="border p-2">Cliente</th>
            <th className="border p-2">Produto</th>
            <th className="border p-2">Valor</th>
            <th className="border p-2">Status</th>
            <th className="border p-2">Data</th>
          </tr>
        </thead>
        <tbody>
          {orders.map(order => (
            <tr key={order.id}>
              <td className="border p-2">{order.id}</td>
              <td className="border p-2">{order.cliente}</td>
              <td className="border p-2">{order.produto}</td>
              <td className="border p-2">{order.valor}</td>
              <td className="border p-2">{order.status}</td>
              <td className="border p-2">{order.dataCriacao?.substring(0, 10)}</td>
            </tr>
          ))}
        </tbody>
      </table>






      
      <Link to="/" className="text-blue-500 underline mt-4">Back</Link>
    </div>
  );
};

const App: React.FC = () => 
(
  <Router>
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/add" element={<AddOrder />} />
      <Route path="/list" element={<ListOrders />} />
    </Routes>
  </Router>
);

export default App;