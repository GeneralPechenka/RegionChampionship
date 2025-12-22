// src/components/auth/LoginForm.jsx
import React, { useState } from 'react';
import { Form, Button, Card, Alert, Container, Row, Col } from 'react-bootstrap';
import { Person, Lock } from 'react-bootstrap-icons';
import { useNavigate } from 'react-router-dom';
import JwtService from '../../services/JwtService';
import AuthService from '../../services/AuthService';

const LoginForm = ({ onLoginSuccess }) => {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      // Вызываем реальный API
      const result = await AuthService.login(email, password);
      
      // Сохраняем токен и данные через JWT сервис
      JwtService.setToken(result.token, result.userInfo);
      
      // Переход на дашборд
      navigate('/dashboard');
      
      // Уведомляем родительский компонент
      if (onLoginSuccess) {
        onLoginSuccess();
      }

    } catch (err) {
      // Обработка ошибок axios
      const errorMessage = err.response?.data || err.message || 'Ошибка входа';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  // Быстрый вход для демо (тестовые данные)
  const quickLogin = (type) => {
    const testUsers = {
      admin: { email: 'admin@test.com', password: 'admin123' },
      engineer: { email: 'engineer@test.com', password: 'engineer123' },
      franchisee: { email: 'franchisee@test.com', password: 'franchisee123' }
    };
    
    setEmail(testUsers[type].email);
    setPassword(testUsers[type].password);
  };

  return (
    <Container fluid className="min-vh-100 d-flex align-items-center justify-content-center bg-light">
      <Row className="w-100 justify-content-center">
        <Col xs={12} sm={8} md={6} lg={4}>
          <Card className="shadow border-0">
            <Card.Body className="p-4">
              <div className="text-center mb-4">
                <h4 className="fw-bold">Вход в систему</h4>
                <p className="text-muted small">Введите email и пароль</p>
              </div>

              {error && (
                <Alert variant="danger" className="py-2 text-center">
                  {error}
                </Alert>
              )}

              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3">
                  <Form.Label>
                    <Person className="me-2" />
                    Email
                  </Form.Label>
                  <Form.Control
                    type="email"
                    placeholder="Введите email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                    className="py-2"
                  />
                </Form.Group>

                <Form.Group className="mb-4">
                  <Form.Label>
                    <Lock className="me-2" />
                    Пароль
                  </Form.Label>
                  <Form.Control
                    type="password"
                    placeholder="Введите пароль"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                    className="py-2"
                  />
                </Form.Group>

                <Button
                  variant="primary"
                  type="submit"
                  className="w-100 py-2"
                  disabled={loading}
                >
                  {loading ? 'Вход...' : 'Войти'}
                </Button>
              </Form>

              <div className="text-center mt-4">
                <p className="text-muted small mb-2">Тестовые данные:</p>
                <div className="d-flex gap-2 justify-content-center">
                  <Button
                    variant="outline-secondary"
                    size="sm"
                    onClick={() => quickLogin('admin')}
                  >
                    Админ
                  </Button>
                  <Button
                    variant="outline-secondary"
                    size="sm"
                    onClick={() => quickLogin('engineer')}
                  >
                    Инженер
                  </Button>
                  <Button
                    variant="outline-secondary"
                    size="sm"
                    onClick={() => quickLogin('franchisee')}
                  >
                    Франчайзи
                  </Button>
                </div>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default LoginForm;