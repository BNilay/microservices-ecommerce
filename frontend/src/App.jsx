import { useEffect, useState } from "react";
import "./App.css";

const API_BASE_URL = "http://localhost:5000";

function App() {
  const [email, setEmail] = useState("test@example.com");
  const [password, setPassword] = useState("123456");

  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [products, setProducts] = useState([]);
  const [selectedProductId, setSelectedProductId] = useState("");
  const [quantity, setQuantity] = useState(1);
  const [customerId, setCustomerId] = useState(5);

  const [message, setMessage] = useState("");
  const [orderResult, setOrderResult] = useState(null);

  const getOrderStatusText = (status) => {
    switch (status) {
      case 0:
        return "Pending";
      case 1:
        return "Confirmed";
      case 2:
        return "Shipped";
      case 3:
        return "Delivered";
      case 4:
        return "Cancelled";
      default:
        return "Unknown";
    }
  };

  const loadProducts = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/products`);

      if (!response.ok) {
        setMessage("Ürünler getirilemedi.");
        return;
      }

      const data = await response.json();

      setProducts(data);

      if (data.length > 0) {
        setSelectedProductId(data[0].id);
      }
    } catch (error) {
      setMessage("Ürün servisine ulaşılamadı. API Gateway veya Docker çalışıyor mu kontrol et.");
    }
  };

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (token) {
      setIsLoggedIn(true);
    }

    loadProducts();
  }, []);

  const login = async () => {
    setMessage("");
    setOrderResult(null);

    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          email,
          password
        })
      });

      if (!response.ok) {
        setMessage("Login başarısız. Email veya şifreyi kontrol et.");
        return;
      }

      const data = await response.json();

      localStorage.setItem("token", data.token);

      setIsLoggedIn(true);
      setMessage("Login başarılı. Token ekranda gösterilmeden arka planda kaydedildi.");

      loadProducts();
    } catch (error) {
      setMessage("API Gateway bağlantı hatası.");
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    setIsLoggedIn(false);
    setOrderResult(null);
    setMessage("Çıkış yapıldı. Token silindi.");
  };

  const createOrder = async () => {
    setMessage("");
    setOrderResult(null);

    const token = localStorage.getItem("token");

    if (!token) {
      setMessage("Sipariş oluşturmak için önce login olmalısın.");
      return;
    }

    if (!selectedProductId) {
      setMessage("Sipariş oluşturmak için ürün seçmelisin.");
      return;
    }

    if (Number(quantity) <= 0) {
      setMessage("Quantity değeri 1 veya daha büyük olmalı.");
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/api/orders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({
          customerId: Number(customerId),
          paymentMethod: 0,
          items: [
            {
              productId: Number(selectedProductId),
              quantity: Number(quantity)
            }
          ]
        })
      });

      if (!response.ok) {
        setMessage("Sipariş oluşturulamadı. Customer, ürün veya stok bilgisini kontrol et.");
        return;
      }

      const data = await response.json();

      setOrderResult(data);

      if (data.status === 1) {
        setMessage("Sipariş başarılı. Ödeme tamamlandı ve sipariş Confirmed oldu.");
      } else if (data.status === 4) {
        setMessage("Ödeme başarısız. Sipariş Cancelled oldu ve stok geri yüklendi.");
      } else {
        setMessage("Sipariş oluşturuldu.");
      }

      loadProducts();
    } catch (error) {
      setMessage("Sipariş oluşturulurken hata oluştu.");
    }
  };

  return (
    <div className="page">
      <div className="container">
        <h1>Microservices E-Commerce</h1>

        <p className="subtitle">
          React frontend, API Gateway üzerinden mikroservislere bağlanır.
        </p>

        <section className="card">
          <h2>Login</h2>

          <div className="form-row">
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />

            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            {!isLoggedIn ? (
              <button onClick={login}>Login</button>
            ) : (
              <button className="secondary" onClick={logout}>
                Logout
              </button>
            )}
          </div>

          <p className={isLoggedIn ? "success" : "info"}>
            {isLoggedIn
              ? "Kullanıcı giriş yaptı. Token ekranda gösterilmeden arka planda saklanıyor."
              : "Sipariş işlemleri için giriş yapılmalı."}
          </p>
        </section>

        <section className="card">
          <div className="section-header">
            <h2>Products</h2>

            <button className="secondary" onClick={loadProducts}>
              Refresh Products
            </button>
          </div>

          <div className="products">
            {products.length === 0 ? (
              <p>Henüz ürün yüklenmedi.</p>
            ) : (
              products.map((product) => (
                <div className="product-card" key={product.id}>
                  <h3>{product.name}</h3>
                  <p>Price: {product.price}</p>
                  <p>Stock: {product.stockQuantity}</p>
                  <p>Category: {product.category}</p>
                </div>
              ))
            )}
          </div>
        </section>

        <section className="card">
          <h2>Create Order</h2>

          <div className="form-row">
            <input
              type="number"
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value)}
              placeholder="Customer Id"
            />

            <select
              value={selectedProductId}
              onChange={(e) => setSelectedProductId(e.target.value)}
            >
              {products.length === 0 ? (
                <option value="">Ürün bulunamadı</option>
              ) : (
                products.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.name} - {product.price}
                  </option>
                ))
              )}
            </select>

            <input
              type="number"
              min="1"
              value={quantity}
              onChange={(e) => setQuantity(e.target.value)}
              placeholder="Quantity"
            />

            <button onClick={createOrder}>Create Order</button>
          </div>
        </section>

        {message && <div className="message">{message}</div>}

        {orderResult && (
          <section className="card">
            <h2>Order Result</h2>

            <div className="result">
              <p>Order Id: {orderResult.id}</p>
              <p>Customer Id: {orderResult.customerId}</p>
              <p>
                Status: {orderResult.status} - {getOrderStatusText(orderResult.status)}
              </p>
              <p>Total Amount: {orderResult.totalAmount}</p>
              <p>Created At: {orderResult.createdAt}</p>
            </div>
          </section>
        )}
      </div>
    </div>
  );
}

export default App;