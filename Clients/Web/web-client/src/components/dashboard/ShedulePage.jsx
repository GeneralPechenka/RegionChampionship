
import React, { useState, useEffect } from 'react';
import {
    Container, Row, Col, Card, Button, Form,
    Badge, Alert, Tabs, Tab, ProgressBar
} from 'react-bootstrap';
import {
    BsCalendarEvent, BsPeople, BsClock, BsGear,
    BsExclamationTriangle, BsCheckCircle, BsXCircle,
    BsArrowRight, BsArrowLeft, BsPlus,
    BsPersonCheck, BsCalendarWeek, BsCalendarDay,
    BsCalculator, BsList, BsGripVertical,
    BsExclamationTriangleFill
} from 'react-icons/bs';
import { DndProvider, useDrag, useDrop } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';

const SchedulePage = () => {
    const [viewMode, setViewMode] = useState('week'); // day, week
    const [employeeView, setEmployeeView] = useState('all'); // all, single
    const [selectedEmployee, setSelectedEmployee] = useState('all');
    const [tasks, setTasks] = useState([]);
    const [employees, setEmployees] = useState([]);
    const [showAssignmentModal, setShowAssignmentModal] = useState(false);
    const [selectedTask, setSelectedTask] = useState(null);
    const [emergencyTasks, setEmergencyTasks] = useState([]);
    const [workloadStats, setWorkloadStats] = useState({});

    // Инициализация данных по ТЗ
    useEffect(() => {
        // Данные сотрудников по ТЗ
        const mockEmployees = [
            {
                id: 1,
                name: 'Иванов И.И.',
                position: 'Старший инженер',
                skills: ['CoffeeMaster 3000', 'SnackPro 200', 'DrinkCool 500'],
                maxHoursPerDay: 10,
                maxHoursPerWeek: 40,
                currentDailyHours: 6,
                currentWeeklyHours: 28,
                status: 'available'
            },
            {
                id: 2,
                name: 'Петров П.П.',
                position: 'Инженер',
                skills: ['SnackPro 200', 'DrinkCool 500'],
                maxHoursPerDay: 10,
                maxHoursPerWeek: 40,
                currentDailyHours: 8,
                currentWeeklyHours: 32,
                status: 'busy'
            },
            {
                id: 3,
                name: 'Сидоров С.С.',
                position: 'Младший инженер',
                skills: ['CoffeeMaster 3000'],
                maxHoursPerDay: 10,
                maxHoursPerWeek: 40,
                currentDailyHours: 4,
                currentWeeklyHours: 20,
                status: 'available'
            }
        ];

        // Данные задач по ТЗ
        const mockTasks = [
            {
                id: 1,
                taId: 'TA-001',
                model: 'CoffeeMaster 3000',
                location: 'ТЦ "МЕГА"',
                type: 'planned',
                priority: 'normal',
                status: 'new',
                estimatedHours: 3,
                travelTime: 2, // 1 час туда + 1 час обратно по ТЗ
                totalHours: 5,
                scheduledDate: '2024-04-15',
                assignedTo: null,
                emergency: false
            },
            {
                id: 2,
                taId: 'TA-045',
                model: 'SnackPro 200',
                location: 'Аэропорт',
                type: 'emergency',
                priority: 'high',
                status: 'new',
                estimatedHours: 4,
                travelTime: 2,
                totalHours: 6,
                scheduledDate: '2024-04-15',
                assignedTo: null,
                emergency: true
            },
            {
                id: 3,
                taId: 'TA-128',
                model: 'DrinkCool 500',
                location: 'Университет',
                type: 'planned',
                priority: 'normal',
                status: 'assigned',
                estimatedHours: 2,
                travelTime: 2,
                totalHours: 4,
                scheduledDate: '2024-04-16',
                assignedTo: 1,
                emergency: false
            }
        ];

        // Аварийные задачи по ТЗ
        const emergencyTasks = mockTasks.filter(task => task.emergency);

        // Расчет нагрузки по ТЗ
        const stats = calculateWorkloadStats(mockEmployees, mockTasks);

        setEmployees(mockEmployees);
        setTasks(mockTasks);
        setEmergencyTasks(emergencyTasks);
        setWorkloadStats(stats);
    }, []);

    // Расчет статистики нагрузки по ТЗ
    const calculateWorkloadStats = (employees, tasks) => {
        const stats = {
            totalTasks: tasks.length,
            emergencyTasks: tasks.filter(t => t.emergency).length,
            assignedTasks: tasks.filter(t => t.assignedTo).length,
            unassignedTasks: tasks.filter(t => !t.assignedTo).length,
            overloadedEmployees: employees.filter(e => e.currentDailyHours > 10).length,
            availableEmployees: employees.filter(e => e.currentDailyHours < 8).length
        };
        return stats;
    };

    // Автоназначение задач по ТЗ алгоритму
    const autoAssignTasks = () => {
        const unassignedTasks = tasks.filter(task => !task.assignedTo && !task.emergency);
        const availableEmployees = employees.filter(emp => emp.status === 'available');

        let updatedTasks = [...tasks];
        let updatedEmployees = [...employees];

        unassignedTasks.forEach(task => {
            // Поиск сотрудника по навыкам (по ТЗ)
            const suitableEmployee = availableEmployees.find(emp =>
                emp.skills.includes(task.model) &&
                emp.currentDailyHours + task.totalHours <= emp.maxHoursPerDay &&
                emp.currentWeeklyHours + task.totalHours <= emp.maxHoursPerWeek
            );

            if (suitableEmployee) {
                // Назначаем задачу
                const taskIndex = updatedTasks.findIndex(t => t.id === task.id);
                const empIndex = updatedEmployees.findIndex(e => e.id === suitableEmployee.id);

                if (taskIndex !== -1 && empIndex !== -1) {
                    updatedTasks[taskIndex] = {
                        ...updatedTasks[taskIndex],
                        assignedTo: suitableEmployee.id,
                        status: 'assigned'
                    };

                    updatedEmployees[empIndex] = {
                        ...updatedEmployees[empIndex],
                        currentDailyHours: updatedEmployees[empIndex].currentDailyHours + task.totalHours,
                        currentWeeklyHours: updatedEmployees[empIndex].currentWeeklyHours + task.totalHours
                    };
                }
            } else {
                // Нет подходящего сотрудника - уведомление по ТЗ
                Alert.info(`Нет доступных сотрудников для ТА ${task.taId}`);
            }
        });

        setTasks(updatedTasks);
        setEmployees(updatedEmployees);
        setWorkloadStats(calculateWorkloadStats(updatedEmployees, updatedTasks));
    };

    // Обработка аварийной задачи по ТЗ
    const handleEmergencyTask = (taskId) => {
        const task = tasks.find(t => t.id === taskId);
        if (!task || !task.emergency) return;

        // По ТЗ: аварийную задачу назначаем в этот же день
        const today = new Date().toISOString().split('T')[0];
        const updatedTasks = tasks.map(t => {
            if (t.id === taskId) {
                return { ...t, scheduledDate: today, priority: 'highest' };
            }
            // По ТЗ: другие задачи переносим на следующие дни
            if (t.scheduledDate === today && t.id !== taskId && t.type === 'planned') {
                const nextDay = new Date(today);
                nextDay.setDate(nextDay.getDate() + 1);
                return { ...t, scheduledDate: nextDay.toISOString().split('T')[0] };
            }
            return t;
        });

        setTasks(updatedTasks);

        // Автоматически назначаем на первого доступного сотрудника
        const availableEmployee = employees.find(emp =>
            emp.skills.includes(task.model) &&
            emp.status === 'available'
        );

        if (availableEmployee) {
            assignTaskToEmployee(taskId, availableEmployee.id);
        } else {
            Alert.warning('Нет доступных сотрудников для аварийной задачи!');
        }
    };

    // Назначение задачи сотруднику
    const assignTaskToEmployee = (taskId, employeeId) => {
        const task = tasks.find(t => t.id === taskId);
        const employee = employees.find(e => e.id === employeeId);

        if (!task || !employee) return;

        // Проверка перегрузки по ТЗ (>10 часов/день)
        if (employee.currentDailyHours + task.totalHours > 10) {
            Alert.warning(`Сотрудник ${employee.name} будет перегружен!`);
            return;
        }

        // Проверка навыков по ТЗ
        if (!employee.skills.includes(task.model)) {
            Alert.warning(`Сотрудник ${employee.name} не может обслуживать ${task.model}`);
            return;
        }

        const updatedTasks = tasks.map(t =>
            t.id === taskId
                ? { ...t, assignedTo: employeeId, status: 'assigned' }
                : t
        );

        const updatedEmployees = employees.map(emp => {
            if (emp.id === employeeId) {
                return {
                    ...emp,
                    currentDailyHours: emp.currentDailyHours + task.totalHours,
                    currentWeeklyHours: emp.currentWeeklyHours + task.totalHours
                };
            }
            return emp;
        });

        setTasks(updatedTasks);
        setEmployees(updatedEmployees);
        setWorkloadStats(calculateWorkloadStats(updatedEmployees, updatedTasks));
        setShowAssignmentModal(false);
    };

    // Drag-and-drop компонент задачи
    const TaskItem = ({ task }) => {
        const [{ isDragging }, drag] = useDrag({
            type: 'task',
            item: { id: task.id },
            collect: (monitor) => ({
                isDragging: monitor.isDragging(),
            }),
        });

        return (
            <div
                ref={drag}
                className={`task-item p-3 mb-2 border rounded ${isDragging ? 'opacity-50' : ''}`}
                style={{
                    backgroundColor: task.emergency ? '#fff5f5' : '#f8f9fa',
                    borderLeft: `4px solid ${task.emergency ? '#dc3545' : task.priority === 'high' ? '#ffc107' : '#0d6efd'}`,
                    cursor: 'move'
                }}
            >
                <div className="d-flex justify-content-between align-items-start">
                    <div>
                        <h6 className="mb-1">
                            <BsGripVertical className="me-2 text-muted" />
                            {task.taId} - {task.model}
                        </h6>
                        <small className="text-muted d-block">{task.location}</small>
                        <div className="d-flex gap-2 mt-2">
                            <Badge bg={task.emergency ? 'danger' : task.priority === 'high' ? 'warning' : 'primary'}>
                                {task.emergency ? 'Авария' : task.priority === 'high' ? 'Высокий' : 'Нормальный'}
                            </Badge>
                            <Badge bg="secondary">{task.estimatedHours}ч работы</Badge>
                            <Badge bg="info">{task.travelTime}ч дорога</Badge>
                        </div>
                    </div>
                    <div className="text-end">
                        <small className="d-block text-muted">{task.scheduledDate}</small>
                        {task.assignedTo ? (
                            <Badge bg="success" className="mt-1">Назначено</Badge>
                        ) : (
                            <Badge bg="secondary" className="mt-1">Не назначено</Badge>
                        )}
                    </div>
                </div>
            </div>
        );
    };

    // Drop зона для сотрудника
    const EmployeeDropZone = ({ employee }) => {
        const [{ isOver }, drop] = useDrop({
            accept: 'task',
            drop: (item) => assignTaskToEmployee(item.id, employee.id),
            collect: (monitor) => ({
                isOver: monitor.isOver(),
            }),
        });

        return (
            <div
                ref={drop}
                className={`employee-card p-3 border rounded ${isOver ? 'border-primary border-3' : ''}`}
                style={{
                    backgroundColor: isOver ? '#e7f1ff' : 'white',
                    minHeight: '200px'
                }}
            >
                <h5 className="mb-2">{employee.name}</h5>
                <p className="text-muted small mb-2">{employee.position}</p>

                <div className="mb-3">
                    <small className="d-block mb-1"><strong>Навыки:</strong></small>
                    <div className="d-flex flex-wrap gap-1">
                        {employee.skills.map(skill => (
                            <Badge key={skill} bg="light" text="dark" className="border">
                                {skill}
                            </Badge>
                        ))}
                    </div>
                </div>

                <div className="mb-2">
                    <small>Загрузка сегодня:</small>
                    <ProgressBar
                        now={employee.currentDailyHours}
                        max={employee.maxHoursPerDay}
                        variant={employee.currentDailyHours > 8 ? 'warning' : 'success'}
                        className="mt-1"
                    />
                    <small className="text-muted">
                        {employee.currentDailyHours} / {employee.maxHoursPerDay} часов
                    </small>
                </div>

                <div className="mb-2">
                    <small>Загрузка за неделю:</small>
                    <ProgressBar
                        now={employee.currentWeeklyHours}
                        max={employee.maxHoursPerWeek}
                        variant={employee.currentWeeklyHours > 35 ? 'warning' : 'success'}
                        className="mt-1"
                    />
                    <small className="text-muted">
                        {employee.currentWeeklyHours} / {employee.maxHoursPerWeek} часов
                    </small>
                </div>
            </div>
        );
    };

    // Кнопка формирования графика по ТЗ
    const handleGenerateSchedule = () => {
        // Обновление статусов по ТЗ
        const updatedTasks = tasks.map(task => {
            if (task.assignedTo && task.status === 'assigned') {
                return { ...task, status: 'in_progress' };
            }
            return task;
        });

        setTasks(updatedTasks);

        Alert.success('График работ сформирован! Статусы обновлены.');

        // Здесь должна быть логика сохранения в БД
        // saveToDatabase(updatedTasks);
    };

    return (
        <DndProvider backend={HTML5Backend}>
            <Container fluid>
                <Row className="mb-4">
                    <Col>
                        <h1 className="fw-bold">График работ</h1>
                        <p className="text-muted">
                            Распределение задач для выездных инженеров с учетом загруженности и навыков
                        </p>
                    </Col>
                </Row>

                {/* По ТЗ: Статистика и кнопки действий */}
                <Row className="mb-4">
                    <Col>
                        <Card>
                            <Card.Body>
                                <div className="d-flex justify-content-between align-items-center flex-wrap gap-3">
                                    <div>
                                        <h5 className="mb-0">Статистика распределения</h5>
                                        <small className="text-muted">
                                            {workloadStats.totalTasks} задач, {workloadStats.emergencyTasks} аварийных
                                        </small>
                                    </div>

                                    <div className="d-flex gap-2">
                                        <Button
                                            variant="outline-primary"
                                            onClick={autoAssignTasks}
                                        >
                                            <BsCalculator className="me-2" />
                                            Автоназначение
                                        </Button>

                                        {/* В кнопке "Балансировка" */}
                                        <Button
                                            variant="warning"
                                            onClick={() => {
                                                Alert.info('Запущена балансировка нагрузки...');
                                            }}
                                        >
                                            <BsGear className="me-2" /> {/* Заменили BsBalance на BsGear */}
                                            Балансировка
                                        </Button>

                                        <Button
                                            variant="success"
                                            onClick={handleGenerateSchedule}
                                        >
                                            <BsCheckCircle className="me-2" />
                                            Сформировать график
                                        </Button>
                                    </div>
                                </div>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>

                {/* По ТЗ: Аварийные задачи */}
                {emergencyTasks.length > 0 && (
                    <Row className="mb-4">
                        <Col>
                            <Alert variant="danger" className="d-flex align-items-center">
                                <BsExclamationTriangleFill size={24} className="me-3 flex-shrink-0" />
                                <div className="flex-grow-1">
                                    <Alert.Heading>Аварийные задачи требуют срочного назначения!</Alert.Heading>
                                    <p className="mb-0">
                                        {emergencyTasks.length} задач со статусом "Авария".
                                        Они будут назначены сегодня, другие задачи перенесены.
                                    </p>
                                </div>
                                <Button
                                    variant="outline-danger"
                                    onClick={() => emergencyTasks.forEach(t => handleEmergencyTask(t.id))}
                                >
                                    Назначить срочно
                                </Button>
                            </Alert>
                        </Col>
                    </Row>
                )}

                {/* По ТЗ: Табы режимов просмотра */}
                <Row className="mb-4">
                    <Col>
                        <Tabs defaultActiveKey="week" className="mb-3">
                            <Tab eventKey="day" title={
                                <>
                                    <BsCalendarDay className="me-2" />
                                    Дневной вид (по часам)
                                </>
                            }>
                                <Card>
                                    <Card.Body>
                                        <h5>Дневной график - {new Date().toLocaleDateString()}</h5>
                                        <p className="text-muted">
                                            Детализация по часам для каждого сотрудника
                                        </p>
                                        {/* Здесь будет детальный почасовой график */}
                                    </Card.Body>
                                </Card>
                            </Tab>

                            <Tab eventKey="week" title={
                                <>
                                    <BsCalendarWeek className="me-2" />
                                    Недельный вид
                                </>
                            }>
                                <Card>
                                    <Card.Body>
                                        <h5>Недельный график загрузки</h5>
                                        <p className="text-muted">
                                            Обзор загруженности по рабочим дням
                                        </p>
                                        {/* Здесь будет недельный график */}
                                    </Card.Body>
                                </Card>
                            </Tab>
                        </Tabs>
                    </Col>
                </Row>

                {/* Основной контент с drag-and-drop */}
                <Row>
                    {/* Колонка с задачами */}
                    <Col lg={5}>
                        <Card className="h-100">
                            <Card.Header>
                                <Card.Title className="mb-0">
                                    <BsCalendarEvent className="me-2" />
                                    Задачи на обслуживание
                                </Card.Title>
                            </Card.Header>
                            <Card.Body style={{ maxHeight: '600px', overflowY: 'auto' }}>
                                <div className="mb-3">
                                    <h6>Не назначенные задачи ({tasks.filter(t => !t.assignedTo).length})</h6>
                                    {tasks
                                        .filter(task => !task.assignedTo)
                                        .map(task => (
                                            <TaskItem key={task.id} task={task} />
                                        ))}
                                </div>

                                <div>
                                    <h6>Назначенные задачи ({tasks.filter(t => t.assignedTo).length})</h6>
                                    {tasks
                                        .filter(task => task.assignedTo)
                                        .map(task => (
                                            <div key={task.id} className="task-item p-3 mb-2 border rounded bg-light">
                                                <div className="d-flex justify-content-between">
                                                    <div>
                                                        <h6 className="mb-1">{task.taId}</h6>
                                                        <small>{task.location}</small>
                                                    </div>
                                                    <div>
                                                        <Badge bg="success">Назначено</Badge>
                                                    </div>
                                                </div>
                                            </div>
                                        ))}
                                </div>
                            </Card.Body>
                        </Card>
                    </Col>

                    {/* Колонка с сотрудниками */}
                    <Col lg={7}>
                        <Card className="h-100">
                            <Card.Header>
                                <div className="d-flex justify-content-between align-items-center">
                                    <Card.Title className="mb-0">
                                        <BsPeople className="me-2" />
                                        Сотрудники
                                    </Card.Title>
                                    <Form.Select
                                        value={employeeView}
                                        onChange={(e) => setEmployeeView(e.target.value)}
                                        style={{ width: '200px' }}
                                    >
                                        <option value="all">Все сотрудники</option>
                                        <option value="single">По одному сотруднику</option>
                                    </Form.Select>
                                </div>
                            </Card.Header>
                            <Card.Body>
                                {employeeView === 'all' ? (
                                    <Row>
                                        {employees.map(employee => (
                                            <Col key={employee.id} md={6} className="mb-3">
                                                <EmployeeDropZone employee={employee} />
                                            </Col>
                                        ))}
                                    </Row>
                                ) : (
                                    <div>
                                        <Form.Select
                                            value={selectedEmployee}
                                            onChange={(e) => setSelectedEmployee(e.target.value)}
                                            className="mb-3"
                                        >
                                            <option value="all">Выберите сотрудника</option>
                                            {employees.map(emp => (
                                                <option key={emp.id} value={emp.id}>
                                                    {emp.name} - {emp.position}
                                                </option>
                                            ))}
                                        </Form.Select>

                                        {selectedEmployee !== 'all' && (
                                            <EmployeeDropZone
                                                employee={employees.find(e => e.id === parseInt(selectedEmployee))}
                                            />
                                        )}
                                    </div>
                                )}

                                {/* Предупреждения о перегрузке по ТЗ */}
                                {employees.filter(e => e.currentDailyHours > 8).length > 0 && (
                                    <Alert variant="warning" className="mt-3">
                                        <BsExclamationTriangle className="me-2" />
                                        {employees.filter(e => e.currentDailyHours > 8).length} сотрудников
                                        загружены более чем на 8 часов/день
                                    </Alert>
                                )}
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>

                {/* Информация по ТЗ алгоритмам */}
                <Row className="mt-4">
                    <Col>
                        <Card>
                            <Card.Header>
                                <Card.Title className="mb-0">
                                    <BsGear className="me-2" />
                                    Алгоритмы распределения по ТЗ
                                </Card.Title>
                            </Card.Header>
                            <Card.Body>
                                <Row>
                                    <Col md={4}>
                                        <h6>1. Расчет времени</h6>
                                        <p className="small">
                                            <strong>Формула:</strong> Общее время = Время обслуживания + 2 часа (дорога)<br />
                                            <small>1 час до точки + 1 час обратно</small>
                                        </p>
                                    </Col>
                                    <Col md={4}>
                                        <h6>2. Учет навыков</h6>
                                        <p className="small">
                                            Сотрудник назначается только на ТА моделей, которые может обслуживать
                                        </p>
                                    </Col>
                                    <Col md={4}>
                                        <h6>3. Балансировка нагрузки</h6>
                                        <p className="small">
                                            Максимум: 10 часов/день, 40 часов/неделю на сотрудника
                                        </p>
                                    </Col>
                                </Row>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </DndProvider>
    );
};

export default SchedulePage;