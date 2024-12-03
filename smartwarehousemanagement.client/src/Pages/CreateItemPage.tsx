import { useState } from "react";
import { Item } from "../Types";
import AuthorizeView from "../Components/AuthorizeView";

function CreateItemPage() {
    const [formData, setFormData] = useState<Item>({
        id: 0,
        name: "",
        price: 0,
        weightInKg: 0,
        size: "",
        shelf: "",
        shelfPosition: "",
        category: "",
        quantityInStock: 0,
    });

    const [infoText, setInfoText] = useState<string>("");
    const [infoTextColor, setInfoTextColor] = useState<string>("");


    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: name === "price" || name === "quantityInStock" || name === "id"
                ? Number(value)
                : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setInfoText("");

        // Create Item
        const itemResponse = await fetch("https://localhost:7013/inventory/Item", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(formData),
        });

        if (!itemResponse.ok) {
            setInfoTextColor("red");
            setInfoText(`Failed to create item: ${itemResponse.statusText}`);
            return;
        }

        setFormData({
            id: 0,
            name: "",
            price: 0,
            weightInKg: 0,
            size: "",
            shelf: "",
            shelfPosition: "",
            category: "",
            quantityInStock: 0,
        });

        setInfoTextColor("green");
        setInfoText("Item created");
    };

    return (
        <AuthorizeView>
            <div className="d-flex flex-column align-items-center justify-content-center">
                <form onSubmit={handleSubmit}>
                    <div className="form-group mb-3">
                        <label htmlFor="id">ID:</label>
                        <input
                            type="number"
                            id="id"
                            name="id"
                            value={formData.id}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="name">Name:</label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="price">Price:</label>
                        <input
                            type="number"
                            id="price"
                            name="price"
                            value={formData.price}
                            onChange={handleChange}
                            step="0.01"
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="weightInKg">Weight (Kg):</label>
                        <input
                            type="number"
                            id="weightInKg"
                            name="weightInKg"
                            value={formData.weightInKg}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="size">Size:</label>
                        <input
                            type="text"
                            id="size"
                            name="size"
                            value={formData.size}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="shelf">Shelf:</label>
                        <input
                            type="text"
                            id="shelf"
                            name="shelf"
                            value={formData.shelf}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="shelfPosition">Shelf Position:</label>
                        <input
                            type="text"
                            id="shelfPosition"
                            name="shelfPosition"
                            value={formData.shelfPosition}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="category">Category:</label>
                        <input
                            type="text"
                            id="category"
                            name="category"
                            value={formData.category}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <div className="form-group mb-3">
                        <label htmlFor="quantityInStock">Quantity in Stock:</label>
                        <input
                            type="number"
                            id="quantityInStock"
                            name="quantityInStock"
                            value={formData.quantityInStock}
                            onChange={handleChange}
                            required
                            className="form-control"
                        />
                    </div>
                    <button type="submit" className="btn btn-primary">Add Item</button>
                </form>
                <a style={{ color: infoTextColor }}>{infoText}</a>
            </div>
        </AuthorizeView>
    );
}

export default CreateItemPage;

