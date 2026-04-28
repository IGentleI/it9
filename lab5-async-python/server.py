import asyncio
import json
from pathlib import Path
from typing import Any

HOST = "127.0.0.1"
PORT = 5000
MENU_FILE = Path(__file__).with_name("menu.json")


async def load_menu() -> list[dict[str, Any]]:
    if not MENU_FILE.exists():
        demo_menu = [
            {"id": 1, "name": "Бургер", "price": 220},
            {"id": 2, "name": "Картофель фри", "price": 120},
            {"id": 3, "name": "Кола", "price": 90},
        ]
        await save_menu(demo_menu)

    return json.loads(MENU_FILE.read_text(encoding="utf-8"))


async def save_menu(menu: list[dict[str, Any]]) -> None:
    MENU_FILE.write_text(json.dumps(menu, ensure_ascii=False, indent=2), encoding="utf-8")


async def send_packet(writer: asyncio.StreamWriter, packet: dict[str, Any]) -> None:
    writer.write((json.dumps(packet, ensure_ascii=False) + "\n").encode("utf-8"))
    await writer.drain()


def calculate_receipt(menu: list[dict[str, Any]], order_ids: list[int]) -> dict[str, Any]:
    selected = [dish for dish in menu if dish["id"] in order_ids]
    total = sum(dish["price"] for dish in selected)
    return {
        "items": selected,
        "count": len(selected),
        "total": total,
    }


async def handle_client(
    reader: asyncio.StreamReader,
    writer: asyncio.StreamWriter,
    menu_lock: asyncio.Lock,
) -> None:
    addr = writer.get_extra_info("peername")
    print(f"Клиент подключен: {addr}")

    try:
        while True:
            raw = await reader.readline()
            if not raw:
                break

            try:
                packet = json.loads(raw.decode("utf-8"))
            except json.JSONDecodeError:
                await send_packet(writer, {"command": "Error", "message": "Неверный JSON"})
                continue

            command = packet.get("command")

            if command == "WaitMenu":
                async with menu_lock:
                    menu = await load_menu()
                await send_packet(writer, {"command": "Menu", "menu": menu})

            elif command == "AddDish":
                dish = packet.get("dish")
                if not isinstance(dish, dict):
                    await send_packet(writer, {"command": "Error", "message": "Нет данных блюда"})
                    continue

                dish_id = dish.get("id")
                dish_name = dish.get("name")
                dish_price = dish.get("price")

                if not isinstance(dish_id, int) or dish_id <= 0:
                    await send_packet(writer, {"command": "Error", "message": "Неверный id"})
                    continue
                if not isinstance(dish_name, str) or not dish_name.strip():
                    await send_packet(writer, {"command": "Error", "message": "Неверное название"})
                    continue
                if not isinstance(dish_price, (int, float)) or dish_price <= 0:
                    await send_packet(writer, {"command": "Error", "message": "Неверная цена"})
                    continue

                async with menu_lock:
                    menu = await load_menu()
                    if any(item["id"] == dish_id for item in menu):
                        await send_packet(writer, {"command": "Error", "message": "ID уже существует"})
                        continue

                    menu.append({"id": dish_id, "name": dish_name.strip(), "price": dish_price})
                    await save_menu(menu)

                await send_packet(writer, {"command": "Ok", "message": "Блюдо добавлено"})

            elif command == "Order":
                order_ids = packet.get("order_ids", [])
                if not isinstance(order_ids, list):
                    await send_packet(writer, {"command": "Error", "message": "Неверный формат заказа"})
                    continue

                valid_ids = [x for x in order_ids if isinstance(x, int)]

                async with menu_lock:
                    menu = await load_menu()
                receipt = calculate_receipt(menu, valid_ids)

                await send_packet(writer, {"command": "Receipt", "receipt": receipt})

            elif command == "Payment":
                total = packet.get("expected_total", 0)
                payment = packet.get("payment_amount", 0)

                if payment < total:
                    await send_packet(
                        writer,
                        {
                            "command": "PaymentResult",
                            "accepted": False,
                            "message": "Пойдешь мыть посуду!!!",
                        },
                    )
                else:
                    await send_packet(
                        writer,
                        {
                            "command": "PaymentResult",
                            "accepted": True,
                            "message": "Оплата принята. Приятного аппетита!",
                        },
                    )

            else:
                await send_packet(writer, {"command": "Error", "message": "Неизвестная команда"})

    finally:
        writer.close()
        await writer.wait_closed()
        print(f"Клиент отключен: {addr}")


async def main() -> None:
    print(f"Сервер запущен на {HOST}:{PORT}")
    menu_lock = asyncio.Lock()

    server = await asyncio.start_server(
        lambda r, w: handle_client(r, w, menu_lock),
        host=HOST,
        port=PORT,
    )

    async with server:
        await server.serve_forever()


if __name__ == "__main__":
    asyncio.run(main())
