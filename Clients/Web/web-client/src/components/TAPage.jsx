// components/TAPage.js
import React, { useState, useRef, useEffect } from 'react';
import { 
    Container, Row, Col, Card, Button, Form, 
    Alert, Table, Badge, Modal, ProgressBar,
    InputGroup, Tab, Tabs, Dropdown, DropdownButton,
    ListGroup, Spinner, Pagination
} from 'react-bootstrap';
import { 
  Upload, Download, FileExcel, FileText, 
  CheckCircle, XCircle, InfoCircle, Search,
  Plus, Filter, SortAlphaDown, SortAlphaUp,
  Trash, Pencil, Eye, Save, Printer,
  Database, Clock, 
  Building, Hash, Calendar, Person,
  Gear, ShieldCheck, Cpu, Compass, 
  ExclamationTriangle, List, Circle, PinMap, ArrowClockwise
} from 'react-bootstrap-icons';
import * as XLSX from 'xlsx';
import VendingService from '../services/VendingService';

const TAPage = () => {
  // Состояния
  const [file, setFile] = useState(null);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [uploadStatus, setUploadStatus] = useState(null);
  const [validationErrors, setValidationErrors] = useState([]);
  const [showPreviewModal, setShowPreviewModal] = useState(false);
  const [showTemplateModal, setShowTemplateModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [selectedTA, setSelectedTA] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortField, setSortField] = useState('name');
  const [sortDirection, setSortDirection] = useState('asc');
  const [activeTab, setActiveTab] = useState('list');
  const [loading, setLoading] = useState(false);
  const [loadingDetails, setLoadingDetails] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const fileInputRef = useRef(null);
  
  const [isDragOver, setIsDragOver] = useState(false);
  const dropAreaRef = useRef(null);

  // Состояния для данных
  const [taData, setTAData] = useState([]);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 20,
    totalCount: 0
  });

 // Эффект для добавления обработчиков drag and drop
 useEffect(() => {
    const dropArea = dropAreaRef.current;
    if (!dropArea) return;

    const handleDragOver = (e) => {
      e.preventDefault();
      e.stopPropagation();
      setIsDragOver(true);
    };

    const handleDragLeave = (e) => {
      e.preventDefault();
      e.stopPropagation();
      setIsDragOver(false);
    };

    const handleDrop = (e) => {
      e.preventDefault();
      e.stopPropagation();
      setIsDragOver(false);
      
      const files = e.dataTransfer.files;
      if (files && files.length > 0) {
        const file = files[0];
        handleFileUpload({ target: { files: [file] } });
      }
    };

    dropArea.addEventListener('dragover', handleDragOver);
    dropArea.addEventListener('dragleave', handleDragLeave);
    dropArea.addEventListener('drop', handleDrop);

    return () => {
      dropArea.removeEventListener('dragover', handleDragOver);
      dropArea.removeEventListener('dragleave', handleDragLeave);
      dropArea.removeEventListener('drop', handleDrop);
    };
  }, []);


  // Загрузка данных при монтировании
  useEffect(() => {
    loadVendingMachines();
  }, [pagination.page]);

  // Загрузка вендинговых аппаратов
  const loadVendingMachines = async (page = 1) => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await VendingService.getVendingMachines(page, pagination.pageSize);
      
      setTAData(response.items || []);
      setPagination({
        ...pagination,
        page: response.page || page,
        totalCount: response.totalCount || 0
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Загрузка детальной информации
  const loadVendingMachineDetails = async (id) => {
    setLoadingDetails(true);
    setError(null);
    
    try {
      const data = await VendingService.getVendingMachine(id);
      setSelectedTA(data);
      setShowDetailsModal(true);
    } catch (err) {
      setError(err.message);
      setShowDetailsModal(false);
    } finally {
      setLoadingDetails(false);
    }
  };

  // Удаление аппарата
  const handleDeleteTA = async (id) => {
    if (!window.confirm('Вы уверены, что хотите удалить этот аппарат?')) {
      return;
    }

    try {
      await VendingService.deleteVendingMachine(id);
      setSuccess('Аппарат успешно удален');
      loadVendingMachines(pagination.page);
      
      // Автоматическое скрытие уведомления через 3 секунды
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError(err.message);
    }
  };

  // Экспорт в CSV
  const handleExportToCsv = async () => {
    try {
      const blob = await VendingService.exportToCsv();
      VendingService.downloadFile(blob, `vending-machines-export-${new Date().toISOString().split('T')[0]}.csv`);
      setSuccess('Экспорт успешно завершен');
      
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError(err.message);
    }
  };

  // Импорт из CSV
  const handleImportFromCsv = async () => {
    if (!file || validationErrors.length > 0) {
      setError('Пожалуйста, исправьте ошибки перед загрузкой');
      return;
    }

    try {
      const result = await VendingService.importFromCsv(file);
      
      if (result.errors && result.errors.length > 0) {
        setValidationErrors(result.errors.map((error, index) => ({
          row: index + 2,
          field: 'import',
          error: error
        })));
        
        setUploadStatus({
          type: 'warning',
          title: 'Обнаружены ошибки при импорте',
          message: `Загружено ${result.successCount} аппаратов, ошибок: ${result.errorCount}`
        });
      } else {
        setUploadStatus({
          type: 'success',
          title: 'Успешно загружено',
          message: result.message || `Данные успешно загружены. Загружено ${result.successCount} аппаратов`
        });
        
        setSuccess('Данные успешно импортированы');
        setTimeout(() => setSuccess(null), 3000);
        
        // Обновляем список
        setTimeout(() => {
          loadVendingMachines(1);
          setFile(null);
          setUploadProgress(0);
          setUploadStatus(null);
          setValidationErrors([]);
          if (fileInputRef.current) fileInputRef.current.value = '';
        }, 2000);
      }
    } catch (err) {
      setError(err.message);
    }
  };

  // Поиск аппаратов
  const handleSearch = async () => {
    if (!searchTerm.trim()) {
      loadVendingMachines(1);
      return;
    }

    setLoading(true);
    try {
      const filters = {
        model: searchTerm,
        serialNumber: searchTerm,
        location: searchTerm
      };
      
      const response = await VendingService.searchVendingMachines(filters, 1, pagination.pageSize);
      
      setTAData(response.items || []);
      setPagination({
        ...pagination,
        page: 1,
        totalCount: response.totalCount || 0
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Получение статистики
  const [statistics, setStatistics] = useState(null);
  useEffect(() => {
    const loadStatistics = async () => {
      try {
        const stats = await VendingService.getStatistics();
        setStatistics(stats);
      } catch (err) {
        console.error('Ошибка при загрузке статистики:', err);
      }
    };
    
    loadStatistics();
  }, []);

  // Обработка файла (остаётся похожая логика для клиентской валидации)
  const handleFileUpload = (e) => {
    const uploadedFile = e.target.files[0];
    if (!uploadedFile) return;

    const validExtensions = ['.xlsx', '.xls', '.csv'];
    const fileExtension = uploadedFile.name.slice(uploadedFile.name.lastIndexOf('.')).toLowerCase();
    
    if (!validExtensions.includes(fileExtension)) {
      setUploadStatus({
        type: 'error',
        title: 'Ошибка формата',
        message: `Неподдерживаемый формат файла. Допустимые форматы: ${validExtensions.join(', ')}`
      });
      return;
    }

    setFile(uploadedFile);
    setUploadProgress(0);
    setValidationErrors([]);
    simulateFileProcessing(uploadedFile);
  };

  const simulateFileProcessing = (uploadedFile) => {
    const progressInterval = setInterval(() => {
      setUploadProgress(prev => {
        if (prev >= 100) {
          clearInterval(progressInterval);
          // Клиентская валидация остаётся
          validateFileContent(uploadedFile);
          return 100;
        }
        return prev + 10;
      });
    }, 100);
  };

  // Клиентская валидация файла (упрощённая версия)
  const validateFileContent = async (uploadedFile) => {
    try {
      // Тут можно оставить базовую клиентскую валидацию
      setUploadStatus({
        type: 'info',
        title: 'Файл проверен',
        message: 'Файл готов к загрузке. Серверная проверка произойдёт при импорте.'
      });
    } catch (error) {
      setUploadStatus({
        type: 'error',
        title: 'Ошибка чтения файла',
        message: `Не удалось прочитать файл: ${error.message}`
      });
    }
  };

  // Скачивание шаблона
  const handleDownloadTemplate = () => {
    const templateData = [{
      id: 'TA-NEW-001',
      serialNumber: 'NEW-SERIAL-001',
      inventoryNumber: 'INV-NEW-001',
      model: 'Новая модель',
      manufacturer: 'Производитель',
      type: 'coffee',
      paymentTypes: 'card,cash,qr',
      location: 'Местоположение',
      address: 'Адрес',
      country: 'Россия',
      productionDate: '2024-01-01',
      commissioningDate: '2024-01-15',
      lastVerification: '2024-01-10',
      verificationInterval: 6,
      nextVerification: '2024-07-10',
      resourceHours: 10000,
      currentHours: 0,
      nextMaintenance: '2024-04-15',
      maintenanceTime: 3,
      status: 'active',
      statusText: 'Работает',
      totalRevenue: 0,
      lastInventory: '2024-01-01',
      lastVerificationBy: 'ФИО сотрудника',
      franchisee: 'Франчайзи #1',
      createdAt: '2024-01-01',
      updatedAt: '2024-01-01'
    }];
    
    const worksheet = XLSX.utils.json_to_sheet(templateData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Шаблон ТА');
    XLSX.writeFile(workbook, 'шаблон_загрузки_ТА.xlsx');
    setShowTemplateModal(false);
  };

  // Фильтрация и сортировка клиентской стороны
  const filteredData = taData.filter(ta => {
    if (!searchTerm) return true;
    
    const searchLower = searchTerm.toLowerCase();
    return (
      ta.name?.toLowerCase().includes(searchLower) ||
      ta.model?.toLowerCase().includes(searchLower) ||
      ta.location?.toLowerCase().includes(searchLower) ||
      ta.serialNumber?.toLowerCase().includes(searchLower)
    );
  }).sort((a, b) => {
    const aValue = a[sortField];
    const bValue = b[sortField];
    
    if (aValue == null) return 1;
    if (bValue == null) return -1;
    
    if (sortDirection === 'asc') {
      return aValue > bValue ? 1 : -1;
    } else {
      return aValue < bValue ? 1 : -1;
    }
  });

  const handleSort = (field) => {
    if (sortField === field) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortField(field);
      setSortDirection('asc');
    }
  };

  const getSortIcon = (field) => {
    if (sortField !== field) return null;
    return sortDirection === 'asc' ? <SortAlphaUp /> : <SortAlphaDown />;
  };

  // Функция для отображения статуса
  const getStatusBadge = (status) => {
    switch (status) {
      case 'Работает': return <Badge bg="success">Активен</Badge>;
      case 'Обслуживается': return <Badge bg="info">На обслуживании</Badge>;
      case 'Выведен из строя': return <Badge bg="warning">Неактивен</Badge>;
      case 'Сломан': return <Badge bg="danger">Неисправен</Badge>;
      default: return <Badge bg="secondary">{status}</Badge>;
    }
  };

  // Пагинация
  const totalPages = Math.ceil(pagination.totalCount / pagination.pageSize);
  const renderPagination = () => {
    const items = [];
    
    for (let number = 1; number <= totalPages; number++) {
      items.push(
        <Pagination.Item
          key={number}
          active={number === pagination.page}
          onClick={() => loadVendingMachines(number)}
        >
          {number}
        </Pagination.Item>
      );
    }
    
    return (
      <Pagination>
        <Pagination.Prev 
          disabled={pagination.page <= 1}
          onClick={() => loadVendingMachines(pagination.page - 1)}
        />
        {items}
        <Pagination.Next 
          disabled={pagination.page >= totalPages}
          onClick={() => loadVendingMachines(pagination.page + 1)}
        />
      </Pagination>
    );
  };

  return (
    <Container fluid className="py-4">
      <Row className="mb-4">
        <Col>
          <h1 className="fw-bold display-6">Торговые аппараты (ТА)</h1>
          <p className="text-muted lead">
            Управление торговыми аппаратами. Загрузка данных из Excel/CSV файлов
          </p>
        </Col>
        <Col xs="auto">
          <Button variant="outline-primary" onClick={() => loadVendingMachines()}>
            <ArrowClockwise className="me-2" />Обновить
          </Button>
        </Col>
      </Row>

      {/* Уведомления об ошибках и успехах */}
      {error && (
        <Alert variant="danger" className="d-flex align-items-center" dismissible onClose={() => setError(null)}>
          <Circle className="me-3 flex-shrink-0" size={24} />
          <div className="flex-grow-1">
            <Alert.Heading>Ошибка</Alert.Heading>
            <p className="mb-0">{error}</p>
          </div>
        </Alert>
      )}

      {success && (
        <Alert variant="success" className="d-flex align-items-center" dismissible onClose={() => setSuccess(null)}>
          <CheckCircle className="me-3 flex-shrink-0" size={24} />
          <div className="flex-grow-1">
            <Alert.Heading>Успешно</Alert.Heading>
            <p className="mb-0">{success}</p>
          </div>
        </Alert>
      )}

      <Tabs
        activeKey={activeTab}
        onSelect={(k) => setActiveTab(k)}
        className="mb-4"
      >
        <Tab eventKey="upload" title={<><Upload className="me-2" />Загрузка ТА</>}>
          <Row>
            <Col lg={8}>
              <Card className="shadow-sm">
                <Card.Header className="bg-primary text-white">
                  <Card.Title className="mb-0">
                    <Database className="me-2" />
                    Загрузка данных ТА из файла
                  </Card.Title>
                </Card.Header>
                <Card.Body>
                  <Form>
                    <Form.Group className="mb-4">
                      <Form.Label><strong>Выберите файл для загрузки</strong></Form.Label>
                      <Form.Text className="d-block mb-3 text-muted">
                        Поддерживаемые форматы: .xlsx, .xls, .csv. 
                        <br />
                        <strong>Серверная валидация:</strong> Проверка уникальности серийных номеров, корректность дат, логические проверки
                      </Form.Text>
                      
                      <div 
  ref={dropAreaRef}
  className={`
    border rounded p-5 text-center 
    ${isDragOver ? 'border-primary border-2 bg-light' : 'border-dashed'} 
    ${file ? 'bg-light' : 'bg-light'}
    transition-all
  `}
  style={{
    cursor: 'pointer',
    borderStyle: isDragOver ? 'solid' : 'dashed',
    borderWidth: '2px',
    transition: 'all 0.3s ease',
    minHeight: '200px',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: isDragOver ? '#e7f3ff' : file ? '#f8f9fa' : '#f8f9fa'
  }}
  onClick={() => fileInputRef.current?.click()}
  onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f0f7ff'}
  onMouseLeave={(e) => e.currentTarget.style.backgroundColor = isDragOver ? '#e7f3ff' : '#f8f9fa'}
>
  {isDragOver ? (
    <>
      <Upload size={64} className="text-primary mb-4" />
      <h5 className="mb-3 text-primary">Отпустите файл для загрузки</h5>
      <p className="text-muted">Файл будет загружен автоматически</p>
    </>
  ) : file ? (
    <>
      <CheckCircle size={64} className="text-success mb-4" />
      <h5 className="mb-3">Файл выбран</h5>
      <p className="text-success mb-1">
        <strong>{file.name}</strong>
      </p>
      <p className="text-muted">
        {Math.round(file.size / 1024)} KB
      </p>
      <div className="mt-3">
        <Button 
          variant="outline-secondary" 
          size="sm"
          onClick={(e) => {
            e.stopPropagation();
            setFile(null);
            setUploadProgress(0);
            setUploadStatus(null);
            setValidationErrors([]);
            if (fileInputRef.current) fileInputRef.current.value = '';
          }}
        >
          <Trash className="me-1" />Удалить файл
        </Button>
      </div>
    </>
  ) : (
    <>
      <FileExcel size={64} className="text-muted mb-4" />
      <h5 className="mb-3">Перетащите файл или нажмите для выбора</h5>
      <p className="text-muted mb-4">
        <small>Поддерживаемые форматы: .xlsx, .xls, .csv</small>
        <br />
        <small>Максимальный размер: 10MB</small>
      </p>
      <Button variant="outline-primary" size="sm">
        Выбрать файл
      </Button>
    </>
  )}
  
  {/* Скрытый input для загрузки файла */}
  <Form.Control
    type="file"
    accept=".xlsx,.xls,.csv"
    onChange={handleFileUpload}
    className="d-none"
    ref={fileInputRef}
  />
</div>
                    </Form.Group>

                    {file && (
                      <Alert variant="info" className="d-flex align-items-center">
                        <InfoCircle className="me-3 flex-shrink-0" size={24} />
                        <div className="flex-grow-1">
                          <Alert.Heading>Выбран файл</Alert.Heading>
                          <p className="mb-0">
                            <strong>{file.name}</strong> ({Math.round(file.size / 1024)} KB)
                            <br />
                            <small>Последнее изменение: {new Date(file.lastModified).toLocaleString()}</small>
                          </p>
                          <div className="mt-2">
                            <Button 
                              variant="outline-primary" 
                              size="sm" 
                              className="me-2"
                              onClick={() => setShowPreviewModal(true)}
                            >
                              <FileText className="me-1" />Структура файла
                            </Button>
                            <Button 
                              variant="outline-danger" 
                              size="sm"
                              onClick={() => {
                                setFile(null);
                                setUploadProgress(0);
                                setUploadStatus(null);
                                setValidationErrors([]);
                                if (fileInputRef.current) fileInputRef.current.value = '';
                              }}
                            >
                              <Trash className="me-1" />Удалить
                            </Button>
                          </div>
                        </div>
                      </Alert>
                    )}

                    {uploadProgress > 0 && uploadProgress < 100 && (
                      <div className="mb-4">
                        <div className="d-flex justify-content-between mb-1">
                          <span>Проверка файла...</span>
                          <span>{uploadProgress}%</span>
                        </div>
                        <ProgressBar now={uploadProgress} animated />
                      </div>
                    )}

                    {uploadStatus && (
                      <Alert variant={uploadStatus.type} className="d-flex align-items-center">
                        {uploadStatus.type === 'success' && <CheckCircle className="me-3" size={24} />}
                        {uploadStatus.type === 'warning' && <ExclamationTriangle className="me-3" size={24} />}
                        {uploadStatus.type === 'error' && <XCircle className="me-3" size={24} />}
                        {uploadStatus.type === 'info' && <InfoCircle className="me-3" size={24} />}
                        <div className="flex-grow-1">
                          <Alert.Heading>{uploadStatus.title}</Alert.Heading>
                          <p className="mb-0">{uploadStatus.message}</p>
                        </div>
                      </Alert>
                    )}

                    {validationErrors.length > 0 && (
                      <Card className="border-warning mb-4">
                        <Card.Header className="bg-warning text-dark">
                          <Card.Title className="mb-0 d-flex justify-content-between align-items-center">
                            <span>Обнаружены ошибки при импорте</span>
                            <Badge bg="danger">{validationErrors.length}</Badge>
                          </Card.Title>
                        </Card.Header>
                        <Card.Body>
                          <div className="table-responsive">
                            <Table size="sm" bordered hover>
                              <thead>
                                <tr><th>Строка</th><th>Тип</th><th>Ошибка</th></tr>
                              </thead>
                              <tbody>
                                {validationErrors.map((error, index) => (
                                  <tr key={index}>
                                    <td className="fw-bold">{error.row}</td>
                                    <td>
                                      {error.error.includes('уже существует') ? (
                                        <Badge bg="danger">Дубликат</Badge>
                                      ) : error.error.includes('Отсутствует') ? (
                                        <Badge bg="warning">Обязательное поле</Badge>
                                      ) : (
                                        <Badge bg="info">Валидация</Badge>
                                      )}
                                    </td>
                                    <td className="text-danger">{error.error}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </Table>
                          </div>
                        </Card.Body>
                      </Card>
                    )}

                    <div className="d-flex justify-content-between mt-4">
                      <div>
                        <Button variant="outline-secondary" onClick={() => setShowTemplateModal(true)}>
                          <Download className="me-2" />Скачать шаблон
                        </Button>
                      </div>
                      <div>
                        <Button 
                          variant="primary" 
                          disabled={!file}
                          onClick={handleImportFromCsv}
                          size="lg"
                        >
                          {loading ? (
                            <>
                              <Spinner as="span" animation="border" size="sm" className="me-2" />
                              Загрузка...
                            </>
                          ) : (
                            <>
                              <Upload className="me-2" />Импортировать в систему
                            </>
                          )}
                        </Button>
                      </div>
                    </div>
                  </Form>
                </Card.Body>
              </Card>
            </Col>

            <Col lg={4}>
              <Card className="shadow-sm mb-4">
                <Card.Header>
                  <Card.Title className="mb-0"><InfoCircle className="me-2" />Требования к файлу</Card.Title>
                </Card.Header>
                <Card.Body>
                  <ListGroup variant="flush">
                    <ListGroup.Item><CheckCircle className="text-success me-2" />Формат: .xlsx, .xls, .csv</ListGroup.Item>
                    <ListGroup.Item><CheckCircle className="text-success me-2" />Уникальные серийные номера</ListGroup.Item>
                    <ListGroup.Item><CheckCircle className="text-success me-2" />Уникальные инвентарные номера</ListGroup.Item>
                    <ListGroup.Item><CheckCircle className="text-success me-2" />Корректность дат (YYYY-MM-DD)</ListGroup.Item>
                    <ListGroup.Item><CheckCircle className="text-success me-2" />Положительный ресурс ТА</ListGroup.Item>
                    <ListGroup.Item><CheckCircle className="text-success me-2" />Логические проверки дат</ListGroup.Item>
                  </ListGroup>
                </Card.Body>
              </Card>

              <Card className="shadow-sm">
                <Card.Header>
                  <Card.Title className="mb-0"><Database className="me-2" />Статистика ТА</Card.Title>
                </Card.Header>
                <Card.Body>
                  {statistics ? (
                    <>
                      <div className="d-flex justify-content-between mb-3">
                        <div className="text-center">
                          <h2 className="text-primary">{statistics.summary?.totalCount || 0}</h2>
                          <small className="text-muted">Всего ТА</small>
                        </div>
                        <div className="text-center">
                          <h2 className="text-success">{statistics.summary?.activeCount || 0}</h2>
                          <small className="text-muted">Активные</small>
                        </div>
                        <div className="text-center">
                          <h2 className="text-warning">{statistics.summary?.maintenanceCount || 0}</h2>
                          <small className="text-muted">На ТО</small>
                        </div>
                      </div>
                      <div className="mb-3">
                        <div className="d-flex justify-content-between">
                          <span>Требуют поверки:</span>
                          <span className="fw-bold">{statistics.upcoming?.verificationDue || 0}</span>
                        </div>
                        <div className="d-flex justify-content-between">
                          <span>Требуют обслуживания:</span>
                          <span className="fw-bold">{statistics.upcoming?.maintenanceDue || 0}</span>
                        </div>
                      </div>
                    </>
                  ) : (
                    <div className="text-center py-3">
                      <Spinner animation="border" variant="primary" />
                    </div>
                  )}
                  <Button variant="outline-primary" className="w-100" onClick={() => setActiveTab('list')}>
                    <Eye className="me-2" />Просмотреть все ТА
                  </Button>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Tab>

        <Tab eventKey="list" title={<><List className="me-2" />Список ТА</>}>
          <Card className="shadow-sm">
            <Card.Header className="d-flex justify-content-between align-items-center">
              <Card.Title className="mb-0">
                <Cpu className="me-2" />
                Все торговые аппараты ({pagination.totalCount})
                {loading && <Spinner animation="border" size="sm" className="ms-2" />}
              </Card.Title>
              <div className="d-flex gap-2">
                <InputGroup style={{ width: '300px' }}>
                  <InputGroup.Text><Search /></InputGroup.Text>
                  <Form.Control
                    placeholder="Поиск по модели, серийному номеру, местоположению..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                  />
                  <Button variant="outline-secondary" onClick={handleSearch}>
                    <Search />
                  </Button>
                </InputGroup>
                <Button variant="outline-success" onClick={handleExportToCsv}>
                  <Download className="me-2" />Экспорт
                </Button>
                <Button variant="primary" onClick={() => setActiveTab('upload')}>
                  <Upload className="me-2" />Импорт из файла
                </Button>
              </div>
            </Card.Header>
            
            <Card.Body className="p-0">
              {loading ? (
                <div className="text-center py-5">
                  <Spinner animation="border" variant="primary" />
                  <p className="mt-2">Загрузка данных...</p>
                </div>
              ) : (
                <>
                  <div className="table-responsive">
                    <Table hover className="mb-0">
                      <thead className="bg-light">
                        <tr>
                          <th style={{ width: '80px' }}>
                            <Button variant="link" className="p-0 text-decoration-none" onClick={() => handleSort('name')}>
                              Название {getSortIcon('name')}
                            </Button>
                          </th>
                          <th>
                            <Button variant="link" className="p-0 text-decoration-none" onClick={() => handleSort('model')}>
                              Модель {getSortIcon('model')}
                            </Button>
                          </th>
                          <th>
                            <Button variant="link" className="p-0 text-decoration-none" onClick={() => handleSort('serialNumber')}>
                              Серийный номер {getSortIcon('serialNumber')}
                            </Button>
                          </th>
                          <th>
                            <Button variant="link" className="p-0 text-decoration-none" onClick={() => handleSort('location')}>
                              Местоположение {getSortIcon('location')}
                            </Button>
                          </th>
                          <th>Статус</th>
                          <th>
                            <Button variant="link" className="p-0 text-decoration-none" onClick={() => handleSort('nextMaintenanceDate')}>
                              Следующее ТО {getSortIcon('nextMaintenanceDate')}
                            </Button>
                          </th>
                          <th style={{ width: '150px' }}>Действия</th>
                        </tr>
                      </thead>
                      <tbody>
                        {filteredData.map((ta) => (
                          <tr key={ta.id}>
                            <td className="fw-bold"><Hash size={12} className="me-1" />{ta.name || 'Без названия'}</td>
                            <td><Cpu size={16} className="me-2 text-muted" />{ta.model}</td>
                            <td><code>{ta.serialNumber}</code></td>
                            <td><Compass size={14} className="me-2 text-muted" /><small>{ta.location}</small></td>
                            <td>{getStatusBadge(ta.status)}</td>
                            <td>
                              <Calendar size={14} className="me-2 text-muted" />
                              {ta.nextMaintenanceDate ? new Date(ta.nextMaintenanceDate).toLocaleDateString() : 'Не указано'}
                              {ta.nextMaintenanceDate && new Date(ta.nextMaintenanceDate) < new Date() && (
                                <Printer size={14} className="ms-2 text-danger" />
                              )}
                            </td>
                            <td>
                              <div className="d-flex gap-1">
                                <Button 
                                  variant="outline-primary" 
                                  size="sm" 
                                  onClick={() => loadVendingMachineDetails(ta.id)}
                                  disabled={loadingDetails}
                                >
                                  {loadingDetails && selectedTA?.id === ta.id ? (
                                    <Spinner animation="border" size="sm" />
                                  ) : (
                                    <Eye size={14} />
                                  )}
                                </Button>
                                <Button variant="outline-secondary" size="sm">
                                  <Pencil size={14} />
                                </Button>
                                <Button 
                                  variant="outline-danger" 
                                  size="sm"
                                  onClick={() => handleDeleteTA(ta.id)}
                                >
                                  <Trash size={14} />
                                </Button>
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </Table>
                  </div>
                  
                  {filteredData.length === 0 && (
                    <div className="text-center py-5">
                      <FileExcel size={48} className="text-muted mb-3" />
                      <h5>Торговые аппараты не найдены</h5>
                      <p className="text-muted">
                        {searchTerm ? 'Попробуйте изменить критерии поиска' : 'Начните с загрузки данных из файла'}
                      </p>
                      <Button variant="primary" onClick={() => setActiveTab('upload')}>
                        <Upload className="me-2" />Загрузить ТА из файла
                      </Button>
                    </div>
                  )}
                </>
              )}
            </Card.Body>
            
            <Card.Footer className="d-flex justify-content-between align-items-center">
              <div>
                Показано <strong>{filteredData.length}</strong> из <strong>{pagination.totalCount}</strong> записей
                <span className="ms-3">
                  Страница <strong>{pagination.page}</strong> из <strong>{totalPages}</strong>
                </span>
              </div>
              <div>
                {renderPagination()}
              </div>
            </Card.Footer>
          </Card>
        </Tab>
      </Tabs>

      {/* Модальное окно структуры файла */}
      <Modal show={showPreviewModal} onHide={() => setShowPreviewModal(false)} size="lg">
        <Modal.Header closeButton>
          <Modal.Title><FileText className="me-2" />Структура файла ТА</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Alert variant="info" className="d-flex align-items-center">
            <InfoCircle className="me-3" size={24} />
            <div>Файл должен содержать все необходимые поля для корректного импорта</div>
          </Alert>
          
          <Table bordered>
            <thead><tr><th>Поле</th><th>Обязательное</th><th>Описание</th><th>Пример</th></tr></thead>
            <tbody>
              <tr><td><code>serialNumber</code></td><td><Badge bg="danger">Да</Badge></td><td>Серийный номер (уникальный)</td><td>CM3000-2023-001</td></tr>
              <tr><td><code>inventoryNumber</code></td><td><Badge bg="warning">Да</Badge></td><td>Инвентарный номер</td><td>INV-2023-001</td></tr>
              <tr><td><code>model</code></td><td><Badge bg="danger">Да</Badge></td><td>Модель аппарата</td><td>CoffeeMaster 3000</td></tr>
              <tr><td><code>manufacturer</code></td><td><Badge bg="warning">Да</Badge></td><td>Производитель</td><td>CoffeeTech Inc.</td></tr>
              <tr><td><code>location</code></td><td><Badge bg="danger">Да</Badge></td><td>Местоположение</td><td>ТЦ "МЕГА", 1 этаж</td></tr>
              <tr><td><code>productionDate</code></td><td><Badge bg="danger">Да</Badge></td><td>Дата изготовления</td><td>2024-01-01</td></tr>
              <tr><td><code>commissioningDate</code></td><td><Badge bg="danger">Да</Badge></td><td>Дата ввода в эксплуатацию</td><td>2024-01-15</td></tr>
              <tr><td><code>resourceHours</code></td><td><Badge bg="warning">Да</Badge></td><td>Ресурс ТА в часах</td><td>10000</td></tr>
            </tbody>
          </Table>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowPreviewModal(false)}>Закрыть</Button>
          <Button variant="primary" onClick={() => setShowTemplateModal(true)}>
            <Download className="me-2" />Скачать шаблон
          </Button>
        </Modal.Footer>
      </Modal>

      {/* Модальное окно скачивания шаблона */}
      <Modal show={showTemplateModal} onHide={() => setShowTemplateModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title><Download className="me-2" />Скачать шаблон файла</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <p>Выберите формат шаблона для загрузки данных ТА:</p>
          <div className="d-grid gap-2">
            <Button variant="outline-success" size="lg" className="text-start" onClick={handleDownloadTemplate}>
              <FileExcel className="me-3" size={24} />
              <div><strong>Excel шаблон (.xlsx)</strong><br /><small>Рекомендуемый формат</small></div>
            </Button>
            <Button 
              variant="outline-info" 
              size="lg" 
              className="text-start"
              onClick={() => {
                const csvContent = `serialNumber,inventoryNumber,model,manufacturer,paymentTypes,location,address,productionDate,commissioningDate,lastVerification,verificationInterval,resourceHours,currentHours,nextMaintenance,maintenanceTime,status,totalRevenue,lastInventory,lastVerificationBy,franchisee
NEW-SERIAL-001,INV-NEW-001,Новая модель,Производитель,"card,cash,qr",Местоположение,Адрес,2024-01-01,2024-01-15,2024-01-10,6,10000,0,2024-04-15,3,active,0,2024-01-01,ФИО сотрудника,Франчайзи #1`;
                const blob = new Blob(['\ufeff' + csvContent], { type: 'text/csv;charset=utf-8;' });
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = 'шаблон_ТА.csv';
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
                setShowTemplateModal(false);
              }}
            >
              <FileText className="me-3" size={24} />
              <div><strong>CSV шаблон (.csv)</strong><br /><small>Простой текстовый формат</small></div>
            </Button>
          </div>
        </Modal.Body>
      </Modal>

      {/* Модальное окно деталей */}
      <Modal show={showDetailsModal} onHide={() => setShowDetailsModal(false)} size="xl">
        <Modal.Header closeButton>
          <Modal.Title>
            <Cpu className="me-2" />
            Детальная информация
            {selectedTA && `: ${selectedTA.name || selectedTA.serialNumber}`}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {loadingDetails ? (
            <div className="text-center py-5">
              <Spinner animation="border" variant="primary" />
              <p className="mt-2">Загрузка данных...</p>
            </div>
          ) : selectedTA ? (
            <Row>
              <Col md={6}>
                <Card className="mb-3">
                  <Card.Header><Card.Title className="mb-0">Основная информация</Card.Title></Card.Header>
                  <Card.Body>
                    <table className="table table-sm">
                      <tbody>
                        <tr><th style={{ width: '40%' }}>ID:</th><td><code>{selectedTA.id}</code></td></tr>
                        <tr><th>Название:</th><td><strong>{selectedTA.name}</strong></td></tr>
                        <tr><th>Серийный номер:</th><td><code>{selectedTA.serialNumber}</code></td></tr>
                        <tr><th>Инвентарный номер:</th><td><code>{selectedTA.inventoryNumber}</code></td></tr>
                        <tr><th>Модель:</th><td>{selectedTA.model}</td></tr>
                        <tr><th>Производитель:</th><td>{selectedTA.manufacturer}</td></tr>
                        <tr><th>Статус:</th><td>{getStatusBadge(selectedTA.status)}</td></tr>
                      </tbody>
                    </table>
                  </Card.Body>
                </Card>
                
                <Card className="mb-3">
                  <Card.Header><Card.Title className="mb-0">Местоположение</Card.Title></Card.Header>
                  <Card.Body>
                    <p><PinMap className="me-2" />{selectedTA.location}</p>
                    <p className="text-muted">{selectedTA.address}</p>
                    {selectedTA.company && (
                      <p><Building className="me-2" />Компания: {selectedTA.company.name}</p>
                    )}
                  </Card.Body>
                </Card>
              </Col>
              
              <Col md={6}>
                <Card className="mb-3">
                  <Card.Header><Card.Title className="mb-0">Даты и технические данные</Card.Title></Card.Header>
                  <Card.Body>
                    <table className="table table-sm">
                      <tbody>
                        <tr><th style={{ width: '40%' }}>Дата изготовления:</th>
                          <td>{selectedTA.manufactureDate ? new Date(selectedTA.manufactureDate).toLocaleDateString() : 'Не указано'}</td>
                        </tr>
                        <tr><th>Дата ввода в эксплуатацию:</th>
                          <td>{selectedTA.commissioningDate ? new Date(selectedTA.commissioningDate).toLocaleDateString() : 'Не указано'}</td>
                        </tr>
                        <tr><th>Последняя поверка:</th>
                          <td>
                            {selectedTA.lastVerificationDate ? new Date(selectedTA.lastVerificationDate).toLocaleDateString() : 'Не указано'}
                            <br />
                            {selectedTA.lastVerificationEmployee && (
                              <small className="text-muted">Проверил: {selectedTA.lastVerificationEmployee.fullName}</small>
                            )}
                          </td>
                        </tr>
                        <tr><th>Межпроверочный интервал:</th>
                          <td>{selectedTA.verificationIntervalMonths || 0} месяцев</td>
                        </tr>
                        <tr><th>Следующая поверка:</th>
                          <td>
                            <strong>
                              {selectedTA.nextVerificationDate ? new Date(selectedTA.nextVerificationDate).toLocaleDateString() : 'Не указано'}
                            </strong>
                            {selectedTA.nextVerificationDate && new Date(selectedTA.nextVerificationDate) < new Date() && (
                              <Badge bg="danger" className="ms-2">Просрочено</Badge>
                            )}
                          </td>
                        </tr>
                        <tr><th>Следующее ТО:</th>
                          <td>
                            <strong>
                              {selectedTA.nextMaintenanceDate ? new Date(selectedTA.nextMaintenanceDate).toLocaleDateString() : 'Не указано'}
                            </strong>
                            {selectedTA.nextMaintenanceDate && new Date(selectedTA.nextMaintenanceDate) < new Date() && (
                              <Badge bg="warning" className="ms-2">Требует внимания</Badge>
                            )}
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </Card.Body>
                </Card>
                
                <Card>
                  <Card.Header><Card.Title className="mb-0">Технические характеристики</Card.Title></Card.Header>
                  <Card.Body>
                    <table className="table table-sm">
                      <tbody>
                        <tr><th style={{ width: '40%' }}>Ресурс ТА:</th>
                          <td>{selectedTA.resourceHours?.toLocaleString() || 0} часов</td>
                        </tr>
                        <tr><th>Текущая наработка:</th>
                          <td>
                            {selectedTA.currentHours?.toLocaleString() || 0} часов
                            {selectedTA.resourceHours && selectedTA.currentHours && (
                              <ProgressBar 
                                now={(selectedTA.currentHours / selectedTA.resourceHours) * 100}
                                variant={(selectedTA.currentHours / selectedTA.resourceHours) > 0.8 ? 'warning' : 'success'}
                                className="mt-1"
                                label={`${Math.round((selectedTA.currentHours / selectedTA.resourceHours) * 100)}%`}
                              />
                            )}
                          </td>
                        </tr>
                        <tr><th>Время обслуживания:</th>
                          <td>{selectedTA.maintenanceDurationHours || 0} часа</td>
                        </tr>
                        <tr><th>Общая выручка:</th>
                          <td className="text-success">
                            <strong>{selectedTA.totalRevenue?.toLocaleString() || 0} ₽</strong>
                          </td>
                        </tr>
                        <tr><th>Дата добавления в систему:</th>
                          <td>{selectedTA.createdAt ? new Date(selectedTA.createdAt).toLocaleString() : 'Не указано'}</td>
                        </tr>
                      </tbody>
                    </table>
                  </Card.Body>
                </Card>
              </Col>
            </Row>
          ) : (
            <Alert variant="warning">Не удалось загрузить данные аппарата</Alert>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowDetailsModal(false)}>Закрыть</Button>
          <Button variant="primary"><Pencil className="me-2" />Редактировать</Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default TAPage;