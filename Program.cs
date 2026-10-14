using System;

namespace progettinoProgettoso
{
    class Program
    {
        static Dipendente[] dip = new Dipendente[100];
        static Dipartimento[] di = new Dipartimento[100];
        static int nD = 0;
        static int nDi = 0;

        static void Main()
        {
            while (true)
            {
                Console.Clear();

                int s = Menu("DBMS",
                    "CREATE (Inserimento)",
                    "RETRIEVE (Stampa)",
                    "UPDATE (Modifica)",
                    "DELETE (Eliminazione)",
                    "Esci");

                if (s == 0) break;

                if (s == 1) Create();
                else if (s == 2) Retrieve();
                else if (s == 3) Update();
                else if (s == 4) Delete();

                Console.WriteLine("\nPremi un tasto per continuare...");
                Console.ReadKey();
            }

            Console.WriteLine("Programma terminato.");
        }
        static int Menu(string titolo, params string[] voci)
        {
            Console.WriteLine("\n--- " + titolo + " ---");
            for (int i = 0; i < voci.Length; i++)
                Console.WriteLine((i + 1) + " - " + voci[i]);
            Console.WriteLine("0 - Indietro");

            while (true)
            {
                int n = DBMS.IntCheck("Scelta: ");
                if (n >= 0 && n <= voci.Length)
                    return n;
                Console.WriteLine("Scelta non valida!");
            }
        }

        static string Str(string msg)
        {
            Console.Write(msg);
            string s = Console.ReadLine();
            return s == null ? "" : s.Trim();
        }

        static string StrObbligatoria(string msg)
        {
            while (true)
            {
                string s = Str(msg);
                if (s != "") return s;
                Console.WriteLine("Campo obbligatorio!");
            }
        }

        static string CodiceNuovo()
        {
            while (true)
            {
                string s = Str("Codice dipartimento: ");
                if (s == "")
                    Console.WriteLine("Il codice e' obbligatorio!");
                else if (!DBMS.PKCheck(di, s, nDi))
                    Console.WriteLine("PK gia' esistente!");
                else
                    return s;
            }
        }

        static int MatricolaNuova(int escludi)
        {
            while (true)
            {
                int m = DBMS.IntCheck("Matricola: ");

                if (m <= 0)
                    Console.WriteLine("La matricola deve essere positiva!");
                else if (!MatricolaLibera(m, escludi))
                    Console.WriteLine("Matricola gia' esistente!");
                else
                    return m;
            }
        }

        static bool MatricolaLibera(int m, int escludi)
        {
            for (int i = 0; i < nD; i++)
                if (i != escludi && dip[i].Matricola == m)
                    return false;
            return true;
        }

        static string FK()
        {
            while (true)
            {
                string s = Str("Codice dipartimento FK (INVIO = NULL): ");
                if (s == "") return null;
                if (DBMS.FkCheck(di, s, nDi)) return s;
                Console.WriteLine("Dipartimento inesistente!");
            }
        }
        static void Create()
        {
            int s = Menu("INSERIMENTO", "Dipartimento", "Dipendente");
            if (s == 0) return;

            if (s == 1)
            {
                if (nDi >= di.Length) { Console.WriteLine("Tabella piena!"); return; }
                di[nDi] = new Dipartimento(
                    CodiceNuovo(),
                    StrObbligatoria("Nome: "),
                    StrObbligatoria("Citta: "));
                nDi++;
                Console.WriteLine("Dipartimento inserito!");
            }
            else
            {
                if (nD >= dip.Length) { Console.WriteLine("Tabella piena!"); return; }
                int m = MatricolaNuova(-1);
                string n = StrObbligatoria("Nome: ");
                string c = StrObbligatoria("Cognome: ");
                string f = FK();
                dip[nD] = new Dipendente(m, n, c, f);
                nD++;
                Console.WriteLine("Dipendente inserito!");
            }
        }

        static void Retrieve()
        {
            int s = Menu("STAMPA", "Dipartimenti", "Dipendenti", "Entrambe");
            if (s == 0) return;

            if (s == 1 || s == 3) StampaDip();
            if (s == 2 || s == 3) StampaDipendenti();
        }

        static void StampaDip()
        {
            Console.WriteLine("\n=== DIPARTIMENTI ===");
            if (nDi == 0) { Console.WriteLine("Nessuno."); return; }

            for (int i = 0; i < nDi; i++)
                Console.WriteLine(di[i].CodiceDipartimento + " | " +
                    di[i].NomeDipartimento + " | " + di[i].Città);
        }

