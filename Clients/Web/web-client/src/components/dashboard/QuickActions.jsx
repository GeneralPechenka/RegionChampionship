// src/components/dashboard/QuickActions.jsx
import React from 'react';
import { Button, Row, Col } from 'react-bootstrap';
import { Upload, Calendar, People, Download } from 'react-bootstrap-icons';

const QuickActions = () => {
  const actions = [
    { icon: <Upload />, title: 'Загрузить ТА', desc: 'Excel/CSV импорт' },
    { icon: <Calendar />, title: 'Календарь ТО', desc: 'Планирование работ' },
    { icon: <People />, title: 'График работ', desc: 'Распределение задач' },
    { icon: <Download />, title: 'Отчеты', desc: 'Статистика и аналитика' },
  ];

  const handleClick = (title) => {
    alert(`Выбрано: ${title}\n(Функционал будет добавлен позже)`);
  };

  return (
    <Row xs={1} md={2} className="g-3">
      {actions.map((item, index) => (
        <Col key={index}>
          <Button 
            variant="outline-primary" 
            className="w-100 p-4 h-100"
            onClick={() => handleClick(item.title)}
          >
            <div className="d-flex flex-column align-items-center">
              <div className="mb-3" style={{ fontSize: '2rem' }}>
                {item.icon}
              </div>
              <h6 className="fw-bold">{item.title}</h6>
              <small className="text-muted">{item.desc}</small>
            </div>
          </Button>
        </Col>
      ))}
    </Row>
  );
};

export default QuickActions;