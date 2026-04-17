import React from 'react';
import type { ProductDto } from '../types';
import styles from './ProductTable.module.css';

interface Props {
  products: ProductDto[];
  loading: boolean;
}

const SKELETON_ROWS = 4;

function ColourBadge({ colour }: { colour: string }) {
  return (
    <span className={styles.badgeWrapper}>
      <span
        className={styles.dot}
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
    <div className={styles.card}>
      <table className={styles.table}>
        <thead>
          <tr>
            <th>Name</th>
            <th>Description</th>
            <th>Price</th>
            <th>Colour</th>
            <th>Created</th>
          </tr>
        </thead>
        <tbody>
          {loading
            ? Array.from({ length: SKELETON_ROWS }).map((_, i) => (
                <tr key={i} className={styles.skeletonRow}>
                  {Array.from({ length: 5 }).map((_, j) => (
                    <td key={j}>
                      <span className={styles.skeleton} />
                    </td>
                  ))}
                </tr>
              ))
            : products.length === 0
            ? (
                <tr>
                  <td colSpan={5} className={styles.empty}>
                    No products found. Create one to get started.
                  </td>
                </tr>
              )
            : products.map((p) => (
                <tr key={p.id}>
                  <td className={styles.nameCell}>{p.name}</td>
                  <td className={styles.descCell}>{p.description || '—'}</td>
                  <td className={styles.priceCell}>{formatCurrency(p.price)}</td>
                  <td>
                    <ColourBadge colour={p.colour} />
                  </td>
                  <td className={styles.dateCell}>{formatDate(p.createdAt)}</td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  );
}
