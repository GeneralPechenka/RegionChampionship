// src/components/ErrorPage.jsx
import React from 'react';
import { Container, Button, Card } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

const ErrorPage = ({ code = 500, title = 'Ошибка', message = 'Что-то пошло не так' }) => {
  const navigate = useNavigate();

  return (
    <Container className="d-flex justify-content-center align-items-center vh-100">
      <Card className="text-center p-4" style={{ maxWidth: '500px' }}>
        <Card.Body>
          <h1 className="text-danger display-1 fw-bold">{code}</h1>
          <h2 className="mt-3">{title}</h2>
          <p className="text-muted mt-3">{message}</p>
          <Button 
            variant="primary" 
            className="mt-4"
            onClick={() => navigate('/')}
          >
            На главную
          </Button>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default ErrorPage;