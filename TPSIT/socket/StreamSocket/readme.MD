Certo! Ecco il contenuto pronto da salvare come file `.md` e caricare su GitHub.

Suggerimento: salvalo come `RIPASSO_TPSIT_SOCKET_PYTHON.md`.

````markdown
# RIPASSO TPSIT — SOCKET PYTHON

## TCP/STREAM, UDP/DATAGRAM e SERVER HTTP

## 1. IDEA GENERALE

Una **socket** è un'interfaccia che permette a un programma di comunicare attraverso una rete.

Per indicare un endpoint si usano principalmente:

```text
IP + PORTA
```

Esempio:

```text
127.0.0.1 : 5000
```

```text
127.0.0.1 → indirizzo IPv4
5000      → porta
```

### Non confondere

```text
socket ≠ porta
AF_INET ≠ TCP
HTTP ≠ socket
HTML ≠ HTTP
JSON ≠ HTTP
```

---

# 2. AF_INET, SOCK_STREAM, SOCK_DGRAM

Quando scrivo:

```python
socket.socket(socket.AF_INET, socket.SOCK_STREAM)
```

ci sono due informazioni diverse.

### `AF_INET`

Indica la famiglia di indirizzi:

```text
AF_INET → IPv4
AF_INET6 → IPv6
```

### `SOCK_STREAM`

Indica un socket di tipo stream, normalmente associato a:

```text
TCP
```

### `SOCK_DGRAM`

Indica un socket di tipo datagram, normalmente associato a:

```text
UDP
```

### Da ricordare

```text
AF_INET      → IPv4
SOCK_STREAM  → TCP
SOCK_DGRAM   → UDP
```

---

# 3. TCP / STREAM

TCP è orientato alla connessione e fornisce un **flusso di byte affidabile e ordinato**.

## SERVER TCP

Sequenza fondamentale:

```text
socket()
↓
bind()
↓
listen()
↓
accept()
↓
recv()/send()
↓
close()
```

## CLIENT TCP

```text
socket()
↓
connect()
↓
send()/recv()
↓
close()
```

Queste due sequenze bisogna saperle riconoscere immediatamente.

---

# 4. `socket()`

```python
server = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)
```

Crea la socket.

NON significa:

```text
connessione già stabilita
```

Significa semplicemente:

```text
creazione del socket
```

---

# 5. `bind()`

```python
server.bind(("127.0.0.1", 5000))
```

Associa la socket a un indirizzo locale:

```text
127.0.0.1 : 5000
```

### NON significa:

```text
connettiti al server
```

### Significa:

```text
associa questa socket a questo indirizzo locale
```

Nel modello tipico:

```text
SERVER → bind()
CLIENT → connect()
```

---

# 6. `listen()`

```python
server.listen(1)
```

Mette il server TCP in ascolto delle richieste di connessione.

Non:

```text
riceve i dati
accetta direttamente il client
invia dati
```

È il passaggio che prepara il socket server ad accettare connessioni.

### `listen(1)` / `listen(5)`

Il numero riguarda il **backlog**, cioè la coda delle connessioni pendenti.

Non significa:

```text
listen(5) = massimo 5 client
```

e non significa:

```text
5 thread
```

---

# 7. `accept()`

```python
conn, addr = server.accept()
```

`accept()` accetta una connessione TCP e restituisce:

```text
conn → socket della connessione con quel client
addr → indirizzo del client
```

Esempio:

```python
addr = ("127.0.0.1", 53142)
```

Quindi:

```text
server → socket di ascolto
conn   → socket usato per comunicare con quel client
addr   → indirizzo del client
```

## IMPORTANTE

Dopo:

```python
conn, addr = server.accept()
```

la comunicazione con quel client avviene normalmente tramite:

```python
conn.recv(...)
conn.send(...)
conn.sendall(...)
```

non tramite il socket di ascolto `server`.

---

# 8. `connect()`

Il client normalmente fa:

```python
client.connect(("127.0.0.1", 5000))
```

Serve a tentare di stabilire una connessione con l'endpoint remoto.

Schema:

```text
SERVER
bind()
listen()
accept()

CLIENT
connect()
```

---

# 9. `recv()`

```python
data = conn.recv(1024)
```

Significa:

```text
ricevi fino a 1024 byte
```

NON:

```text
ricevi esattamente 1024 byte
```

Può restituire meno dati.

Inoltre `recv()` restituisce normalmente:

```python
bytes
```

Esempio:

```python
data = conn.recv(1024)
print(data)
```

può mostrare:

```text
b'Ciao'
```

---

# 10. `send()` E `sendall()`

### `send()`

```python
conn.send(data)
```

può inviare solo una parte dei byte richiesti.

### `sendall()`

```python
conn.sendall(data)
```

continua a tentare l'invio fino al completamento oppure a un errore.

### Da ricordare

```text
send()    → possibile invio parziale
sendall() → tenta di inviare tutto
```

---

# 11. `bytes` E `str`

Questa distinzione è fondamentale.

```python
"Ciao"
```

→ `str`

```python
b"Ciao"
```

→ `bytes`

Le socket lavorano con dati binari, quindi bisogna convertire il testo quando necessario.

---

# 12. `encode()` E `decode()`

## Regola da sapere a memoria

```text
encode() → str → bytes
decode() → bytes → str
```

Esempio:

```python
"Ciao".encode("utf-8")
```

→ `bytes`

E:

```python
b"Ciao".decode("utf-8")
```

→ `str`

### Schema

```text
STRINGA
   ↓
encode()
   ↓
BYTES
   ↓
rete
   ↓
BYTES
   ↓
decode()
   ↓
STRINGA
```

---

# 13. TCP = STREAM

Questo è un concetto importantissimo.

TCP è un **flusso di byte**.

NON conserva i confini delle singole `send()`.

Per esempio:

```python
conn.sendall(b"ABC")
conn.sendall(b"DEF")
```

non significa che il server riceverà necessariamente:

```text
recv() → ABC
recv() → DEF
```

Potrebbe ricevere il flusso in modo diverso.

Quindi:

```text
una send() ≠ una recv()
```

Se l'applicazione deve distinguere i messaggi, deve usare un proprio sistema di delimitazione.

Esempi:

```text
\n
lunghezza del messaggio
protocollo applicativo
```

---

# 14. `recv()` PUÒ RESTITUIRE `b""`

Se il peer ha chiuso ordinatamente la connessione TCP:

```python
data = conn.recv(1024)
```

può restituire:

```python
b""
```

Per questo è comune trovare:

```python
if not data:
    break
```

---

# 15. THREE-WAY HANDSHAKE

TCP stabilisce la connessione attraverso il classico:

```text
CLIENT                 SERVER

SYN       ────────────→

          ←──────── SYN-ACK

ACK       ────────────→
```

Quindi:

```text
SYN
SYN-ACK
ACK
```

### UDP?

Niente three-way handshake TCP.

---

# 16. SERVER TCP COMPLETO

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

server = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)

server.bind((HOST, PORT))
server.listen()

conn, addr = server.accept()

data = conn.recv(1024)

print(data.decode("utf-8"))

conn.sendall(b"Ciao client")

conn.close()
server.close()
```

### Sequenza:

```text
socket
↓
bind
↓
listen
↓
accept
↓
recv
↓
sendall
↓
close
```

---

# 17. CLIENT TCP COMPLETO

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

client = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)

client.connect((HOST, PORT))

client.sendall(
    "Ciao server".encode("utf-8")
)

data = client.recv(1024)

print(data.decode("utf-8"))

client.close()
```

### Sequenza:

```text
socket
↓
connect
↓
sendall
↓
recv
↓
close
```

---

# 18. SERVER TCP CON PIÙ CLIENT

Un server può usare:

```python
while True:
    conn, addr = server.accept()

    data = conn.recv(1024)

    conn.sendall(data)

    conn.close()
```

Questo permette di trattare più client nel tempo.

Senza thread/processi/async, il modello è generalmente **sequenziale**:

```text
client 1
↓
servizio
↓
close

client 2
↓
servizio
↓
close
```

Il `while True` da solo NON rende il server concorrente.

---

# 19. UDP / DATAGRAM

UDP è normalmente usato con:

```python
socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)
```

UDP è connectionless nel modello classico.

Non utilizzi normalmente:

```text
listen()
accept()
```

---

# 20. UDP SERVER

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

server = socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)

server.bind((HOST, PORT))

data, addr = server.recvfrom(1024)

print(data.decode("utf-8"))

server.sendto(
    "Ciao".encode("utf-8"),
    addr
)

server.close()
```

### Sequenza:

```text
socket
↓
bind
↓
recvfrom
↓
sendto
↓
close
```

---

# 21. UDP CLIENT

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

client = socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)

client.sendto(
    "Ciao server".encode("utf-8"),
    (HOST, PORT)
)

data, addr = client.recvfrom(1024)

print(data.decode("utf-8"))

client.close()
```

### Sequenza:

```text
socket
↓
sendto
↓
recvfrom
↓
close
```

---

# 22. `sendto()`

```python
client.sendto(
    data,
    (HOST, PORT)
)
```

Invia un datagramma specificando il destinatario.

È tipico di UDP.

---

# 23. `recvfrom()`

```python
data, addr = server.recvfrom(1024)
```

restituisce:

```text
data → dati
addr → indirizzo del mittente
```

Quindi:

```text
accept()
→ socket + indirizzo

recvfrom()
→ datagramma + indirizzo
```

---

# 24. TCP VS UDP

| TCP / STREAM               | UDP / DATAGRAM                      |
| -------------------------- | ----------------------------------- |
| `SOCK_STREAM`              | `SOCK_DGRAM`                        |
| orientato alla connessione | connectionless nel modello classico |
| `listen()`                 | no                                  |
| `accept()`                 | no                                  |
| `connect()` tipico client  | non necessaria per il normale invio |
| `send()` / `sendall()`     | `sendto()`                          |
| `recv()`                   | `recvfrom()`                        |
| stream di byte             | datagrammi                          |
| affidabile                 | non garantisce la consegna          |
| ordine garantito           | ordine non garantito                |
| three-way handshake        | nessun three-way handshake TCP      |

### Da non imparare:

```text
TCP = lento
UDP = veloce
```

come regola assoluta.

---

# 25. HTTP

HTTP è un **protocollo applicativo**.

Nel vostro server scolastico:

```text
HTTP
↓
TCP
↓
IP
↓
socket
```

Quindi:

```text
HTTP ≠ TCP
```

Un normale server TCP può esistere senza essere un server web.

Diventa un server web quando gestisce richieste e risposte HTTP.

---

# 26. BROWSER = CLIENT

Nel web server:

```text
Browser → client
Python  → server
```

Quando il browser visita:

```text
http://127.0.0.1:8080/
```

il browser si connette al server Python tramite TCP.

---

# 27. HTTP REQUEST

Esempio:

```http
GET / HTTP/1.1
Host: localhost:8080
Connection: close

```

La prima riga contiene:

```text
METODO + TARGET + VERSIONE
```

Quindi:

```text
GET
/
HTTP/1.1
```

---

# 28. HTTP RESPONSE

Esempio:

```http
HTTP/1.1 200 OK
Content-Type: text/html; charset=utf-8
Content-Length: 42
Connection: close

<html>...</html>
```

Struttura:

```text
STATUS LINE
HEADER
HEADER
HEADER
RIGA VUOTA
BODY
```

---

# 29. REQUEST VS RESPONSE

### Request

```text
GET / HTTP/1.1
```

→ richiesta del client.

### Response

```text
HTTP/1.1 200 OK
```

→ risposta del server.

Non confonderle.

---

# 30. `\r\n`

Nella sintassi HTTP/1.1 le righe sono terminate normalmente con:

```text
\r\n
```

Quindi:

```text
HTTP/1.1 200 OK\r\n
Content-Type: text/html\r\n
```

---

# 31. `\r\n\r\n`

È una sequenza importantissima:

```text
\r\n\r\n
```

Indica la separazione tra:

```text
HEADER
↓
BODY
```

Quindi:

```text
HTTP/1.1 200 OK\r\n
Content-Type: text/html\r\n
\r\n
<html>...</html>
```

---

# 32. `Content-Type`

Indica il tipo di contenuto del body.

Esempi:

```text
text/html
application/json
text/plain
```

Quindi:

```http
Content-Type: text/html
```

→ body HTML.

---

# 33. `Content-Length`

Indica la lunghezza del body in **byte**.

Per un body UTF-8:

```python
body_bytes = body.encode("utf-8")
```

e:

```python
len(body_bytes)
```

fornisce il numero di byte da usare per `Content-Length`.

Non confondere sempre:

```text
numero caratteri
```

con:

```text
numero byte
```

---

# 34. `Connection: close`

```http
Connection: close
```

indica che la connessione verrà chiusa.

Nel vostro codice infatti trovi:

```python
conn.close()
```

---

# 35. CODICI HTTP

I principali:

```text
200 → OK
201 → Created
301 → Moved Permanently
302 → Found
400 → Bad Request
401 → Unauthorized
403 → Forbidden
404 → Not Found
500 → Internal Server Error
```

Famiglie:

```text
1xx → informativi
2xx → successo
3xx → redirezione
4xx → errore lato richiesta/client
5xx → errore lato server
```

### 404

```text
404 Not Found
```

NON significa:

```text
server spento
```

Significa che la risorsa richiesta non è stata trovata.

---

# 36. SERVER HTTP MINIMALE

