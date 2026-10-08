import socket # Importazione del modulo socket, necessario per la creazione di un'architettura C/S TCP a basso livello.
import json # Importazione del modulo json necessario per la lettura e la serializzazione dei dati dei calciatori.

'''
Si tenta di caricare il file index.html presente nella cartella corrente,
in caso di file non trovato viene implementato un placeholder con una frase di errore.
'''

try:
    with open('index.html', 'r', encoding='utf-8') as f:
        page = f.read()
except FileNotFoundError:
    page = "<h1>Error: File Not Found!</h1>"
    
    
'''
Si tenta di caricare il file soccerplayers.json presente nella cartella corrente,
in caso di file non trovato o errore nella decodifica del JSON viene segnalato un errore.
'''

try:
    with open('soccerplayers.json', 'r', encoding='utf-8') as f:
        giocatori = json.load(f)
    if not giocatori:
        raise ValueError("Error: the list is empty!")
except FileNotFoundError:
    print(f"Error: JSON Not Found!")
except (json.JSONDecodeError, ValueError) as e:
    print(f"Parsing error: {e}")


HOST = '0.0.0.0'  #indirizzo speciale che indica al server di ascoltare su tutte le interfacce di rete disponibili
PORT = 8080 #porta di ascolto, in questo caso rientrando nel range 1024 - 49151 si tratta di una Registered Port

        
ss = socket.socket(socket.AF_INET, socket.SOCK_STREAM) #creazione di un oggetto socket stream (TCP) appartenente alla famiglia di protocolli AF_INET (IPv4) 
ss.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1) #riutilizzo immediato  dell'indirizzo/porta dopo la chiusura del server. Necessario per evitare l'errore: "Address Already In Use"
ss.bind((HOST, PORT)) #associazione del socket all'indirizzo e alla porta scelti.
ss.listen(1) #configurazione del socket in ascolto e imposta a 1 il backlog delle connessioni pendenti
print(f'Server listen on port: {PORT}...') #messaggio di indicazione
goAway = True #flag necessario per il loop del server

while goAway: #loop del server
    '''
    una connessione in entrata viene accettata e il programma resta fermo qui finché un client non si connette. 
    Restituisce una tupla che contiene il socket da utilizzare per la comunicazione e l'indirizzo del client
    '''
    con, addr = ss.accept() 
    request = con.recv(8192).decode('utf-8').strip()  #ricezione fino a 8192 byte (8kB) dalla connessione, li decodifica in UTF-8 e rimuove gli spazi bianchi iniziali e finali
    
    print(f'{addr[0]}:{addr[1]} >> Request received!') #messaggio di indicazione della richiesta pervenuta dal client
    
    fl = request.split('\r\n')[0] #estrazione della prima riga ([0]) della richiesta HTTP (la "request line"). Si assume che le righe siano separate da CRLF come da specifica HTTP e un client non conforme potrebbe rompere questo parsing.
    parts = fl.split(' ') #divisione della prima riga in parti separate da spazio.
    
    if len(parts) >= 2:
        method = parts[0] #estrazione del metodo richiesto (GET, POST, ...)
        path = parts[1] #estrazione del percorso richiesto ('/', '/players')
    else:
        '''
        inizializzazione delle righe in caso di formato non atteso.
        '''
        method = ''
        path = ''
        
        
    '''
    se la richiesta indica GET /, si restituisce una pagina HTML e la risposta HTTP viene costruita con codice 200, con Content-Type HTML.
    Il Content-Length  è calcolato sulla codifica UTF-8 del corpo, quindi sul numero di byte effettivi.
    Connection close invece indica al client che la connessione verrà chiusa dopo questa risposta perchè il server non supporta keep-alive.
    '''
    
    if method == 'GET' and path == '/':
        rendered_html = page
        body = rendered_html.replace('\r', '').replace('\n', '\r\n')
        res = (
            "HTTP/1.1 200 OK\r\n"
            "Content-Type: text/html; charset=utf-8\r\n"
            f"Content-Length: {len(body.encode('utf-8'))}\r\n"
            "Connection: close\r\n"
            "\r\n"
            + body
        ).encode('utf-8')




    elif method == 'GET' and path == '/players':
        '''
        altrimenti, se la richiesta indica GET /players, si restituisce il JSON contenente l'intero elenco dei calciatori.
        La risposta HTTP viene costruita con codice 200, con Content-Type JSON e header CORS, che permettono a pagine servite da altre origini di chiamare /players attraverso il browser (politiche CORS gestite dal browser)
        '''
        try:
            '''
            Si tenta di ricaricare il file soccerplayers.json presente nella cartella corrente per avere dati aggiornati,
            in caso di file non trovato o errore nella decodifica del JSON viene segnalato un errore.
            '''
            with open('soccerplayers.json', 'r', encoding='utf-8') as f:
                giocatori = json.load(f)
            if not giocatori:
                raise ValueError("Error: the list is empty!")
        except FileNotFoundError:
            print(f"Error: JSON Not Found!")
        except (json.JSONDecodeError, ValueError) as e:
            print(f"Parsing error: {e}")
            
        rendered_json = json.dumps(giocatori, ensure_ascii=False) #conversione della lista Python in una stringa JSON e l'argomento ensure_ascii=False permette di mantenere caratteri accentati/non ASCII.
         
        body = rendered_json.replace('\r', '').replace('\n', '\r\n') #normalizzazione dei fine riga in CRLF poichè il protocollo HTTP richiede CRLF come separatore di riga.
        res = (
            "HTTP/1.1 200 OK\r\n"
            "Content-Type: application/json; charset=utf-8\r\n"
            f"Content-Length: {len(body.encode('utf-8'))}\r\n"
            "Access-Control-Allow-Origin: *\r\n"
            "Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\n"
            "Access-Control-Allow-Headers: Content-Type, Authorization, X-Requested-With\r\n"
            "Access-Control-Allow-Credentials: true\r\n"
            "Connection: close\r\n"
            "\r\n"
            + body
        ).encode('utf-8')

    else:
        '''
        altrimenti, si restituisce un file HTML contenente un placeholder con codice 404 Not Found.
        '''
        rendered_html = '<h1>404 Not Found</h1>'
        body = rendered_html.replace('\r', '').replace('\n', '\r\n')
        res = (
            "HTTP/1.1 404 Not Found\r\n"
            "Content-Type: text/html; charset=utf-8\r\n"
            f"Content-Length: {len(body.encode('utf-8'))}\r\n"
            "Connection: close\r\n"
            "\r\n"
            + body
        ).encode('utf-8')



    con.sendall(res) #invio di tutta la risposta al client
    con.close()  #chiusura della connessione con il client
    
    print(f'{addr[0]}:{addr[1]}  >> Response HTTP "{path}"') #messaggio di indicazione che segnala la risposta HTTP con il percorso'

ss.close() #chiusura del socket 
print('Server was remote stopped...') #messaggio di spegnimento
