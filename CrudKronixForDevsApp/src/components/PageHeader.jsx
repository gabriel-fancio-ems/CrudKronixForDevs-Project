// Cabeçalho comum das páginas. Recebe textos pela propriedade e permite um
// elemento opcional à direita, como o contador de medicamentos.
function PageHeader({ eyebrow, title, description, trailing }) {
  return (
    <header className="page-heading">
      <div>
        <p className="eyebrow">{eyebrow}</p>
        <h1>{title}</h1>
        <p>{description}</p>
      </div>
      {trailing}
    </header>
  )
}

export default PageHeader
