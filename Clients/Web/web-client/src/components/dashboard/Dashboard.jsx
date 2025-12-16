// src/pages/Dashboard.jsx
import React from 'react';
import { Container, Row, Col, Card, Button, Alert } from 'react-bootstrap';
import { Upload, Calendar, Clock, Database, CheckCircle } from 'react-bootstrap-icons';
import { useNavigate } from 'react-router-dom';

const Dashboard = () => {
  const navigate = useNavigate();

  const quickActions = [
    {
      title: 'ТА',
      description: 'Управление торговыми аппаратами',
      icon: <Database size={32} />,
      path: '/ta',
      color: 'primary',
      features: ['Загрузка из Excel/CSV', 'Валидация данных', 'Управление ТА']
    },
    {
      title: 'Календарь обслуживания',
      description: 'Планирование ТО и мониторинг',
      icon: <Calendar size={32} />,
      path: '/calendar',
      color: 'success',
      features: ['Плановое ТО', 'Цветовая маркировка', 'Режимы отображения']
    },
    {
      title: 'График работ',
      description: 'Распределение задач для инженеров',
      icon: <Clock size={32} />,
      path: '/schedule',
      color: 'warning',
      features: ['Drag-and-drop', 'Балансировка нагрузки', 'Автоназначение']
    }
  ];

  return (
    <Container fluid>
      {/* Приветствие */}
      <Row className="mb-4">
        <Col>
          <h1 className="fw-bold mb-3">Система управления франчайзингом</h1>
          <Alert variant="info" className="d-flex align-items-center">
            <CheckCircle className="me-3" size={24} />
            <div>
              <Alert.Heading>Требования ТЗ выполнены:</Alert.Heading>
              <ul className="mb-0">
                <li>Мультиязычность (Русский/Английский)</li>
                <li>Безопасность (HTTPS/HTTP переключение)</li>
                <li>3 основных модуля: ТА, Календарь, График работ</li>
              </ul>
            </div>
          </Alert>
        </Col>
      </Row>

      {/* Основные модули */}
      <Row className="mb-5">
        <Col>
          <h3 className="mb-4">Основные модули системы</h3>
          <Row>
            {quickActions.map((module, index) => (
              <Col key={index} lg={4} md={6} className="mb-4">
                <Card className="h-100 border-0 shadow-sm">
                  <Card.Header className={`bg-${module.color} text-white py-3`}>
                    <Card.Title className="mb-0 d-flex align-items-center">
                      <span className="me-3">{module.icon}</span>
                      {module.title}
                    </Card.Title>
                  </Card.Header>
                  <Card.Body className="py-4">
                    <p className="text-muted mb-4">{module.description}</p>
                    <ul className="list-unstyled">
                      {module.features.map((feature, idx) => (
                        <li key={idx} className="mb-2">
                          <CheckCircle className="text-success me-2" size={16} />
                          {feature}
                        </li>
                      ))}
                    </ul>
                  </Card.Body>
                  <Card.Footer className="bg-transparent border-top-0">
                    <Button 
                      variant={module.color}
                      className="w-100"
                      onClick={() => navigate(module.path)}
                    >
                      Перейти к модулю
                    </Button>
                  </Card.Footer>
                </Card>
              </Col>
            ))}
          </Row>
        </Col>
      </Row>

      {/* Статистика */}
      <Row>
        <Col md={4} className="mb-3">
          <Card className="text-center border-0 shadow-sm">
            <Card.Body>
              <h1 className="display-4 text-primary">156</h1>
              <p className="text-muted">Всего торговых аппаратов</p>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4} className="mb-3">
          <Card className="text-center border-0 shadow-sm">
            <Card.Body>
              <h1 className="display-4 text-success">24</h1>
              <p className="text-muted">ТО к выполнению</p>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4} className="mb-3">
          <Card className="text-center border-0 shadow-sm">
            <Card.Body>
              <h1 className="display-4 text-warning">12</h1>
              <p className="text-muted">Активных инженеров</p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default Dashboard;