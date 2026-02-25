import React, { useState } from 'react'
import { createRoot } from 'react-dom/client'

function App() {
  const [dashboard, setDashboard] = useState(null)

  const loadDashboard = async () => {
    const response = await fetch('/api/practice/dashboard')
    const data = await response.json()
    setDashboard(data)
  }

  return (
    <main style={{ fontFamily: 'sans-serif', padding: 24 }}>
      <h1>MediTransact Office Console</h1>
      <button onClick={loadDashboard}>Load dashboard</button>
      {dashboard && <pre>{JSON.stringify(dashboard, null, 2)}</pre>}
    </main>
  )
}

createRoot(document.getElementById('root')).render(<App />)
