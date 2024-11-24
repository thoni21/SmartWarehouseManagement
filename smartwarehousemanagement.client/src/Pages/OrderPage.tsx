import { useState, useEffect } from "react";
import { Item, Order, OrderItem } from "../Types";
import OrderItemComponent from "../Components/OrderItemComponent";
import AuthorizeView from "../Components/AuthorizeView";

interface OrderItemType {
    selectedItem: Item | null;
    quantity: number;
}

function OrderPage() {
    const [items, setItems] = useState<Item[] | null>(null);
    const [orderItems, setOrderItems] = useState<OrderItemType[]>([
        { selectedItem: null, quantity: 1 },
    ]);
    const [totalPrice, setTotalPrice] = useState<number>(0);

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


    const handlePurchase = async (orderItems: OrderItemType[]) => {
        orderItems.forEach((orderItem) => {
            if ((orderItem.selectedItem?.quantityInStock ?? 0) < orderItem.quantity) {
                throw new Error("Not enough items left in stock");
            }
            setTotalPrice((prev) => { return prev + (orderItem.selectedItem?.price ?? 0)  })
        });

        //TODO add actual email of user, generate orderNr
        const exampleOrder: Order = {
            id: 0,
            customer: "testUser" ,
            orderNr: "ORD12345",
            shipped: false,
            cancelled: false,
            orderDate: new Date(),
            price: totalPrice
        };

        try {
            // Create the main order
            const response = await fetch("https://localhost:7013/orders/Order", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(exampleOrder),
            });

            if (!response.ok) {
                throw new Error(`Failed to create order: ${response.statusText}`);
            }

            const orderResponse: Order = await response.json();

            // Create each order item
            await Promise.all(
                orderItems.map(async (orderItem) => {

                    const payload: OrderItem = {
                        id: 0,
                        order: orderResponse,
                        item: orderItem.selectedItem,
                        quantity: orderItem.quantity,
                        price: orderItem.quantity * (orderItem.selectedItem?.price ?? 0),
                    };

                    const itemResponse = await fetch("https://localhost:7013/order/items/OrderItems", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify(payload),
                    });

                    if (!itemResponse.ok) {
                        const errorText = await itemResponse.text();
                        throw new Error(`Failed to add item to order: ${errorText}`);
                    }
                    console.log("Item response:", await itemResponse.json());
                })
            );
        } catch (error) {
            console.error("Error handling purchase:", error);
        }
    };

    return (
        <AuthorizeView>
            <h1>Order Page</h1>
            {orderItems.map((orderItem, index) => (
                <OrderItemComponent
                    key={index}
                    quantity={orderItem.quantity}
                    selectedItem={orderItem.selectedItem}
                    items={items}
                    onQuantityChange={(value) => handleQuantityChange(index, value)}
                    onItemChange={(item) => handleItemChange(index, item)}
                />
            ))}

            <div className="d-flex align-items-center justify-content-center">
                <button onClick={handleAddRowsToBasket}>Add Item</button>
                <button onClick={() => { handlePurchase(orderItems) }}>Purchase</button>
            </div>
        </AuthorizeView>
    );
}

export default OrderPage;
