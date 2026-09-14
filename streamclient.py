import socket

HOST = '172.16.13.5'  
PORT = 16767

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect((HOST, PORT))
print(f"[CLIENT] Connesso al server {HOST}:{PORT}")

attivo = True
while attivo:
    msg_da_inviare = input('[Write] >_: ').strip()
    client.send(msg_da_inviare.encode('utf-8'))
    
    if 'end' in msg_da_inviare.lower():
        attivo = False
        break

    msg_ricevuto = client.recv(4096).decode('utf-8')
    print(f"[Server] >> {msg_ricevuto}")
    
    if 'end' in msg_ricevuto.lower():
        print("[Server] Server disconnected.")
        attivo = False

# Chiusura del socket
client.close()
print("[CLIENT] Disconnesso.")
