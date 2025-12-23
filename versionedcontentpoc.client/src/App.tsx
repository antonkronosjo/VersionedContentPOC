import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import './App.css'
import HomePage from './pages/HomePage';

function App() {
  return (
      <Router>
          <div>
              {/* Navigation */}
              <nav style={{ padding: '1rem', borderBottom: '1px solid #ccc' }}>
                  <Link to="/" style={{ marginRight: 10 }}>Home</Link>
                  <Link to="/about" style={{ marginRight: 10 }}>About</Link>
                  <Link to="/contact">Contact</Link>
              </nav>

              {/* Routes */}
              <Routes>
                  <Route path="/" element={<HomePage />} />
              </Routes>
          </div>
      </Router>
  )
}

export default App
