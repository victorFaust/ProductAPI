import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { getProducts } from '../api/productsApi';
import type { ProductDto } from '../types';
import ProductTable from '../components/ProductTable';
import CreateProductForm from '../components/CreateProductForm';
import styles from './ProductsPage.module.css';

export default function ProductsPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [colour, setColour] = useState('');
  const [debouncedColour, setDebouncedColour] = useState('');

  // Debounce the colour filter input by 300ms
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedColour(colour), 300);
    return () => clearTimeout(timer);
  }, [colour]);

  const fetchProducts = useCallback(async () => {
    setLoading(true);
    try {
      const data = await getProducts(debouncedColour || undefined);
      setProducts(data);
    } catch {
      setProducts([]);
    } finally {
      setLoading(false);
    }
  }, [debouncedColour]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <div className={styles.page}>
      <header className={styles.header}>
        <h1 className={styles.title}>Products</h1>
        <button className={styles.logoutBtn} onClick={handleLogout}>
          Logout
        </button>
      </header>

      <div className={styles.toolbar}>
        <input
          type="text"
          className={styles.filterInput}
          placeholder="Filter by colour…"
          value={colour}
          onChange={(e) => setColour(e.target.value)}
        />
      </div>

      <div className={styles.content}>
        <div className={styles.tableSection}>
          <ProductTable products={products} loading={loading} />
        </div>
        <aside className={styles.sidebar}>
          <CreateProductForm onProductCreated={fetchProducts} />
        </aside>
      </div>
    </div>
  );
}