```python
import socket

HOST = "127.0.0.1"
PORT = 8080

server = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)

server.bind((HOST, PORT))
server.listen()

while True:
    conn, addr = server.accept()

    request = conn.recv(4096)

    print(request.decode("utf-8"))

    body = """
<html>
<body>
    <h1>Hello World</h1>
</body>
</html>
"""

    body_bytes = body.encode("utf-8")

    response = (
        "HTTP/1.1 200 OK\r\n"
        "Content-Type: text/html; charset=utf-8\r\n"
        f"Content-Length: {len(body_bytes)}\r\n"
        "Connection: close\r\n"
        "\r\n"
    ).encode("utf-8") + body_bytes

    conn.sendall(response)
    conn.close()
```

---

# 37. IL WEB SERVER RIGA PER RIGA

```python
request = conn.recv(4096)
```

Riceve i byte della richiesta HTTP.

```python
request.decode("utf-8")
```

Li converte in testo.

Poi il server costruisce:

```text
HTTP response
```

con:

```text
status line
header
header
riga vuota
body
```

Infine:

```python
conn.sendall(response)
```

invia la risposta al browser.

---

# 38. COME SI ANALIZZA LA REQUEST

Puoi prendere la prima riga:

```python
first_line = request.split("\r\n")[0]
```

Se hai:

```text
GET /players HTTP/1.1
```

puoi fare:

```python
parts = first_line.split(" ")
```

e ottenere concettualmente:

```text
parts[0] → GET
parts[1] → /players
parts[2] → HTTP/1.1
```

Quindi:

```python
method = parts[0]
path = parts[1]
version = parts[2]
```

---

# 39. ESEMPIO DI ROUTING

```python
if method == "GET" and path == "/":
    ...
elif method == "GET" and path == "/players":
    ...
else:
    ...
```

Questo permette al server di rispondere in modo diverso a seconda della risorsa richiesta.

---

# 40. `serverweb.py`: COSA SUCCEDE

Nel vostro primo server:

```text
GET /
```

restituisce:

```text
index.html
```

Mentre:

```text
GET /players
```

restituisce:

```text
JSON dei giocatori
```

Qualunque altro caso gestito dal ramo finale:

```text
404 Not Found
```

---

# 41. `index.html`: COSA SUCCEDE

La pagina contiene:

```html
<button id="loadButton">
    Load Players
</button>
```

e:

```html
<tbody id="playersTable">
</tbody>
```

Poi JavaScript esegue:

```javascript
fetch('/players')
```

che provoca una richiesta HTTP:

```text
GET /players
```

al server.

---

# 42. FLUSSO COMPLETO DEL PRIMO SERVER

```text
Browser
↓
GET /
↓
Python server
↓
accept()
↓
recv()
↓
decode()
↓
method = GET
path = /
↓
legge index.html
↓
HTTP 200
↓
HTML
↓
Browser
↓
esegue JavaScript
↓
fetch('/players')
↓
GET /players
↓
Python server
↓
legge JSON
↓
json.dumps()
↓
HTTP 200
↓
JSON
↓
Browser
↓
response.json()
↓
creazione tabella
```

Questa sequenza è molto importante.

---

# 43. `json.load()` E `json.dumps()`

Non serve studiare JSON in profondità.

Basta ricordare:

```python
json.load(f)
```

→ legge JSON da file e lo converte in oggetti Python.

```python
json.dumps(obj)
```

→ converte un oggetto Python in una stringa JSON.

Nel primo server:

```text
soccerplayers.json
↓
json.load()
↓
giocatori
↓
json.dumps()
↓
HTTP response
```

---

# 44. `Content-Type` DEL JSON

Per `/players`:

```http
Content-Type: application/json; charset=utf-8
```

perché il body contiene JSON.

Quindi:

```text
/       → text/html
/players → application/json
```

---

# 45. `fetch()` E `response.json()`

Nel browser:

```javascript
const response = await fetch('/players');
```

`fetch()` effettua la richiesta HTTP e restituisce una Promise.

Poi:

```javascript
const players = await response.json();
```

interpreta il body della risposta come JSON.

Non confondere:

```text
Python json.load()
```

con:

```text
JavaScript response.json()
```

---

# 46. `response.ok`

```javascript
if (!response.ok) {
    throw new Error(`HTTP ${response.status}`);
}
```

`response.ok` è normalmente vero per gli status:

```text
200–299
```

e falso per gli altri.

Quindi:

```text
200 → true
404 → false
500 → false
```

---

# 47. DOM

Il JavaScript crea la tabella dinamicamente:

```javascript
const row = document.createElement('tr');
```

→ crea una riga.

```javascript
const cell = document.createElement('td');
```

→ crea una cella.

```javascript
row.appendChild(cell);
```

→ aggiunge la cella alla riga.

```javascript
table.appendChild(row);
```

→ aggiunge la riga alla tabella.

---

# 48. `textContent`

```javascript
cell.textContent = value;
```

inserisce il contenuto come testo.

Non è la stessa cosa di `innerHTML`.

---

# 49. `addEventListener`

```javascript
document
    .getElementById('loadButton')
    .addEventListener('click', loadPlayers);
```

significa:

```text
quando viene fatto click
↓
esegui loadPlayers
```

Attenzione:

```text
loadPlayers
```

non:

```text
loadPlayers()
```

perché qui si passa il riferimento alla funzione.

---

# 50. `loadPlayers()` AUTOMATICO

Alla fine hai anche:

```javascript
loadPlayers();
```

Quindi la funzione viene eseguita anche automaticamente.

Il pulsante permette di richiamarla nuovamente.

---

# 51. SECONDO SERVER WEB

Il secondo server è sempre:

```text
IPv4 + TCP + HTTP
```

ma invece di restituire una pagina già pronta, costruisce HTML dinamico.

---

# 52. `random.choice()`

```python
random_mess = random.choice(messaggi)
```

sceglie casualmente un elemento della lista.

---

# 53. `.format()`

Nel file:

```html
<p>{messaggio}</p>
```

Python fa:

```python
htmlpage = page.format(
    messaggio=random_mess
)
```

e sostituisce:

```text
{messaggio}
```

con il messaggio vero.

Quindi:

```text
template HTML
+
dato
↓
HTML finale
```

---

# 54. `datetime`

Il secondo server costruisce:

```http
Date: ...
```

utilizzando:

```python
datetime.now(timezone.utc)
```

La data viene poi formattata come stringa HTTP.

---

# 55. HEADER `Server`

Nel secondo programma:

```http
Server: Apache/2.7.10 (...)
```

è semplicemente un header inviato dal programma.

Non significa automaticamente che il programma Python stia eseguendo Apache.

---

# 56. `Date`, `Server`, `Content-Type`, `Content-Length`

Sono tutti:

```text
HEADER HTTP
```

Non sono funzioni della socket.

---

# 57. `SO_REUSEADDR`

Nel primo server:

```python
ss.setsockopt(
    socket.SOL_SOCKET,
    socket.SO_REUSEADDR,
    1
)
```

serve a impostare l'opzione di riutilizzo dell'indirizzo locale in determinate condizioni.

Serve soprattutto a evitare alcuni problemi nel riavvio rapido del server, come:

```text
Address already in use
```

