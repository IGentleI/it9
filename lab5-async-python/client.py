import asyncio
import json
from typing import Any

HOST = "127.0.0.1"
PORT = 5000


async def send_packet(writer: asyncio.StreamWriter, packet: dict[str, Any]) -> None:
    writer.write((json.dumps(packet, ensure_ascii=False) + "\n").encode("utf-8"))
    await writer.drain()


async def receive_packet(reader: asyncio.StreamReader) -> dict[str, Any] | None:
    raw = await reader.readline()
    if not raw:
        return None
    return json.loads(raw.decode("utf-8"))


async def run_admin(reader: asyncio.StreamReader, writer: asyncio.StreamWriter) -> None:
    print("=== Режим администратора ===")
    dish_id = int(input("ID блюда: "))
    dish_name = input("Название блюда: ").strip()
    dish_price = float(input("Цена блюда: "))

    await send_packet(
        writer,
        {
            "command": "AddDish",
            "dish": {"id": dish_id, "name": dish_name, "price": dish_price},
        },
    )

    response = await receive_packet(reader)
    if response:
        print(response.get("message", "Нет ответа"))


async def run_customer(reader: asyncio.StreamReader, writer: asyncio.StreamWriter) -> None:
    print("=== Режим клиента ===")

    await send_packet(writer, {"command": "WaitMenu"})
    response = await receive_packet(reader)

    menu = response.get("menu", []) if response else []
    if not menu:
        print("Меню пустое")
        return

    print("Меню:")
    for dish in menu:
        print(f"{dish['id']}. {dish['name']} - {dish['price']} руб.")

    raw_ids = input("Введите ID блюд через запятую (пример: 1,2): ")
    order_ids = []
    for x in raw_ids.split(","):
        x = x.strip()
        if x.isdigit():
            order_ids.append(int(x))

    await send_packet(writer, {"command": "Order", "order_ids": order_ids})
    receipt_response = await receive_packet(reader)

    receipt = receipt_response.get("receipt") if receipt_response else None
    if not receipt:
        print("Не удалось получить чек")
        return

    print("\nЧек:")
    for item in receipt["items"]:
        print(f"- {item['name']}: {item['price']} руб.")
    print(f"Количество: {receipt['count']}")
    print(f"Итого: {receipt['total']} руб.")

    payment = float(input("Внесите оплату: "))
    await send_packet(
        writer,
        {
            "command": "Payment",
            "payment_amount": payment,
            "expected_total": receipt["total"],
        },
    )

    payment_result = await receive_packet(reader)
    if payment_result:
        print(payment_result.get("message", "Нет ответа"))


async def main() -> None:
    print("Выберите режим: 1 - администратор, 2 - голодный клиент")
    mode = input("Ваш выбор: ").strip()

    reader, writer = await asyncio.open_connection(HOST, PORT)

    try:
        if mode == "1":
            await run_admin(reader, writer)
        else:
            await run_customer(reader, writer)
    finally:
        writer.close()
        await writer.wait_closed()


if __name__ == "__main__":
    asyncio.run(main())
