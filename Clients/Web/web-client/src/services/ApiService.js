// services/ApiService.js

import axios from 'axios';
import JwtService  from './JwtService';

// Вспомогательная функция для очистки токена от недопустимых символов
const sanitizeToken = (token) => {
  if (!token) return '';
  
  // Удаляем пробелы в начале и конце
  token = token.trim();
  
  // Удаляем не-ASCII символы и специальные символы, кроме стандартных для JWT
  // JWT состоит из: a-z, A-Z, 0-9, -, _, ., =
  const jwtRegex = /^[A-Za-z0-9\-_=]+\.[A-Za-z0-9\-_=]+\.[A-Za-z0-9\-_=]+$/;
  
  // Если токен не соответствует формату JWT, пытаемся очистить его
  if (!jwtRegex.test(token)) {
    // Удаляем все символы кроме разрешённых для JWT
    token = token.replace(/[^A-Za-z0-9\-_=.]/g, '');
  }
  
  return token;
};

// Создаём экземпляр axios с базовыми настройками
const ApiService = axios.create({
  baseURL: 'https://localhost:7000',
  timeout: 30000, // 30 секунд
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});

// Добавляем интерсептор для автоматической вставки токена в каждый запрос
ApiService.interceptors.request.use(
  (config) => {
    const token = JwtService.getToken();
    if (token) {
      const sanitizedToken = sanitizeToken(token);
      if (sanitizedToken) {
        config.headers['X-Auth-Token'] = sanitizedToken;
      }
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Интерсептор для обработки ответов
ApiService.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    // Обработка ошибок авторизации
    if (error.response && error.response.status === 401) {
      // Токен истёк или недействителен
      console.warn('Сессия истекла. Требуется повторная авторизация.');
      // Здесь можно вызвать метод для выхода из системы или обновления токена
      // Например: window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Экспортируем конфигурированный экземпляр axios
export default ApiService;

// Экспортируем вспомогательные функции для управления конфигурацией
export const configureApiService = {
  /**
   * Установить базовый URL для API
   * @param {string} baseUrl - базовый URL
   */
  setBaseUrl(baseUrl) {
    ApiService.defaults.baseURL = baseUrl;
  },

  /**
   * Установить таймаут для запросов
   * @param {number} timeout - таймаут в миллисекундах
   */
  setTimeout(timeout) {
    ApiService.defaults.timeout = timeout;
  },

  /**
   * Установить кастомные заголовки по умолчанию
   * @param {Object} headers - объект с заголовками
   */
  setDefaultHeaders(headers) {
    ApiService.defaults.headers.common = {
      ...ApiService.defaults.headers.common,
      ...headers
    };
  },

  /**
   * Очистить токен авторизации
   */
  clearAuthToken() {
    delete ApiService.defaults.headers.common['X-Auth-Token'];
  },

  /**
   * Установить токен авторизации вручную
   * @param {string} token - JWT токен
   */
  setAuthToken(token) {
    const sanitizedToken = sanitizeToken(token);
    if (sanitizedToken) {
      ApiService.defaults.headers.common['X-Auth-Token'] = sanitizedToken;
    }
  },

  /**
   * Получить текущую конфигурацию API сервиса
   * @returns {Object} - текущая конфигурация
   */
  getConfig() {
    return {
      baseURL: ApiService.defaults.baseURL,
      timeout: ApiService.defaults.timeout,
      headers: { ...ApiService.defaults.headers.common }
    };
  }
};