// src/App.jsx
import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Navbar, Nav, Button, Container } from 'react-bootstrap'; // Добавили Container
import { Globe, ShieldCheck } from 'react-bootstrap-icons';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

// Импорт страниц - ПРАВИЛЬНЫЕ ПУТИ:
import Dashboard from './components/dashboard/Dashboard';
import TAPage from './components/TaPage'; // ИЛИ './components/TAPage' в зависимости от структуры
import CalendarPage from './components/dashboard/CalendarPage'; // ИЛИ './components/CalendarPage'
import SchedulePage from './components/dashboard/ShedulePage';
import ErrorPage from './components/ErrorPage';
import LoginForm from './components/loginForm/LoginForm';
import JwtService from './services/JwtService';

function App() {
  const [language, setLanguage] = useState('ru');
  const [isAuthenticated, setIsAuthenticated] = useState(JwtService.isAuthenticated());

  useEffect(() => {
    const checkAuth = () => {
      setIsAuthenticated(JwtService.isAuthenticated());
    };
    checkAuth();
  }, []);

  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
  };

  const handleLogout = () => {
    JwtService.removeToken();
    setIsAuthenticated(false);
  };

  const ProtectedRoute = ({ children }) => {
    if (!isAuthenticated) {
      return <Navigate to="/login" replace />;
    }
    return children;
  };

  return (
    <Router>
      <div className="app-container">
        {isAuthenticated && (
          <Navbar bg="dark" variant="dark" expand="lg" className="border-bottom">
            <Container fluid>
              <Navbar.Brand href="/" className="fw-bold">
                <span className="text-warning">Vending</span> Franchise System
              </Navbar.Brand>
              
              <Navbar.Toggle aria-controls="navbar-nav" />
              
              <Navbar.Collapse id="navbar-nav">
                <Nav className="me-auto">
                  <Nav.Link href="/">Главная</Nav.Link>
                  <Nav.Link href="/ta">ТА</Nav.Link>
                  <Nav.Link href="/calendar">Календарь</Nav.Link>
                  <Nav.Link href="/schedule">График работ</Nav.Link>
                </Nav>
                
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
                  
                  <Button 
                    variant="outline-danger" 
                    size="sm"
                    onClick={handleLogout}
                  >
                    Выйти
                  </Button>
                </div>
              </Navbar.Collapse>
            </Container>
          </Navbar>
        )}

        <main className="main-content">
          <Container fluid className="py-4">
            <Routes>
              {/* Публичные маршруты */}
              <Route path="/login" element={
                <LoginForm onLoginSuccess={handleLoginSuccess} />
              } />
              
              {/* Защищенные маршруты */}
              <Route path="/" element={
                <ProtectedRoute>
                  <Dashboard />
                </ProtectedRoute>
              } />
              
              <Route path="/dashboard" element={
                <ProtectedRoute>
                  <Dashboard />
                </ProtectedRoute>
              } />
              
              <Route path="/ta" element={
                <ProtectedRoute>
                  <TAPage />
                </ProtectedRoute>
              } />
              
              <Route path="/calendar" element={
                <ProtectedRoute>
                  <CalendarPage />
                </ProtectedRoute>
              } />
              
              <Route path="/schedule" element={
                <ProtectedRoute>
                  <SchedulePage />
                </ProtectedRoute>
              } />
              
              {/* Маршрут ошибки */}
              <Route path="/error" element={<ErrorPage />} />
              
              {/* Перенаправление */}
              <Route path="*" element={
                isAuthenticated ? 
                  <Navigate to="/" replace /> : 
                  <Navigate to="/login" replace />
              } />
            </Routes>
          </Container>
        </main>
      </div>
    </Router>
  );
}

export default App;