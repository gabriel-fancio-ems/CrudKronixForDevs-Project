import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
})

function withAuthorization(token) {
  return {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  }
}

export async function login(email, password) {
  const { data } = await api.post('/auth/login', { email, password })
  return data
}

export async function getMedicamentos(token) {
  const { data } = await api.get('/medicamentos', withAuthorization(token))
  return data
}

export async function createMedicamento(medicamento, token) {
  const { data } = await api.post(
    '/medicamentos',
    medicamento,
    withAuthorization(token),
  )
  return data
}

export async function getMedicamentoById(id, token) {
  const { data } = await api.get(
    `/medicamentos/${id}`,
    withAuthorization(token),
  )
  return data
}

export async function updateMedicamento(id, medicamento, token) {
  const { data } = await api.put(
    `/medicamentos/${id}`,
    medicamento,
    withAuthorization(token),
  )
  return data
}

export async function deleteMedicamento(id, token) {
  const { data } = await api.delete(
    `/medicamentos/${id}`,
    withAuthorization(token),
  )
  return data
}

// Envia o arquivo binário no formato multipart/form-data. O navegador e Axios
// adicionam o boundary do multipart automaticamente. O campo "arquivo" deve
// corresponder ao parâmetro IFormFile recebido pelo endpoint do backend.
export async function createAnexo(file, token) {
  const formData = new FormData()
  formData.append('arquivo', file)
  const { data } = await api.post(
    '/anexos',
    formData,
    withAuthorization(token),
  )
  return data
}

// Recupera todos os anexos visíveis ao usuário autenticado.
export async function getAnexos(token) {
  const { data } = await api.get('/anexos', withAuthorization(token))
  return data
}

// Busca os metadados ou conteúdo de um anexo individual pelo identificador.
export async function getAnexoById(id, token) {
  const { data } = await api.get(`/anexos/${id}`, withAuthorization(token))
  return data
}

// Remove permanentemente o anexo correspondente ao identificador informado.
export async function deleteAnexo(id, token) {
  const { data } = await api.delete(`/anexos/${id}`, withAuthorization(token))
  return data
}
