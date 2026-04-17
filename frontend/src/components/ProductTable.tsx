import type { ProductDto } from '../types';

interface Props {
  products: ProductDto[];
  loading: boolean;
}

const SKELETON_ROWS = 4;

function ColourBadge({ colour }: { colour: string }) {
  return (
    <span className="inline-flex items-center gap-1.5 text-sm">
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

export default function ProductTable({ products, loading }: Props) {
  return (
    <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
      <table className="w-full text-sm text-left">
        <thead className="bg-slate-50 border-b border-slate-200">
          <tr>
            <th className="px-4 py-3 font-semibold text-slate-600">Name</th>
            <th className="px-4 py-3 font-semibold text-slate-600">Description</th>
            <th className="px-4 py-3 font-semibold text-slate-600">Price</th>
            <th className="px-4 py-3 font-semibold text-slate-600">Colour</th>
            <th className="px-4 py-3 font-semibold text-slate-600">Created</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {loading
            ? Array.from({ length: SKELETON_ROWS }).map((_, i) => (
                <tr key={i}>
                  {Array.from({ length: 5 }).map((_, j) => (
                    <td key={j} className="px-4 py-3">
                      <span className="block h-4 bg-slate-200 rounded animate-pulse w-24" />
                    </td>
                  ))}
                </tr>
              ))
            : products.length === 0
            ? (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-slate-400">
                    No products found. Create one to get started.
                  </td>
                </tr>
              )
            : products.map((p) => (
                <tr key={p.id} className="hover:bg-slate-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-slate-900">{p.name}</td>
                  <td className="px-4 py-3 text-slate-500 max-w-xs truncate">{p.description || '—'}</td>
                  <td className="px-4 py-3 font-mono text-slate-800">{formatCurrency(p.price)}</td>
                  <td className="px-4 py-3">
                    <ColourBadge colour={p.colour} />
                  </td>
                  <td className="px-4 py-3 text-slate-500">{formatDate(p.createdAt)}</td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  );
}