### NON significa:

```text
più client contemporaneamente
```

---

# 58. `0.0.0.0` VS `127.0.0.1`

### Primo server

```python
HOST = "0.0.0.0"
```

→ ascolto sulle interfacce IPv4 locali disponibili.

### Secondo server

```python
HOST = "127.0.0.1"
```

→ loopback, quindi accesso locale.

---

# 59. `end` NEL SECONDO SERVER

Il secondo server contiene:

```python
if MSG.lower() == "end":
    goAway = False
    con.close()
    break
```

Se il client invia:

```text
end
```

il server termina il proprio ciclo.

`lower()` permette di riconoscere anche:

```text
END
End
eNd
```

---

# 60. TRABOCCHETTI DA SAPERE

## Socket

```text
AF_INET = TCP
```

❌

```text
AF_INET = IPv4
```

✅

---

```text
SOCK_STREAM = UDP
```

❌

```text
SOCK_STREAM = TCP
```

✅

---

```text
SOCK_DGRAM = TCP
```

❌

```text
SOCK_DGRAM = UDP
```

✅

---

## TCP

```text
bind() = connect()
```

❌

---

```text
listen() = recv()
```

❌

---

```text
accept() = ricevo direttamente i dati
```

❌

---

```text
accept() → connection socket + address
```

✅

---

```text
recv(1024) = esattamente 1024 byte
```

❌

---

```text
recv(1024) = fino a 1024 byte
```

✅

---

```text
recv() = str
```

❌

---

```text
recv() = bytes
```

✅

---

```text
encode() = bytes → str
```

❌

---

```text
encode() = str → bytes
```

✅

---

```text
decode() = str → bytes
```

❌

---

```text
decode() = bytes → str
```

✅

---

```text
send() = invio sicuramente tutto
```

❌

---

```text
sendall() = tenta di inviare tutto
```

✅

---

```text
TCP: una send() = una recv()
```

❌

---

```text
TCP = stream di byte
```

✅

---

## UDP

```text
UDP usa accept()
```

❌

---

```text
UDP usa listen()
```

❌

---

```text
UDP usa sendto()
```

✅

---

```text
UDP usa recvfrom()
```

✅

---

```text
recvfrom() → dati + indirizzo
```

✅

---

## HTTP

```text
GET / HTTP/1.1
```

✅ request line

---

```text
HTTP/1.1 200 OK
```

✅ status line

---

```text
200
```

✅ status code HTTP

---

```text
404
```

✅ risorsa non trovata

---

```text
404 = server spento
```

❌

---

```text
Content-Type: text/html
```

✅ body HTML

---

```text
Content-Type: application/json
```

✅ body JSON

---

```text
Content-Length
```

✅ lunghezza del body in byte

---

```text
\r\n
```

✅ fine di una riga HTTP

---

```text
\r\n\r\n
```

✅ separazione header/body

---

```text
HTTP = TCP
```

❌

---

```text
HTTP può viaggiare sopra TCP
```

✅

---

# 61. COSA IMPARARE A MEMORIA

## TCP SERVER

```text
socket
bind
listen
accept
recv/send
close
```

## TCP CLIENT

```text
socket
connect
send/recv
close
```

## UDP SERVER

```text
socket
bind
recvfrom
sendto
close
```

## UDP CLIENT

```text
socket
sendto
recvfrom
close
```

## CONVERSIONI

```text
str
↓ encode
bytes

bytes
↓ decode
str
```

## HTTP

```text
REQUEST
↓
GET + path + HTTP version

RESPONSE
↓
status + headers + blank line + body
```

---

# 62. LE 15 COSE CHE DEVI SAPERE AL 100%

```text
1. AF_INET → IPv4
2. SOCK_STREAM → TCP
3. SOCK_DGRAM → UDP
4. bind() → indirizzo locale
5. listen() → ascolto TCP
6. accept() → connection socket + address
7. connect() → connessione verso server
8. recv() → fino a N byte
9. sendall() → invio completo
10. encode() → str → bytes
11. decode() → bytes → str
12. TCP → stream
13. UDP → datagrammi
14. HTTP request → GET / ... HTTP/...
15. HTTP response → 200/404 + header + body
```

# 63. MODELLO MENTALE DEFINITIVO

Quando vedi:

```python
socket.AF_INET
```

pensa:

```text
IPv4
```

Quando vedi:

```python
socket.SOCK_STREAM
```

pensa:

```text
TCP
```

Quando vedi:

```python
socket.SOCK_DGRAM
```

pensa:

```text
UDP
```

Quando vedi:

```python
server.accept()
```

pensa:

```text
nuova connessione
↓
conn + addr
```

Quando vedi:

```python
conn.recv(...)
```

pensa:

```text
ricevo bytes
```

Quando vedi:

```python
.decode()
```

pensa:

```text
bytes → stringa
```

Quando vedi:

```python
.encode()
```

pensa:

```text
stringa → bytes
```

Quando vedi:

```python
sendto(...)
```

pensa:

```text
UDP
```

Quando vedi:

```python
GET /players HTTP/1.1
```

pensa:

```text
HTTP REQUEST
```

Quando vedi:

```text
HTTP/1.1 200 OK
```

pensa:

```text
HTTP RESPONSE
```

Quando vedi:

```text
\r\n\r\n
```

pensa:

```text
fine degli header
inizio del body
```

---

# 64. FORMULA FINALE

```text
TCP SERVER
socket → bind → listen → accept → recv/send → close

TCP CLIENT
socket → connect → send/recv → close

UDP SERVER
socket → bind → recvfrom/sendto → close

UDP CLIENT
socket → sendto/recvfrom → close

WEB SERVER
TCP
↓
HTTP REQUEST
↓
analisi GET/path
↓
HTTP RESPONSE
↓
HTML oppure JSON
```

## Obiettivo del compito

Devi essere in grado di:

```text
1. riconoscere quale protocollo stai usando;
2. mettere le istruzioni socket nell'ordine corretto;
3. correggere errori in un client/server;
4. distinguere ss/server socket da con/connection socket;
5. gestire bytes e stringhe con encode/decode;
6. capire cosa fa recv/send/sendto/recvfrom;
7. leggere una richiesta HTTP;
8. costruire una semplice risposta HTTP;
9. distinguere HTML e JSON;
10. seguire il flusso completo Browser → Server → Response.
```

### 🚨 Ultima regola anti-trabocchetto

Quando leggi una risposta multipla, fai attenzione alle parole:

```text
sempre
esattamente
necessariamente
esclusivamente
automaticamente
```

Molte affermazioni diventano false proprio perché sono formulate in modo assoluto.

Esempio:

```text
recv(1024) riceve esattamente 1024 byte
```

❌

```text
recv(1024) riceve fino a 1024 byte
```

✅

Questa è la versione da **studiare**, mentre il documento gigantesco può restare come riferimento quando trovi qualcosa che non ricordi.
````Certo! Ecco il contenuto pronto da salvare come file `.md` e caricare su GitHub.

