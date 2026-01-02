import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import './App.css'
import Game from './pages/Game'
import Admin from './pages/Admin'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/admin" element={<Admin />} />
        <Route path="/:id" element={<Game />} />
        <Route path="/" element={<Navigate to="/admin" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
