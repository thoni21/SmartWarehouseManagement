import React from "react";
import { Item } from "../Types";

interface OrderItemProps {
    quantity: number;
    selectedItem: Item | null;
    items: Item[] | null;
    onQuantityChange: (value: number) => void;
    onItemChange: (item: Item | null) => void;
    onRemove: () => void;
}

const OrderItem: React.FC<OrderItemProps> = ({
    quantity,
    selectedItem,
    items,
    onQuantityChange,
    onItemChange,
    onRemove,
}) => {
    return (
        <div className="d-flex align-items-center justify-content-center">
            <div>
                <input
                    className="me-3"
                    style={{ maxWidth: "50px", textAlign: "center" }}
                    type="number"
                    value={quantity}
                    onChange={(e) => {
                        const value = e.target.value;
                        onQuantityChange(value === "" ? 0 : Number(value));
                    }}
                />
            </div>
            <div>
                <select
                    onChange={(e) => {
                        const item = items?.find(
                            (i) => i.id.toString() === e.target.value
                        );
                        onItemChange(item || null);
                    }}
                    value={selectedItem?.id ?? "default"}
                >
                    <option value="default">Choose an item</option>
                    {items?.map((item) => (
                        <option key={item.id} value={item.id}>
                            {item.name}
                        </option>
                    ))}
                </select>
            </div>
            <div>
                <span className="ms-3">{selectedItem?.price ?? "0"}</span>
            </div>
            <button
                onClick={onRemove}
                style={{
                    background: "none",
                    color: "red",
                    border: "none",
                    fontSize: "16px",
                    cursor: "pointer",
                    padding: "0",
                }}
                className="ms-3"
            >
                X
            </button>
        </div>
    );
};

export default OrderItem;