Suggerimento: salvalo come `RIPASSO_TPSIT_SOCKET_PYTHON.md`.

````markdown
# RIPASSO TPSIT — SOCKET PYTHON

## TCP/STREAM, UDP/DATAGRAM e SERVER HTTP

## 1. IDEA GENERALE

Una **socket** è un'interfaccia che permette a un programma di comunicare attraverso una rete.

Per indicare un endpoint si usano principalmente:

```text
IP + PORTA
```

Esempio:

```text
127.0.0.1 : 5000
```

```text
127.0.0.1 → indirizzo IPv4
5000      → porta
```

### Non confondere

```text
socket ≠ porta
AF_INET ≠ TCP
HTTP ≠ socket
HTML ≠ HTTP
JSON ≠ HTTP
```

---

# 2. AF_INET, SOCK_STREAM, SOCK_DGRAM

Quando scrivo:

```python
socket.socket(socket.AF_INET, socket.SOCK_STREAM)
```

ci sono due informazioni diverse.

### `AF_INET`

Indica la famiglia di indirizzi:

```text
AF_INET → IPv4
AF_INET6 → IPv6
```

### `SOCK_STREAM`

Indica un socket di tipo stream, normalmente associato a:

```text
TCP
```

### `SOCK_DGRAM`

Indica un socket di tipo datagram, normalmente associato a:

```text
UDP
```

### Da ricordare

```text
AF_INET      → IPv4
SOCK_STREAM  → TCP
SOCK_DGRAM   → UDP
```

---

# 3. TCP / STREAM

TCP è orientato alla connessione e fornisce un **flusso di byte affidabile e ordinato**.

## SERVER TCP

Sequenza fondamentale:

```text
socket()
↓
bind()
↓
listen()
↓
accept()
↓
recv()/send()
↓
close()
```

## CLIENT TCP

```text
socket()
↓
connect()
↓
send()/recv()
↓
close()
```

Queste due sequenze bisogna saperle riconoscere immediatamente.

---

# 4. `socket()`

```python
server = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)
```

Crea la socket.

NON significa:

```text
connessione già stabilita
```

Significa semplicemente:

```text
creazione del socket
```

---

# 5. `bind()`

```python
server.bind(("127.0.0.1", 5000))
```

Associa la socket a un indirizzo locale:

```text
127.0.0.1 : 5000
```

### NON significa:

```text
connettiti al server
```

### Significa:

```text
associa questa socket a questo indirizzo locale
```

Nel modello tipico:

```text
SERVER → bind()
CLIENT → connect()
```

---

# 6. `listen()`

```python
server.listen(1)
```

Mette il server TCP in ascolto delle richieste di connessione.

Non:

```text
riceve i dati
accetta direttamente il client
invia dati
```

È il passaggio che prepara il socket server ad accettare connessioni.

### `listen(1)` / `listen(5)`

Il numero riguarda il **backlog**, cioè la coda delle connessioni pendenti.

Non significa:

```text
listen(5) = massimo 5 client
```

e non significa:

```text
5 thread
```

---

# 7. `accept()`

```python
conn, addr = server.accept()
```

`accept()` accetta una connessione TCP e restituisce:

```text
conn → socket della connessione con quel client
addr → indirizzo del client
```

Esempio:

```python
addr = ("127.0.0.1", 53142)
```

Quindi:

```text
server → socket di ascolto
conn   → socket usato per comunicare con quel client
addr   → indirizzo del client
```

## IMPORTANTE

Dopo:

```python
conn, addr = server.accept()
```

la comunicazione con quel client avviene normalmente tramite:

```python
conn.recv(...)
conn.send(...)
conn.sendall(...)
```

non tramite il socket di ascolto `server`.

---

# 8. `connect()`

Il client normalmente fa:

```python
client.connect(("127.0.0.1", 5000))
```

Serve a tentare di stabilire una connessione con l'endpoint remoto.

Schema:

```text
SERVER
bind()
listen()
accept()

CLIENT
connect()
```

---

# 9. `recv()`

```python
data = conn.recv(1024)
```

Significa:

```text
ricevi fino a 1024 byte
```

NON:

```text
ricevi esattamente 1024 byte
```

Può restituire meno dati.

Inoltre `recv()` restituisce normalmente:

```python
bytes
```

Esempio:

```python
data = conn.recv(1024)
print(data)
```

può mostrare:

```text
b'Ciao'
```

---

# 10. `send()` E `sendall()`

### `send()`

```python
conn.send(data)
```

può inviare solo una parte dei byte richiesti.

### `sendall()`

```python
conn.sendall(data)
```

continua a tentare l'invio fino al completamento oppure a un errore.

### Da ricordare

```text
send()    → possibile invio parziale
sendall() → tenta di inviare tutto
```

---

# 11. `bytes` E `str`

Questa distinzione è fondamentale.

```python
"Ciao"
```

→ `str`

```python
b"Ciao"
```

→ `bytes`

Le socket lavorano con dati binari, quindi bisogna convertire il testo quando necessario.

---

# 12. `encode()` E `decode()`

## Regola da sapere a memoria

```text
encode() → str → bytes
decode() → bytes → str
```

Esempio:

```python
"Ciao".encode("utf-8")
```

→ `bytes`

E:

```python
b"Ciao".decode("utf-8")
```

→ `str`

### Schema

```text
STRINGA
   ↓
encode()
   ↓
BYTES
   ↓
rete
   ↓
BYTES
   ↓
decode()
   ↓
STRINGA
```

---

# 13. TCP = STREAM

Questo è un concetto importantissimo.

TCP è un **flusso di byte**.

NON conserva i confini delle singole `send()`.

Per esempio:

```python
conn.sendall(b"ABC")
conn.sendall(b"DEF")
```

non significa che il server riceverà necessariamente:

```text
recv() → ABC
recv() → DEF
```

Potrebbe ricevere il flusso in modo diverso.

Quindi:

```text
una send() ≠ una recv()
```

Se l'applicazione deve distinguere i messaggi, deve usare un proprio sistema di delimitazione.

Esempi:

```text
\n
lunghezza del messaggio
protocollo applicativo
```

---

# 14. `recv()` PUÒ RESTITUIRE `b""`

Se il peer ha chiuso ordinatamente la connessione TCP:

```python
data = conn.recv(1024)
```

può restituire:

```python
b""
```

Per questo è comune trovare:

```python
if not data:
    break
```

---

# 15. THREE-WAY HANDSHAKE

TCP stabilisce la connessione attraverso il classico:

```text
CLIENT                 SERVER

SYN       ────────────→

          ←──────── SYN-ACK

ACK       ────────────→
```

Quindi:

```text
SYN
SYN-ACK
ACK
```

### UDP?

Niente three-way handshake TCP.

---

# 16. SERVER TCP COMPLETO

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

server = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)

server.bind((HOST, PORT))
server.listen()

conn, addr = server.accept()

