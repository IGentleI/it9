# Сайт храма (Flask)

## Запуск

```bash
cd church_site
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
python app.py
```

Откройте: `http://127.0.0.1:5000`.

## Админ-панель

- Вход: `http://127.0.0.1:5000/admin/login`
- По умолчанию:
  - логин: `admin`
  - пароль: `admin123`

Рекомендуется переопределить через переменные окружения:

- `CHURCH_ADMIN_LOGIN`
- `CHURCH_ADMIN_PASSWORD`
- `FLASK_SECRET_KEY`

Только администратор может:
- добавлять новости с фотографиями,
- добавлять клириков с фотографиями,
- обновлять текущее расписание (фото и/или текст вручную).

## Фон сайта

Положите фото храма в `static/images/church.jpg`.
