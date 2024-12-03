import './App.css';

import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './Pages/HomePage.tsx';
import Login from './Pages/Login.tsx';
import Register from './Pages/Register.tsx';
import OrderPage from './Pages/OrderPage.tsx';
import ShippingPage from './Pages/ShippingPage.tsx';
import ViewOrdersPage from './Pages/ViewOrdersPage.tsx';
import CreateItemPage from './Pages/CreateItemPage.tsx';


function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/" element={<HomePage />} />
                <Route path="/order" element={<OrderPage />} />
                <Route path="/shipping" element={<ShippingPage />} />
                <Route path="/view/orders" element={<ViewOrdersPage />} />
                <Route path="/item" element={<CreateItemPage />} />
            </Routes>
        </BrowserRouter>
    );
}
export default App;