data = conn.recv(1024)

print(data.decode("utf-8"))

conn.sendall(b"Ciao client")

conn.close()
server.close()
```

### Sequenza:

```text
socket
↓
bind
↓
listen
↓
accept
↓
recv
↓
sendall
↓
close
```

---

# 17. CLIENT TCP COMPLETO

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

client = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)

client.connect((HOST, PORT))

client.sendall(
    "Ciao server".encode("utf-8")
)

data = client.recv(1024)

print(data.decode("utf-8"))

client.close()
```

### Sequenza:

```text
socket
↓
connect
↓
sendall
↓
recv
↓
close
```

---

# 18. SERVER TCP CON PIÙ CLIENT

Un server può usare:

```python
while True:
    conn, addr = server.accept()

    data = conn.recv(1024)

    conn.sendall(data)

    conn.close()
```

Questo permette di trattare più client nel tempo.

Senza thread/processi/async, il modello è generalmente **sequenziale**:

```text
client 1
↓
servizio
↓
close

client 2
↓
servizio
↓
close
```

Il `while True` da solo NON rende il server concorrente.

---

# 19. UDP / DATAGRAM

UDP è normalmente usato con:

```python
socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)
```

UDP è connectionless nel modello classico.

Non utilizzi normalmente:

```text
listen()
accept()
```

---

# 20. UDP SERVER

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

server = socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)

server.bind((HOST, PORT))

data, addr = server.recvfrom(1024)

print(data.decode("utf-8"))

server.sendto(
    "Ciao".encode("utf-8"),
    addr
)

server.close()
```

### Sequenza:

```text
socket
↓
bind
↓
recvfrom
↓
sendto
↓
close
```

---

# 21. UDP CLIENT

```python
import socket

HOST = "127.0.0.1"
PORT = 5000

client = socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)

client.sendto(
    "Ciao server".encode("utf-8"),
    (HOST, PORT)
)

data, addr = client.recvfrom(1024)

print(data.decode("utf-8"))

client.close()
```

### Sequenza:

```text
socket
↓
sendto
↓
recvfrom
↓
close
```

---

# 22. `sendto()`

```python
client.sendto(
    data,
    (HOST, PORT)
)
```

Invia un datagramma specificando il destinatario.

È tipico di UDP.

---

# 23. `recvfrom()`

```python
data, addr = server.recvfrom(1024)
```

restituisce:

```text
data → dati
addr → indirizzo del mittente
```

Quindi:

```text
accept()
→ socket + indirizzo

recvfrom()
→ datagramma + indirizzo
```

---

# 24. TCP VS UDP

| TCP / STREAM               | UDP / DATAGRAM                      |
| -------------------------- | ----------------------------------- |
| `SOCK_STREAM`              | `SOCK_DGRAM`                        |
| orientato alla connessione | connectionless nel modello classico |
| `listen()`                 | no                                  |
| `accept()`                 | no                                  |
| `connect()` tipico client  | non necessaria per il normale invio |
| `send()` / `sendall()`     | `sendto()`                          |
| `recv()`                   | `recvfrom()`                        |
| stream di byte             | datagrammi                          |
| affidabile                 | non garantisce la consegna          |
| ordine garantito           | ordine non garantito                |
| three-way handshake        | nessun three-way handshake TCP      |

### Da non imparare:

```text
TCP = lento
UDP = veloce
```

come regola assoluta.

---

# 25. HTTP

HTTP è un **protocollo applicativo**.

Nel vostro server scolastico:

```text
HTTP
↓
TCP
↓
IP
↓
socket
```

Quindi:

```text
HTTP ≠ TCP
```

Un normale server TCP può esistere senza essere un server web.

Diventa un server web quando gestisce richieste e risposte HTTP.

---

# 26. BROWSER = CLIENT

Nel web server:

```text
Browser → client
Python  → server
```

Quando il browser visita:

```text
http://127.0.0.1:8080/
```

il browser si connette al server Python tramite TCP.

---

# 27. HTTP REQUEST

Esempio:

```http
GET / HTTP/1.1
Host: localhost:8080
Connection: close

```

La prima riga contiene:

```text
METODO + TARGET + VERSIONE
```

Quindi:

```text
GET
/
HTTP/1.1
```

---

# 28. HTTP RESPONSE

Esempio:

```http
HTTP/1.1 200 OK
Content-Type: text/html; charset=utf-8
Content-Length: 42
Connection: close

<html>...</html>
```

Struttura:

```text
STATUS LINE
HEADER
HEADER
HEADER
RIGA VUOTA
BODY
```

---

# 29. REQUEST VS RESPONSE

### Request

```text
GET / HTTP/1.1
```

→ richiesta del client.

### Response

```text
HTTP/1.1 200 OK
```

→ risposta del server.

Non confonderle.

---

# 30. `\r\n`

Nella sintassi HTTP/1.1 le righe sono terminate normalmente con:

```text
\r\n
```

Quindi:

```text
HTTP/1.1 200 OK\r\n
Content-Type: text/html\r\n
```

---

# 31. `\r\n\r\n`

È una sequenza importantissima:

```text
\r\n\r\n
```

Indica la separazione tra:

```text
HEADER
↓
BODY
```

Quindi:

```text
HTTP/1.1 200 OK\r\n
Content-Type: text/html\r\n
\r\n
<html>...</html>
```

---

# 32. `Content-Type`

Indica il tipo di contenuto del body.

Esempi:

```text
text/html
application/json
text/plain
```

Quindi:

```http
Content-Type: text/html
```

→ body HTML.

---

# 33. `Content-Length`

Indica la lunghezza del body in **byte**.

Per un body UTF-8:

```python
body_bytes = body.encode("utf-8")
```

e:

```python
len(body_bytes)
```

fornisce il numero di byte da usare per `Content-Length`.

Non confondere sempre:

```text
numero caratteri
```

con:

```text
numero byte
```

---

# 34. `Connection: close`

```http
Connection: close
```

indica che la connessione verrà chiusa.

Nel vostro codice infatti trovi:

```python
conn.close()
```

---

# 35. CODICI HTTP

I principali:

```text
200 → OK
201 → Created
301 → Moved Permanently
302 → Found
400 → Bad Request
401 → Unauthorized
403 → Forbidden
404 → Not Found
500 → Internal Server Error
```

Famiglie:

```text
1xx → informativi
2xx → successo
3xx → redirezione
4xx → errore lato richiesta/client
5xx → errore lato server
```

### 404

```text
404 Not Found
```

NON significa:

```text
server spento
```

Significa che la risorsa richiesta non è stata trovata.

---

# 36. SERVER HTTP MINIMALE

```python
import socket

HOST = "127.0.0.1"
PORT = 8080

server = socket.socket(
    socket.AF_INET,
    socket.SOCK_STREAM
)

server.bind((HOST, PORT))
server.listen()

