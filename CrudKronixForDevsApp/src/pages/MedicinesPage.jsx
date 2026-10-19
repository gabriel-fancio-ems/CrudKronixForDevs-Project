import { useCallback, useEffect, useMemo, useState } from 'react'
import MedicineForm from '../components/MedicineForm'
import MedicineTable from '../components/MedicineTable'
import PageHeader from '../components/PageHeader'
import {
  createMedicamento,
  deleteMedicamento,
  getMedicamentos,
  updateMedicamento,
} from '../services/api'
import { getErrorMessage } from '../utils/errors'

const emptyMedicine = {
  nome: '',
  principioAtivo: '',
  fabricante: '',
  preco: '',
  quantidadeEstoque: '',
  dataValidade: '',
}

// Consulta a API e valida o formato esperado antes de permitir que a página
// trate o resultado como uma lista de registros.
async function requestMedicines(token) {
  const result = await getMedicamentos(token)
  if (!Array.isArray(result)) {
    throw new Error('A API retornou uma lista de medicamentos inválida.')
  }
  return result
}

// Controla a tela de medicamentos: dados da API, formulário, filtro, ordenação
// e operações de criação, atualização e remoção.
function MedicinesPage({ token }) {
  const [medicines, setMedicines] = useState([])
  const [form, setForm] = useState(emptyMedicine)
  const [editingId, setEditingId] = useState(null)
  const [search, setSearch] = useState('')
  const [sort, setSort] = useState({ key: 'nome', direction: 'asc' })
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')

  // Reutiliza a consulta da lista após mudanças. `showLoading` permite exibir
  // o estado de carregamento nas atualizações disparadas por ações do usuário.
  const loadMedicines = useCallback(
    async (showLoading = false) => {
      if (showLoading) setLoading(true)
      try {
        setMedicines(await requestMedicines(token))
        setError('')
      } catch (loadError) {
        setError(getErrorMessage(loadError))
      } finally {
        setLoading(false)
      }
    },
    [token],
  )

  // Faz a consulta inicial quando a página é montada ou quando o token muda.
  // A flag impede atualizações de estado se a tela desmontar antes da resposta.
  useEffect(() => {
    let active = true
    requestMedicines(token)
      .then((result) => {
        if (!active) return
        setMedicines(result)
        setError('')
      })
      .catch((loadError) => {
        if (active) setError(getErrorMessage(loadError))
      })
      .finally(() => {
        if (active) setLoading(false)
      })
    return () => {
      active = false
    }
  }, [token])

  // Calcula uma cópia filtrada e ordenada, sem modificar a lista original.
  // O cálculo só é repetido quando lista, busca ou critério de ordenação mudam.
  const visibleMedicines = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('pt-BR')
    return [...medicines]
      .filter((medicine) =>
        [medicine.nome, medicine.principioAtivo, medicine.fabricante]
          .filter(Boolean)
          .some((value) => value.toLocaleLowerCase('pt-BR').includes(term)),
      )
      .sort((first, second) => {
        const firstValue = first[sort.key] ?? ''
        const secondValue = second[sort.key] ?? ''
        const comparison =
          typeof firstValue === 'number' && typeof secondValue === 'number'
            ? firstValue - secondValue
            : String(firstValue).localeCompare(String(secondValue), 'pt-BR', {
                numeric: true,
                sensitivity: 'base',
              })
        return sort.direction === 'asc' ? comparison : -comparison
      })
  }, [medicines, search, sort])

  // Atualiza somente o campo alterado, identificando-o pelo atributo `name`
  // do input; os demais valores digitados são preservados.
  function updateField(event) {
    setForm((current) => ({ ...current, [event.target.name]: event.target.value }))
  }

  // Limpa os valores do formulário e encerra o modo de edição.
  function resetForm() {
    setForm(emptyMedicine)
    setEditingId(null)
  }

  // Alterna a direção da ordenação ao clicar novamente na mesma coluna;
  // ao escolher outra coluna, inicia a ordenação crescente.
  function toggleSort(key) {
    setSort((current) => ({
      key,
      direction:
        current.key === key && current.direction === 'asc' ? 'desc' : 'asc',
    }))
  }

  // Decide entre criar e atualizar conforme exista um ID em edição. Converte
  // campos numéricos e transforma validade vazia em `null` antes da request.
  async function handleSubmit(event) {
    event.preventDefault()
    setSaving(true)
    setError('')
    setNotice('')
    const payload = {
      ...form,
      preco: Number(form.preco),
      quantidadeEstoque: Number(form.quantidadeEstoque),
      dataValidade: form.dataValidade || null,
    }

    try {
      if (editingId) {
        await updateMedicamento(editingId, payload, token)
        setNotice('Medicamento atualizado com sucesso.')
      } else {
        await createMedicamento(payload, token)
        setNotice('Medicamento cadastrado com sucesso.')
      }
      resetForm()
      await loadMedicines(true)
    } catch (saveError) {
      setError(getErrorMessage(saveError))
    } finally {
      setSaving(false)
    }
  }

  // Copia os valores do medicamento selecionado para o formulário, ativa o
  // modo de edição e desloca a página até o formulário.
  function startEditing(medicine) {
    setForm({
      nome: medicine.nome ?? '',
      principioAtivo: medicine.principioAtivo ?? '',
      fabricante: medicine.fabricante ?? '',
      preco: medicine.preco ?? '',
      quantidadeEstoque: medicine.quantidadeEstoque ?? '',
      dataValidade: medicine.dataValidade?.slice(0, 10) ?? '',
    })
    setEditingId(medicine.id)
    setNotice('')
    document.querySelector('.form-card')?.scrollIntoView({ behavior: 'smooth' })
  }

  // Solicita confirmação antes da exclusão e, em caso de sucesso, atualiza a
  // lista. Se o item excluído estiver sendo editado, limpa também o formulário.
  async function handleDelete(medicine) {
    if (!window.confirm(`Excluir o medicamento "${medicine.nome}"?`)) return
    setError('')
    setNotice('')
    try {
      await deleteMedicamento(medicine.id, token)
      setNotice('Medicamento excluído com sucesso.')
      if (editingId === medicine.id) resetForm()
      await loadMedicines(true)
    } catch (deleteError) {
      setError(getErrorMessage(deleteError))
    }
  }

  return (
    <>
      <PageHeader
        eyebrow="CATÁLOGO"
        title="Medicamentos"
        description="Cadastre e gerencie os medicamentos disponíveis."
        trailing={<span className="count-badge">{medicines.length} itens</span>}
      />
      <MedicineForm
        form={form}
        editing={Boolean(editingId)}
        saving={saving}
        error={error}
        notice={notice}
        onFieldChange={updateField}
        onSubmit={handleSubmit}
        onCancel={resetForm}
      />
      <MedicineTable
        medicines={medicines}
        visibleMedicines={visibleMedicines}
        search={search}
        loading={loading}
        error={error}
        sort={sort}
        onSearch={setSearch}
        onSort={toggleSort}
        onEdit={startEditing}
        onDelete={handleDelete}
      />
    </>
  )
}

export default MedicinesPage
