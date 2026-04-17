import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useTheme } from '../context/ThemeContext';
import { toast } from 'react-toastify';
import { getProducts } from '../api/productsApi';
import type { ProductDto } from '../types';
import ProductTable from '../components/ProductTable';
import ProductModal from '../components/ProductModal';

export default function ProductsPage() {
  const { logout, username } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();

  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [colour, setColour] = useState('');
  const [debouncedColour, setDebouncedColour] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editProduct, setEditProduct] = useState<ProductDto | null>(null);

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
    toast.info('You have been logged out.');
    navigate('/login');
  }

  return (
    <div className="min-h-screen bg-slate-100 dark:bg-slate-900 flex flex-col">
      <header className="bg-white dark:bg-slate-800 border-b border-slate-200 dark:border-slate-700 px-6 py-4 flex items-center justify-between">
        <h1 className="text-xl font-bold text-slate-900 dark:text-white">APP</h1>
        <div className="flex items-center gap-3">
          {/* Theme toggle */}
          <button
            className="w-8 h-8 flex items-center justify-center rounded-lg text-slate-500 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors cursor-pointer"
            onClick={toggleTheme}
            aria-label="Toggle theme"
          >
            {theme === 'dark' ? '☀️' : '🌙'}
          </button>
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-full bg-indigo-600 flex items-center justify-center text-white text-sm font-bold select-none">
              {username ? username.charAt(0).toUpperCase() : '?'}
            </div>
            <span className="text-sm font-medium text-slate-700 dark:text-slate-300">{username}</span>
          </div>
          <button
            className="text-sm font-medium text-white bg-slate-700 hover:bg-slate-800 dark:bg-slate-600 dark:hover:bg-slate-500 px-3 py-1.5 rounded-lg transition-colors cursor-pointer"
            onClick={handleLogout}
          >
            Logout
          </button>
        </div>
      </header>

      <div className="px-6 py-4 flex items-center justify-between">
        <input
          type="text"
          className="border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-800 text-slate-900 dark:text-white rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 w-56"
          placeholder="Filter by colour…"
          value={colour}
          onChange={(e) => setColour(e.target.value)}
        />
        <button
          className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-semibold px-4 py-2 rounded-lg transition-colors cursor-pointer"
          onClick={() => setShowModal(true)}
        >
          + Create Product
        </button>
      </div>

      <div className="px-6 pb-8 flex-1">
        <ProductTable products={products} loading={loading} onEdit={setEditProduct} onDeleted={fetchProducts} />
      </div>

      {showModal && (
        <ProductModal
          onClose={() => setShowModal(false)}
          onProductCreated={fetchProducts}
        />
      )}

      {editProduct && (
        <ProductModal
          product={editProduct}
          onClose={() => setEditProduct(null)}
          onProductCreated={fetchProducts}
        />
      )}
    </div>
  );
}
