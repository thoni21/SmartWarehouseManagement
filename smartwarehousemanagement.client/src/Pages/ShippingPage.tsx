import { useState } from "react";
import AuthorizeView from "../Components/AuthorizeView.tsx";
import { Order, Shipment } from "../Types.tsx";

function ShippingPage() {
    const [orderId, setOrderId] = useState<number>(0);
    const [weightOfShipment, setWeightOfShipment] = useState<number>(0);
    const [sizeOfShipment, setSizeOfShipment] = useState<string | undefined>();
    const [carrier, setCarrier] = useState<string | undefined>();
    const [trackingNumber, setTrackingNumber] = useState<string | undefined>();
    const [orderError, setOrderError] = useState<string | null>(null);
    const [confirmationMessage, setConfirmationMessage] = useState<string | null>(null);

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        
        const response = await fetch(`https://localhost:7013/orders/Order/${orderId}`);

        if (!response.ok) {
            setOrderError(`Order with ID ${orderId} not found.`);
            return;
        }

        const order: Order = await response.json();

        const shipment: Shipment = {
            id: 0,
            order: order,
            dateOfShipment: new Date(),
            weightOfShipment: weightOfShipment,
            carrier: carrier,
            sizeOfShipment: sizeOfShipment,
            trackingNumber: trackingNumber,
        };

        setOrderError(null);

        // Create shipment
        const shipmentResponse = await fetch("https://localhost:7013/shipment/Shipment", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(shipment),
        });

        if (!shipmentResponse.ok) {
            setOrderError(`Failed to create shipment: ${shipmentResponse.statusText}`);
            return;
        }

        setConfirmationMessage("Shipment has successfully been created.")
    };

    return (
        <AuthorizeView>
            <div className="d-flex flex-column align-items-center justify-content-center">
                <form onSubmit={handleSubmit} style={{ width: "100%", maxWidth: "500px" }}>
                    <div className="mb-3">
                        <label htmlFor="order" className="form-label">
                            Order ID
                        </label>
                        <input
                            type="number"
                            id="order"
                            className="form-control"
                            value={orderId}
                            onChange={(e) => setOrderId(parseInt(e.target.value, 10))}
                            required
                        />
                        {orderError && <small className="text-danger">{orderError}</small>}
                    </div>
                    <div className="mb-3">
                        <label htmlFor="weightOfShipment" className="form-label">
                            Weight of Shipment
                        </label>
                        <input
                            type="number"
                            id="weightOfShipment"
                            className="form-control"
                            value={weightOfShipment}
                            onChange={(e) => setWeightOfShipment(parseInt(e.target.value, 10))}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="sizeOfShipment" className="form-label">
                            Size of Shipment
                        </label>
                        <input
                            type="text"
                            id="sizeOfShipment"
                            className="form-control"
                            value={sizeOfShipment || ""}
                            onChange={(e) => setSizeOfShipment(e.target.value)}
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="carrier" className="form-label">
                            Carrier
                        </label>
                        <input
                            type="text"
                            id="carrier"
                            className="form-control"
                            value={carrier || ""}
                            onChange={(e) => setCarrier(e.target.value)}
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="trackingNumber" className="form-label">
                            Tracking Number
                        </label>
                        <input
                            type="text"
                            id="trackingNumber"
                            className="form-control"
                            value={trackingNumber || ""}
                            onChange={(e) => setTrackingNumber(e.target.value)}
                        />
                    </div>
                    <button type="submit" className="btn btn-primary"
                        onClick={() => { setConfirmationMessage("") }}>
                        Submit
                    </button>
                    <button
                        type="reset"
                        className="btn btn-secondary ms-2"
                        onClick={() => {
                            setOrderId(0);
                            setWeightOfShipment(0);
                            setSizeOfShipment(undefined);
                            setCarrier(undefined);
                            setTrackingNumber(undefined);
                            setOrderError(null);
                            setConfirmationMessage("");
                        }}>
                        Reset
                    </button>
                </form>
                <div>
                    <a style={{ color: "green" }}>{confirmationMessage}</a>
                </div>
            </div>
        </AuthorizeView>
    );
}

export default ShippingPage;
