// src/components/dashboard/StatsCards.jsx
import React from 'react';
import { Card, Col } from 'react-bootstrap';
import { Cpu, CashStack, CalendarCheck, ExclamationTriangle } from 'react-bootstrap-icons';

const StatsCards = ({ stats = {} }) => {
  const cards = [
    { 
      title: 'Всего ТА', 
      value: stats.totalTA || 0, 
      icon: <Cpu size={24} />, 
      color: 'primary' 
    },
    { 
      title: 'Активные ТА', 
      value: stats.activeTA || 0, 
      icon: <CashStack size={24} />, 
      color: 'success' 
    },
    { 
      title: 'ТО к выполнению', 
      value: stats.maintenanceDue || 0, 
      icon: <CalendarCheck size={24} />, 
      color: 'warning' 
    },
    { 
      title: 'Аварийные', 
      value: stats.emergencyTasks || 0, 
      icon: <ExclamationTriangle size={24} />, 
      color: 'danger' 
    },
  ];

  return (
    <Container fluid>
      {cards.map((card, index) => (
        <Col key={index} md={3} sm={6} className="mb-3">
          <Card className="shadow-sm">
            <Card.Body>
              <div className="d-flex justify-content-between">
                <div>
                  <h6 className="text-muted small mb-1">{card.title}</h6>
                  <h2 className="fw-bold mb-0">{card.value}</h2>
                </div>
                <div style={{ color: card.color }}>
                  {card.icon}
                </div>
              </div>
            </Card.Body>
          </Card>
        </Col>
      ))}
    </Container>
  );
};

export default StatsCards;