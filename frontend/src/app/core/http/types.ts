export type Me = {
  id: string;
  email: string;
  fullName: string;
  role: string;
};

export type ClientListItem = {
  id: string;
  documentType: string;
  documentNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  addressLine1: string;
  city: string;
  notes: string;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string;
};

export type ClientUpsert = {
  documentType: string;
  documentNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  addressLine1: string;
  city: string;
  notes: string;
  isActive: boolean;
};

export type ProductListItem = {
  id: string;
  sku: string;
  name: string;
  category: string;
  description: string;
  barcode: string;
  imageUrl: string;
  price: number;
  stock: number;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string;
};

export type ProductUpsert = {
  sku: string;
  name: string;
  category: string;
  description: string;
  barcode: string;
  imageUrl: string;
  price: number;
  stock: number;
  isActive: boolean;
};
