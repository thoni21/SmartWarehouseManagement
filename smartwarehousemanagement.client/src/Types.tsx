export type Item = {
    id: number;
    name: string;
    price: number;
    weightInKg: string;
    size: string;
    shelf: string;
    shelfPosition: string;
    category: string;
    quantityInStock: number;
}

export type Order = {
    id: number;
    customer: string; 
    orderNr: string;
    orderDate?: Date; 
    price?: number; 
    shipped: boolean; 
    cancelled: boolean; 
};

export type OrderItem = {
    id: number;
    order: Order;
    item: Item | null;
    quantity: number;
    price: number;
}

export type User = {
    email: string;
}

export type Shipment = {
    id: number;
    order: Order;
    dateOfShipment: Date;
    weightOfShipment: number;
    sizeOfShipment?: string;
    carrier?: string;
    trackingNumber?: string;
};
