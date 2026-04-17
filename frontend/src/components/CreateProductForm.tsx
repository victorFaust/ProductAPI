import { useState, type FormEvent } from 'react';
import { toast } from 'react-toastify';
import { createProduct, updateProduct } from '../api/productsApi';
import type { CreateProductRequest, ProductDto } from '../types';

interface Props {
  onProductCreated: () => void;
  product?: ProductDto;
}

const EMPTY_FORM: CreateProductRequest = {
  name: '',
  description: '',
  price: 0,
  colour: '',
};

export default function CreateProductForm({ onProductCreated, product }: Props) {
  const isEdit = Boolean(product);
  const [form, setForm] = useState<CreateProductRequest>(
    product
      ? { name: product.name, description: product.description, price: product.price, colour: product.colour }
      : EMPTY_FORM
  );
  const [errors, setErrors] = useState<Partial<Record<keyof CreateProductRequest, string>>>({});
  const [status, setStatus] = useState<'idle' | 'loading' | 'error'>('idle');
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
      if (isEdit && product) {
        await updateProduct(product.id, form);
        toast.success('Product updated successfully!');
      } else {
        await createProduct(form);
        setForm(EMPTY_FORM);
        setErrors({});
        toast.success('Product created successfully!');
      }
      setStatus('idle');
      onProductCreated();
    } catch {
      const msg = isEdit ? 'Failed to update product.' : 'Failed to create product.';
      setServerError(msg);
      setStatus('error');
      toast.error(msg);
    }
  }

  function handleChange(field: keyof CreateProductRequest, value: string | number) {
    setForm((prev) => ({ ...prev, [field]: value }));
    if (errors[field]) setErrors((prev) => ({ ...prev, [field]: undefined }));
  }

  const inputBase = 'w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-slate-700 text-slate-900 dark:text-white';
  const inputNormal = `${inputBase} border-slate-300 dark:border-slate-600`;
  const inputError = `${inputBase} border-red-500`;

  return (
    <div>
      <h2 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">
        {isEdit ? 'Edit Product' : 'New Product'}
      </h2>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4" noValidate>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium text-slate-700 dark:text-slate-300" htmlFor="pName">
            Name <span className="text-red-500">*</span>
          </label>
          <input
            id="pName"
            type="text"
            className={errors.name ? inputError : inputNormal}
            value={form.name}
            onChange={(e) => handleChange('name', e.target.value)}
          />
          {errors.name && <span className="text-xs text-red-600">{errors.name}</span>}
        </div>

        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium text-slate-700 dark:text-slate-300" htmlFor="pDescription">
            Description
          </label>
          <input
            id="pDescription"
            type="text"
            className={inputNormal}
            value={form.description}
            onChange={(e) => handleChange('description', e.target.value)}
          />
        </div>

        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium text-slate-700 dark:text-slate-300" htmlFor="pPrice">
            Price <span className="text-red-500">*</span>
          </label>
          <input
            id="pPrice"
            type="number"
            min="0.01"
            step="0.01"
            className={errors.price ? inputError : inputNormal}
            value={form.price === 0 ? '' : form.price}
            onChange={(e) => handleChange('price', parseFloat(e.target.value) || 0)}
          />
          {errors.price && <span className="text-xs text-red-600">{errors.price}</span>}
        </div>

        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium text-slate-700 dark:text-slate-300" htmlFor="pColour">
            Colour <span className="text-red-500">*</span>
          </label>
          <input
            id="pColour"
            type="text"
            className={errors.colour ? inputError : inputNormal}
            value={form.colour}
            onChange={(e) => handleChange('colour', e.target.value)}
          />
          {errors.colour && <span className="text-xs text-red-600">{errors.colour}</span>}
        </div>

        {status === 'error' && (
          <p className="text-sm text-red-600">{serverError}</p>
        )}

        <button
          type="submit"
          className="bg-indigo-600 hover:bg-indigo-700 disabled:opacity-50 text-white font-semibold rounded-lg py-2 text-sm transition-colors cursor-pointer"
          disabled={status === 'loading'}
        >
          {status === 'loading' ? (isEdit ? 'Saving…' : 'Creating…') : (isEdit ? 'Save Changes' : 'Create Product')}
        </button>
      </form>
    </div>
  );
}
