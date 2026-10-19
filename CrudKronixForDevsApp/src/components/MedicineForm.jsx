import { Plus } from 'lucide-react'

// Formulário controlado: todos os valores vêm da página de Medicamentos e
// cada alteração ou envio é comunicado de volta por callbacks.
function MedicineForm({
  form,
  editing,
  saving,
  error,
  notice,
  onFieldChange,
  onSubmit,
  onCancel,
}) {
  return (
    <section className="card form-card">
      <div className="section-heading">
        <div>
          <h2>{editing ? 'Editar medicamento' : 'Cadastrar medicamento'}</h2>
          <p>Preencha os dados do produto.</p>
        </div>
        {editing && (
          <button className="button button-secondary" onClick={onCancel}>
            Cancelar edição
          </button>
        )}
      </div>
      <form onSubmit={onSubmit}>
        <div className="form-grid">
          <label className="field">
            <span>Nome</span>
            <input
              name="nome"
              value={form.nome}
              onChange={onFieldChange}
              required
            />
          </label>
          <label className="field">
            <span>Princípio ativo</span>
            <input
              name="principioAtivo"
              value={form.principioAtivo}
              onChange={onFieldChange}
              required
            />
          </label>
          <label className="field">
            <span>Fabricante</span>
            <input
              name="fabricante"
              value={form.fabricante}
              onChange={onFieldChange}
              required
            />
          </label>
          <label className="field">
            <span>Preço (R$)</span>
            <input
              name="preco"
              type="number"
              min="0.01"
              step="0.01"
              value={form.preco}
              onChange={onFieldChange}
              required
            />
          </label>
          <label className="field">
            <span>Quantidade em estoque</span>
            <input
              name="quantidadeEstoque"
              type="number"
              min="0"
              step="1"
              value={form.quantidadeEstoque}
              onChange={onFieldChange}
              required
            />
          </label>
          <label className="field">
            <span>Data de validade <small>(opcional)</small></span>
            <input
              name="dataValidade"
              type="date"
              value={form.dataValidade}
              onChange={onFieldChange}
            />
          </label>
        </div>
        {error && <p className="message message-error">{error}</p>}
        {notice && <p className="message message-success">{notice}</p>}
        <div className="form-actions">
          <button className="button button-primary" disabled={saving}>
            <Plus size={18} aria-hidden="true" />
            {saving
              ? 'Salvando...'
              : editing
                ? 'Salvar alterações'
                : 'Cadastrar medicamento'}
          </button>
        </div>
      </form>
    </section>
  )
}

export default MedicineForm
