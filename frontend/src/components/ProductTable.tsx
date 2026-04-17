import { useState } from 'react';
import { toast } from 'react-toastify';
import { deleteProduct } from '../api/productsApi';
import type { ProductDto } from '../types';

interface Props {
  products: ProductDto[];
  loading: boolean;
  onEdit: (product: ProductDto) => void;
  onDeleted: () => void;
}

const SKELETON_ROWS = 4;

function ColourBadge({ colour }: { colour: string }) {
  return (
    <span className="inline-flex items-center gap-1.5 text-sm dark:text-slate-300">
      <span
        className="w-3 h-3 rounded-full inline-block flex-shrink-0"
        style={{ backgroundColor: colour.toLowerCase() }}
        aria-hidden="true"
      />
      {colour}
    </span>
  );
}

function formatCurrency(value: number) {
  return new Intl.NumberFormat('en-GB', {
    style: 'currency',
    currency: 'GBP',
  }).format(value);
}

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  });
}

function DeleteButton({ id, onDeleted }: { id: string; onDeleted: () => void }) {
  const [confirming, setConfirming] = useState(false);
  const [loading, setLoading] = useState(false);

  async function handleDelete() {
    setLoading(true);
    try {
      await deleteProduct(id);
      toast.success('Product deleted.');
      onDeleted();
    } catch {
      toast.error('Failed to delete product.');
      setLoading(false);
      setConfirming(false);
    }
  }

  if (confirming) {
    return (
      <span className="inline-flex items-center gap-1">
        <button
          className="text-xs text-red-600 hover:text-red-800 font-semibold cursor-pointer disabled:opacity-50"
          onClick={handleDelete}
          disabled={loading}
        >
          {loading ? '…' : 'Confirm'}
        </button>
        <button
          className="text-xs text-slate-500 hover:text-slate-700 cursor-pointer"
          onClick={() => setConfirming(false)}
        >
          Cancel
        </button>
      </span>
    );
  }

  return (
    <button
      className="text-xs text-red-500 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300 cursor-pointer"
      onClick={() => setConfirming(true)}
    >
      Delete
    </button>
  );
}

export default function ProductTable({ products, loading, onEdit, onDeleted }: Props) {
  return (
    <div className="bg-white dark:bg-slate-800 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-700 overflow-hidden">
      <table className="w-full text-sm text-left">
        <thead className="bg-slate-50 dark:bg-slate-700/50 border-b border-slate-200 dark:border-slate-700">
          <tr>
            <th className="px-4 py-3 font-semibold text-slate-600 dark:text-slate-300">Name</th>
            <th className="px-4 py-3 font-semibold text-slate-600 dark:text-slate-300">Description</th>
            <th className="px-4 py-3 font-semibold text-slate-600 dark:text-slate-300">Price</th>
            <th className="px-4 py-3 font-semibold text-slate-600 dark:text-slate-300">Colour</th>
            <th className="px-4 py-3 font-semibold text-slate-600 dark:text-slate-300">Created</th>
            <th className="px-4 py-3 font-semibold text-slate-600 dark:text-slate-300">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-700">
          {loading
            ? Array.from({ length: SKELETON_ROWS }).map((_, i) => (
                <tr key={i}>
                  {Array.from({ length: 6 }).map((_, j) => (
                    <td key={j} className="px-4 py-3">
                      <span className="block h-4 bg-slate-200 dark:bg-slate-700 rounded animate-pulse w-24" />
                    </td>
                  ))}
                </tr>
              ))
            : products.length === 0
            ? (
                <tr>
                  <td colSpan={6} className="px-4 py-8 text-center text-slate-400 dark:text-slate-500">
                    No products found. Create one to get started.
                  </td>
                </tr>
              )
            : products.map((p) => (
                <tr key={p.id} className="hover:bg-slate-50 dark:hover:bg-slate-700/40 transition-colors">
                  <td className="px-4 py-3 font-medium text-slate-900 dark:text-white">{p.name}</td>
                  <td className="px-4 py-3 text-slate-500 dark:text-slate-400 max-w-xs truncate">{p.description || '—'}</td>
                  <td className="px-4 py-3 font-mono text-slate-800 dark:text-slate-200">{formatCurrency(p.price)}</td>
                  <td className="px-4 py-3">
                    <ColourBadge colour={p.colour} />
                  </td>
                  <td className="px-4 py-3 text-slate-500 dark:text-slate-400">{formatDate(p.createdAt)}</td>
                  <td className="px-4 py-3">
                    <span className="inline-flex items-center gap-3">
                      <button
                        className="text-xs text-indigo-600 hover:text-indigo-800 dark:text-indigo-400 dark:hover:text-indigo-300 cursor-pointer"
                        onClick={() => onEdit(p)}
                      >
                        Edit
                      </button>
                      <DeleteButton id={p.id} onDeleted={onDeleted} />
                    </span>
                  </td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  );
}

