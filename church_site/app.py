from __future__ import annotations

import json
import os
from datetime import datetime
from functools import wraps
from pathlib import Path
from uuid import uuid4

from flask import (
    Flask,
    flash,
    redirect,
    render_template,
    request,
    session,
    url_for,
)
from werkzeug.utils import secure_filename

BASE_DIR = Path(__file__).resolve().parent
DATA_FILE = BASE_DIR / "data" / "content.json"
UPLOAD_DIR = BASE_DIR / "static" / "uploads"

ALLOWED_EXTENSIONS = {"png", "jpg", "jpeg", "webp"}
DEFAULT_ADMIN_LOGIN = os.getenv("CHURCH_ADMIN_LOGIN", "admin")
DEFAULT_ADMIN_PASSWORD = os.getenv("CHURCH_ADMIN_PASSWORD", "admin123")

app = Flask(__name__)
app.config["SECRET_KEY"] = os.getenv("FLASK_SECRET_KEY", "change-me-in-production")
app.config["MAX_CONTENT_LENGTH"] = 8 * 1024 * 1024


DEFAULT_DATA = {
    "news": [
        {
            "id": "n1",
            "title": "Пасхальная встреча молодежи",
            "text": "После вечернего богослужения состоится приходская встреча для молодежи.",
            "date": "2026-04-14",
            "image": "",
        }
    ],
    "schedule": {
        "text": "Суббота — Всенощное бдение 17:00\nВоскресенье — Литургия 08:30",
        "image": "",
        "updated_at": "",
    },
    "about": "Храм посвящен подвигу Новомучеников и Исповедников Российских.",
    "relics": "В храме пребывает ковчег с частицами мощей святых.",
    "clergy": [
        {
            "id": "c1",
            "name": "протоиерей Александр",
            "role": "Настоятель",
            "bio": "Окормляет приход и ведет катехизические беседы.",
            "photo": "",
        }
    ],
    "contacts": {
        "address": "г. [Ваш город], ул. [Название улицы], д. [номер]",
        "phone": "+7 (000) 000-00-00",
        "email": "info@hram-example.ru",
    },
}


def ensure_storage() -> None:
    DATA_FILE.parent.mkdir(parents=True, exist_ok=True)
    UPLOAD_DIR.mkdir(parents=True, exist_ok=True)
    if not DATA_FILE.exists():
        save_data(DEFAULT_DATA)


def load_data() -> dict:
    ensure_storage()
    with DATA_FILE.open("r", encoding="utf-8") as f:
        return json.load(f)


def save_data(data: dict) -> None:
    DATA_FILE.parent.mkdir(parents=True, exist_ok=True)
    with DATA_FILE.open("w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)


def is_allowed_file(filename: str) -> bool:
    return "." in filename and filename.rsplit(".", 1)[1].lower() in ALLOWED_EXTENSIONS


def save_upload(file_storage) -> str:
    if not file_storage or not file_storage.filename:
        return ""
    filename = secure_filename(file_storage.filename)
    if not filename or not is_allowed_file(filename):
        return ""
    ext = filename.rsplit(".", 1)[1].lower()
    unique_name = f"{uuid4().hex}.{ext}"
    destination = UPLOAD_DIR / unique_name
    file_storage.save(destination)
    return f"uploads/{unique_name}"


def login_required(view_func):
    @wraps(view_func)
    def wrapped(*args, **kwargs):
        if not session.get("is_admin"):
            flash("Только администратор может изменять контент.", "error")
            return redirect(url_for("admin_login"))
        return view_func(*args, **kwargs)

    return wrapped


@app.route("/")
def index():
    data = load_data()
    return render_template("index.html", data=data)


@app.route("/admin/login", methods=["GET", "POST"])
def admin_login():
    if request.method == "POST":
        login = request.form.get("login", "")
        password = request.form.get("password", "")
        if login == DEFAULT_ADMIN_LOGIN and password == DEFAULT_ADMIN_PASSWORD:
            session["is_admin"] = True
            flash("Вход выполнен.", "success")
            return redirect(url_for("admin_panel"))
        flash("Неверный логин или пароль.", "error")
    return render_template("admin_login.html")


@app.route("/admin/logout")
def admin_logout():
    session.pop("is_admin", None)
    flash("Вы вышли из панели администратора.", "success")
    return redirect(url_for("index"))


@app.route("/admin")
@login_required
def admin_panel():
    data = load_data()
    return render_template("admin.html", data=data)


@app.post("/admin/news/add")
@login_required
def add_news():
    data = load_data()
    title = request.form.get("title", "").strip()
    text = request.form.get("text", "").strip()
    if not title or not text:
        flash("Заполните заголовок и текст новости.", "error")
        return redirect(url_for("admin_panel"))

    image_path = save_upload(request.files.get("image"))
    news_item = {
        "id": uuid4().hex,
        "title": title,
        "text": text,
        "date": datetime.now().strftime("%Y-%m-%d"),
        "image": image_path,
    }
    data["news"].insert(0, news_item)
    save_data(data)
    flash("Новость добавлена.", "success")
    return redirect(url_for("admin_panel"))


@app.post("/admin/clergy/add")
@login_required
def add_clergy():
    data = load_data()
    name = request.form.get("name", "").strip()
    role = request.form.get("role", "").strip()
    bio = request.form.get("bio", "").strip()
    if not name or not role:
        flash("Укажите имя и сан/должность.", "error")
        return redirect(url_for("admin_panel"))

    photo_path = save_upload(request.files.get("photo"))
    clergy_item = {
        "id": uuid4().hex,
        "name": name,
        "role": role,
        "bio": bio,
        "photo": photo_path,
    }
    data["clergy"].append(clergy_item)
    save_data(data)
    flash("Клирик добавлен.", "success")
    return redirect(url_for("admin_panel"))


@app.post("/admin/schedule/update")
@login_required
def update_schedule():
    data = load_data()
    text = request.form.get("text", "").strip()
    image_path = save_upload(request.files.get("image"))

    data["schedule"]["text"] = text
    if image_path:
        data["schedule"]["image"] = image_path
    data["schedule"]["updated_at"] = datetime.now().strftime("%Y-%m-%d %H:%M")
    save_data(data)

    flash("Расписание обновлено.", "success")
    return redirect(url_for("admin_panel"))


if __name__ == "__main__":
    ensure_storage()
    app.run(debug=True)