        static void StampaDipendenti()
        {
            Console.WriteLine("\n=== DIPENDENTI ===");
            if (nD == 0) { Console.WriteLine("Nessuno."); return; }

            for (int i = 0; i < nD; i++)
            {
                string f = dip[i].CodiceDipartimento;
                if (f == null) f = "NULL";
                Console.WriteLine(dip[i].Matricola + " | " +
                    dip[i].Nome + " " + dip[i].Cognome + " | " + f);
            }
        }

        static void Update()
        {
            int s = Menu("MODIFICA", "Dipartimento", "Dipendente");
            if (s == 0) return;

            if (s == 1) ModificaDip();
            else ModificaDipendente();
        }

        static void ModificaDip()
        {
            if (nDi == 0) { Console.WriteLine("Nessun dipartimento."); return; }

            string vecchio = Str("Codice da modificare: ");

            var r = DBMS.PkFinder(di, vecchio, nDi);
            if (!r.Item1) { Console.WriteLine("Non trovato!"); return; }

            string nuovo = Str("Nuovo codice: ");
            if (nuovo == "") { Console.WriteLine("Vuoto!"); return; }
            if (nuovo == vecchio) { Console.WriteLine("Invariato."); return; }
            if (!DBMS.PKCheck(di, nuovo, nDi)) { Console.WriteLine("Gia' usato!"); return; }

            r = DBMS.PkFinder(di, vecchio, nDi);
            if (!r.Item1) return;
            di[r.Item2].CodiceDipartimento = nuovo;

            for (int i = 0; i < nD; i++)
                if (dip[i].CodiceDipartimento == vecchio)
                    dip[i].CodiceDipartimento = nuovo;

            Console.WriteLine("Modificato!");
        }

        static void ModificaDipendente()
        {
            if (nD == 0) { Console.WriteLine("Nessun dipendente."); return; }

            int m = DBMS.IntCheck("Matricola da modificare: ");
            int pos = TrovaMatricola(m);
            if (pos == -1) { Console.WriteLine("Non trovato!"); return; }

            int s = Menu("Cosa modificare?", "Matricola", "Codice dipartimento (FK)");
            if (s == 0) return;

            if (s == 1)
                dip[pos].Matricola = MatricolaNuova(pos);
            else
                dip[pos].CodiceDipartimento = FK();

            Console.WriteLine("Modificato!");
        }
        static void Delete()
        {
            int s = Menu("ELIMINAZIONE", "Dipartimento", "Dipendente");
            if (s == 0) return;

            if (s == 1) EliminaDip();
            else EliminaDipendente();
        }

        static void EliminaDipendente()
        {
            if (nD == 0) { Console.WriteLine("Nessun dipendente."); return; }

            int m = DBMS.IntCheck("Matricola da eliminare: ");
            int pos = TrovaMatricola(m);
            if (pos == -1) { Console.WriteLine("Non trovato!"); return; }

            for (int i = pos; i < nD - 1; i++)
                dip[i] = dip[i + 1];
            dip[--nD] = null;

            Console.WriteLine("Eliminato!");
        }

        static void EliminaDip()
        {
            if (nDi == 0) { Console.WriteLine("Nessun dipartimento."); return; }

            string codice = Str("Codice da eliminare: ");
            if (!DBMS.PkFinder(di, codice, nDi).Item1)
            {
                Console.WriteLine("Non trovato!");
                return;
            }

            int s = Menu("Politica di cancellazione",
                "SET NULL", "CASCADE", "RESTRICT");
            if (s == 0) return;

            bool ok;
            if (s == 1) ok = DBMS.SetNullFk(dip, di, codice, ref nDi, nD);
            else if (s == 2) ok = DBMS.Cascade(dip, di, codice, ref nDi, ref nD);
            else ok = DBMS.Restrict(dip, di, codice, ref nDi, nD);

            if (ok)
                Console.WriteLine("Eliminato!");
            else if (s == 3)
                Console.WriteLine("RESTRICT: esistono dipendenti collegati.");
            else
                Console.WriteLine("Impossibile completare l'eliminazione.");
        }
        static int TrovaMatricola(int m)
        {
            for (int i = 0; i < nD; i++)
                if (dip[i].Matricola == m)
                    return i;
            return -1;
        }
    }
}