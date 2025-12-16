import React from 'react';
import { Navbar, Container, Nav, Form, NavDropdown,Button } from 'react-bootstrap';
import { Bell, PersonCircle, Gear } from 'react-bootstrap-icons';

const Header = () => {
  return (
    <Navbar bg="white" expand="lg" className="header shadow-sm">
      <Container fluid>
        <Navbar.Brand href="/" className="logo">
          <span className="text-primary">Vending</span>
          <span className="text-dark">Franchise</span>
        </Navbar.Brand>
        
        <Navbar.Toggle aria-controls="navbar" />
        
        <Navbar.Collapse id="navbar">
          <Nav className="ms-auto align-items-center">
            <Form className="d-flex me-3">
              <Form.Control
                type="search"
                placeholder="Поиск ТА, задач..."
                className="me-2"
                aria-label="Search"
              />
            </Form>
            
            <Nav.Link href="#" className="position-relative">
              <Bell size={20} />
              <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                3
              </span>
            </Nav.Link>
            
            <Nav.Link href="#">
              <Gear size={20} />
            </Nav.Link>
            
            <NavDropdown
              title={
                <>
                  <PersonCircle size={20} className="me-2" />
                  <span>Александр Петров</span>
                </>
              }
              align="end"
            >
              <NavDropdown.Item href="#profile">Профиль</NavDropdown.Item>
              <NavDropdown.Item href="#settings">Настройки</NavDropdown.Item>
              <NavDropdown.Divider />
              <NavDropdown.Item href="#logout">Выйти</NavDropdown.Item>
            </NavDropdown>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default Header;