// src/App.jsx
import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Container, Navbar, Nav, Button } from 'react-bootstrap';
import { Globe, ShieldCheck } from 'react-bootstrap-icons';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

// Импорт страниц
//import TAPage from './pages/TAPage';
import CalendarPage from './components/dashboard/CalendarPage';
import SchedulePage from './components/dashboard/ShedulePage';
import Dashboard from './components/dashboard/Dashboard';

function App() {
  const [language, setLanguage] = useState('ru');
  const [isHttps, setIsHttps] = useState(false);

  return (
    <Router>
      <div className="app-container">
        {/* Навбар строго по ТЗ */}
        <Navbar bg="dark" variant="dark" expand="lg" className="border-bottom">
          <Container fluid>
            <Navbar.Brand href="/" className="fw-bold">
              <span className="text-warning">Vending</span> Franchise System
            </Navbar.Brand>
            
            <Navbar.Toggle aria-controls="basic-navbar-nav" />
            
            <Navbar.Collapse id="basic-navbar-nav">
              <Nav className="me-auto">
                <Nav.Link href="/">Главная</Nav.Link>
                <Nav.Link href="/ta">ТА</Nav.Link>
                <Nav.Link href="/calendar">Календарь обслуживания</Nav.Link>
                <Nav.Link href="/schedule">График работ</Nav.Link>
              </Nav>
              
              {/* Требования ТЗ: мультиязычность и безопасность */}
              <div className="d-flex align-items-center gap-3">
                <div className="d-flex align-items-center">
                  <Globe className="me-2 text-muted" />
                  <select 
                    className="form-select form-select-sm bg-dark text-white border-secondary"
                    value={language}
                    onChange={(e) => setLanguage(e.target.value)}
                    style={{ width: 'auto' }}
                  >
                    <option value="ru">🇷🇺 Русский</option>
                    <option value="en">🇺🇸 English</option>
                  </select>
                </div>
                
                <div className="d-flex align-items-center">
                  <ShieldCheck className="me-2 text-muted" />
                  <Button 
                    variant={isHttps ? "success" : "outline-success"} 
                    size="sm"
                    onClick={() => setIsHttps(!isHttps)}
                  >
                    {isHttps ? 'HTTPS' : 'HTTP'}
                  </Button>
                </div>
              </div>
            </Navbar.Collapse>
          </Container>
        </Navbar>

        {/* Основной контент */}
        <Container fluid className="main-content py-4">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            {/*<Route path="/ta" element={<TAPage />} />*/}
            <Route path="/calendar" element={<CalendarPage />} />
            <Route path="/schedule" element={<SchedulePage />} />
            
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </Container>
      </div>
    </Router>
  );
}

export default App;