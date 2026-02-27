import React, { useState } from 'react'
import { createRoot } from 'react-dom/client'

const shellStyles = {
  app: {
    minHeight: '100vh',
    display: 'grid',
    gridTemplateRows: 'auto 1fr auto',
    backgroundColor: '#f3f6fb',
    color: '#1f2a37',
    fontFamily: 'Inter, Segoe UI, sans-serif',
  },
  header: {
    backgroundColor: '#0a4a7a',
    color: '#ffffff',
    boxShadow: '0 2px 8px rgba(0, 0, 0, 0.15)',
  },
  headerTop: {
    padding: '20px 24px 14px',
  },
  headerTitle: {
    margin: 0,
    fontSize: '1.5rem',
    fontWeight: 650,
    letterSpacing: '0.01em',
  },
  navBar: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '10px 24px',
    borderTop: '1px solid rgba(255, 255, 255, 0.2)',
    backgroundColor: '#0b3f69',
  },
  navLinks: {
    display: 'flex',
    gap: '18px',
    listStyle: 'none',
    margin: 0,
    padding: 0,
    fontSize: '0.95rem',
  },
  navLinkItem: {
    cursor: 'pointer',
    opacity: 0.95,
  },
  dropdownWrapper: {
    position: 'relative',
  },
  menuButton: {
    border: '1px solid rgba(255, 255, 255, 0.35)',
    borderRadius: 6,
    padding: '8px 12px',
    background: 'rgba(255, 255, 255, 0.08)',
    color: '#fff',
    fontSize: '0.9rem',
    cursor: 'pointer',
  },
  dropdown: {
    position: 'absolute',
    right: 0,
    top: 'calc(100% + 10px)',
    width: 280,
    backgroundColor: '#ffffff',
    color: '#1f2a37',
    borderRadius: 8,
    boxShadow: '0 8px 24px rgba(0, 0, 0, 0.18)',
    border: '1px solid #d8e0ea',
    padding: 16,
    zIndex: 10,
  },
  dropdownTitle: {
    margin: '0 0 12px',
    fontSize: '1rem',
  },
  formField: {
    display: 'grid',
    gap: 6,
    marginBottom: 10,
    fontSize: '0.85rem',
  },
  input: {
    border: '1px solid #c9d4e0',
    borderRadius: 6,
    padding: '9px 10px',
    fontSize: '0.9rem',
  },
  signInButton: {
    marginTop: 4,
    width: '100%',
    border: 0,
    borderRadius: 6,
    padding: '10px 12px',
    backgroundColor: '#0a4a7a',
    color: '#fff',
    fontWeight: 600,
    cursor: 'pointer',
  },
  content: {
    display: 'grid',
    gridTemplateColumns: '1fr 1.8fr 1fr',
    gap: 16,
    padding: 20,
  },
  panel: {
    backgroundColor: '#ffffff',
    borderRadius: 10,
    border: '1px solid #d8e0ea',
    padding: 16,
    boxShadow: '0 1px 2px rgba(31, 42, 55, 0.08)',
    minHeight: 280,
  },
  panelTitle: {
    marginTop: 0,
    marginBottom: 12,
    fontSize: '1.05rem',
  },
  actionButton: {
    border: 0,
    borderRadius: 6,
    padding: '10px 12px',
    backgroundColor: '#1f6cb5',
    color: '#fff',
    fontWeight: 600,
    cursor: 'pointer',
  },
  dashboardOutput: {
    marginTop: 12,
    backgroundColor: '#f7f9fc',
    border: '1px solid #e2e8f0',
    borderRadius: 6,
    padding: 12,
    fontSize: '0.85rem',
    overflowX: 'auto',
  },
  footer: {
    padding: '14px 20px',
    borderTop: '1px solid #d8e0ea',
    backgroundColor: '#ffffff',
    fontSize: '0.85rem',
    color: '#52606d',
  },
}

function App() {
  const [dashboard, setDashboard] = useState(null)
  const [showLogin, setShowLogin] = useState(false)

  const loadDashboard = async () => {
    const response = await fetch('/api/practice/dashboard')
    const data = await response.json()
    setDashboard(data)
  }

  return (
    <div style={shellStyles.app}>
      <header style={shellStyles.header}>
        <div style={shellStyles.headerTop}>
          <h1 style={shellStyles.headerTitle}>MediTransact Office Console</h1>
        </div>
        <nav style={shellStyles.navBar}>
          <ul style={shellStyles.navLinks}>
            <li style={shellStyles.navLinkItem}>Home</li>
            <li style={shellStyles.navLinkItem}>Claims</li>
            <li style={shellStyles.navLinkItem}>Scheduling</li>
            <li style={shellStyles.navLinkItem}>Reports</li>
          </ul>
          <div style={shellStyles.dropdownWrapper}>
            <button style={shellStyles.menuButton} onClick={() => setShowLogin((open) => !open)}>
              Account ▾
            </button>
            {showLogin && (
              <div style={shellStyles.dropdown}>
                <h2 style={shellStyles.dropdownTitle}>Sign In</h2>
                <label style={shellStyles.formField}>
                  Username
                  <input type="text" name="username" autoComplete="username" style={shellStyles.input} />
                </label>
                <label style={shellStyles.formField}>
                  Password
                  <input
                    type="password"
                    name="password"
                    autoComplete="current-password"
                    style={shellStyles.input}
                  />
                </label>
                <button style={shellStyles.signInButton}>Sign In</button>
              </div>
            )}
          </div>
        </nav>
      </header>

      <main style={shellStyles.content}>
        <section style={shellStyles.panel}>
          <h2 style={shellStyles.panelTitle}>Left Panel</h2>
          <p>Navigation shortcuts and status widgets can be displayed here.</p>
        </section>

        <section style={shellStyles.panel}>
          <h2 style={shellStyles.panelTitle}>Center Panel</h2>
          <button style={shellStyles.actionButton} onClick={loadDashboard}>
            Load dashboard
          </button>
          {dashboard && <pre style={shellStyles.dashboardOutput}>{JSON.stringify(dashboard, null, 2)}</pre>}
        </section>

        <section style={shellStyles.panel}>
          <h2 style={shellStyles.panelTitle}>Right Panel</h2>
          <p>Alerts, tasks, and quick actions can be pinned on this side.</p>
        </section>
      </main>

      <footer style={shellStyles.footer}>© {new Date().getFullYear()} MediTransact — Internal Use Only</footer>
    </div>
  )
}

createRoot(document.getElementById('root')).render(<App />)
