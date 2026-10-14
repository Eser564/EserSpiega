using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace progettinoProgettoso
{
    static class DBMS
    {
        public static void StampaErrore(string mess)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(mess);
            Console.ReadKey();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void InsertionSort(Dipendente[] d, int n) //per Matricola!
        {
            for(int i = 1; i < n; i++)
            {
                Dipendente key = d[i];
                int j = i - 1;
                while(j >= 0 && d[j].Matricola > key.Matricola)
                {
                    d[j + 1] = d[j];
                    j--;
                }
                d[j+1] = key;
            }
        }

        static void InsertionSort(Dipartimento[] d, int n) //per CodiceDipartimento!
        {
            for (int i = 1; i < n; i++)
            {
                Dipartimento key = d[i];
                int j = i - 1;
                while (j >= 0 && string.CompareOrdinal(d[j].CodiceDipartimento, key.CodiceDipartimento) > 0)
                {
                    d[j + 1] = d[j];
                    j--;
                }
                d[j+1] = key;
            }
        }

        public static bool PKCheck(Dipartimento[] d, string val, int n)
        {
            if (string.IsNullOrEmpty(val))
                return false;

            InsertionSort(d, n);
            return BinaryRec(d, 0, n - 1, val);
        }

        public static bool PKCheck(Dipendente[] d, int val, int n)
        {
            if (val <= 0)
                return false;

            InsertionSort(d, n);
            return BinaryRec(d, 0, n - 1, val);
        }

        static bool BinaryRec(Dipendente[] d, int a, int b, int val)
        {
            if (a > b)
                return true;

            int m = a + (b - a) / 2;

            if (d[m].Matricola == val)
                return false;

            if (d[m].Matricola < val)
                return BinaryRec(d, m + 1, b, val);
            else
                return BinaryRec(d, a, m - 1, val);
        }


        static bool BinaryRec(Dipartimento[] d, int a, int b, string val)
        {
            if (a > b)
                return true;

            int m = a + (b - a) / 2;
            int c = string.CompareOrdinal(d[m].CodiceDipartimento, val);

            if (c== 0)
                return false;

            if (c < 0)
                return BinaryRec(d, m + 1, b, val);
            else
                return BinaryRec(d, a, m - 1, val);
        } 

        static (bool, int) PKFinderRec(Dipartimento[] d, int a, int b, string val)
        {
            if (a > b)
                return (false, -1);

            int m = a + (b - a) / 2;
            int c = string.CompareOrdinal(d[m].CodiceDipartimento, val);

            if (c == 0)
                return (true, m);

            if (c < 0)
                return PKFinderRec(d, m + 1, b, val);
            else
                return PKFinderRec(d, a, m - 1, val);
        }

        public static (bool, int) PkFinder(Dipartimento[] d, string val, int n)
        {
            if (string.IsNullOrEmpty(val))
                return (false, -1);

            InsertionSort(d, n);
            return PKFinderRec(d, 0, n - 1, val);
        }

        public static bool FkCheck(Dipartimento[] d, string val, int n)
        {
            if (string.IsNullOrEmpty(val))
                return false;

            InsertionSort(d, n);
            return PKFinderRec(d, 0, n - 1, val).Item1;
        }

        static void PKDelete(Dipartimento[] d, ref int n, int pos)
        {
            if (pos < 0 || pos >= n)
                return;

            for (int i = pos; i < n - 1; i++)
            {
                d[i] = d[i + 1];
            }
            n--;
        }

        static void FkDelete(Dipendente[] d, ref int n, int pos)
        {
            if (pos < 0 || pos >= n)
                return;

            for (int i = pos; i < n - 1; i++)
            {
                d[i] = d[i + 1];
            }
            n--;
        }

        public static bool SetNullFk(Dipendente[] d, Dipartimento[] di, string val, ref int nDi,  int nD)
        {
            var results = PkFinder(di, val, nDi);
            if (!results.Item1)
                return false;

            for (int i = 0; i < nD; i++)
                if (d[i].CodiceDipartimento == val)
                    d[i].CodiceDipartimento = null;
            PKDelete(di, ref nDi, results.Item2);
            return true;
        }

        public static bool Cascade(Dipendente[] d, Dipartimento[] di, string val, ref int nDi, ref int nD)
        {;
            var results = PkFinder(di, val, nDi);
            if (!results.Item1)
                return false;

            for (int i = 0; i < nD;)
                if (d[i].CodiceDipartimento == val) //se trovo cancello in cascata ogni FK associata alla tabella lato 1
                    FkDelete(d, ref nD, i);
                else
                    i++;
            PKDelete(di, ref nDi, results.Item2);
            return true;
        }

        public static bool Restrict(Dipendente[] d, Dipartimento[] di, string val, ref int nDi, int nD)
        {
            var results = PkFinder(di,val,nDi);
            if (!results.Item1)
                return false;

            for (int i = 0; i < nD; i++)
                if (d[i].CodiceDipartimento == val) //se trovo non devo cancellare nulla
                    return false; 
            PKDelete(di, ref nDi, results.Item2); //rimozione avviene solo quando non trovo nulla
            return true;
        }

        public static int IntCheck(string mess)
        {
            int n;
            bool x;
            do
            {
                Console.WriteLine(mess);
                x = int.TryParse(Console.ReadLine(), out n);
                if (!x)
                    StampaErrore("Errore >> Inserire un intero valido!");
            } while (!x);
            return n;
        }
    }
}
