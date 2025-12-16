import React from 'react';
import { 
  Container, 
  Row, 
  Col, 
  Alert, 
  Badge, 
  Card, 
  Button 
} from 'react-bootstrap';

const CalendarPage = () => {
  // Функция для расчета следующего ТО (примерная реализация)
  const calculateNextMaintenance = (lastDate, intervalMonths) => {
    const date = new Date(lastDate);
    date.setMonth(date.getMonth() + intervalMonths);
    return date.toLocaleDateString('ru-RU');
  };

  return (
    <Container>
      {/* Пример расчета по формуле ТЗ */}
      <Row className="mt-4">
        <Col>
          <Alert variant="light" className="border">
            <Alert.Heading>Пример расчета по формуле ТЗ:</Alert.Heading>
            <p>
              <strong>Дата последней проверки:</strong> 01.05.2025<br />
              <strong>Межпроверочный интервал:</strong> 6 месяцев<br />
              <strong>Дата следующего ТО:</strong> {calculateNextMaintenance('2025-05-01', 6)}
            </p>
            <hr />
            <p className="mb-0">
              <strong>Пример с ресурсом аппарата:</strong><br />
              ТА с ресурсом 10,000 часов. Текущая наработка: 8,500 часов (85% лимита).<br />
              <Badge bg="warning" className="mt-1">Требуется плановое ТО (достигнуто 85% ресурса)</Badge>
            </p>
          </Alert>
        </Col>
      </Row>

      {/* Дополнительная функциональность по ТЗ */}
      <Row className="mt-4">
        <Col md={6}>
          <Card>
            <Card.Header>
              <Card.Title className="mb-0">Интерактивная карта событий</Card.Title>
            </Card.Header>
            <Card.Body>
              <p className="text-muted small mb-3">
                При наведении на цветовую маркировку в календаре отображается информация о ТА:
              </p>
              <div className="d-flex flex-wrap gap-3">
                <div className="border rounded p-3 text-center" style={{ cursor: 'pointer' }}>
                  <Badge bg="success" className="mb-2">15</Badge>
                  <div className="small">
                    <strong>TA-001</strong><br />
                    CoffeeMaster 3000<br />
                    <small>ТЦ "МЕГА"</small><br />
                    <small className="text-muted">Франчайзи #1</small>
                  </div>
                </div>
                <div className="border rounded p-3 text-center" style={{ cursor: 'pointer' }}>
                  <Badge bg="warning" className="mb-2">10</Badge>
                  <div className="small">
                    <strong>TA-045</strong><br />
                    SnackPro 200<br />
                    <small>Аэропорт</small><br />
                    <small className="text-muted">Франчайзи #2</small>
                  </div>
                </div>
                <div className="border rounded p-3 text-center" style={{ cursor: 'pointer' }}>
                  <Badge bg="danger" className="mb-2">5</Badge>
                  <div className="small">
                    <strong>TA-128</strong><br />
                    DrinkCool 500<br />
                    <small>Университет</small><br />
                    <small className="text-muted">Франчайзи #1</small>
                  </div>
                </div>
              </div>
            </Card.Body>
          </Card>
        </Col>

        <Col md={6}>
          <Card>
            <Card.Header>
              <Card.Title className="mb-0">Статистика обслуживания</Card.Title>
            </Card.Header>
            <Card.Body>
              <div className="d-flex justify-content-between mb-3">
                <div>
                  <h6 className="text-muted mb-1">Всего ТО в этом месяце</h6>
                  <h3 className="fw-bold">18</h3>
                </div>
                <div>
                  <h6 className="text-muted mb-1">Просрочено</h6>
                  <h3 className="fw-bold text-danger">3</h3>
                </div>
                <div>
                  <h6 className="text-muted mb-1">Ближайшие 7 дней</h6>
                  <h3 className="fw-bold text-warning">7</h3>
                </div>
              </div>
              
              <div className="progress mb-2" style={{ height: '10px' }}>
                <div 
                  className="progress-bar bg-success" 
                  style={{ width: '60%' }}
                  title="Выполнено вовремя"
                ></div>
                <div 
                  className="progress-bar bg-warning" 
                  style={{ width: '25%' }}
                  title="Запланировано"
                ></div>
                <div 
                  className="progress-bar bg-danger" 
                  style={{ width: '15%' }}
                  title="Просрочено"
                ></div>
              </div>
              
              <div className="d-flex justify-content-between small text-muted">
                <span>Вовремя: 60%</span>
                <span>Запланировано: 25%</span>
                <span>Просрочено: 15%</span>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      {/* Кнопки действий по ТЗ */}
      <Row className="mt-4">
        <Col>
          <Card>
            <Card.Body className="d-flex justify-content-between align-items-center">
              <div>
                <h5 className="mb-1">Управление графиком обслуживания</h5>
                <p className="text-muted mb-0">
                  Автоматическое формирование задач на основе календаря ТО
                </p>
              </div>
              <div className="d-flex gap-2">
                <Button variant="outline-primary">
                  Экспорт графика
                </Button>
                <Button variant="primary">
                  Сформировать задачи
                </Button>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default CalendarPage;