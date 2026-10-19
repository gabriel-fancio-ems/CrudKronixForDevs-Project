import { useCallback, useEffect, useState } from 'react'
import { Upload } from 'lucide-react'
import PageHeader from '../components/PageHeader'
import { createAnexo, deleteAnexo, getAnexoById, getAnexos } from '../services/api'
import { getErrorMessage } from '../utils/errors'
import { formatDate } from '../utils/formatDate'

// Gerencia os anexos persistidos pela API: carrega a lista, envia arquivos
// multipart, consulta detalhes e exclui itens após confirmação do usuário.
function AttachmentsPage({ token }) {
  const [attachments, setAttachments] = useState([])
  const [loading, setLoading] = useState(true)
  const [uploading, setUploading] = useState(false)
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')

  // Atualiza a tabela usando a resposta da API e informa explicitamente caso
  // o servidor retorne um formato diferente da lista esperada.
  const loadAttachments = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      const result = await getAnexos(token)
      if (!Array.isArray(result)) {
        throw new Error('A API retornou uma lista de anexos inválida.')
      }
      setAttachments(result)
    } catch (loadError) {
      setError(getErrorMessage(loadError))
    } finally {
      setLoading(false)
    }
  }, [token])

  // Carrega a lista inicial. A flag impede que uma resposta tardia altere o
  // estado depois que a página desmontar ou a sessão trocar de token.
  useEffect(() => {
    let active = true
    getAnexos(token)
      .then((result) => {
        if (!Array.isArray(result)) {
          throw new Error('A API retornou uma lista de anexos inválida.')
        }
        if (active) setAttachments(result)
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

  // Envia cada arquivo selecionado em uma request multipart separada, para
  // manter o contrato de POST /api/anexos que recebe um único arquivo.
  async function uploadFiles(event) {
    const selectedFiles = Array.from(event.target.files ?? [])
    event.target.value = ''
    if (selectedFiles.length === 0) return

    setUploading(true)
    setError('')
    setNotice('')
    try {
      for (const file of selectedFiles) {
        await createAnexo(file, token)
      }
      setNotice(
        selectedFiles.length === 1
          ? 'Anexo enviado com sucesso.'
          : `${selectedFiles.length} anexos enviados com sucesso.`,
      )
      await loadAttachments()
    } catch (uploadError) {
      const message = getErrorMessage(uploadError)
      await loadAttachments()
      setError(message)
    } finally {
      setUploading(false)
    }
  }

  // Consulta os detalhes remotos e mostra os campos mais úteis em uma
  // notificação simples, sem presumir que a API ofereça uma tela de edição.
  async function viewAttachment(attachment) {
    try {
      const details = await getAnexoById(attachment.id, token)
      const name =
        details.fileName || details.name || attachment.fileName || attachment.name
      const contentType = details.contentType || attachment.contentType
      const size = details.length ?? details.size ?? attachment.length ?? attachment.size
      window.alert(
        [
          `Arquivo: ${name || '—'}`,
          `Tipo: ${contentType || '—'}`,
          `Tamanho: ${size == null ? '—' : `${size} bytes`}`,
        ].join('\n'),
      )
    } catch (detailsError) {
      setError(getErrorMessage(detailsError))
    }
  }

  // Solicita confirmação e remove o registro pela API; a tabela é recarregada
  // depois da operação para refletir o estado persistido no servidor.
  async function removeAttachment(attachment) {
    const name = attachment.fileName || attachment.name || 'este anexo'
    if (!window.confirm(`Remover o anexo "${name}"?`)) return
    setError('')
    setNotice('')
    try {
      await deleteAnexo(attachment.id, token)
      setNotice('Anexo excluído com sucesso.')
      await loadAttachments()
    } catch (deleteError) {
      setError(getErrorMessage(deleteError))
    }
  }

  return (
    <>
      <PageHeader
        eyebrow="DOCUMENTOS"
        title="Anexos"
        description="Organize os arquivos associados ao catálogo."
      />
      <section className="card table-card">
        <div className="section-heading table-heading">
          <div>
            <h2>Arquivos anexados</h2>
            <p>Arquivos armazenados pela API.</p>
          </div>
          <label className="button button-primary upload-button">
            <Upload size={18} aria-hidden="true" />
            {uploading ? 'Enviando...' : 'Fazer upload'}
            <input
              type="file"
              multiple
              disabled={uploading}
              onChange={uploadFiles}
            />
          </label>
        </div>
        {error && <p className="message message-error">{error}</p>}
        {notice && <p className="message message-success">{notice}</p>}
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Arquivo</th>
                <th>Tamanho</th>
                <th>Adicionado em</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td className="empty-state" colSpan={4}>
                    Carregando anexos...
                  </td>
                </tr>
              ) : attachments.length === 0 ? (
                <tr>
                  <td className="empty-state" colSpan={4}>
                    Nenhum anexo cadastrado ainda.
                  </td>
                </tr>
              ) : (
                attachments.map((attachment) => (
                  <tr key={attachment.id}>
                    <td className="medicine-name">
                      {attachment.fileName || attachment.name || 'Arquivo sem nome'}
                    </td>
                    <td>
                      {Number(
                        attachment.length ?? attachment.size ?? 0,
                      ).toLocaleString('pt-BR')}{' '}
                      bytes
                    </td>
                    <td>
                      {formatDate(
                        attachment.createdAt ||
                          attachment.uploadedAt ||
                          attachment.addedAt,
                      )}
                    </td>
                    <td>
                      <div className="row-actions">
                        <button
                          className="text-action"
                          onClick={() => viewAttachment(attachment)}
                        >
                          Detalhes
                        </button>
                        <button
                          className="text-action text-danger"
                          onClick={() => removeAttachment(attachment)}
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
        <p className="local-note">
          O envio é feito em multipart/form-data. Cada arquivo é enviado
          individualmente para a API.
        </p>
      </section>
    </>
  )
}

export default AttachmentsPage
