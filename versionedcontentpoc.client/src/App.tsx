import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import './App.css'
import HomePage from './pages/HomePage';
import CreateContentPage from './pages/CreateContentPage';
import SelectContentPage from './pages/SelectContentPage';
import UpdateContentPage from './pages/UpdateContentPage';

function App() {
  return (
      <Router>
          <div>
              {/* Navigation */}
              <nav style={{ padding: '1rem', borderBottom: '1px solid #ccc' }}>
                  <Link to="/" style={{ marginRight: 10 }}>Home</Link>
                  <Link to="/create" style={{ marginRight: 10 }}>Create</Link>
              </nav>

              {/* Routes */}
              <Routes>
                  <Route path="/" element={<HomePage />} />
                  <Route path="/create" element={<SelectContentPage />} />
                  <Route path="/create/:contentType" element={<CreateContentPage />} />
                  <Route path="/update/:contentId" element={<UpdateContentPage />} />
              </Routes>
          </div>
      </Router>
  )
}

export default App
