# Ripasso: Stack ISO/OSI, TCP/IP e Socket

Appunti sullo stack ISO/OSI, il modello TCP/IP, i socket e un esempio pratico di stream socket Client/Server in Python. Materiale delle lezioni di **Sistemi e Reti / TPSIT**.

## Indice

- [Stack ISO/OSI](#stack-isoosi)
- [OSI vs TCP/IP](#osi-vs-tcpip)
- [I socket](#i-socket)
- [Host e porta](#host-e-porta)
- [Divisione delle porte](#divisione-delle-porte)
- [Fasi di comunicazione Client/Server](#fasi-di-comunicazione-clientserver)
- [Implementazione Python: stream socket C/S](#implementazione-python-stream-socket-cs)

---

## Stack ISO/OSI

Il modello **ISO/OSI** divide la comunicazione di rete in **7 livelli**, ciascuno con un compito preciso:

| # | Livello | Funzione |
|---|---|---|
| 7 | Applicazione | Interfaccia con l'utente (HTTP, FTP, SMTP...) |
| 6 | Presentazione | Formattazione dati, crittografia, compressione |
| 5 | Sessione | Apertura/gestione/chiusura delle sessioni di comunicazione |
| 4 | Trasporto | Comunicazione end-to-end (TCP, UDP) |
| 3 | Rete | Instradamento dei pacchetti (IP, routing) |
| 2 | Data Link | Trasmissione su un singolo collegamento fisico (MAC, switch) |
| 1 | Fisico | Trasmissione dei bit sul mezzo fisico (cavi, segnali) |

## OSI vs TCP/IP

Il modello **TCP/IP**, effettivamente usato su Internet, ha **4 livelli** invece di 7:

| TCP/IP | Corrisponde a OSI |
|---|---|
| Applicazione | Applicazione + Presentazione + Sessione (7-6-5) |
| Trasporto | Trasporto (4) |
| Internet | Rete (3) |
| Accesso alla rete | Data Link + Fisico (2-1) |

**Differenza chiave:** OSI è un modello **di riferimento/didattico**, teorico e molto dettagliato; TCP/IP è il modello **realmente implementato**, più semplice perché accorpa alcuni livelli (presentazione e sessione confluiscono nell'applicazione).

## I socket

Un **socket** è l'astrazione software che rappresenta l'endpoint di una comunicazione di rete tra due processi, identificato dalla coppia **(IP, porta)**.

Tipi principali:

- **Stream Socket** (`SOCK_STREAM`) → protocollo **TCP**
  Con connessione: affidabile, garantisce ordine e consegna dei dati. Richiede un handshake iniziale (*three-way handshake*).
- **Datagram Socket** (`SOCK_DGRAM`) → protocollo **UDP**
  Senza connessione: non garantisce ordine né consegna, ma è più veloce e leggero (usato per streaming, VoIP, DNS...).
- **RAW Socket** (`SOCK_RAW`)
  Accesso diretto ai protocolli di livello inferiore (es. IP), bypassando TCP/UDP. Usato per strumenti di rete avanzati (ping, sniffer). Richiede privilegi di amministratore.

## Host e porta

- **Host**: indirizzo IP che identifica univocamente un dispositivo in rete.
- **Porta**: numero da 0 a 65535 che identifica un processo/servizio specifico su quell'host.

La coppia **IP:porta** identifica in modo univoco un endpoint di comunicazione.

## Divisione delle porte

Le 65536 porte disponibili (0–65535) sono divise in tre fasce, definite dalla IANA:

| Fascia | Nome | Scopo |
|---|---|---|
| **0 – 1023** | Well-known ports (porte note) | Riservate ai servizi/protocolli standard e universalmente riconosciuti, assegnate dalla IANA. Es: `20/21` FTP, `22` SSH, `25` SMTP, `53` DNS, `80` HTTP, `443` HTTPS. Su molti sistemi richiedono privilegi di amministratore per essere aperte da un programma. |
| **1024 – 49151** | Registered ports (porte registrate) | Assegnabili a servizi/applicazioni specifiche registrate presso la IANA (es. porte usate da software particolari, database, giochi online), ma senza le stesse restrizioni delle well-known. Non richiedono privilegi di root. |
| **49152 – 65535** | Dynamic / Private / Ephemeral ports (porte dinamiche o temporanee) | Non assegnate stabilmente a nessun servizio: il sistema operativo le assegna **temporaneamente e in automatico** a un processo, tipicamente lato client, per la durata di una singola connessione. Una volta chiusa la connessione, la porta torna disponibile. |

> Nota: il range esatto delle porte dinamiche/temporanee (assegnate automaticamente dal SO) è **49152–65535**, non da 4000 in poi — 4000 rientra ancora nelle registered ports. È comunque comune, specialmente in laboratorio o negli esempi didattici, usare porte "alte" (es. 5000, 8080) per i propri programmi client/server proprio per evitare conflitti con le well-known ports e non dover avere privilegi di amministratore.

## Significato delle funzioni socket

| Funzione | Lato | Significato |
|---|---|---|
| `socket()` | Client e Server | Crea il socket, cioè l'endpoint di comunicazione. Si specifica la famiglia di indirizzi (`AF_INET` = IPv4) e il tipo (`SOCK_STREAM` = TCP, `SOCK_DGRAM` = UDP). |
| `bind()` | Server | Associa il socket a un indirizzo IP e a una porta specifica della macchina locale, così il sistema sa a chi consegnare i pacchetti in arrivo su quella porta. |
| `listen()` | Server | Mette il socket in stato di ascolto, pronto ad accettare richieste di connessione in arrivo. Il parametro numerico indica quante connessioni possono restare in coda in attesa di essere accettate. |
| `accept()` | Server | Si blocca finché non arriva una richiesta di connessione da un client; quando arriva, la accetta e restituisce un **nuovo socket** dedicato a quella connessione (più l'indirizzo del client), lasciando il socket originale libero di accettarne altre. |
| `connect()` | Client | Il client richiede l'apertura di una connessione verso l'indirizzo IP e la porta del server (avvia il three-way handshake nel caso del TCP). |
| `send()` | Client e Server | Invia dati sulla connessione. Non è garantito che invii tutti i byte richiesti in una sola chiamata: restituisce il numero di byte effettivamente spediti. |
| `sendall()` | Client e Server | Come `send()`, ma continua a inviare finché **tutti** i dati passati non sono stati trasmessi, gestendo internamente eventuali invii parziali. Più comodo e sicuro di `send()` per messaggi di dimensione nota. |
| `recv()` | Client e Server | Riceve dati dalla connessione, fino a un massimo di byte indicato come argomento (es. `recv(1024)`); restituisce i dati ricevuti come sequenza di byte. |
| `close()` | Client e Server | Chiude il socket e rilascia la connessione e le risorse associate. |

## Fasi di comunicazione Client/Server

Con uno **stream socket (TCP)**:

**Server:**
`socket()` → `bind()` → `listen()` → `accept()` → `recv()` / `send()` → `close()`

**Client:**
`socket()` → `connect()` → `send()` / `recv()` → `close()`

## Implementazione Python: stream socket C/S

Esempio semplice di comunicazione Client/Server tramite stream socket TCP.

**server.py**
```python
import socket

HOST = '127.0.0.1'
PORT = 5000

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind((HOST, PORT))
server.listen(1)
print("Server in ascolto...")

conn, addr = server.accept()
print(f"Connesso da {addr}")

while True:
    dati = conn.recv(1024).decode()
    if not dati or dati.lower() == "fine":
        break
    print(f"Client: {dati}")
    risposta = input("Server: ")
    conn.send(risposta.encode())

conn.close()
server.close()
```

**client.py**
```python
import socket

HOST = '127.0.0.1'
PORT = 5000

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect((HOST, PORT))

while True:
    msg = input("Client: ")
    client.send(msg.encode())
    if msg.lower() == "fine":
        break
    risposta = client.recv(1024).decode()
    print(f"Server: {risposta}")

client.close()
```
