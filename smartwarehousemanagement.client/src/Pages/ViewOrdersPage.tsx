import { useState } from "react";
import AuthorizeView from "../Components/AuthorizeView";
import { OrderItem } from "../Types";

function ViewOrders(){

    const [orderId, setOrderId] = useState<number | null>(null);
    const [orderItems, setOrderItems] = useState<OrderItem[]>([]);


    const getOrderWithId = async () => {

        const response = await fetch(`https://localhost:7013/order/items/OrderItems/${orderId}/items`);

        const order: OrderItem[] = await response.json();

        if (!order[0].item) {
            throw new Error("No items in order")
        }

        if (order.length === 0 || !order[0].item) {
            throw new Error("No items in order");
        }

        setOrderItems(order);
    }

    return (
        <AuthorizeView>
            <div className="d-flex flex-column align-items-center">
                <div className="mb-3">
                    <label htmlFor="order" className="form-label">
                        Order ID
                    </label>
                    <input
                        type="number"
                        id="order"
                        className="form-control"
                        value={orderId || ""}
                        onChange={(e) => {
                            const value = e.target.value;
                            setOrderId(value === "" ? 0 : parseInt(value, 10));
                        }}
                        required
                    />
                </div>
                <div className="mb3">
                    <button onClick={getOrderWithId}>
                        Find Order
                    </button>
                </div>
                <div style={{ border: "1px solid #ccc", padding: "16px", borderRadius: "8px", maxWidth: "600px" }} className="mt-3">
                    <h2>Item Details</h2>
                    {orderItems.length > 0 ? (
                        orderItems.map((orderItem, index) => {
                            if (!orderItem.item) { throw new Error("OrderItem has no items") }
                            return (
                                <div key={index} style={{ marginBottom: "16px" }}>
                                    <h4>Item {index + 1}</h4>
                                    <p><strong>Name:</strong> {orderItem.item.name}</p>
                                    <p><strong>Weight:</strong> {orderItem.item.weightInKg} kg</p>
                                    <p><strong>Size:</strong> {orderItem.item.size}</p>
                                    <p><strong>Shelf:</strong> {orderItem.item.shelf}</p>
                                    <p><strong>Shelf Position:</strong> {orderItem.item.shelfPosition}</p>
                                    <p><strong>Category:</strong> {orderItem.item.category}</p>
                                </div>
                            );
                        })
                    ) : (
                        <p>No items found for this order.</p>
                    )}
                </div>
            </div>
        </AuthorizeView>  
    );
}

export default ViewOrders;