while True:
    conn, addr = server.accept()

    request = conn.recv(4096)

    print(request.decode("utf-8"))

    body = """
<html>
<body>
    <h1>Hello World</h1>
</body>
</html>
"""

    body_bytes = body.encode("utf-8")

    response = (
        "HTTP/1.1 200 OK\r\n"
        "Content-Type: text/html; charset=utf-8\r\n"
        f"Content-Length: {len(body_bytes)}\r\n"
        "Connection: close\r\n"
        "\r\n"
    ).encode("utf-8") + body_bytes

    conn.sendall(response)
    conn.close()
```

---

# 37. IL WEB SERVER RIGA PER RIGA

```python
request = conn.recv(4096)
```

Riceve i byte della richiesta HTTP.

```python
request.decode("utf-8")
```

Li converte in testo.

Poi il server costruisce:

```text
HTTP response
```

con:

```text
status line
header
header
riga vuota
body
```

Infine:

```python
conn.sendall(response)
```

invia la risposta al browser.

---

# 38. COME SI ANALIZZA LA REQUEST

Puoi prendere la prima riga:

```python
first_line = request.split("\r\n")[0]
```

Se hai:

```text
GET /players HTTP/1.1
```

puoi fare:

```python
parts = first_line.split(" ")
```

e ottenere concettualmente:

```text
parts[0] → GET
parts[1] → /players
parts[2] → HTTP/1.1
```

Quindi:

```python
method = parts[0]
path = parts[1]
version = parts[2]
```

---

# 39. ESEMPIO DI ROUTING

```python
if method == "GET" and path == "/":
    ...
elif method == "GET" and path == "/players":
    ...
else:
    ...
```

Questo permette al server di rispondere in modo diverso a seconda della risorsa richiesta.

---

# 40. `serverweb.py`: COSA SUCCEDE

Nel vostro primo server:

```text
GET /
```

restituisce:

```text
index.html
```

Mentre:

```text
GET /players
```

restituisce:

```text
JSON dei giocatori
```

Qualunque altro caso gestito dal ramo finale:

```text
404 Not Found
```

---

# 41. `index.html`: COSA SUCCEDE

La pagina contiene:

```html
<button id="loadButton">
    Load Players
</button>
```

e:

```html
<tbody id="playersTable">
</tbody>
```

Poi JavaScript esegue:

```javascript
fetch('/players')
```

che provoca una richiesta HTTP:

```text
GET /players
```

al server.

---

# 42. FLUSSO COMPLETO DEL PRIMO SERVER

```text
Browser
↓
GET /
↓
Python server
↓
accept()
↓
recv()
↓
decode()
↓
method = GET
path = /
↓
legge index.html
↓
HTTP 200
↓
HTML
↓
Browser
↓
esegue JavaScript
↓
fetch('/players')
↓
GET /players
↓
Python server
↓
legge JSON
↓
json.dumps()
↓
HTTP 200
↓
JSON
↓
Browser
↓
response.json()
↓
creazione tabella
```

Questa sequenza è molto importante.

---

# 43. `json.load()` E `json.dumps()`

Non serve studiare JSON in profondità.

Basta ricordare:

```python
json.load(f)
```

→ legge JSON da file e lo converte in oggetti Python.

```python
json.dumps(obj)
```

→ converte un oggetto Python in una stringa JSON.

Nel primo server:

```text
soccerplayers.json
↓
json.load()
↓
giocatori
↓
json.dumps()
↓
HTTP response
```

---

# 44. `Content-Type` DEL JSON

Per `/players`:

```http
Content-Type: application/json; charset=utf-8
```

perché il body contiene JSON.

Quindi:

```text
/       → text/html
/players → application/json
```

---

# 45. `fetch()` E `response.json()`

Nel browser:

```javascript
const response = await fetch('/players');
```

`fetch()` effettua la richiesta HTTP e restituisce una Promise.

Poi:

```javascript
const players = await response.json();
```

interpreta il body della risposta come JSON.

Non confondere:

```text
Python json.load()
```

con:

```text
JavaScript response.json()
```

---

# 46. `response.ok`

```javascript
if (!response.ok) {
    throw new Error(`HTTP ${response.status}`);
}
```

`response.ok` è normalmente vero per gli status:

```text
200–299
```

e falso per gli altri.

Quindi:

```text
200 → true
404 → false
500 → false
```

---

# 47. DOM

Il JavaScript crea la tabella dinamicamente:

```javascript
const row = document.createElement('tr');
```

→ crea una riga.

```javascript
const cell = document.createElement('td');
```

→ crea una cella.

```javascript
row.appendChild(cell);
```

→ aggiunge la cella alla riga.

```javascript
table.appendChild(row);
```

→ aggiunge la riga alla tabella.

---

# 48. `textContent`

```javascript
cell.textContent = value;
```

inserisce il contenuto come testo.

Non è la stessa cosa di `innerHTML`.

---

# 49. `addEventListener`

```javascript
document
    .getElementById('loadButton')
    .addEventListener('click', loadPlayers);
```

significa:

```text
quando viene fatto click
↓
esegui loadPlayers
```

Attenzione:

```text
loadPlayers
```

non:

```text
loadPlayers()
```

perché qui si passa il riferimento alla funzione.

---

# 50. `loadPlayers()` AUTOMATICO

Alla fine hai anche:

```javascript
loadPlayers();
```

Quindi la funzione viene eseguita anche automaticamente.

Il pulsante permette di richiamarla nuovamente.

---

# 51. SECONDO SERVER WEB

Il secondo server è sempre:

```text
IPv4 + TCP + HTTP
```

ma invece di restituire una pagina già pronta, costruisce HTML dinamico.

---

# 52. `random.choice()`

```python
random_mess = random.choice(messaggi)
```

sceglie casualmente un elemento della lista.

---

# 53. `.format()`

Nel file:

```html
<p>{messaggio}</p>
```

Python fa:

```python
htmlpage = page.format(
    messaggio=random_mess
)
```

e sostituisce:

```text
{messaggio}
```

con il messaggio vero.

Quindi:

```text
template HTML
+
dato
↓
HTML finale
```

---

# 54. `datetime`

Il secondo server costruisce:

```http
Date: ...
```

utilizzando:

```python
datetime.now(timezone.utc)
```

La data viene poi formattata come stringa HTTP.

---

# 55. HEADER `Server`

Nel secondo programma:

```http
Server: Apache/2.7.10 (...)
```

è semplicemente un header inviato dal programma.

Non significa automaticamente che il programma Python stia eseguendo Apache.

---

# 56. `Date`, `Server`, `Content-Type`, `Content-Length`

Sono tutti:

```text
HEADER HTTP
```

Non sono funzioni della socket.

---

# 57. `SO_REUSEADDR`

Nel primo server:

```python
ss.setsockopt(
    socket.SOL_SOCKET,
    socket.SO_REUSEADDR,
    1
)
```

serve a impostare l'opzione di riutilizzo dell'indirizzo locale in determinate condizioni.

Serve soprattutto a evitare alcuni problemi nel riavvio rapido del server, come:

```text
Address already in use
```

### NON significa:

```text
più client contemporaneamente
```

---

# 58. `0.0.0.0` VS `127.0.0.1`

### Primo server

```python
HOST = "0.0.0.0"
```

→ ascolto sulle interfacce IPv4 locali disponibili.

### Secondo server

```python
HOST = "127.0.0.1"
```

→ loopback, quindi accesso locale.

---

# 59. `end` NEL SECONDO SERVER

Il secondo server contiene:

```python
if MSG.lower() == "end":
    goAway = False
    con.close()
    break
