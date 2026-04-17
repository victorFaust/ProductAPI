import React, { useState, type FormEvent } from 'react';
import { createProduct } from '../api/productsApi';
import type { CreateProductRequest } from '../types';
import styles from './CreateProductForm.module.css';

interface Props {
  onProductCreated: () => void;
}

const EMPTY_FORM: CreateProductRequest = {
  name: '',
  description: '',
  price: 0,
  colour: '',
};

export default function CreateProductForm({ onProductCreated }: Props) {
  const [form, setForm] = useState<CreateProductRequest>(EMPTY_FORM);
  const [errors, setErrors] = useState<Partial<Record<keyof CreateProductRequest, string>>>({});
  const [status, setStatus] = useState<'idle' | 'loading' | 'success' | 'error'>('idle');
  const [serverError, setServerError] = useState('');

  function validate(): boolean {
    const next: typeof errors = {};
    if (!form.name.trim()) next.name = 'Name is required.';
    if (form.price <= 0) next.price = 'Price must be greater than 0.';
    if (!form.colour.trim()) next.colour = 'Colour is required.';
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!validate()) return;
    setStatus('loading');
    setServerError('');
    try {
      await createProduct(form);
      setForm(EMPTY_FORM);
      setErrors({});
      setStatus('success');
      onProductCreated();
      setTimeout(() => setStatus('idle'), 3000);
    } catch {
      setServerError('Failed to create product. Please try again.');
      setStatus('error');
    }
  }

  function handleChange(field: keyof CreateProductRequest, value: string | number) {
    setForm((prev) => ({ ...prev, [field]: value }));
    if (errors[field]) setErrors((prev) => ({ ...prev, [field]: undefined }));
  }

  return (
    <div className={styles.card}>
      <h2 className={styles.heading}>New Product</h2>
      <form onSubmit={handleSubmit} className={styles.form} noValidate>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="pName">
            Name <span className={styles.required}>*</span>
          </label>
          <input
            id="pName"
            type="text"
            className={`${styles.input} ${errors.name ? styles.inputError : ''}`}
            value={form.name}
            onChange={(e) => handleChange('name', e.target.value)}
          />
          {errors.name && <span className={styles.fieldError}>{errors.name}</span>}
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="pDescription">
            Description
          </label>
          <input
            id="pDescription"
            type="text"
            className={styles.input}
            value={form.description}
            onChange={(e) => handleChange('description', e.target.value)}
          />
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="pPrice">
            Price <span className={styles.required}>*</span>
          </label>
          <input
            id="pPrice"
            type="number"
            min="0.01"
            step="0.01"
            className={`${styles.input} ${errors.price ? styles.inputError : ''}`}
            value={form.price === 0 ? '' : form.price}
            onChange={(e) => handleChange('price', parseFloat(e.target.value) || 0)}
          />
          {errors.price && <span className={styles.fieldError}>{errors.price}</span>}
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="pColour">
            Colour <span className={styles.required}>*</span>
          </label>
          <input
            id="pColour"
            type="text"
            className={`${styles.input} ${errors.colour ? styles.inputError : ''}`}
            value={form.colour}
            onChange={(e) => handleChange('colour', e.target.value)}
          />
          {errors.colour && <span className={styles.fieldError}>{errors.colour}</span>}
        </div>

        {status === 'success' && (
          <p className={styles.success}>Product created successfully!</p>
        )}
        {status === 'error' && (
          <p className={styles.errorMsg}>{serverError}</p>
        )}

        <button
          type="submit"
          className={styles.button}
          disabled={status === 'loading'}
        >
          {status === 'loading' ? 'Creating…' : 'Create Product'}
        </button>
      </form>
    </div>
  );
}
