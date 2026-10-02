import socket

HOST = '172.16.13.5'
PORT = 16767

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)		# socket.AF_INET, socket.SOCK_STREAM)

print(f'TCP Server Listening on {HOST}:{PORT}..')

s.bind((HOST, PORT))
s.listen(1)


con, addr = s.accept()
print(f"Client connesso: [{addr[0]}:{addr[1]}]")

isWorking = True
while isWorking:
    msg = con.recv(4096).decode()

    if not msg:
        print('Connessione terminata e muto')
        break

    print (f'[{addr[0]}:{addr[1]}] << "{msg}"...')
    ack = input('[write] >_').strip()
    con.sendall(ack.encode())
    isWorking = 'end' not in msg.lower()


con.close()
s.close()
print('Server Stopped..')
