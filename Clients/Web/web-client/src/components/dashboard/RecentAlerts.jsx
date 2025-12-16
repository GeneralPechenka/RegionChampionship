// src/components/dashboard/RecentAlerts.jsx
import React from 'react';
import { ExclamationTriangle, InfoCircle, CheckCircle, Clock } from 'react-bootstrap-icons';

const RecentAlerts = ({ alerts = [] }) => {
  if (!alerts.length) {
    return <div className="p-3 text-muted">Нет новых уведомлений</div>;
  }

  return (
    <div>
      {alerts.map((alert, index) => (
        <div key={index} className="p-3 border-bottom">
          <div className="d-flex">
            <div className="me-3">
              {alert.type === 'warning' && <ExclamationTriangle className="text-warning" />}
              {alert.type === 'info' && <InfoCircle className="text-info" />}
              {alert.type === 'success' && <CheckCircle className="text-success" />}
            </div>
            <div>
              <p className="mb-1">{alert.message || 'Уведомление'}</p>
              <small className="text-muted">
                <Clock size={12} className="me-1" />
                {alert.time || 'Только что'}
              </small>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default RecentAlerts;