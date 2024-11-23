import { useState, useEffect } from "react";
import AuthorizeView from "../Components/AuthorizeView";
import { Item } from "../Types";
import OrderItem from "../Components/OrderItem";

interface OrderItemType {
    selectedItem: Item | null;
    quantity: number;
}

function Order() {
    const [items, setItems] = useState<Item[] | null>(null);
    const [orderItems, setOrderItems] = useState<OrderItemType[]>([
        { selectedItem: null, quantity: 1 },
    ]);

    useEffect(() => {
        fetch("https://localhost:7013/inventory/Item")
            .then((response) => response.json())
            .then((res) => setItems(res))
            .catch((err) => console.log(err));
    }, []);

    const handleQuantityChange = (index: number, value: number) => {
        setOrderItems((prevItems) => {
            const newOrderItems = [...prevItems];
            newOrderItems[index].quantity = value;
            return newOrderItems;
        });
    };

    const handleItemChange = (index: number, item: Item | null) => {
        setOrderItems((prevItems) => {
            const newOrderItems = [...prevItems];
            newOrderItems[index].selectedItem = item;
            return newOrderItems;
        });
    };
   
    const handleAddRowsToBasket = () => {
        setOrderItems((prevItems) => [
            ...prevItems,
            { selectedItem: null, quantity: 1 },
        ]);
        console.log(orderItems);
    };

    return (
        <AuthorizeView>
            <h1>Order Page</h1>
            {orderItems.map((orderItem, index) => (
                <OrderItem
                    key={index}
                    quantity={orderItem.quantity}
                    selectedItem={orderItem.selectedItem}
                    items={items}
                    onQuantityChange={(value) => handleQuantityChange(index, value)}
                    onItemChange={(item) => handleItemChange(index, item)}
                />
            ))}

            <div>
                <button onClick={handleAddRowsToBasket}>Add Item</button>
            </div>
        </AuthorizeView>
    );
}

export default Order;
