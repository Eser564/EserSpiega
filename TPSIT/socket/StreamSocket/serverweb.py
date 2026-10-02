import socket
from datetime import datetime, timezone
import random
import json

HOST = '127.0.0.1'
PORT = 16767

ss = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
ss.bind((HOST, PORT))
ss.listen(1)
print(f'Server listen on port: {PORT}...')
goAway = True

while goAway:
    con, addr = ss.accept()
    MSG = con.recv(4096).decode('utf-8').strip()
    print(f'{addr[0]}:{addr[1]} (recv) << "{MSG}"..')
    
    if MSG.lower() == 'end':
        goAway = False
        con.close()
        break
    
    try:
        with open('motd.json', 'r', encoding='utf-8') as f:
            dati = json.load(f)
        messaggi = dati.get('messaggi', [])
        if not messaggi:
            raise ValueError("La lista 'messaggi' è vuota nel file JSON")
    except FileNotFoundError:
        print(f"Json non trovato!")
    except (json.JSONDecodeError, ValueError) as e:
        print(f"Errore nel parsing di motd: {e}")

    try:
        with open('test.html', 'r', encoding='utf-8') as f:
            page = f.read()
    except FileNotFoundError:
        page = "<h1>Errore: Umano! Sei nel posto sbagliato! Avrai una stipsi!</h1>"

    random_mess = random.choice(messaggi)
    htmlpage = page.format(messaggio=random_mess)


    date_str = datetime.now(timezone.utc).strftime("%a, %d %b %Y %H:%M:%S GMT")
    
    headresponse = (
        f"HTTP/1.1 200 OK\r\n"
        f"Date: {date_str}\r\n"
        f"Server: Apache/2.7.10 (Skynet OS - Apuzzo is coming)\r\n"
        f"Content-Type: text/html; charset=UTF-8\r\n"
        f"Content-Length: {len(htmlpage.encode('utf-8'))}\r\n"
        f"Connection: close\r\n\r\n"
    )

    response = headresponse + htmlpage
    
    con.sendall(response.encode('utf-8'))
    print(f'{addr[0]}:{addr[1]} (send) >> HTTP Response sent..')
    con.close()

ss.close()
print('Server was remote stopped...')
