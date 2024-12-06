import { useState, useEffect } from "react";
import { Item, Order, OrderItem } from "../Types";
import OrderItemComponent from "../Components/OrderItemComponent";
import AuthorizeView from "../Components/AuthorizeView";
import { useUserContext } from "../context";

interface OrderItemType {
    selectedItem: Item | null;
    quantity: number;
}

function OrderContent() {
    const [items, setItems] = useState<Item[] | null>(null);
    const [orderItems, setOrderItems] = useState<OrderItemType[]>([
        { selectedItem: null, quantity: 1 },
    ]);
    const [totalPrice, setTotalPrice] = useState<number>(0);
    const [errorText, setErrorText] = useState<string>("");

    const user = useUserContext()

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

    const handleRemoveItem = (index: number) => {
        setOrderItems((prevItems) => {
            const newOrderItems = prevItems.filter((_, i) => i !== index);
            return newOrderItems;
        });
    };

    const handlePurchase = async (orderItems: OrderItemType[]) => {
        setErrorText("")
        orderItems.forEach((orderItem) => {
            if ((orderItem.selectedItem?.quantityInStock ?? 0) < orderItem.quantity) {
                setErrorText("Not enough items left in stock of item: " + orderItem.selectedItem?.name);
                return;
            }
            setTotalPrice((prev) => { return prev + (orderItem.selectedItem?.price ?? 0)  })
        });

        const exampleOrder: Order = {
            id: 0,
            customer: user.email,
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
        <div>
            <h1>Order Page</h1>
            {orderItems.map((orderItem, index) => (
                <OrderItemComponent
                    key={index}
                    quantity={orderItem.quantity}
                    selectedItem={orderItem.selectedItem}
                    items={items}
                    onQuantityChange={(value) => handleQuantityChange(index, value)}
                    onItemChange={(item) => handleItemChange(index, item)}
                    onRemove={() => { handleRemoveItem(index) }}
                />
            ))}

            <div className="d-flex align-items-center justify-content-center">
                <button onClick={handleAddRowsToBasket}>Add Item</button>
                <button onClick={() => { handlePurchase(orderItems) }}>Purchase</button>
            </div>
            <a style={{ color: "red" }}>{errorText}</a>
            <a href="/">
                <button className="mt-3">Home</button>
            </a>
        </div>
    );
}

function OrderPage() {
    return (
        <AuthorizeView>
            <OrderContent />
        </AuthorizeView>
    )
}

export default OrderPage;
