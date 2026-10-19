import { Search } from 'lucide-react'
import { formatDate } from '../utils/formatDate'

const columns = [
  ['nome', 'Medicamento'],
  ['principioAtivo', 'Princípio ativo'],
  ['fabricante', 'Fabricante'],
  ['preco', 'Preço'],
  ['quantidadeEstoque', 'Estoque'],
  ['dataValidade', 'Validade'],
]

// Apresenta a lista recebida da página. Busca, ordenação e ações são delegadas
// ao componente pai para manter a tabela focada somente na apresentação.
function MedicineTable({
  medicines,
  visibleMedicines,
  search,
  loading,
  error,
  sort,
  onSearch,
  onSort,
  onEdit,
  onDelete,
}) {
  return (
    <section className="card table-card">
      <div className="section-heading table-heading">
        <div>
          <h2>Medicamentos cadastrados</h2>
          <p>Consulte, ordene ou atualize os registros.</p>
        </div>
        <label className="search-field">
          <Search size={18} aria-hidden="true" />
          <input
            value={search}
            onChange={(event) => onSearch(event.target.value)}
            placeholder="Buscar medicamento"
            aria-label="Buscar medicamento"
          />
        </label>
      </div>
      {error && !loading && <p className="message message-error">{error}</p>}
      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              {columns.map(([key, label]) => (
                <th key={key}>
                  <button
                    className="sort-button"
                    onClick={() => onSort(key)}
                    aria-label={`Ordenar por ${label}`}
                  >
                    {label}
                    <span>
                      {sort.key === key
                        ? sort.direction === 'asc'
                          ? '↑'
                          : '↓'
                        : '↕'}
                    </span>
                  </button>
                </th>
              ))}
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td className="empty-state" colSpan={7}>
                  Carregando medicamentos...
                </td>
              </tr>
            ) : visibleMedicines.length === 0 ? (
              <tr>
                <td className="empty-state" colSpan={7}>
                  {search
                    ? 'Nenhum medicamento encontrado.'
                    : 'Nenhum medicamento cadastrado ainda.'}
                </td>
              </tr>
            ) : (
              visibleMedicines.map((medicine) => (
                <tr key={medicine.id}>
                  <td className="medicine-name">{medicine.nome}</td>
                  <td>{medicine.principioAtivo}</td>
                  <td>{medicine.fabricante}</td>
                  <td>
                    {Number(medicine.preco).toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: 'BRL',
                    })}
                  </td>
                  <td>{medicine.quantidadeEstoque}</td>
                  <td>{formatDate(medicine.dataValidade)}</td>
                  <td>
                    <div className="row-actions">
                      <button
                        className="text-action"
                        onClick={() => onEdit(medicine)}
                      >
                        Editar
                      </button>
                      <button
                        className="text-action text-danger"
                        onClick={() => onDelete(medicine)}
                      >
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
      <p className="table-footer">
        Exibindo {visibleMedicines.length} de {medicines.length} medicamentos
      </p>
    </section>
  )
}

export default MedicineTable
