import { useState, useEffect } from "react";
import AuthorizeView from "../Components/AuthorizeView";
import { Item } from "../Types";

function Order() {
    const [items, setItems] = useState<Item[] | null>(null)
    const [selectedItem, setSelectedItem] = useState<Item | null>()
    const [quantity, setQuantity] = useState<number>(0)

    useEffect(() => {
        fetch("https://localhost:7013/inventory/Item")
            .then(response => response.json())
            .then(res => setItems(res))
            .catch(err => console.log(err))
    }, [])

    return (
        <AuthorizeView>
            <h1>Order Page</h1>
            <div className="d-flex align-items-center justify-content-center">
                <div>
                    <input
                        className="me-3"
                        style={{ maxWidth: "50px", textAlign: "center" }}
                        type="number"
                        value={quantity}
                        onChange={(e) => {
                            const value = e.target.value;
                            setQuantity(value === "" ? 0 : Number(value));
                        }}
                    >
                    </input>
                </div>
                <div>
                    <select onChange={(e) => {
                        const item = items?.find((i) => i.id.toString() === e.target.value);
                        setSelectedItem(item);
                        console.log(item);
                    }}
                        defaultValue="default">

                        <option value="default">Choose an item</option>

                        {items
                            ? items.map((item) => {
                                return <option key={item.id} value={item.id}>{item.name}</option>
                            })
                            : null}

                    </select>
                </div>
                <div>
                    <p className="ms-3">{selectedItem?.price ?? "0"}</p>
                </div>
            </div>
        </AuthorizeView>
    );
}

export default Order;