// services/VendingService.js


import ApiService from './ApiService';
import JwtService from './JwtService';


export class VendingService {
  
    static baseUrl = '/Core/Vendings';
    
    /**
     * Получить список вендинговых аппаратов с пагинацией
     * @param {number} page - номер страницы (начинается с 1)
     * @param {number} pageSize - размер страницы (по умолчанию 20)
     * @returns {Promise<Object>} - пагинированный список аппаратов
     */
    static async getVendingMachines(page = 1, pageSize = 20) {
      try {
        const url = `https://localhost:7000${this.baseUrl}?page=${page}&pageSize=${pageSize}`;
        console.log('url:',url);
        console.log('jwt:',JwtService.getToken());
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при получении списка вендинговых аппаратов:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Получить детальную информацию о вендинговом аппарате
     * @param {string} id - UUID аппарата
     * @returns {Promise<Object>} - детальная информация об аппарате
     */
    static async getVendingMachine(id) {
      try {
        if (!id) {
          throw new Error('ID аппарата не указан');
        }
        
        const url = `${this.baseUrl}/${id}`;
        
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error(`Ошибка при получении аппарата с ID ${id}:`, error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Создать новый вендинговый аппарат
     * @param {Object} vendingData - данные для создания аппарата
     * @returns {Promise<Object>} - созданный аппарат с ID
     */
    static async createVendingMachine(vendingData) {
      try {
        if (!vendingData) {
          throw new Error('Данные для создания не предоставлены');
        }
        
        const response = await ApiService.post(this.baseUrl, vendingData);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при создании вендингового аппарата:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Обновить существующий вендинговый аппарат
     * @param {string} id - UUID аппарата
     * @param {Object} vendingData - обновленные данные аппарата
     * @returns {Promise<void>}
     */
    static async updateVendingMachine(id, vendingData) {
      try {
        if (!id) {
          throw new Error('ID аппарата не указан');
        }
        
        const url = `${this.baseUrl}/${id}`;
        
        await ApiService.put(url, vendingData);
        
        return { success: true, message: 'Аппарат успешно обновлен' };
      } catch (error) {
        console.error(`Ошибка при обновлении аппарата с ID ${id}:`, error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Удалить вендинговый аппарат
     * @param {string} id - UUID аппарата
     * @returns {Promise<Object>}
     */
    static async deleteVendingMachine(id) {
      try {
        if (!id) {
          throw new Error('ID аппарата не указан');
        }
        
        const url = `${this.baseUrl}/${id}`;
        
        await ApiService.delete(url);
        
        return { success: true, message: 'Аппарат успешно удален' };
      } catch (error) {
        console.error(`Ошибка при удалении аппарата с ID ${id}:`, error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Импортировать аппараты из CSV файла
     * @param {File} file - CSV файл с данными аппаратов
     * @returns {Promise<Object>} - результат импорта
     */
    static async importFromCsv(file) {
      try {
        if (!file) {
          throw new Error('Файл не выбран');
        }
        
        const formData = new FormData();
        formData.append('file', file);
        
        const url = `${this.baseUrl}/import`;
        
        // Для multipart/form-data убираем стандартный Content-Type
        const response = await ApiService.post(url, formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при импорте CSV:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Экспортировать аппараты в CSV файл
     * @param {Array<string>} ids - массив ID для экспорта (опционально)
     * @returns {Promise<Blob>} - CSV файл
     */
    static async exportToCsv(ids = []) {
      try {
        let url = `${this.baseUrl}/export`;
        
        if (ids && ids.length > 0) {
          const idsParam = ids.join(',');
          url += `?ids=${encodeURIComponent(idsParam)}`;
        }
        
        const response = await ApiService.get(url, {
          responseType: 'blob'
        });
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при экспорте в CSV:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Поиск аппаратов по фильтрам
     * @param {Object} filters - объект фильтров
     * @param {number} page - номер страницы
     * @param {number} pageSize - размер страницы
     * @returns {Promise<Object>} - отфильтрованный список
     */
    static async searchVendingMachines(filters = {}, page = 1, pageSize = 20) {
      try {
        // Создаем query параметры
        const params = new URLSearchParams({
          page: page.toString(),
          pageSize: pageSize.toString()
        });
        
        // Добавляем фильтры
        Object.keys(filters).forEach(key => {
          if (filters[key] !== undefined && filters[key] !== null && filters[key] !== '') {
            params.append(key, filters[key]);
          }
        });
        
        const url = `${this.baseUrl}/search?${params.toString()}`;
        
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при поиске аппаратов:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Получить статистику по аппаратам
     * @returns {Promise<Object>} - статистические данные
     */
    static async getStatistics() {
      try {
        const url = `${this.baseUrl}/statistics`;
        
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при получении статистики:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Проверить доступность серийного номера
     * @param {string} serialNumber - серийный номер для проверки
     * @param {string} excludeId - ID аппарата, который нужно исключить из проверки
     * @returns {Promise<Object>} - результат проверки
     */
    static async checkSerialNumber(serialNumber, excludeId = null) {
      try {
        if (!serialNumber) {
          throw new Error('Серийный номер не указан');
        }
        
        let url = `${this.baseUrl}/check-serial/${encodeURIComponent(serialNumber)}`;
        
        if (excludeId) {
          url += `?excludeId=${excludeId}`;
        }
        
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при проверке серийного номера:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Получить аппарат по серийному номеру
     * @param {string} serialNumber - серийный номер
     * @returns {Promise<Object>} - данные аппарата
     */
    static async getBySerialNumber(serialNumber) {
      try {
        if (!serialNumber) {
          throw new Error('Серийный номер не указан');
        }
        
        const url = `${this.baseUrl}/by-serial/${encodeURIComponent(serialNumber)}`;
        
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при получении аппарата по серийному номеру:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Получить аппараты, требующие поверки
     * @param {number} daysThreshold - пороговое количество дней (по умолчанию 30)
     * @param {number} page - номер страницы
     * @param {number} pageSize - размер страницы
     * @returns {Promise<Object>} - список аппаратов
     */
    static async getMachinesDueForVerification(daysThreshold = 30, page = 1, pageSize = 20) {
      try {
        const url = `${this.baseUrl}/due-for-verification?daysThreshold=${daysThreshold}&page=${page}&pageSize=${pageSize}`;
        
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при получении аппаратов, требующих поверки:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Получить аппараты, требующие обслуживания
     * @param {number} daysThreshold - пороговое количество дней (по умолчанию 30)
     * @param {number} page - номер страницы
     * @param {number} pageSize - размер страницы
     * @returns {Promise<Object>} - список аппаратов
     */
    static async getMachinesDueForMaintenance(daysThreshold = 30, page = 1, pageSize = 20) {
      try {
        const url = `${this.baseUrl}/due-for-maintenance?daysThreshold=${daysThreshold}&page=${page}&pageSize=${pageSize}`;
        
        const response = await ApiService.get(url);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при получении аппаратов, требующих обслуживания:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Массовое обновление статусов аппаратов
     * @param {Array<string>} ids - массив ID аппаратов
     * @param {string} status - новый статус
     * @returns {Promise<Object>} - результат обновления
     */
    static async updateBulkStatus(ids, status) {
      try {
        if (!ids || !ids.length) {
          throw new Error('Не указаны ID аппаратов');
        }
        
        if (!status) {
          throw new Error('Не указан новый статус');
        }
        
        const url = `${this.baseUrl}/bulk-status`;
        const payload = { ids, status };
        
        const response = await ApiService.patch(url, payload);
        
        return response.data;
      } catch (error) {
        console.error('Ошибка при массовом обновлении статусов:', error);
        throw this._handleError(error);
      }
    }
    
    /**
     * Обработка ошибок HTTP запросов
     * @param {Error} error - ошибка от axios
     * @returns {Error} - обработанная ошибка
     * @private
     */
    static _handleError(error) {
      if (error.response) {
        // Ошибка от сервера
        const { status, data } = error.response;
        
        switch (status) {
          case 400:
            return new Error(data.message || data.title || 'Неверный запрос');
          case 401:
            // Автоматически обрабатывается интерсептором
            return new Error('Требуется авторизация. Пожалуйста, войдите в систему.');
          case 403:
            return new Error('Доступ запрещен. У вас недостаточно прав для выполнения этого действия.');
          case 404:
            return new Error(data.message || 'Ресурс не найден');
          case 409:
            return new Error(data.message || 'Конфликт данных. Возможно, запись уже существует.');
          case 422:
            const validationErrors = data.errors ? 
              Object.values(data.errors).flat().join(', ') : 
              data.message || 'Ошибка валидации данных';
            return new Error(validationErrors);
          case 500:
            return new Error('Внутренняя ошибка сервера. Пожалуйста, попробуйте позже.');
          default:
            return new Error(data?.message || data?.title || `Ошибка сервера: ${status}`);
        }
      } else if (error.request) {
        // Запрос был сделан, но ответ не получен
        return new Error('Нет ответа от сервера. Проверьте подключение к интернету.');
      } else {
        // Ошибка при настройке запроса
        return new Error(error.message || 'Ошибка при выполнении запроса');
      }
    }
    
    /**
     * Скачать файл
     * @param {Blob} blob - blob данные файла
     * @param {string} fileName - имя файла
     */
    static downloadFile(blob, fileName = 'export.csv') {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    }
    
    /**
     * Установить базовый URL для API
     * @param {string} baseUrl - базовый URL
     */
    static setBaseUrl(baseUrl) {
      configureApiService.setBaseUrl(baseUrl);
    }
    
    /**
     * Установить таймаут для запросов
     * @param {number} timeout - таймаут в миллисекундах
     */
    static setTimeout(timeout) {
      configureApiService.setTimeout(timeout);
    }
    
    /**
     * Очистить токен авторизации
     */
    static clearAuthToken() {
      configureApiService.clearAuthToken();
    }
  }
  
  export default VendingService;