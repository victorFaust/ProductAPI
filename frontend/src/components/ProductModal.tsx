import { useEffect } from 'react';
import CreateProductForm from './CreateProductForm';

interface Props {
  onClose: () => void;
  onProductCreated: () => void;
}

export default function ProductModal({ onClose, onProductCreated }: Props) {

  useEffect(() => {
    function onKey(e: KeyboardEvent) {
      if (e.key === 'Escape') onClose();
    }
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [onClose]);

  function handleCreated() {
    onProductCreated();
    onClose();
  }

  return (
    <div
      className="fixed inset-0 bg-black/45 flex items-center justify-center z-50 p-4"
      onClick={onClose}
    >
      <div
        className="bg-white rounded-2xl shadow-xl w-full max-w-md p-6 relative"
        onClick={(e) => e.stopPropagation()}
      >
        <button
          className="absolute top-4 right-4 text-slate-400 hover:text-slate-600 text-lg leading-none cursor-pointer"
          onClick={onClose}
          aria-label="Close"
        >
          ✕
        </button>
        <CreateProductForm onProductCreated={handleCreated} />
      </div>
    </div>
  );
}