```

Se il client invia:

```text
end
```

il server termina il proprio ciclo.

`lower()` permette di riconoscere anche:

```text
END
End
eNd
```

---

# 60. TRABOCCHETTI DA SAPERE

## Socket

```text
AF_INET = TCP
```

❌

```text
AF_INET = IPv4
```

✅

---

```text
SOCK_STREAM = UDP
```

❌

```text
SOCK_STREAM = TCP
```

✅

---

```text
SOCK_DGRAM = TCP
```

❌

```text
SOCK_DGRAM = UDP
```

✅

---

## TCP

```text
bind() = connect()
```

❌

---

```text
listen() = recv()
```

❌

---

```text
accept() = ricevo direttamente i dati
```

❌

---

```text
accept() → connection socket + address
```

✅

---

```text
recv(1024) = esattamente 1024 byte
```

❌

---

```text
recv(1024) = fino a 1024 byte
```

✅

---

```text
recv() = str
```

❌

---

```text
recv() = bytes
```

✅

---

```text
encode() = bytes → str
```

❌

---

```text
encode() = str → bytes
```

✅

---

```text
decode() = str → bytes
```

❌

---

```text
decode() = bytes → str
```

✅

---

```text
send() = invio sicuramente tutto
```

❌

---

```text
sendall() = tenta di inviare tutto
```

✅

---

```text
TCP: una send() = una recv()
```

❌

---

```text
TCP = stream di byte
```

✅

---

## UDP

```text
UDP usa accept()
```

❌

---

```text
UDP usa listen()
```

❌

---

```text
UDP usa sendto()
```

✅

---

```text
UDP usa recvfrom()
```

✅

---

```text
recvfrom() → dati + indirizzo
```

✅

---

## HTTP

```text
GET / HTTP/1.1
```

✅ request line

---

```text
HTTP/1.1 200 OK
```

✅ status line

---

```text
200
```

✅ status code HTTP

---

```text
404
```

✅ risorsa non trovata

---

```text
404 = server spento
```

❌

---

```text
Content-Type: text/html
```

✅ body HTML

---

```text
Content-Type: application/json
```

✅ body JSON

---

```text
Content-Length
```

✅ lunghezza del body in byte

---

```text
\r\n
```

✅ fine di una riga HTTP

---

```text
\r\n\r\n
```

✅ separazione header/body

---

```text
HTTP = TCP
```

❌

---

```text
HTTP può viaggiare sopra TCP
```

✅

---

# 61. COSA IMPARARE A MEMORIA

## TCP SERVER

```text
socket
bind
listen
accept
recv/send
close
```

## TCP CLIENT

```text
socket
connect
send/recv
close
```

## UDP SERVER

```text
socket
bind
recvfrom
sendto
close
```

## UDP CLIENT

```text
socket
sendto
recvfrom
close
```

## CONVERSIONI

```text
str
↓ encode
bytes

bytes
↓ decode
str
```

## HTTP

```text
REQUEST
↓
GET + path + HTTP version

RESPONSE
↓
status + headers + blank line + body
```

---

# 62. LE 15 COSE CHE DEVI SAPERE AL 100%

```text
1. AF_INET → IPv4
2. SOCK_STREAM → TCP
3. SOCK_DGRAM → UDP
4. bind() → indirizzo locale
5. listen() → ascolto TCP
6. accept() → connection socket + address
7. connect() → connessione verso server
8. recv() → fino a N byte
9. sendall() → invio completo
10. encode() → str → bytes
11. decode() → bytes → str
12. TCP → stream
13. UDP → datagrammi
14. HTTP request → GET / ... HTTP/...
15. HTTP response → 200/404 + header + body
```

# 63. MODELLO MENTALE DEFINITIVO

Quando vedi:

```python
socket.AF_INET
```

pensa:

```text
IPv4
```

Quando vedi:

```python
socket.SOCK_STREAM
```

pensa:

```text
TCP
```

Quando vedi:

```python
socket.SOCK_DGRAM
```

pensa:

```text
UDP
```

Quando vedi:

```python
server.accept()
```

pensa:

```text
nuova connessione
↓
conn + addr
```

Quando vedi:

```python
conn.recv(...)
```

pensa:

```text
ricevo bytes
```

Quando vedi:

```python
.decode()
```

pensa:

```text
bytes → stringa
```

Quando vedi:

```python
.encode()
```

pensa:

```text
stringa → bytes
```

Quando vedi:

```python
sendto(...)
```

pensa:

```text
UDP
```

Quando vedi:

```python
GET /players HTTP/1.1
```

pensa:

```text
HTTP REQUEST
```

Quando vedi:

```text
HTTP/1.1 200 OK
```

pensa:

```text
HTTP RESPONSE
```

Quando vedi:

```text
\r\n\r\n
```

pensa:

```text
fine degli header
inizio del body
```

---

# 64. FORMULA FINALE

```text
TCP SERVER
socket → bind → listen → accept → recv/send → close

TCP CLIENT
socket → connect → send/recv → close

UDP SERVER
socket → bind → recvfrom/sendto → close

UDP CLIENT
socket → sendto/recvfrom → close

WEB SERVER
TCP
↓
HTTP REQUEST
↓
analisi GET/path
↓
HTTP RESPONSE
↓
HTML oppure JSON
```

## Obiettivo del compito

Devi essere in grado di:

```text
1. riconoscere quale protocollo stai usando;
2. mettere le istruzioni socket nell'ordine corretto;
3. correggere errori in un client/server;
4. distinguere ss/server socket da con/connection socket;
5. gestire bytes e stringhe con encode/decode;
6. capire cosa fa recv/send/sendto/recvfrom;
7. leggere una richiesta HTTP;
8. costruire una semplice risposta HTTP;
9. distinguere HTML e JSON;
10. seguire il flusso completo Browser → Server → Response.
```

### 🚨 Ultima regola anti-trabocchetto

Quando leggi una risposta multipla, fai attenzione alle parole:

```text
sempre
esattamente
necessariamente
esclusivamente
automaticamente
```

Molte affermazioni diventano false proprio perché sono formulate in modo assoluto.

Esempio:

```text
recv(1024) riceve esattamente 1024 byte
```

❌

```text
recv(1024) riceve fino a 1024 byte
```

✅

Questa è la versione da **studiare**, mentre il documento gigantesco può restare come riferimento quando trovi qualcosa che non ricordi.
````
